using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jobFinding_Akdeniz.Models;


namespace jobFinding_Akdeniz.Controllers
{
    public class SirketController : Controller
    {
        private DBEntities db = new DBEntities();
        public ActionResult Index()
        {
            //job_type jb = new job_type();
            //jb.jobTypeName = "asadsad";
            //db.SaveChanges();
            //business_stream bs = new business_stream();
            //var asd = db.business_stream.Add(bs); 
            return View();
        }
    }
}