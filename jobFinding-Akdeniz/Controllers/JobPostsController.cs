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

        public ActionResult Index(int? page, string searchSt, Nullable<int> city, Nullable<int> department, Nullable<int> bDepartment, Nullable<int> jobType)
        {
            int pageIndex = page ?? 1;
            int dataCount = 1;
            var country = db.countries.Where(x => x.countryId == city).FirstOrDefault();
            var bDept = db.business_departments.Where(x => x.businessDepId == bDepartment).FirstOrDefault();
            var posts = from jp in db.job_post
                        where  (city == null ? true : jp.job_location.city == country.countryName) && (department == null ? true : jp.company.businessID == department) && (bDepartment == null ? true : jp.department == bDept.businessDepName) && (jobType == null ? true : jp.jobTypeID == jobType) && (searchSt == null ? true : jp.jobPostTitle.Contains(searchSt)) && (jp.isActivePost == "1")
                        select jp;
            ViewBag.Count = posts.ToList().Count;
            var pagedPost = posts.ToList().OrderByDescending(x => x.postCreatedDate.Date).ToPagedList(pageIndex, dataCount);
            return View(pagedPost);
        }


        public ActionResult Details(int id)
        {
            job_post post = new job_post();
            if(RouteData.Values["id"] != null)
            {
                post = db.job_post.Where(x => x.jobPostId == id).FirstOrDefault();
            }
            return View(post);
        }

        public ActionResult CompanyDetails(int id)
        { 
            company company = new company();
            if(RouteData.Values["id"] != null)
            {
                company = db.company.Where(x => x.companyId == id).FirstOrDefault();
            }
            return View(company);
        }

        [UserCheck]
        public ActionResult ApplyJob(int id)
        {
            if (RouteData.Values["id"] != null)
            {
                job_post_activity activity = new job_post_activity();
                activity.applyDate = DateTime.Now;
                activity.jobPostID = id;
                activity.userAccountID = LoginStatus.Current.UserId;
                db.job_post_activity.Add(activity);
                db.SaveChanges();
            }

            return Redirect("/ilan-detaylari/" + id);

        }
    }
}