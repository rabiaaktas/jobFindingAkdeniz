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

        public ActionResult Index(string searchSt, string city, string department, string bDepartment, string jobType, int? page)
        {
            int pageIndex = page ?? 1;
            int dataCount = 50;
            int cityId = 0;
            if(!string.IsNullOrEmpty(city))
            {
                cityId = Convert.ToInt32(city);
            }
            var depId = Convert.ToInt32(department);
            var bDepId = Convert.ToInt32(bDepartment);
            var jobId = Convert.ToInt32(jobType);
            var country = db.countries.Where(x => x.countryId == cityId).FirstOrDefault();
            var bDept = db.business_departments.Where(x => x.businessDepId == bDepId).FirstOrDefault();
            var posts = db.job_post.AsQueryable();
            posts = posts.Where(x => x.isActivePost == "1");
            //var posts = (from jp in db.job_post
            //            where  (city == null ? true : jp.job_location.city == country.countryName) && (department == null ? true : jp.company.businessID == depId) && (jobType == null ? true : jp.jobTypeID == jobId) && (searchSt == null ? true : jp.jobPostTitle.Contains(searchSt)) && (jp.isActivePost == "1")
            //            select jp);
            if (!string.IsNullOrEmpty(searchSt))
            {
                posts = posts.Where(x => x.jobPostTitle.Contains(searchSt));
            }
            if (!string.IsNullOrEmpty(city))
            {
                posts = posts.Where(x => x.job_location.city == country.countryName);
            }
            if (!string.IsNullOrEmpty(department))
            {
                posts = posts.Where(x => x.company.businessID == depId);
            }
            if (!string.IsNullOrEmpty(jobType))
            {
                posts = posts.Where(x => x.jobTypeID == jobId);
            }
            if (!string.IsNullOrEmpty(bDepartment))
            {
                posts = posts.Where(x => x.department == bDept.businessDepName);
            }
            ViewBag.Count = posts.ToList().Count;
            ViewBag.searchSt = searchSt;
            ViewBag.city = city;
            ViewBag.department = department;
            ViewBag.bDepartment = bDepartment;
            ViewBag.jobType = jobType;
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