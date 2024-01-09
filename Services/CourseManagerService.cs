using Ass2RM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using static Ass2RM.Entities.Student;

namespace Ass2RM.Services
{
	public class CourseManagerService : ICourseManagerService
	{
		private readonly IConfiguration _configuration;
		public readonly CourseManagerDbContext _courseManagerDbContext;

		public CourseManagerService(CourseManagerDbContext courseManagerDbContext, IConfiguration configuration)
		{
			_courseManagerDbContext = courseManagerDbContext;
			_configuration = configuration;
		}

		public List<Course> GetAllCourses()
		{
			return _courseManagerDbContext.Courses.Include(c => c.Students).OrderByDescending(c => c.StartDate).ToList();
		}

		public Course? GetCourseById(int id)
		{
			return _courseManagerDbContext.Courses.Include(c => c.Students).FirstOrDefault(c => c.CourseId == id);
		}

		public int AddCourse(Course subject)
		{
			_courseManagerDbContext.Courses.Add(subject);
			_courseManagerDbContext.SaveChanges();

			return subject.CourseId;
		}

		public void UpdateCourse(Course subject)
		{
			_courseManagerDbContext.Courses.Update(subject);
			_courseManagerDbContext.SaveChanges();
		}

		public Student? GetStudentById(int courseId, int studentId)
		{
			return _courseManagerDbContext.Students.Include(s => s.Course).FirstOrDefault(s => s.CourseId == courseId && s.StudentId == studentId);
		}

		public void UpdateConfirmationStatus(int courseId, int studentId, EnrollmentConfirmationStatus status)
		{
			var student = GetStudentById(courseId, studentId);

			if (student == null) return;

			student.Status = status;

			_courseManagerDbContext.SaveChanges();
		}

		public Course? AddStudentToCourseById(int courseId, Student student)
		{
			var subject = GetCourseById(courseId);
			if (subject == null) return null;

			subject.Students?.Add(student);
			_courseManagerDbContext.SaveChanges();

			return subject;
		}

		public void SendEnrollmentEmailByCourseId(int courseId, string scheme, string host)
		{

			var subject = GetCourseById(courseId);
			if (subject == null) return;
			var students = subject.Students.Where(s => s.Status == EnrollmentConfirmationStatus.ConfirmationMessageNotSent).ToList();

			try
			{
				var smtpHost = _configuration["SmtpSettings:Host"];
				var smtpPort = _configuration["SmtpSettings:Port"];
				var fromAddress = _configuration["SmtpSettings:FromAddress"];
				var fromPassword = _configuration["SmtpSettings:FromPassword"];

				using var smtpClient = new SmtpClient(smtpHost);
				smtpClient.Port = string.IsNullOrEmpty(smtpPort) ? 587 : Convert.ToInt32(smtpPort);
				smtpClient.Credentials = new NetworkCredential(fromAddress, fromPassword);
				smtpClient.EnableSsl = true;

				foreach (var student in students)
				{
					var responseUrl = $"{scheme}://{host}/courses/{courseId}/enroll/{student.StudentId}";

					var mailMessage = new MailMessage
					{
						From = new MailAddress(fromAddress),
						Subject = $"[Action Required] Confirm \"{student?.Course?.CourseName}\" Enrollment",
						Body = CreateBody(student, responseUrl),
						IsBodyHtml = true
					};

					if (student.StudentEmail == null) return;

					mailMessage.To.Add(student.StudentEmail);

					smtpClient.Send(mailMessage);

					student.Status = EnrollmentConfirmationStatus.ConfirmationMessageSent;
				}

				_courseManagerDbContext.SaveChanges();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}


		}

		private string CreateBody(Student student, string responseUrl)
		{
			return $@"
                <h1>Hello {student.StudentName}: </h1>
                <p>Your Request to enroll in the course {student.Course.CourseName} 
                   in room {student.Course.RoomNumber} 
                   starting {student.Course.StartDate.Value.ToShortDateString()} 
                    with instructor {student.Course.Instructor}. </p>
                <p>
                <p> We are pleased to have you in the course so if you could 
                 <a href={responseUrl}>confirm your enrollment <a/> as soon as possible that would be appreciated!
                </p>
                <p>Sincerely,</p>
                <p>The Course Manager App</p>

			";
		}





	}
}
