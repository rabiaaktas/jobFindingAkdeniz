using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using jobFinding_Akdeniz.Models;
using jobFinding_Akdeniz.Models.HelperModels;
using PagedList;
using static jobFinding_Akdeniz.States;

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
                               orderby DbFunctions.TruncateTime(jp.postCreatedDate) descending
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

        [UserCheckCompany]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(job_post post)
        {
            var jp = db.job_post.FirstOrDefault(x => x.jobPostId == post.jobPostId);
            var jl = db.job_location.FirstOrDefault(x => x.jobLocationId == jp.jobLocationID);
            if (jp != null)
            {
                jp.jobPostTitle = post.jobPostTitle;
                jp.postEndedDay = post.postEndedDay;
                jp.department = post.department;
                jp.job_location.city = post.job_location.city;
                jp.job_location.streetAddress = post.job_location.streetAddress;
                jp.jobTypeID = post.jobTypeID;
                jp.jobDescription = post.jobDescription;
                if(post.educationInfo != null)
                {
                    jp.educationInfo = post.educationInfo;
                }
                if (post.militaryStiation != null)
                {
                    jp.militaryStiation = post.militaryStiation;
                }
                if(post.experienceStatus != null)
                {
                    jp.experienceStatus = post.experienceStatus;
                }
                db.SaveChanges();
                return RedirectToAction("CompanyPosts", "Company");
            }
            else
            {
                ViewBag.Warning = "Düzenleme gerçekleştirilemedi.";
                return View();
            }
        }

        [UserCheckCompany]
        public ActionResult PostAdd()
        {
            var jb = new job_post();
            return View(jb);
        }

        [UserCheckCompany]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostAdd(job_post jb)
        {
            var status = db.sp_InsertCompanyPost(jb.job_location.city, jb.job_location.streetAddress, jb.jobTypeID, LoginStatus.Current.companyId,"0", jb.postEndedDay, jb.jobDescription,jb.jobPostTitle, jb.department, jb.experienceStatus, jb.educationInfo, jb.militaryStiation);
            db.SaveChanges();
            return RedirectToAction("CompanyPosts", "Company");
        }

        [UserCheckCompany]
        public ActionResult FindStudentEmployee()
        {
            return View();
        }

        [UserCheckCompany]
        public ActionResult ResultStudentEmployee(Nullable<int> intrestedSectorId, string statusStd, string department, Nullable<int> languageID, int? page)
        {
            int pageIndex = page ?? 1;
            int dataCount = 1;
            var departmentId = Convert.ToInt32(department);
            var dp = db.departments.Where(x => x.departmentsId == departmentId).FirstOrDefault();
            var users = from ua in db.user_account
                        join stu in db.user_student on ua.userAccountId equals stu.userAccountID
                        join edu in db.user_education on ua.userAccountId equals edu.userAccountId
                        join ue in db.user_experinence_detail on ua.userAccountId equals ue.userAccountID
                        join us in db.user_language_skill on ua.userAccountId equals us.userAccountID
                        where ((string.IsNullOrEmpty(department) ? true : edu.department == dp.departmentName)) && ((string.IsNullOrEmpty(statusStd) ? true : stu.statusStd == statusStd)) && (ua.userTypeID == 2) && (intrestedSectorId == null ? true : stu.intrestedSectorId == intrestedSectorId) && (languageID == null ? true : us.languageID == languageID) && (ua.userIsActive == "1")
                        select ua;
            var userList = users.Distinct().ToList().OrderByDescending(x => x.firstName).ToPagedList(pageIndex, dataCount);
            ViewBag.statusStd = statusStd;
            ViewBag.languageID = languageID;
            ViewBag.intrestedSectorId = intrestedSectorId;
            ViewBag.department = departmentId;
            return View(userList);
        }

        [UserCheckCompany]
        public ActionResult FindTeacherEmployee()
        {
            return View();
        }

        [UserCheckCompany]
        public ActionResult ResultTeacherEmployee(Nullable<int> languageID, string degree, string department, int? page)
        {
            int pageIndex = page ?? 1;
            int dataCount = 10;
            var departmentId = Convert.ToInt32(department);
            var dp = db.departments.Where(x => x.departmentsId == departmentId).FirstOrDefault();
           // var uni = db.universities.Where(x => x.universityId == universityId).FirstOrDefault();
            var users = from ua in db.user_account
                        join tea in db.user_teacher on ua.userAccountId equals tea.userAccountID
                        join edu in db.user_education on ua.userAccountId equals edu.userAccountId
                        join ue in db.user_experinence_detail on ua.userAccountId equals ue.userAccountID
                        join us in db.user_language_skill on ua.userAccountId equals us.userAccountID
                        where ((string.IsNullOrEmpty(department) ? true : edu.department == dp.departmentName)) && ((string.IsNullOrEmpty(degree) ? true : tea.degree == degree)) && (ua.userTypeID == 3) && (languageID == null ? true : us.languageID == languageID) && (ua.userIsActive == "1")
                        select ua;
            var userList = users.Distinct().ToList().OrderByDescending(x => x.firstName).ToPagedList(pageIndex, dataCount);
            ViewBag.degree = degree;
            ViewBag.department = departmentId;
            //ViewBag.universityId = universityId;
            ViewBag.languageID = languageID; 
            return View(userList);
        }

        [UserCheckCompany]
        [RestoreModelStateFromTempData]
        public ActionResult EditProfileCompany()
        {
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }
            if (TempData["Warning"] != null)
            {
                ViewBag.Warning = TempData["Warning"].ToString();
            }
            var companyInfos = db.company.Where(x => x.companyId == LoginStatus.Current.companyId).FirstOrDefault();
            if (companyInfos.companyLogoID != null)
            {
                var companyLogo = db.company_logo.Where(x => x.companyLogoId == companyInfos.companyLogoID).FirstOrDefault();
                ViewBag.companyLogo = companyLogo.companyLogo;
            }
            else
            {
                ViewBag.companyLogo = null;
            }
            return View(companyInfos);
        }

        [UserCheckCompany]
        [SetTempDataModelState]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyInfos(company comp)
        {
            if (ModelState.IsValid)
            {
                var selectedCompany = db.company.Where(x => x.companyId == comp.companyId).FirstOrDefault();
                if (selectedCompany.companyLogoID != null)
                {
                    var companyLogo = db.company_logo.Where(x => x.companyLogoId == selectedCompany.companyLogoID).FirstOrDefault();
                    ViewBag.companyLogo = companyLogo.companyLogo;
                }
                else
                {
                    ViewBag.companyLogo = null;
                }
                selectedCompany.companyName = comp.companyName;
                selectedCompany.companyEmail = comp.companyEmail;
                selectedCompany.businessID = comp.businessID;
                selectedCompany.foundationYear = comp.foundationYear;
                selectedCompany.webSiteUrl = comp.webSiteUrl;
                selectedCompany.companyPhone = comp.companyPhone;
                selectedCompany.companyAddress = comp.companyAddress;
                db.SaveChanges();
                TempData["Success"] = "Bilgileriniz Güncellendi.";
            }
            else
            {
                TempData["Warning"] = "Bilgiler güncellenemedi.";
            }
            return RedirectToAction("EditProfileCompany", "Company");
        }

        [UserCheckCompany]
        public ActionResult ChangePassword()
        {
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }
            if (TempData["Warning"] != null)
            {
                ViewBag.Warning = TempData["Warning"].ToString();
            }
            var companyInfos = db.company.Where(x => x.companyId == LoginStatus.Current.companyId).FirstOrDefault();
            if (companyInfos.companyLogoID != null)
            {
                var companyLogo = db.company_logo.Where(x => x.companyLogoId == companyInfos.companyLogoID).FirstOrDefault();
                ViewBag.companyLogo = companyLogo.companyLogo;
            }
            else
            {
                ViewBag.companyLogo = null;
            }
            var passwordModel = new ChangePasswordModel();
            if (companyInfos != null)
            {
                passwordModel.ID = companyInfos.companyId;
                var old = Crypt.Decrypt(companyInfos.companyPassword);
                passwordModel.OldPassword = old;
            }
            ViewBag.web = companyInfos.webSiteUrl;
            return View(passwordModel);
        }

        [UserCheckCompany]
        [HttpPost]
        [SetTempDataModelState]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel ps)
        {
            if (ModelState.IsValid)
            {
                var companyInfos = db.company.Where(x => x.companyId == LoginStatus.Current.companyId).FirstOrDefault();
                if(companyInfos != null)
                {
                    if(companyInfos.companyLogoID != null)
                    {
                        var companyLogo = db.company_logo.Where(x => x.companyLogoId == companyInfos.companyLogoID).FirstOrDefault();
                        ViewBag.companyLogo = companyLogo.companyLogo;
                    }
                    else
                    {
                        ViewBag.companyLogo = null;
                    }
                    ViewBag.web = companyInfos.webSiteUrl;
                    var oldPass = Crypt.Decrypt(companyInfos.companyPassword);
                    if (oldPass == ps.NewPassword)
                    {
                        ViewBag.Warning = "Yeni parola eskisi ile aynı olamaz. Tekrar deneyiniz.";
                    }
                    else
                    {
                        var changedPs = Crypt.Encrypt(ps.NewPassword);
                        companyInfos.companyPassword = changedPs;
                        ps.NewPassword = "";
                        ps.ConfirmPassword = "";
                        db.SaveChanges();
                        TempData["Success"] = "Parola başarı ile değiştirildi.";
                    }
                }
                else
                {
                    ViewBag.Warning = "Kullanıcı bulunamadı. Tekrar deneyiniz.";
                }
            }
            else
            {
                TempData["Warning"] = "Parola değiştirilemedi.";
            }
            return View(ps);
        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }

        [UserCheckCompany]
        public ActionResult ImageUploadEdit(HttpPostedFileBase file, company comp)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(file.InputStream))
            {
                bytes = br.ReadBytes(file.ContentLength);
            }
            var company = db.company.Where(x => x.companyId == comp.companyId).FirstOrDefault();
            var companyLogo = db.company_logo.Where(x => x.companyLogoId == company.companyLogoID).FirstOrDefault();
            if(companyLogo == null)
            {
                var logo = db.sp_InsertFirstLogo(bytes, company.companyId);
                db.SaveChanges();
            }
            else
            {
                companyLogo.companyLogo = bytes;
                db.SaveChanges();
            }
            return RedirectToAction("EditProfileCompany", "Company");
        }

        [UserCheckCompany]
        public ActionResult ShowCV(int id)
        {
            var user = db.user_account.Where(x => x.userAccountId == id).FirstOrDefault();
            return View(user);
        }
    }
}