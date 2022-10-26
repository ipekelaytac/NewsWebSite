using System;
using Dal_Model;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using newswebsite.security;
using System.IO;

namespace newswebsite.Controllers
{
    public class adminController : Controller
    {
        Dal dal = new Dal();
        string con = "Server=80.93.212.227;Database=StajyerDB;User Id=userstajyer;Password=123456789;";
        public ActionResult login()
        {
            return View();
        }
        [CustomAuthorize]
        public ActionResult Index()
        {
            return View();
        }
        [CustomAuthorize]
        public ActionResult haberEkle()
        {
            return View();
        }
        [CustomAuthorize]
        public ActionResult Hakkimizda()
        {
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds = dal.CommandExecuteDataSet($"select hakkimizda from ai_hakkimizda", dal.myConnection);
            ViewBag.hakkimizda = ds.Tables[0].Rows[0]["hakkimizda"];
            dal.ConClose(dal.myConnection);
            return View();
        }
        public ActionResult hakkimizdaDuzenle()
        {
            var hakkimizda = Request.Unvalidated.Form["hakkimizda"];
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            dal.CommandExecuteNonQuery($" UPDATE ai_hakkimizda SET hakkimizda='{hakkimizda}'", dal.myConnection);
            dal.ConClose(dal.myConnection);
            return Redirect("/admin/Hakkimizda");
        }


        [CustomAuthorize]
        public ActionResult eMail()
        {
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds = dal.CommandExecuteDataSet("select * from ai_eMail ORDER BY id DESC", dal.myConnection);
            ViewBag.Veriler = ds.Tables[0];
            dal.ConClose(dal.myConnection);
            return View();
        }
        [CustomAuthorize]
        public ActionResult eMailGoruntule(int id)
        {
            ViewBag.ID = id;
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds = dal.CommandExecuteDataSet($"select adSoyad,ePosta,telefon,mail from ai_eMail where id={id}", dal.myConnection);
            dal.ConClose(dal.myConnection);
            ViewBag.adSoyad = ds.Tables[0].Rows[0]["adSoyad"];
            ViewBag.ePosta = ds.Tables[0].Rows[0]["ePosta"];
            ViewBag.telefon = ds.Tables[0].Rows[0]["telefon"];
            ViewBag.mail = ds.Tables[0].Rows[0]["mail"];
            return View();
        }
        public ActionResult eMailsil(int id)
        {
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds = dal.CommandExecuteDataSet($"delete from ai_eMail where id={id}", dal.myConnection);
            dal.ConClose(dal.myConnection);
            return Redirect("/admin/eMail");
        }
        [CustomAuthorize]
        public ActionResult sosyalMedya()
        {
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds = dal.CommandExecuteDataSet($"select Facebook,Instagram,Twitter,YouTube from ai_sosyalMedya", dal.myConnection);
            ViewBag.Facebook = ds.Tables[0].Rows[0]["Facebook"];
            ViewBag.Instagram = ds.Tables[0].Rows[0]["Instagram"];
            ViewBag.Twitter = ds.Tables[0].Rows[0]["Twitter"];
            ViewBag.YouTube = ds.Tables[0].Rows[0]["YouTube"];
            dal.ConClose(dal.myConnection);
            return View();
        }
        public ActionResult link()
        {
            var Facebook = Request.Form["Facebook"];
            var Instagram = Request.Unvalidated.Form["Instagram"];
            var Twitter = Request.Form["Twitter"];
            var YouTube = Request.Form["YouTube"];
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            dal.CommandExecuteNonQuery($" UPDATE ai_sosyalMedya SET  Facebook='{Facebook}' ,Instagram='{Instagram}', Twitter='{Twitter}',YouTube='{YouTube}'", dal.myConnection);
            dal.ConClose(dal.myConnection);
            return Redirect("/admin/sosyalMedya");
        }
        [CustomAuthorize]
        public ActionResult tables()
        {
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds = dal.CommandExecuteDataSet("select id,haberAdi from d_haberler ORDER BY id DESC", dal.myConnection);
            ViewBag.Veriler = ds.Tables[0];
            dal.ConClose(dal.myConnection);
            return View();
        }
        public JsonResult giriss(string eMailLogin, string sifreLogin)
        {
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds = dal.CommandExecuteDataSet($"select * from ai_admin where eMailLogin='{eMailLogin}' and sifreLogin='{sifreLogin}'", dal.myConnection);
            dal.ConClose(dal.myConnection);
            if (ds.Tables[0].Rows.Count == 0)
            {

                return new JsonResult()
                {
                    Data = new
                    {
                        Message = "E-Mail  Veya Şifre yanlış",
                        Type = "error"
                    }
                };
            }
            else
            {
                Session["login"] = eMailLogin;
                Session["eMailLogin"] = ds.Tables[0].Rows[0]["eMailLogin"];


                return new JsonResult()
                {
                    Data = new
                    {
                        Message = "Giriş başarılı",
                        Type = "Success",
                        Location = "/admin/Index"

                    }
                };
            }
        }
        public ActionResult sil(int id)
        {
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds = dal.CommandExecuteDataSet($"delete from d_haberler where id={id}", dal.myConnection);
            dal.ConClose(dal.myConnection);
            return Redirect("/admin/tables");
        }
        public ActionResult haberDuzenle(int id)
        {
            ViewBag.ID = id;
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds = dal.CommandExecuteDataSet($"select haberAdi,haberIcerigi,haberResmi from d_haberler where id={id}", dal.myConnection);
            ViewBag.haberAdi = ds.Tables[0].Rows[0]["haberAdi"];
            ViewBag.haberIcerigi = ds.Tables[0].Rows[0]["haberIcerigi"];
            ViewBag.haberResmi = ds.Tables[0].Rows[0]["haberResmi"];
            dal.ConClose(dal.myConnection);
            return View();
        }
        [CustomAuthorize]

        public ActionResult duzenle(int id)
        {
            var haberAdi = Request.Form["haberAdi"];
            var haberIcerigi = Request.Unvalidated.Form["haberIcerigi"];
            var haberResmi = Request.Files[0];
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            if (haberResmi != null)
            {
                var path = Path.Combine(Server.MapPath("/Content/haberResmi/"), haberResmi.FileName);
                haberResmi.SaveAs(path);
                dal.CommandExecuteNonQuery($" UPDATE d_haberler SET  haberAdi='{haberAdi}' ,haberIcerigi='{haberIcerigi}', haberResmi='{"/Content/haberResmi/" + haberResmi.FileName}'  WHERE id={id}", dal.myConnection);
            }
            else
            {
                dal.CommandExecuteNonQuery($" UPDATE d_haberler SET  haberAdi='{haberAdi}' ,haberIcerigi='{haberIcerigi}'  WHERE id={id}", dal.myConnection);

            }
            dal.ConClose(dal.myConnection);
            return Redirect("/admin/haberDuzenle/" + id);
        }
        public  ActionResult ekle()
        {
            var haberAdi = Request.Form["haberAdi"];
            var haberIcerigi = Request.Unvalidated.Form["haberIcerigi"];
            var haberResmi = Request.Files[0];
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            if (haberResmi != null)
            {
                var path = Path.Combine(Server.MapPath("/Content/haberResmi/"), haberResmi.FileName);
                haberResmi.SaveAs(path);
                dal.CommandExecuteNonQuery($"insert d_haberler(haberAdi, haberIcerigi, haberResmi, haberTarihi) values('{haberAdi}','{haberIcerigi}','{"/Content/haberResmi/" + haberResmi.FileName}', getdate())", dal.myConnection);
                //ResizeSettings resizeSetting = new ResizeSettings
                //{
                //    Width = 1200,
                //    Height = 280,
                //    Format = fi.Extension.ToString().Substring(1)
                //};
                //ImageBuilder.Current.Build(path, path, resizeSetting);
            }
            else
            {
                dal.CommandExecuteNonQuery($"insert d_haberler(haberAdi, haberIcerigi, haberTarihi) values('{haberAdi}','{haberIcerigi}',getdate())", dal.myConnection);
            }
            dal.ConClose(dal.myConnection);
   return Redirect("/admin/tables");
        }

        public ActionResult cikis()
        {
            Session.RemoveAll();

            return Redirect("/admin/login");

        }
    }
}