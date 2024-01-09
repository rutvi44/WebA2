using Ass2RM.Models;
using Ass2RM.Entities;
using Ass2RM.Services;
using Microsoft.AspNetCore.Mvc;
using static Ass2RM.Entities.Student;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Ass2RM.Controllers
{
	public class CourseController : AbstractBaseController
	{
		private readonly ICourseManagerService _courseManagerService;

		public CourseController(ICourseManagerService courseManagerService)
		{
			_courseManagerService = courseManagerService;
		}

		[HttpGet("/courses")]
		public ActionResult List()
		{
			SetWelcome();

			var coursesViewModel = new CoursesViewModel()
			{
				Courses = _courseManagerService.GetAllCourses()
			};

			return View(coursesViewModel); 
		}

		[HttpGet("/courses/{id:int}")]
		public ActionResult Manage(int id)
		{
			SetWelcome();

			var subject = _courseManagerService.GetCourseById(id);

			if(subject == null)
				return NotFound();

			var manageCourseViewModel = new ManageCourseViewModel()
			{
				Course = subject,
				Student = new Student(),
				CountConfirmationMessageNotSent = subject.Students.Count(s => s.Status == EnrollmentConfirmationStatus.ConfirmationMessageNotSent),
				CountConfirmationMessageSent = subject.Students.Count(s => s.Status == EnrollmentConfirmationStatus.ConfirmationMessageSent),
				CountEnrollmentConfirmed = subject.Students.Count(s => s.Status == EnrollmentConfirmationStatus.EnrollmentConfirmed),
				CountEnrollmentDeclined = subject.Students.Count(s => s.Status == EnrollmentConfirmationStatus.EnrollmentDeclined)

			};

			return View(manageCourseViewModel);
		}

		[HttpGet("/courses/{courseId:int}/enroll/{studentId:int}")]
		public IActionResult Enroll(int courseId, int studentId)
		{
			SetWelcome();

			var student = _courseManagerService.GetStudentById(courseId, studentId);

			if (student == null)
				return NotFound();

			var enrollStudentView = new EnrollStudentViewModel()
			{
				Student = student
			};

			return View(enrollStudentView);

		}

		[HttpPost("/courses/{courseId:int}/enroll/{studentId:int}")]
		public IActionResult Enroll(int courseId, int studentId, EnrollStudentViewModel enrollStudentViewModel)
		{
			SetWelcome();

			var student = _courseManagerService.GetStudentById(courseId, studentId);
			if (student == null)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				var status = enrollStudentViewModel.Response == "Yes" ?
					EnrollmentConfirmationStatus.EnrollmentConfirmed :
					EnrollmentConfirmationStatus.EnrollmentDeclined;
				_courseManagerService.UpdateConfirmationStatus(courseId, studentId, status);

				return RedirectToAction("ThankYou", new { response = enrollStudentViewModel.Response });
			}
			else
			{
				enrollStudentViewModel.Student = student;
				return View(enrollStudentViewModel);
			}

		}

		[HttpGet("/courses/add")]
		public ActionResult Add()
		{
			SetWelcome();

			var courseViewModel = new CourseViewModel()
			{
				Course = new Course()
			};

			return View(courseViewModel);
		}

		[HttpPost("/courses/add")]
		public IActionResult Add(CourseViewModel courseViewModel)
		{
			SetWelcome();

			if (!ModelState.IsValid) return View(courseViewModel);

			_courseManagerService.AddCourse(courseViewModel.Course);

			TempData["notify"] = $"{courseViewModel.Course.CourseName} added Successfully!";
			TempData["className"] = "success";

			return RedirectToAction("Manage", new { id = courseViewModel.Course.CourseId });
		}

		[HttpGet("/courses/{id:int}/edit")]
		public ActionResult Edit(int id)
		{
			SetWelcome();

			var subject = _courseManagerService.GetCourseById(id);

			if (subject == null)
				return NotFound();

			var courseViewModel = new CourseViewModel()
			{
				Course = subject
			};

			return View(courseViewModel);
		}

		[HttpPost("/courses/{id:int}/edit")]
		public IActionResult Edit(int id, CourseViewModel courseViewModel)
		{
			SetWelcome();

			if (!ModelState.IsValid) return View(courseViewModel);

			_courseManagerService.UpdateCourse(courseViewModel.Course);

			TempData["notify"] = $"{courseViewModel.Course.CourseName} updated Successfully!";
			TempData["className"] = "info";

			return RedirectToAction("Manage", new { id });
		}

		[HttpGet("/thank-you/{response}")]
		public IActionResult ThankYou(string response)
		{
			SetWelcome();

			return View("ThankYou", response);
		}


		[HttpPost("/courses/{courseId:int}/add-student")]
		public IActionResult AddStudent(int courseId, ManageCourseViewModel manageCourseViewModel)
		{
			SetWelcome();

			Course? subject;


			if (ModelState.IsValid)
			{
				subject = _courseManagerService.AddStudentToCourseById(courseId, manageCourseViewModel.Student);

				if (subject == null) return NotFound();

				TempData["notify"] = $"{manageCourseViewModel.Student.StudentName} added to student list";
				TempData["className"] = "success";

				return RedirectToAction("Manage", new { id = courseId });
			}
			else
			{
				subject = _courseManagerService.GetCourseById(courseId);
				if (subject == null) return NotFound();
				manageCourseViewModel.Course = subject;
				manageCourseViewModel.CountConfirmationMessageNotSent = subject.Students.Count(s => s.Status == EnrollmentConfirmationStatus.ConfirmationMessageNotSent);
				manageCourseViewModel.CountConfirmationMessageSent = subject.Students.Count(s => s.Status == EnrollmentConfirmationStatus.ConfirmationMessageSent);
				manageCourseViewModel.CountEnrollmentConfirmed = subject.Students.Count(s => s.Status == EnrollmentConfirmationStatus.EnrollmentConfirmed);
				manageCourseViewModel.CountEnrollmentDeclined = subject.Students.Count(s => s.Status == EnrollmentConfirmationStatus.EnrollmentDeclined);
				return View("Manage", manageCourseViewModel);
			}

		}

		[HttpPost("/courses/{courseId:int}/enroll")]
		public IActionResult SendInvitation(int courseId)
		{
			_courseManagerService.SendEnrollmentEmailByCourseId(courseId, Request.Scheme, Request.Host.ToString());
			return RedirectToAction("Manage", new { id = courseId });

		}


	}
}
