using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIPOSKY2.Models
{
    public class PIPOSKY2DbContext : DbContext
    {
        public PIPOSKY2DbContext()
            : base("PIPOSKY2DbContext")
        {

        }
        public DbSet<User> Users { set; get; }
        public DbSet<Problem> Problems { set; get; }
        public DbSet<Contest> Contests { set; get; }
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
        [Required]
        public string ProblemPath { set; get; }
        [Required]
        public bool Visible { set; get; }
        [Required]
        public bool Downloadable { set; get; }
    }

    public class Contest
    {
        [Key]
        public int ContestID { set; get; }
        [Required]
        public string ContestName { set; get; }
        [Required]
        public DateTime StartTime { set; get; }
        [Required]
        public DateTime EndTime { set; get; }
        public IEnumerable<Problem> Problems { set; get; }
        public string ScorePath { set; get; }
    }
}