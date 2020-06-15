using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using jobFinding_Akdeniz.Models;

namespace jobFinding_Akdeniz.Controllers
{
    public class CompanyController : Controller
    {
        private DBEntities db = new DBEntities();
        // GET: Company
        [UserCheckCompany]
        public ActionResult Index()
        {
            return View();
        }

        [UserCheckCompany]
        public ActionResult CompanyPosts()
        {
            var posts = db.job_post.Where(x => x.companyID == LoginStatus.Current.companyId).ToList();
            return View(posts);
        }

        [UserCheckCompany]
        public ActionResult LoadPostList()
        {
            try
            {
                var draw = Request.Form.GetValues("draw");
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var custData = from jp in db.job_post
                               join jl in db.job_location on jp.jobLocationID equals jl.jobLocationId
                               join jt in db.job_type on jp.jobTypeID equals jt.jobTypeId
                               join co in db.company on jp.companyID equals co.companyId
                               where co.companyId == LoginStatus.Current.companyId
                               select new { jp.jobPostId, co.companyName, jp.jobPostTitle, jp.postCreatedDate, jp.postEndedDay, jp.department, jl.city, jp.isActivePost };
                
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    custData = custData.OrderBy(sortColumn + " " + sortColumnDir);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lengthofSearch = searchValue.Length;
                    custData = custData.Where(x => x.companyName.Substring(0, lengthofSearch).Equals(searchValue) || x.jobPostTitle.Substring(0, lengthofSearch).Equals(searchValue) || x.department.Substring(0, lengthofSearch).Equals(searchValue) || x.city.Substring(0, lengthofSearch).Equals(searchValue));
                }

                recordsTotal = custData.Count();

                var data = custData.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [UserCheckCompany]
        [HttpPost]
        public JsonResult postActivate(job_post post)
        {
            var selected = db.job_post.Where(x => x.jobPostId == post.jobPostId).FirstOrDefault();
            selected.isActivePost = "1";
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        [UserCheckCompany]
        [HttpPost]
        public JsonResult postDeactivate(job_post post)
        {
            var selected = db.job_post.Where(x => x.jobPostId == post.jobPostId).FirstOrDefault();
            selected.isActivePost = "0";
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        [UserCheckCompany]
        [HttpPost]
        public ActionResult Details(int? id)
        {
            ViewBag.Count = db.job_post_activity.Where(x => x.jobPostID == id).Count();
            return PartialView(db.job_post.FirstOrDefault(x => x.jobPostId == id));
        }

        [UserCheckCompany]
        public ActionResult GetApplicants(int? id)
        {
            return View();
        }
       
        [UserCheckCompany]
        public ActionResult LoadApplicants(int? id)
        {
            try
            {
                var draw = Request.Form.GetValues("draw");
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var custData = from jp in db.job_post
                               join jpa in db.job_post_activity on jp.jobPostId equals jpa.jobPostID
                               join ua in db.user_account on jpa.userAccountID equals ua.userAccountId
                               where jpa.jobPostID == id
                               select new { jpa.userAccountID, ua.firstName, ua.lastName, ua.userEmail, jpa.applyDate };
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    custData = custData.OrderBy(sortColumn + " " + sortColumnDir);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lengthofSearch = searchValue.Length;
                    custData = custData.Where(x => x.firstName.Substring(0, lengthofSearch).Equals(searchValue) || x.lastName.Substring(0, lengthofSearch).Equals(searchValue));
                }

                recordsTotal = custData.Count();

                var data = custData.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [UserCheckCompany]
        public ActionResult Edit(int? id)
        {
            return View(db.job_post.FirstOrDefault(x => x.jobPostId == id));
        }
    }
}