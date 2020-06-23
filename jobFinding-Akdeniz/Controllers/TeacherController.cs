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
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }
            if (TempData["Warning"] != null)
            {
                ViewBag.Warning = TempData["Warning"].ToString();
            }
            var userInfo = db.user_account.Where(x => x.userAccountId == LoginStatus.Current.UserId).FirstOrDefault();
            var passwordModel = new ChangePasswordModel();
            if (userInfo != null)
            {
                passwordModel.ID = userInfo.userAccountId;
                var old = Crypt.Decrypt(userInfo.userPassword);
                passwordModel.OldPassword = old;
            }
            ViewBag.email = userInfo.userEmail;
            ViewBag.image = userInfo.userImage;
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
                if (userInfos != null)
                {
                    ViewBag.email = userInfos.userEmail;
                    ViewBag.image = userInfos.userImage;
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

        [UserCheckTeacher]
        public ActionResult ImageUploadEdit(HttpPostedFileBase file, user_account user)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(file.InputStream))
            {
                bytes = br.ReadBytes(file.ContentLength);
            }
            var userImage = db.user_account.Where(x => x.userAccountId == user.userAccountId).FirstOrDefault();
            userImage.userImage = bytes;
            db.SaveChanges();
            return RedirectToAction("EditProfileTeacher", "Teacher");
        }
    }
}