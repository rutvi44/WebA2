using Microsoft.EntityFrameworkCore;
namespace Ass2RM.Entities
{
	public class CourseManagerDbContext : DbContext
	{
		public CourseManagerDbContext(DbContextOptions<CourseManagerDbContext> options) : base(options) { }

		public DbSet<Course> Courses { get; set; }

		public DbSet<Student> Students { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Student>().Property(student => student.Status).HasConversion<string>().HasMaxLength(64);

			modelBuilder.Entity<Course>().HasData(
				new Course
				{
					CourseId = 1,
					CourseName = "Programming Microsoft Web Technologies",
					Instructor = "Vignesh Parameswaran",
					StartDate = new DateTime(2023, 12, 31),
					RoomNumber = "2B14"
				});

			modelBuilder.Entity<Student>().HasData(new Student
			{
				StudentId = 1,
				StudentName = "Rutvi Mistry",
				StudentEmail = "rmistry@conestogac.on.ca",
				CourseId = 1
			});
		}

	}
}
