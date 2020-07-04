using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using jobFinding_Akdeniz.Models;
using jobFinding_Akdeniz.Models.HelperModels;

namespace jobFinding_Akdeniz.Controllers
{
    public class ForgetPasswordController : Controller
    {
        private DBEntities db = new DBEntities(); 
        // GET: ForgetPassword
        public ActionResult ForgetPasswordUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPasswordUser(user_account user)
        {
            bool exist = isEmailExist(user.userEmail);
            if(exist == false)
            {
                ViewBag.Warning = "E-mail adresi bulunamadı.";
            }
            else
            {
                string token = Guid.NewGuid().ToString();
                var verifyUrl = "/sifreyi-sifirla/" + token;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                var selected = db.user_account.Where(x => x.userEmail == user.userEmail).FirstOrDefault();
                selected.resetPasswordCode = token;
                db.SaveChanges();
                var subject = "Parola Yenileme Talebi";
                var body = "Merhaba " + selected.firstName + ", <br/> Hesabınız için parola yenileme talebinde bulundunuz. Aşağıdaki linke tıklayarak parolanızı yenileyebilirsiniz." + "<br/><br/><a href='" + link + "'>Buraya Tıklayınız</a> <br/><br/>" + "Teşekkürler";
                sendEmail(selected.userEmail, body, subject);
                ViewBag.Success = "Parola yenileme linki e-adresinize gönderildi.";
            }
            return View();
        }

        public ActionResult ForgetPasswordCompany()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPasswordCompany(company company)
        {
            bool exist = isEmailExistCompany(company.companyEmail);
            if (exist == false)
            {
                ViewBag.Warning = "E-mail adresi bulunamadı.";
            }
            else
            {
                string token = Guid.NewGuid().ToString();
                var verifyUrl = "/sifrenizi-sifirlayin/" + token;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                var selected = db.company.Where(x => x.companyEmail == company.companyEmail).FirstOrDefault();
                selected.resetPasswordCode = token;
                db.SaveChanges();
                var subject = "Parola Yenileme Talebi";
                var body = "Merhaba " + selected.companyName + ", <br/> Hesabınız için parola yenileme talebinde bulundunuz. Aşağıdaki linke tıklayarak parolanızı yenileyebilirsiniz." + "<br/><br/><a href='" + link + "'>Buraya Tıklayınız</a> <br/><br/>" + "Teşekkürler";
                sendEmail(selected.companyEmail, body, subject);
                ViewBag.Success = "Parola yenileme linki e-adresinize gönderildi.";
            }
            return View();
        }

        [NonAction]
        public void sendEmail(string email, string body, string subject)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("mail-address");
            mail.To.Add(email);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;
            using (SmtpClient sc = new SmtpClient())
            {
                sc.Port = 587;
                sc.Host = "smtp.live.com";
                sc.EnableSsl = true;
                sc.Credentials = new NetworkCredential("mail-address", "password");
                sc.UseDefaultCredentials = false;
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.Send(mail);

            }


        }

        public bool isEmailExist(string email)
        {
            bool exist = true;
            var user = db.user_account.Where(x => x.userEmail == email).FirstOrDefault();
            if (user == null)
            {
                exist = false;
            }
            return exist;
        }

        public bool isEmailExistCompany(string email)
        {
            bool exist = true;
            var user = db.company.Where(x => x.companyEmail == email).FirstOrDefault();
            if (user == null)
            {
                exist = false;
            }
            return exist;
        }

        public ActionResult ResetPasswordUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            var user = db.user_account.Where(x => x.resetPasswordCode == id).FirstOrDefault();
            if (user != null)
            {
                resetPassword us = new resetPassword();
                us.ResetCode = id;
                return View(us);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPasswordUser(resetPassword reset)
        {
            if (ModelState.IsValid)
            {
                var user = db.user_account.Where(x => x.resetPasswordCode == reset.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    if (user.userPassword == reset.NewPassword)
                    {
                        ViewBag.Warning = "Yeni parola eskisi ile aynı olamaz. Tekrar deneyiniz.";
                    }
                    else
                    {
                        var encrypted = Crypt.Encrypt(reset.NewPassword);
                        user.userPassword = encrypted;
                        user.resetPasswordCode = "";
                        db.SaveChanges();
                        ViewBag.Success = "Parola başarıyla değiştirildi.";
                    }

                }
            }
            else
            {
                ViewBag.Warning = "Parola değiştirilemedi. Tekrar deneyiniz.";
            }
            return View(reset);
        }

        public ActionResult ResetPasswordCompany(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            var user = db.company.Where(x => x.resetPasswordCode == id).FirstOrDefault();
            if (user != null)
            {
                resetPassword us = new resetPassword();
                us.ResetCode = id;
                return View(us);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPasswordCompany(resetPassword reset)
        {
            if (ModelState.IsValid)
            {
                var user = db.company.Where(x => x.resetPasswordCode == reset.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    if (user.companyPassword == reset.NewPassword)
                    {
                        ViewBag.Warning = "Yeni parola eskisi ile aynı olamaz. Tekrar deneyiniz.";
                    }
                    else
                    {
                        var encrypted = Crypt.Encrypt(reset.NewPassword);
                        user.companyPassword = encrypted;
                        user.resetPasswordCode = "";
                        db.SaveChanges();
                        ViewBag.Success = "Parola başarıyla değiştirildi.";
                    }

                }
            }
            else
            {
                ViewBag.Warning = "Parola değiştirilemedi. Tekrar deneyiniz.";
            }
            return View(reset);
        }
    }
}