using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace jobFinding_Akdeniz
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Sirket Kayit",
                url: "sirket-kayit",
                defaults: new { controller = "Register", action = "SirketKayit" });

            routes.MapRoute(
               name: "Ogrenci Kayit",
               url: "ogrenci-kayit",
               defaults: new { controller = "Register", action = "OgrenciKayit" });

            routes.MapRoute(
               name: "Ogretmen Kayit",
               url: "ogretmen-kayit",
               defaults: new { controller = "Register", action = "OgretmenKayit" });

            routes.MapRoute(
               name: "İsveren Giris",
               url: "isveren-giris",
               defaults: new { controller = "Login", action = "SirketLogin" });

            routes.MapRoute(
               name: "Ogrenci Giris",
               url: "ogrenci-giris",
               defaults: new { controller = "Login", action = "OgrenciGirisi" });

            routes.MapRoute(
               name: "Ogretmen Giris",
               url: "ogretmen-giris",
               defaults: new { controller = "Login", action = "OgretmenGirisi" });


            routes.MapRoute(
               name: "İsveren İlanlar",
               url: "ilanlarim",
               defaults: new { controller = "Company", action = "CompanyPosts" });

            routes.MapRoute(
               name: "İsveren İlan Basvuranlar",
               url: "ilanlarim/basvuranlar/{id}",
               defaults: new { controller = "Company", action = "GetApplicants", id = UrlParameter.Optional });

            routes.MapRoute(
              name: "İsveren İlan Duzenleme",
              url: "ilanlarim/duzenle/{id}",
              defaults: new { controller = "Company", action = "Edit", id = UrlParameter.Optional });

            routes.MapRoute(
              name: "İsveren İlanEkleme",
              url: "ilan-ekle",
              defaults: new { controller = "Company", action = "PostAdd" });

            routes.MapRoute(
              name: "İsveren Eleman Arama",
              url: "eleman-arama/ogrenci",
              defaults: new { controller = "Company", action = "FindStudentEmployee" });

            routes.MapRoute(
              name: "İsveren Profil",
              url: "profili-duzenle",
              defaults: new { controller = "Company", action = "EditProfileCompany" });

            routes.MapRoute(
              name: "Ogrenci Profil",
              url: "profil-duzenle",
              defaults: new { controller = "Student", action = "EditProfileStudent" });

            routes.MapRoute(
             name: "Ogrenci Deneyim Ozgecmis",
             url: "deneyim-bilgileri",
             defaults: new { controller = "Student", action = "experienceInfos" });
            routes.MapRoute(
             name: "Ogrenci Ozgecmis",
             url: "ozgecmis-bilgileri",
             defaults: new { controller = "Student", action = "StudentInfos" });

            routes.MapRoute(
              name: "Ogretmen Profil",
              url: "profil-ayarları",
              defaults: new { controller = "Teacher", action = "EditProfileTeacher" });

            routes.MapRoute(
              name: "İsveren Sifre Değistir",
              url: "sifre-degistir",
              defaults: new { controller = "Company", action = "ChangePassword" });

            routes.MapRoute(
             name: "Ogrenci Sifre Değistir",
             url: "ogrenci-sifre-degistir",
             defaults: new { controller = "Student", action = "ChangePassword" });

            routes.MapRoute(
             name: "Ogretmen Sifre Değistir",
             url: "sifreni-degistir",
             defaults: new { controller = "Teacher", action = "ChangePassword" });

            routes.MapRoute(
             name: "Sifremi Unuttum",
             url: "sifremi-unuttum",
             defaults: new { controller = "ForgetPassword", action = "ForgetPasswordUser" });

            routes.MapRoute(
            name: "Sirket Sifremi Unuttum",
            url: "sifrenizi-mi-unuttunuz",
            defaults: new { controller = "ForgetPassword", action = "ForgetPasswordCompany" });

            routes.MapRoute(
             name: "Sifremi Unuttum Yenile",
             url: "sifreyi-sifirla/{id}",
             defaults: new { controller = "ForgetPassword", action = "ResetPasswordUser", id = UrlParameter.Optional });

            routes.MapRoute(
             name: "Sirket Sifremi Unuttum Yenile",
             url: "sifrenizi-sifirlayin/{id}",
             defaults: new { controller = "ForgetPassword", action = "ResetPasswordCompany", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
