﻿using IIIProject_travel.Models;
using IIIProject_travel.Services;
using IIIProject_travel.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.Design;

namespace IIIProject_travel.Controllers
{
    public class HomeController : Controller
    {
        //宣告service物件
        private readonly MembersService membersService = new MembersService();
        //宣告寄信用service物件
        private readonly MailService mailService = new MailService();

        // GET: Home
        [AllowAnonymous]        //不須做登入驗證即可進入
        public ActionResult Home(int? id)
        {
            CData c = new CData();
            var x = from m in (new dbJoutaEntities()).tMember
                    where m.f會員編號 > 8 && m.f會員編號 < 13
                    select m;
            var y = from k in (new dbJoutaEntities()).tActivity
                    where !string.IsNullOrEmpty(k.f活動團圖)
                    select k;
            y = y.Take(3);
            c.tMembers = x;
            c.tActivities = y;
            if (id == 0)
            {
                Session.Remove("member");
            }
            return View(c);
        }

        /*[Authorize]*/     //通過驗證才可進入頁面
        public ActionResult QuickMatch()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QuickMatch(int tabNum, double? curLat, double? curLng)
        {
            dbJoutaEntities db = new dbJoutaEntities();

            //tabNum 0飯局 1旅遊
            //curLat curLng 現在定位經緯度
            if (tabNum == 0)
            {
                if (curLat != null && curLng != null)
                {
                    var x = (from t in db.tActivity
                             where (t.f活動類型 == "飯局") && 
                             (t.f活動經度 > curLng - 0.02) && 
                             (t.f活動經度 < curLng + 0.02) && 
                             (t.f活動緯度 > curLat - 0.02) && 
                             (t.f活動緯度 < curLat + 0.02)
                             select new
                             {
                                 mAchieve = t.tMember.f會員稱號,
                                 mAvatar = t.tMember.f會員大頭貼,
                                 mName = t.tMember.f會員名稱,
                                 mRating = t.tMember.f會員評分,
                                 content = t.f活動內容
                             })
                      .OrderBy(t => Guid.NewGuid()).Take(1);

                    return Json(x, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var x = (from t in db.tActivity
                             where t.f活動類型 == "飯局"
                             select new
                             {
                                 mAchieve = t.tMember.f會員稱號,
                                 mAvatar = t.tMember.f會員大頭貼,
                                 mName = t.tMember.f會員名稱,
                                 mRating = t.tMember.f會員評分,
                                 content = t.f活動內容
                             })
                      .OrderBy(t => Guid.NewGuid()).Take(1);

                    return Json(x, JsonRequestBehavior.AllowGet);
                }             
            }
            else
            {
                if (curLat != null && curLng != null)
                {
                    var x = (from t in db.tActivity
                             where (t.f活動類型 == "旅遊") && 
                             (t.f活動經度 > curLng - 0.02) && 
                             (t.f活動經度 < curLng + 0.02) && 
                             (t.f活動緯度 > curLat - 0.02) && 
                             (t.f活動緯度 < curLat + 0.02)
                             select new
                             {
                                 mAchieve = t.tMember.f會員稱號,
                                 mAvatar = t.tMember.f會員大頭貼,
                                 mName = t.tMember.f會員名稱,
                                 mRating = t.tMember.f會員評分,
                                 content = t.f活動內容
                             })
                      .OrderBy(t => Guid.NewGuid()).Take(1);

                    return Json(x, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var x = (from t in db.tActivity
                             where t.f活動類型 == "旅遊"
                             select new
                             {
                                 mAchieve = t.tMember.f會員稱號,
                                 mAvatar = t.tMember.f會員大頭貼,
                                 mName = t.tMember.f會員名稱,
                                 mRating = t.tMember.f會員評分,
                                 content = t.f活動內容
                             })
                      .OrderBy(t => Guid.NewGuid()).Take(1);

                    return Json(x, JsonRequestBehavior.AllowGet);
                }
            }
        }



        public ActionResult Register()
        {
            //判斷使用者是否已經過登入驗證
            //if (User.Identity.IsAuthenticated)
            //若無登入驗證，則導向註冊頁面
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(CRegisterModel p)
        {
            if ((p == null) && (!ModelState.IsValid))
                return View();
            //判斷資料是否通過驗證
            if (ModelState.IsValid)
            {
                //將頁面資料中的密碼填入
                //p.newMember.txtPassword = p.txtPassword;
                //取得信箱驗證碼
                string AuthCode = mailService.getValidationCode();
                //填入驗證碼
                p.fActivationCode = AuthCode;
                //呼叫service註冊新會員
                membersService.Register(p);
                string tempMail = System.IO.File.ReadAllText(
                    Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));

                //宣告Email驗證用Url
                UriBuilder validateUrl = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("emailValidation", "Home", new
                    {
                        email = p.txtEmail,
                        authCode = AuthCode
                    })
                };
                //將資料寫入信中
                string MailBody = mailService.getRegisterMailBody(tempMail, p.txtNickname, validateUrl.ToString().Replace("%3F", "?"));
                //寄信
                mailService.sendRegisterMail(MailBody, p.txtEmail);
                //以tempData儲存註冊訊息
                TempData["RegisterState"] = "註冊成功，請去收取驗證信";
                return RedirectToAction("RegisterResult");
            }
            //未經驗證清空密碼相關欄位
            p.txtPassword = null;
            p.txtPassword_confirm = null;
            //資料回填至view中
            return View(p);
        }

        //註冊結果顯示頁面
        public ActionResult RegisterResult()
        {
            return View();
        }

        //判斷信箱是否被註冊過
        public JsonResult accountCheck(CRegisterModel p)
        {
            //呼叫service來判斷，並回傳結果
            return Json(membersService.accountCheck(p.txtEmail), JsonRequestBehavior.AllowGet);
        }

        //接收驗證信連結傳進來
        public ActionResult emailValidation(string email, string AuthCode)
        {
            //用ViewData儲存，使用Service進行信箱驗證後的結果訊息
            return View();
        }

        //修改密碼
        //[Authorize]
        //public ActionResult ChangePassword()
        //{
        //    return View();
        //}

        //[Authorize]
        //[HttpPost]
        //public ActionResult ChangePassword(CChangePassword p)
        //{
        //    if (ModelState.IsValid)
        //    {
        //    }
        //    return View();
        //}

        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }

    }
}