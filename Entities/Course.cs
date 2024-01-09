using System.ComponentModel.DataAnnotations;

namespace Ass2RM.Entities
{
	public class Course
	{
		public int CourseId { get; set; }

		[Required(ErrorMessage ="What is your course name?")]
		public string? CourseName { get; set; }

		[Required(ErrorMessage = "Who is Instructor?")]
		public string? Instructor { get; set; }

		[Required(ErrorMessage = "When is your class?")]
		public DateTime? StartDate { get; set; }

		[Required(ErrorMessage = "What's your Room Number?")]
		[RegularExpression(@"^\d[A-Z]\d{2}$", ErrorMessage = "Must be in format 1X11")]
		public string? RoomNumber { get; set; }

		public List<Student>? Students { get; set; }
	}
}
