using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jobFinding_Akdeniz.Models;
using jobFinding_Akdeniz.Models.HelperModels;

namespace jobFinding_Akdeniz.Controllers
{
    public class LoginController : Controller
    {
        private DBEntities db = new DBEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SirketLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SirketLogin(company company)
        {
            //var password = Crypt.Encrypt(user.userPassword);
            var data = db.company.Where(x => x.companyEmail == company.companyEmail && x.companyPassword == company.companyPassword && x.isCompanyActive == "1").FirstOrDefault();
            if (data != null)
            {
                LoginStatus.Current.IsLogin = true;
                LoginStatus.Current.Name = data.companyName;
                LoginStatus.Current.companyId = data.companyId;
                LoginStatus.Current.IsActive = data.isCompanyActive;
                LoginStatus.Current.isCompany = true;
                var companyLog = db.company_log.Where(x => x.companyID == data.companyId).FirstOrDefault();
                if (companyLog == null)
                {
                    company_log log = new company_log();
                    log.companyID = data.companyId;
                    log.loginDate = DateTime.Now;
                    string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (ipAddress == "" || ipAddress == null)
                    {
                        ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                    }
                    log.loginIp = ipAddress;
                    db.company_log.Add(log);
                    db.SaveChanges();
                }
                else
                {
                    companyLog.loginDate = DateTime.Now;
                    string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (ipAddress == "" || ipAddress == null)
                    {
                        ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                    }
                    companyLog.loginIp = ipAddress;
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Warning = "Kullanıcı adı ve ya şifre hatalı.";
            }
            return View();
        }
    }
}