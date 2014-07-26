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
        public DbSet<Contest> Contests { set; get; }
		public DbSet<Submit> Submits { get; set; }
        public DbSet<ContestProblem> ContestProblems { set; get; }
        public DbSet<ContestGroup> ContestGroups { set; get; }

    }

	public class DBInitializer : DropCreateDatabaseIfModelChanges<PIPOSKY2DbContext>
	{
		protected override void Seed(PIPOSKY2DbContext context)
		{
			var user = new User {UserEmail = "test@test.com", UserName = "root", UserPwd = "admin", UserType = "root"};
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

        public virtual User Creator { set; get; }
        public string Content { set; get; }
    }

    public class Contest
    {
        [Key]
        public int ContestID { set; get; }
        [Required]
        public int ContestGroupID { set; get; }
        [Required]
        public string ContestName { set; get; }
        [Required]
        public DateTime StartTime { set; get; }
        [Required]
        public DateTime EndTime { set; get; }
        public string ScorePath { set; get; }
    }

    public class ContestGroup
    {
        [Key]
        public int ContestGroupID { set; get; }
        [Required]
        public string ContestGroupName { set; get; }
    }

    public class ContestProblem
    {
        [Key]
        public int ContestProblemID { set; get; }
        [Required]
        public int ContestID { set; get; }
        [Required]
        public int ProblemID { set; get; }
    }

	public class Submit
	{
		[Key]
		public int SubmitID { get; set; }
		[Required]
		public string Lang { get; set; }
		[Required]
		public virtual Problem Prob { get; set; }

		public virtual User User { get; set; }
		[Required]
		public DateTime Time { get; set; }

		[Required]
		public string Source { get; set; }
		public string State { get; set; }
		public string Result { get; set; }
	}
}