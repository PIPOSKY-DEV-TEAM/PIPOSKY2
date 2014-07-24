using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PIPOSKY2.Models
{
    public class SubmitApiModels
	{
		public int SubmitID { get; set; }
		public string Lang { get; set; }
		public int ProbID { get; set; }
		public string Source { get; set; }
	}
    public class SubmitResultApiModels
    {
        public int SubmitID { get; set; }
        public string Lang { get; set; }
        public int ProbID { get; set; }
        public string Source { get; set; }
    }
    
}