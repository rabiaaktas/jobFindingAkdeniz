using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jobFinding_Akdeniz.Models;
using jobFinding_Akdeniz.Models.HelperModels;

namespace jobFinding_Akdeniz.Controllers
{
    public class RegisterController : Controller
    {
        private DBEntities db = new DBEntities();
        // GET: Register
        public ActionResult SirketKayit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SirketKayit(CompanyRegisterModel com)
        {
            if (ModelState.IsValid)
            {
                var company = new company();
                company.businessID = com.businessID;
                company.companyName = com.companyName;
                company.companyEmail = com.companyEmail;
                company.companyAddress = com.companyAddress;
                company.companyPhone = com.companyPhone;
                var pass = Crypt.Encrypt(com.NewPassword);
                company.companyPassword = pass;
                company.registrationCompanyDate = DateTime.Now;
                company.isCompanyActive = "1";
                if (checkExistence(company))
                {
                    ViewBag.Warning = "Eklemeye çalıştığınız şirket zaten var.";
                }
                else
                {
                    db.company.Add(company);
                    db.SaveChanges();
                    ViewBag.Success = "Kayıt başarı ile oluşturuldu.";
                }

            }
            else
            {
                ViewBag.Warning = "Kayıt oluşturulamadı.";
            }
            return View();
        }

        public Boolean checkExistence(company com)
        {
            var exist = false;
            var company = db.company.Where(x => x.companyName == com.companyName && x.companyEmail == com.companyEmail && x.businessID == com.businessID).FirstOrDefault();
            if (company != null)
            {
                exist = true;
            }
            return exist;
        }

        public ActionResult OgrenciKayit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OgrenciKayit(StudentRegisterModel std)
        {
            if (ModelState.IsValid)
            {
                if (isEmailExist(std.userEmail))
                {
                    ViewBag.Warning = "Bu e-mail adresine kayıtlı kullanıcı zaten var.";
                }
                else
                {
                    var GANO = Convert.ToDouble(std.GANO);
                    var GANOINT = Convert.ToInt32(std.GANOINT);
                    var pass = Crypt.Encrypt(std.password);
                    var status = db.sp_InsertStudent(std.userEmail, std.firstName, std.lastName, pass, "1", "0", std.statusStd, std.degreeName, std.universityName, std.startingDate, std.endingDate, GANO, GANOINT, std.department);
                    Console.Write(status);
                    db.SaveChanges();
                    ViewBag.Success = "Kayıt başarı ile oluşturuldu.";
                }

            }
            else
            {
                ViewBag.Warning = "Kayıt oluşturulamadı.";
            }
            return View();
        }

        public bool isEmailExist(string email)
        {
            var exist = false;
            var user = db.user_account.Where(x => x.userEmail == email).FirstOrDefault();
            if (user != null)
            {
                exist = true;
            }
            return exist;
        }

        public ActionResult OgretmenKayit()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OgretmenKayit(TeacherRegisterModel teach)
        {
            if (ModelState.IsValid)
            {
                if (isEmailExist(teach.userEmail))
                {
                    ViewBag.Warning = "Bu e-mail adresine kayıtlı kullanıcı zaten var.";
                }
                else
                {
                    
                    var pass = Crypt.Encrypt(teach.password);
                    var status = db.sp_InsertTeacher(teach.userEmail, teach.firstName, teach.lastName, pass, "0", "0", teach.degree, teach.universityName, teach.description);
                    Console.Write(status);
                    db.SaveChanges();
                    ViewBag.Success = "Kayıt başarı ile oluşturuldu. Hesabınız aktive edildikten sonra kullanmaya başlayabilirsiniz.";
                }

            }
            else
            {
                ViewBag.Warning = "Kayıt oluşturulamadı.";
            }
            return View();
        }
    }
}