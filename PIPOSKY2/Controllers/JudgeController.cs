using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PIPOSKY2.Models;

namespace PIPOSKY2.Controllers
{
    public class JudgeController : ApiController
    {

        PIPOSKY2DbContext db = new PIPOSKY2DbContext();

        // GET api/judge
        public SubmitApiModels Get()
        {
            var ret = new SubmitApiModels();
            var tmp = db.Submits.FirstOrDefault(_ => _.State == "wait");
            if (tmp == null)
            {
                ret.SubmitID = 0;
            }
            else
            {
                ret.SubmitID = tmp.SubmitID;
                ret.ProbID = tmp.Prob.ProblemID;
                ret.Source = tmp.Source;
                ret.Lang = tmp.Lang;
            }
            return ret;
        }

        // POST api/judge
        public void Post([FromBody]SubmitResultApiModels value)
        {
        }


    }
}
