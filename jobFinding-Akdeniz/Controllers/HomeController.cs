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
            var jobPosts = db.job_post.OrderByDescending(x => x.postCreatedDate).ToList();
            ViewBag.LastAdded = jobPosts.Take(10);
            return View();
        }

    }
}