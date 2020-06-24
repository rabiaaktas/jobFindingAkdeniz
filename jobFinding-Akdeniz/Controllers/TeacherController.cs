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
    }
}