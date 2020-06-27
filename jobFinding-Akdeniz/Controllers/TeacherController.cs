using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jobFinding_Akdeniz.Models;
using jobFinding_Akdeniz.Models.HelperModels;
using PagedList;
using static jobFinding_Akdeniz.States;

namespace jobFinding_Akdeniz.Controllers
{
    public class TeacherController : Controller
    {
        private DBEntities db = new DBEntities();
        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }

        [UserCheckTeacher]
        [RestoreModelStateFromTempData]
        public ActionResult EditProfileTeacher()
        {
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }
            if (TempData["Warning"] != null)
            {
                ViewBag.Warning = TempData["Warning"].ToString();
            }
            var userInfos = db.user_account.Where(x => x.userAccountId == LoginStatus.Current.UserId).FirstOrDefault();
            if (userInfos.userImageID != null)
            {
                var image = db.user_image.Where(x => x.userImageId == userInfos.userImageID).FirstOrDefault();
                ViewBag.image = image.userImage;
            }
            else
            {
                ViewBag.image = null;
            }
            return View(userInfos);
        }

        [UserCheckTeacher]
        [SetTempDataModelState]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfileTeacher(user_account user)
        {
            if (ModelState.IsValid)
            {
                var selectedTeacher = db.user_account.Where(x => x.userAccountId == user.userAccountId).FirstOrDefault();
                if (selectedTeacher.userImageID != null)
                {
                    var image = db.user_image.Where(x => x.userImageId == selectedTeacher.userImageID).FirstOrDefault();
                    ViewBag.image = image.userImage;
                }
                else
                {
                    ViewBag.image = null;
                }
                selectedTeacher.firstName = user.firstName;
                selectedTeacher.lastName = user.lastName;
                selectedTeacher.userBday = user.userBday;
                selectedTeacher.userPhone = user.userPhone;
                selectedTeacher.userAddress = user.userAddress;
                selectedTeacher.user_teacher.interestAreas = user.user_teacher.interestAreas;
                db.SaveChanges();
                TempData["Success"] = "Bilgileriniz Güncellendi.";
            }
            else
            {
                TempData["Warning"] = "Bilgiler güncellenemedi.";
            }
            return RedirectToAction("EditProfileTeacher", "Teacher");
        }

        [UserCheckTeacher]
        [RestoreModelStateFromTempData]
        public ActionResult ChangePassword()
        {
            if (TempData["SuccessParola"] != null)
            {
                ViewBag.Success = TempData["SuccessParola"].ToString();
            }
            if (TempData["WarningParola"] != null)
            {
                ViewBag.Warning = TempData["WarningParola"].ToString();
            }
            var userInfo = db.user_account.Where(x => x.userAccountId == LoginStatus.Current.UserId).FirstOrDefault();
            if (userInfo.userImageID != null)
            {
                var image = db.user_image.Where(x => x.userImageId == userInfo.userImageID).FirstOrDefault();
                ViewBag.image = image.userImage;
            }
            else
            {
                ViewBag.image = null;
            }
            var passwordModel = new ChangePasswordModel();
            if (userInfo != null)
            {
                passwordModel.ID = userInfo.userAccountId;
                var old = Crypt.Decrypt(userInfo.userPassword);
                passwordModel.OldPassword = old;
            }
            ViewBag.email = userInfo.userEmail;
            return View(passwordModel);
        }

        [UserCheckTeacher]
        [HttpPost]
        [SetTempDataModelState]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel ps)
        {
            if (ModelState.IsValid)
            {
                var userInfos = db.user_account.Where(x => x.userAccountId == LoginStatus.Current.UserId).FirstOrDefault();
                if (userInfos.userImageID != null)
                {
                    var image = db.user_image.Where(x => x.userImageId == userInfos.userImageID).FirstOrDefault();
                    ViewBag.image = image.userImage;
                }
                else
                {
                    ViewBag.image = null;
                }
                if (userInfos != null)
                {
                    ViewBag.email = userInfos.userEmail;
                    ViewBag.image = userInfos.user_image.userImage;
                    var oldPass = Crypt.Decrypt(userInfos.userPassword);
                    if (oldPass == ps.NewPassword)
                    {
                        TempData["Cant"] = "Yeni parola eskisi ile aynı olamaz. Tekrar deneyiniz.";
                    }
                    else
                    {
                        var changedPs = Crypt.Encrypt(ps.NewPassword);
                        userInfos.userPassword = changedPs;
                        ps.NewPassword = "";
                        ps.ConfirmPassword = "";
                        db.SaveChanges();
                        TempData["SuccessParola"] = "Parola başarı ile değiştirildi.";
                    }
                }
                else
                {
                    ViewBag.Warning = "Kullanıcı bulunamadı. Tekrar deneyiniz.";
                }
            }
            else
            {
                TempData["WarningParola"] = "Parola değiştirilemedi.";
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

        [UserCheckTeacher]
        public ActionResult ImageUploadEdit(HttpPostedFileBase file, user_account user)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(file.InputStream))
            {
                bytes = br.ReadBytes(file.ContentLength);
            }
            var us = db.user_account.Where(x => x.userAccountId == user.userAccountId).FirstOrDefault();
            var userImage = db.user_image.Where(x => x.userImageId == us.userImageID).FirstOrDefault();
            if (userImage == null)
            {
                var logo = db.sp_InsertFirstImageTeac(bytes, us.userAccountId);
                db.SaveChanges();
            }
            else
            {
                userImage.userImage = bytes;
                db.SaveChanges();
            }
            return RedirectToAction("EditProfileTeacher", "Teacher");
        }

        [UserCheckTeacher]
        public ActionResult TeacherInfos()
        {
            var info = db.user_account.Where(x => x.userAccountId == LoginStatus.Current.UserId).FirstOrDefault();
            return View(info);
        }

        [UserCheckTeacher]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEducation(int educationId, string degreeName, string universityName, string department, string startingDate, string endingDate, string GANOINT, string GANO)
        {
            var userEducation = db.user_education.Where(x => x.userAccountId == LoginStatus.Current.UserId && x.educationId == educationId).FirstOrDefault();
            if (userEducation != null)
            {
                userEducation.degreeName = degreeName;
                userEducation.universityName = universityName;
                userEducation.department = department;
                userEducation.startingDate = startingDate;
                userEducation.endingDate = endingDate;
                if (GANOINT != "")
                {
                    var gano = Convert.ToInt32(GANOINT);
                    userEducation.GANOINT = gano;
                }
                if (GANO != "")
                {
                    var ganostr = GANO.Replace(".", ",");
                    var gano = Convert.ToDouble(ganostr);
                    userEducation.GANO = gano;
                }
                db.SaveChanges();
            }
            else
            {
                ViewBag.Warning = "Bilgi bulunamadı.";
            }
            return RedirectToAction("TeacherInfos", "Teacher");
        }

        [UserCheckTeacher]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEducation(string degreeName, string universityName, string department, string startingDate, string endingDate, string GANOINT, string GANO)
        {
            int gano = 0;
            double ganoDouble = 0;
            if (GANOINT != "")
            {
                gano = Convert.ToInt32(GANOINT);
            }
            if (GANO != "")
            {
                var ganostr = GANO.Replace(".", ",");
                ganoDouble = Convert.ToDouble(ganostr);
            }
            var education = db.sp_EducationAdd(LoginStatus.Current.UserId, degreeName, universityName, startingDate, endingDate, ganoDouble, gano, department);

            db.SaveChanges();
            return RedirectToAction("TeacherInfos", "Teacher");
        }

        [UserCheckTeacher]
        [HttpPost]
        public JsonResult DeleteEducation(int id)
        {
            var education = db.user_education.Where(x => x.educationId == id).FirstOrDefault();
            db.user_education.Remove(education);
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        [UserCheckTeacher]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditExperience(int experienceId, string companyName, string jobTitle, string startDate, string endDate, string description)
        {
            Nullable<DateTime> start = null;
            Nullable<DateTime> end = null;
            start = new DateTime();
            end = new DateTime();
            if (startDate != "")
            {
                start = Convert.ToDateTime(startDate);
            }
            if (endDate != "")
            {
                end = Convert.ToDateTime(endDate);
            }
            var userExperience = db.user_experinence_detail.Where(x => x.userAccountID == LoginStatus.Current.UserId && x.experienceId == experienceId).FirstOrDefault();
            if (userExperience != null)
            {
                userExperience.companyName = companyName;
                userExperience.jobTitle = jobTitle;
                userExperience.startDate = start;
                userExperience.endDate = end;
                userExperience.description = description;
                db.SaveChanges();
            }
            else
            {
                ViewBag.Warning = "Bilgi bulunamadı.";
            }
            return RedirectToAction("TeacherInfos", "Teacher");
        }

        [UserCheckTeacher]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExperience(string companyName, string jobTitle, string startDate, string endDate, string description)
        {
            Nullable<DateTime> start = null;
            Nullable<DateTime> end = null;
            start = new DateTime();
            end = new DateTime();
            if (startDate != "")
            {
                start = Convert.ToDateTime(startDate);
            }
            if (endDate != "")
            {
                end = Convert.ToDateTime(endDate);
            }
            var experience = db.sp_InsertExperience(LoginStatus.Current.UserId, start, end, jobTitle, companyName, description);
            db.SaveChanges();
            return RedirectToAction("TeacherInfos", "Teacher");
        }

        [UserCheckTeacher]
        [HttpPost]
        public JsonResult DeleteExperience(int id)
        {
            var experience = db.user_experinence_detail.Where(x => x.experienceId == id).FirstOrDefault();
            db.user_experinence_detail.Remove(experience);
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        [UserCheckTeacher]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLanguage(int skillId, int languageID, string level)
        {
            var userSkill = db.user_language_skill.Where(x => x.userAccountID == LoginStatus.Current.UserId && x.skillId == skillId).FirstOrDefault();
            if (userSkill != null)
            {
                userSkill.languageID = languageID;
                userSkill.level = level;
                db.SaveChanges();
            }
            else
            {
                ViewBag.Warning = "Bilgi bulunamadı.";
            }
            return RedirectToAction("TeacherInfos", "Teacher");
        }

        [UserCheckTeacher]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLanguage(string languageID, string level)
        {
            var id = Convert.ToInt32(languageID);
            var language = db.sp_InsertLanguage(LoginStatus.Current.UserId, id, level);
            db.SaveChanges();
            return RedirectToAction("TeacherInfos", "Teacher");
        }

        [UserCheckTeacher]
        [HttpPost]
        public JsonResult DeleteLanguage(int id)
        {
            var languageSkill = db.user_language_skill.Where(x => x.skillId == id).FirstOrDefault();
            db.user_language_skill.Remove(languageSkill);
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        [UserCheckTeacher]
        public ActionResult AppliedJobs(int? page)
        {
            int pageIndex = page ?? 1;
            int dataCount = 10;
            var myApplies = db.job_post_activity.Where(x => x.userAccountID == LoginStatus.Current.UserId).ToList();
            var applied = myApplies.OrderByDescending(x => x.applyDate).ToPagedList(pageIndex, dataCount);
            return View(applied);
        }

    }
}