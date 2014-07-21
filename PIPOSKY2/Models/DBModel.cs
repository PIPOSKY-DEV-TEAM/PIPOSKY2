using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIPOSKY2.Models
{
    public class SKYDB : DbContext
    {
        public SKYDB()
            : base("DbContext")
        {

        }
        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        [Key]
        public int UserID { set; get; }
        [Required]
        public string UserName { set; get; }
        [Required]
        public string UserPwd { set; get; }
        public string UserType { set; get; }
    }
}