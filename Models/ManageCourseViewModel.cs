using Ass2RM.Entities;

namespace Ass2RM.Models
{
	public class ManageCourseViewModel
	{
		public Course? Course { get; set; }

		public Student? Student { get; set; }

		public int CountConfirmationMessageNotSent { get; set; }

		public int CountConfirmationMessageSent { get; set; }

		public int CountEnrollmentConfirmed { get; set; }

		public int CountEnrollmentDeclined { get; set; }
	}
}
