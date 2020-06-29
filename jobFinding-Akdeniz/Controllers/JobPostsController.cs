using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using jobFinding_Akdeniz.Models;
using System.Linq.Dynamic;

namespace jobFinding_Akdeniz.Controllers
{
    public class JobPostsController : Controller
    {
        private DBEntities db = new DBEntities();
        // GET: JobPosts

        public ActionResult Index(int? page, string searchSt, string city)
        {
            int pageIndex = page ?? 1;
            int dataCount = 50;
            var posts = db.job_post.AsQueryable();
            if (searchSt != null)
            {
                posts = posts.Where(x => x.jobPostTitle.Contains(searchSt));
            }
            if (city != null)
            {
                posts = posts.Where(x => x.job_location.city == city);
            }
            ViewBag.Count = posts.ToList().Count;
            var pagedPost = posts.ToList().OrderByDescending(x => x.postCreatedDate.Date).ToPagedList(pageIndex, dataCount);
            return View(pagedPost);
        }

        [HttpPost]
        public ActionResult Index(int? page)
        {
            var searched = db.job_post.AsQueryable();
            return View();
        }
    }
}