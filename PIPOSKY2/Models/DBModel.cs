using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace PIPOSKY2.Models
{
    public class PIPOSKY2DbContext : DbContext
    {
        public PIPOSKY2DbContext()
            : base("PIPOSKY2DbContext")
        { }
        public DbSet<User> Users { set; get; }
        public DbSet<Problem> Problems { set; get; }
        public DbSet<Homework> Course { set; get; }
		public DbSet<Submit> Submits { get; set; }
        public DbSet<HomeworkProblem> HomeworkProblems { set; get; }
        public DbSet<Course> Courses { set; get; }

    }

	public class DBInitializer : DropCreateDatabaseIfModelChanges<PIPOSKY2DbContext>
	{
		protected override void Seed(PIPOSKY2DbContext context)
		{
			var user = new User {UserEmail = "test@test.com", UserName = "root", UserPwd = "admin123", UserType = "admin"};
			context.Users.AddOrUpdate(user);

            var prob = new Problem { Creator = user, Downloadable = true, ProblemName = "a", Visible = true, ProblemPath = "a.zip" ,Content="TEST"};
			context.Problems.AddOrUpdate(prob);

			var submit = new Submit
			{
				Lang = "C++",
				Prob = prob,
				Result = "",
				Source = "TEST",
				State = "wait",
				Time = DateTime.Now,
				User = user
			};

			context.Submits.AddOrUpdate(submit);
            context.SaveChanges();
			base.Seed(context);
		}

	}

    public class User
    {
        [Key]
        public int UserID { set; get; }
        [Required]
        public string UserName { set; get; }
        [Required]
        public string UserPwd { set; get; }
        [Required]
        public string UserEmail { set; get; }
        public string UserType { set; get; }
    }

    public class Problem
    {
        [Key]
        public int ProblemID { set; get; }
        [Required]
        public string ProblemName { set; get; }
        public string ProblemPath { set; get; }
        [Required]
        public bool Visible { set; get; }
        [Required]
        public bool Downloadable { set; get; }
        [Required]
        public virtual User Creator { set; get; }
        [Required]
        public string Content { set; get; }
    }

    public class Homework
    {
        [Key]
        public int HomeworkID { set; get; }
        [Required]
        public int CourseID { set; get; }
        [Required]
        public string HomeworkName { set; get; }
        [Required]
        public DateTime StartTime { set; get; }
        [Required]
        public DateTime EndTime { set; get; }
        public string ScorePath { set; get; }
    }

    public class Course
    {
        [Key]
        public int CourseID { set; get; }
        [Required]
        public string CourseName { set; get; }
    }

    public class HomeworkProblem
    {
        [Key]
        public int HomeworkProblemID { set; get; }
        [Required]
        public int HomeworkID { set; get; }
        [Required]
        public int ProblemID { set; get; }
    }

	public class Submit
	{
		[Key]
		public int SubmitID { get; set; }
		[Required]
		public string Lang { get; set; }

		public virtual Problem Prob { get; set; }

		public virtual User User { get; set; }
		[Required]
		public DateTime Time { get; set; }

		[Required]
		public string Source { get; set; }
		public string State { get; set; }
        public int Score { get; set; }
		public string Result { get; set; }
        public string CompilerRes { get; set; }
	}
}