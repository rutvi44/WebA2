using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Ass2RM.Entities
{
	public class Student
	{
		public enum EnrollmentConfirmationStatus
		{
			ConfirmationMessageNotSent = 0,
			ConfirmationMessageSent = 1,
			EnrollmentConfirmed = 2,
			EnrollmentDeclined = 3,
		}

		public int StudentId { get; set; }

		[Required(ErrorMessage = "What is student name?")]
		public string? StudentName { get; set;}

		[Required(ErrorMessage = "What is student's Email?")]
		[EmailAddress(ErrorMessage = "Must be a valid Email")]
		public string? StudentEmail { get; set; }

		public EnrollmentConfirmationStatus Status { get; set; } = EnrollmentConfirmationStatus.ConfirmationMessageNotSent;

		public int CourseId { get; set; }

		public Course? Course { get; set; }
	}
}
