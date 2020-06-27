using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using jobFinding_Akdeniz.Models;

namespace jobFinding_Akdeniz.Controllers
{
    public class JobPostsController : Controller
    {
        private DBEntities db = new DBEntities();
        // GET: JobPosts
        public ActionResult Index(int? page)
        {
            int pageIndex = page ?? 1;
            int dataCount = 50;
            var posts = db.job_post.ToList();
            ViewBag.Count = posts.Count;
            var pagedPost = posts.OrderByDescending(x => x.postCreatedDate.Date).ToPagedList(pageIndex, dataCount);
            return View(pagedPost);
        }
    }
}