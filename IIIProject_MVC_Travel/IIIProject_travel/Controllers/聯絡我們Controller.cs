﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IIIProject_travel.Controllers
{
    public class 聯絡我們Controller : Controller
    {
        //GET: 聯絡我們
        public ActionResult List()
        {
            var 意見表 = from t in (new dbJoutaEntities()).tActivity
                      where t.f活動類型 == "聯絡我們"
                      select t;
            return View(意見表);
        }
        public ActionResult New()
        {
            return View();
        }
        //public ActionResult s儲存()
        //{
        //    tActivity x = new tActivity();
        //    x.f帳號 = Request.Form["txt會員帳號"];
        //    x.f聯絡人 = Request.Form["txt聯絡人"];
        //    x.f電子郵件 = Request.Form["txt電子郵件"];
        //    x.f電話 = Request.Form["txt電話"];
        //    x.f意見類型 = Request.Form["txt意見類型"];
        //    x.f意見 = Request.Form["txt意見"];

        //    dbJoutaEntities db = new dbJoutaEntities();
        //    db.tActivity.Add(x);
        //    db.SaveChanges();

        //    return RedirectToAction("New");
        //}

}
}