using Dal_Model;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using KriptoBorsaResponse;
using System;

namespace newswebsite.Controllers
{
    public class homeController : Controller
    {
        Dal dal = new Dal();
        string con = "Server=80.93.212.227;Database=StajyerDB;User Id=userstajyer;Password=123456789;";
        void Borsa()
        {

            string api = new StreamReader((WebRequest.Create("https://api.coincap.io/v2/assets").GetResponse()).GetResponseStream()).ReadToEnd();
            BorsaResponse kriptoparaResponse = JsonConvert.DeserializeObject<BorsaResponse>(api);
            var data = kriptoparaResponse.Data;
            for (int i = 0; i < data.Length; i++)
            {
                data[i]["priceUsd"] = "$" + data[i]["priceUsd"].Substring(0, 6);
                data[i]["changePercent24Hr"] = data[i]["changePercent24Hr"].Substring(0, 6) + "%";
            }
            ViewBag.KriptoData = data;
        }
        public ActionResult Index(int page = 1)
        {
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds = dal.CommandExecuteDataSet($@"Declare @sayfaIndex int = {page}
            select * from(
            SELECT ROW_NUMBER() OVER(ORDER BY id DESC) AS sıra,
            id, haberAdi, haberResmi, haberIcerigi, haberTarihi
            From d_haberler) as d
            where d.sıra between(1 + ((@sayfaIndex - 1) * 14)) and(@sayfaIndex * 14)", dal.myConnection);
            ViewBag.Haber = ds.Tables[0];
            dal.ConClose(dal.myConnection);

            ViewBag.lastIndex = Convert.ToInt32(dal.CommandExecuteDataSet($@"
            Declare @ondalik decimal(35,6)
            Declare @tam int
            Select @ondalik = CONVERT(decimal, count(id)) / CONVERT(decimal, 14) From d_haberler
            Select @tam = count(id) / 14 From d_haberler
            if @ondalik > @tam
            begin
	            Set @tam = @tam + 1
            end

            Select @tam
            ", dal.myConnection).Tables[0].Rows[0][0]);
            ViewBag.index = page;

            int p = (page * 14) - 14;


            ViewBag.page = p;
            Borsa();
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds2 = dal.CommandExecuteDataSet("select top 3 * from d_haberler order by Hit desc", dal.myConnection);
            dal.ConClose(dal.myConnection);
            ViewBag.top3 = ds2.Tables[0];

            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds3 = dal.CommandExecuteDataSet($"select Facebook,Instagram,Twitter,YouTube from ai_sosyalMedya", dal.myConnection);
            ViewBag.Facebook = ds3.Tables[0].Rows[0]["Facebook"];
            ViewBag.Instagram = ds3.Tables[0].Rows[0]["Instagram"];
            ViewBag.Twitter = ds3.Tables[0].Rows[0]["Twitter"];
            ViewBag.YouTube = ds3.Tables[0].Rows[0]["YouTube"];
            dal.ConClose(dal.myConnection);
            return View();
        }
        public ActionResult habericerik(int id)
        {
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            dal.CommandExecuteNonQuery($"update d_haberler set Hit=+1 where id = {id}", dal.myConnection);
            var ds = dal.CommandExecuteDataSet($"select id,  haberIcerigi, haberAdi, haberResmi from d_haberler where id={id}", dal.myConnection);
            ViewBag.HaberIcerik = ds.Tables[0].Rows[0]["haberIcerigi"];
            ViewBag.HaberBaslik = ds.Tables[0].Rows[0]["haberAdi"];
            ViewBag.HaberResim = ds.Tables[0].Rows[0]["haberResmi"];
            dal.ConClose(dal.myConnection);
            Borsa();
            return View();
        }
        public ActionResult hakkimizda() 
        {
            Borsa();
            dal.myConnection = new SqlConnection(con);
            dal.ConOpen(dal.myConnection);
            var ds = dal.CommandExecuteDataSet($"select * from ai_hakkimizda", dal.myConnection);
            ViewBag.hakkimizda = ds.Tables[0].Rows[0]["hakkimizda"];
            dal.ConClose(dal.myConnection);
            return View();
        }
        public ActionResult iletisim()
        {

            Borsa();
            return View();
        }
        public JsonResult mailGonder()
        {
        var adSoyad = Request.Form["adSoyad"];
        var ePosta = Request.Form["ePosta"];
        var telefon = Request.Form["telefon"];
        var mail = Request.Unvalidated.Form["mail"];
        dal.myConnection = new SqlConnection(con);
        dal.ConOpen(dal.myConnection);
            dal.CommandExecuteNonQuery($"insert ai_eMail(adSoyad, ePosta, telefon, mail) values('{adSoyad}', '{ePosta}','{telefon}', '{mail}')", dal.myConnection);
            dal.ConClose(dal.myConnection);
            return new JsonResult()
        {
            Data = new
            {
                Message = "Başarıyla Eklendi.",
                Type = "Success",
                Location = "/admin/Index"
            }
            };
        }
    }
}