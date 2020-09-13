﻿using IIIProject_travel.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace IIIProject_travel.Controllers
{
    public class EatController : Controller
    {
        // GET: Eat
        public ActionResult EatIndex(string msg)
        {
            string Search = ",所有,全部," + msg;
            return View((object)Search);
        }

        [HttpPost]
        public ActionResult EatIndex(tActivity p,int? id)
        {
            //首頁搜尋
            string Search = "";
            Search += Request.Form["txtKeyword"];
            Search += Request.Form["txtType"];
            Search += Request.Form["txtArea"];
            Search += ",";


            if (Session["member"] != null && p.f活動標題 != null)
            {
                HttpPostedFileBase PicFile = Request.Files["PicFile"];
                if (PicFile != null)
                {
                    var photoName = Guid.NewGuid() + Path.GetExtension(PicFile.FileName);
                    var photoPath = Path.Combine(Server.MapPath("~/Content/images/"), photoName);
                    PicFile.SaveAs(photoPath);
                    p.f活動團圖 = photoName;
                }
                tMember Member = (tMember)Session["member"];
                p.f會員編號 = Member.f會員編號;
                dbJoutaEntities db = new dbJoutaEntities();
                db.tActivity.Add(p);
                db.SaveChanges();
            }
            return View((object)Search);
        }

        public IEnumerable<tActivity> AJAXcondition(string p)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CSelect obj = serializer.Deserialize<CSelect>(p);
            dbJoutaEntities db = new dbJoutaEntities();

            var tEat_order = typeof(tActivity).GetProperty(obj.order);

            //不使用if，動態抓取排序條件
            var CountViewList = db.tActivity
                        .AsEnumerable().OrderByDescending(a => tEat_order.GetValue(a, null))
                        .Select(a => a); //升冪

            if (obj.background_color == "rgb(250, 224, 178)")
            {
                CountViewList = db.tActivity
                .AsEnumerable().OrderBy(a => tEat_order.GetValue(a, null))
                .Select(a => a); //降冪
            }

            if (!string.IsNullOrEmpty(obj.contain)) //搜尋欄位若非空
            {
                CountViewList = CountViewList.Where(b => b.f活動標題.Contains(obj.contain))
                            .Select(a => a);
            }
            if (obj.category != "所有")
            {
                CountViewList = CountViewList
                                    .Where(b => b.f活動分類 == obj.category)
                                    .Select(a => a);
            }

            if (obj.label != "全部")
            {
                CountViewList = CountViewList
                                    .Where(b => b.f活動讚數 > Convert.ToInt32(obj.label))
                                    .Select(a => a);
            }
            return CountViewList;
        }

        public ActionResult eatArticleAjax(string p)
        {
            return View(AJAXcondition(p).Where(a => a.f活動類型 == "飯局").Select(a => a));
        }

        public string autoComplete()
        {
            var x = from t in (new dbJoutaEntities()).tActivity
                    where t.f活動類型 == "飯局"
                    select t.f活動標題;

            return string.Join(",", x.ToArray());
        }

        //public JsonResult CountView(string target, string p)
        //{
        //    if (target != null)
        //    {
        //        dbJoutaEntities db = new dbJoutaEntities();
        //        int select = Convert.ToInt32(target);
        //        tActivity theTarget = db.tActivity.FirstOrDefault(x => x.f活動編號 == select);
        //        theTarget.f活動瀏覽次數 = (theTarget.f活動瀏覽次數 + 1);
        //        db.SaveChanges();
        //    }

        //    var FinalList = AJAXcondition(p)
        //                    .Where(a => a.f活動類型 == "飯局")
        //                    .Select(a => a.f活動瀏覽次數);

        //    return Json(FinalList, JsonRequestBehavior.AllowGet);
        //}
    }
}