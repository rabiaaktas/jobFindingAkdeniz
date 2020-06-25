using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jobFinding_Akdeniz.Models;
using jobFinding_Akdeniz.Models.HelperModels;
using static jobFinding_Akdeniz.States;

namespace jobFinding_Akdeniz.Controllers
{
    public class StudentController : Controller
    {

        private DBEntities db = new DBEntities();
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        [UserCheckStudent]
        [RestoreModelStateFromTempData]
        public ActionResult EditProfileStudent()
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

        [UserCheckStudent]
        [SetTempDataModelState]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfileStudent(user_account user)
        {
            if (ModelState.IsValid)
            {
                var selectedStudent = db.user_account.Where(x => x.userAccountId == user.userAccountId).FirstOrDefault();
                if (selectedStudent.userImageID != null)
                {
                    var image = db.user_image.Where(x => x.userImageId == selectedStudent.userImageID).FirstOrDefault();
                    ViewBag.image = image.userImage;
                }
                else
                {
                    ViewBag.image = null;
                }
                selectedStudent.firstName = user.firstName;
                selectedStudent.lastName = user.lastName;
                selectedStudent.userBday = user.userBday;
                selectedStudent.userPhone = user.userPhone;
                selectedStudent.userAddress = user.userAddress;
                if(user.user_student.intrestedSectorId != null)
                {
                    selectedStudent.user_student.intrestedSectorId = user.user_student.intrestedSectorId;
                }
                selectedStudent.user_student.statusStd = user.user_student.statusStd;
                db.SaveChanges();
                TempData["Success"] = "Bilgileriniz Güncellendi.";
            }
            else
            {
                TempData["Warning"] = "Bilgiler güncellenemedi.";
            }
            return RedirectToAction("EditProfileStudent", "Student");
        }

        [UserCheckStudent]
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

        [UserCheckStudent]
        [HttpPost]
        [SetTempDataModelState]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel ps)
        {
            if (ModelState.IsValid)
            {
                var userInfos = db.user_account.Where(x => x.userAccountId == LoginStatus.Current.UserId).FirstOrDefault();
                if (userInfos != null)
                {
                    ViewBag.email = userInfos.userEmail;
                    if (userInfos.userImageID != null)
                    {
                        var image = db.user_image.Where(x => x.userImageId == userInfos.userImageID).FirstOrDefault();
                        ViewBag.image = image.userImage;
                    }
                    else
                    {
                        ViewBag.image = null;
                    }
                    var oldPass = Crypt.Decrypt(userInfos.userPassword);
                    if (oldPass == ps.NewPassword)
                    {
                        ViewBag.Warning = "Yeni parola eskisi ile aynı olamaz. Tekrar deneyiniz.";
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

        [UserCheckStudent]
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
                var logo = db.sp_InsertFirstImageStd(bytes, us.userAccountId);
                db.SaveChanges();
            }
            else
            {
                userImage.userImage = bytes;
                db.SaveChanges();
            }
            return RedirectToAction("EditProfileStudent", "Student");
        }

        [UserCheckStudent]
        public ActionResult StudentInfos()
        {
            var info = db.user_account.Where(x => x.userAccountId == LoginStatus.Current.UserId).FirstOrDefault();
            return View(info);
        }

        [UserCheckStudent]
        public ActionResult experienceInfos()
        {
            var info = db.user_experinence_detail.Where(x => x.userAccountID == LoginStatus.Current.UserId).ToList();
            return View(info);
        }

        [UserCheckStudent]  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEducation(int educationId, string degreeName, string universityName, string department, string startingDate, string endingDate, string GANOINT, string GANO)
        {
            var userEducation = db.user_education.Where(x => x.userAccountId == LoginStatus.Current.UserId && x.educationId == educationId).FirstOrDefault();
            if(userEducation != null)
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
                if(GANO != "")
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
            return RedirectToAction("StudentInfos", "Student");
        }

        [UserCheckStudent]
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
            return RedirectToAction("StudentInfos", "Student");
        }


    }
}