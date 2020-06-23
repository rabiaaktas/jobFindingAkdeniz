﻿using System;
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
                selectedStudent.firstName = user.firstName;
                selectedStudent.lastName = user.lastName;
                selectedStudent.userBday = user.userBday;
                selectedStudent.userPhone = user.userPhone;
                selectedStudent.userAddress = user.userAddress;
                selectedStudent.user_student.intrestedSectorId = user.user_student.intrestedSectorId;
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

        [UserCheckStudent]
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
            return RedirectToAction("EditProfileStudent", "Student");
        }
    }
}