using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jobFinding_Akdeniz.Models;

namespace jobFinding_Akdeniz.Controllers
{
    public class HomeController : Controller
    {
        private DBEntities db = new DBEntities();
        // GET: Home
        public ActionResult Index()
        {
            var jobPosts = db.job_post.ToList();
            return View(jobPosts);
        }

    }
}