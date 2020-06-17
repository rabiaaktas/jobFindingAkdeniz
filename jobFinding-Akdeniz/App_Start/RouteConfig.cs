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
              name: "İsveren İlan Ekleme",
              url: "ilan-ekle",
              defaults: new { controller = "Company", action = "PostAdd" });

            routes.MapRoute(
              name: "İsveren Eleman Arama",
              url: "eleman-arama/ogrenci",
              defaults: new { controller = "Company", action = "FindStudentEmployee" });

            routes.MapRoute(
              name: "İsveren Profil",
              url: "profili-duzenle",
              defaults: new { controller = "Company", action = "EditProfile" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
