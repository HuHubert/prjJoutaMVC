﻿using IIIProject_travel.Models;
using IIIProject_travel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace IIIProject_travel.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Home(int? id)
        {
            if (id==0)
            {
                Session.Remove("member");
                
            }
                return View();   
        }

        public ActionResult QuickMatch()
        {
            return View();
        }

        public ActionResult Login()
        {
            //string code = Session[CDictionary.SK_USERLOGIN_CODE] as string;
            //if (string.IsNullOrEmpty(code))
            //{
            //    Random r = new Random();
            //    code = r.Next(0, 10).ToString() + r.Next(0, 10).ToString() + r.Next(0, 10).ToString() + r.Next(0, 10).ToString();
            //    Session[CDictionary.SK_USERLOGIN_CODE] = code;
            //}
            //ViewBag.CODE = code;
            return View();
        }
        [HttpPost]
        public ActionResult Login(CLogin p)
        {

            //string code = Session[CDictionary.SK_USERLOGIN_CODE] as string;
            //if (!p.txtPassword.Equals(code))
            //{
            //    ViewBag.CODE = code;
            //    return View();
            //}
            String fAccount = p.txtAccount;
            
            tMember cust = (new dbJoutaEntities()).tMember.FirstOrDefault(t => t.f會員帳號== fAccount && t.f會員密碼.Equals(p.txtPassword));
            if (cust == null)
                return View();
            Session[CDictionary.SK_LOGIN_MEMBER] = cust;
            return RedirectToAction("Home");
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Exclude = "fIs信箱已驗證,fActivationCode")]CRegister p)
        {
            var y = p.txtNickname;
            var z = p.txtPassword;
            var d = p.txtPassword_confirm;
            bool Status = false;
            string Message = "";

            //Model Validation
            if (ModelState.IsValid)
            {
                var isExist_mail = is信箱已存在(p.txtEmail);
                if (isExist_mail)
                {
                    ModelState.AddModelError("emailExist", "信箱已存在");
                    return View(p);
                }
                //驗證碼
                p.fActivationCode = Guid.NewGuid();
                //密碼驗證
                string fact_password = p.txtPassword;
                p.txtPassword = C驗證.Hash(p.txtPassword);
                p.txtPassword_confirm = C驗證.Hash(p.txtPassword_confirm);

                p.fIs信箱已驗證 = false;

                using (dbJoutaEntities db = new dbJoutaEntities())
                {
                    tMember NewMember = new tMember();
                    NewMember.f性別 = p.txtGender;
                    NewMember.f會員帳號 = p.txtEmail;
                    NewMember.f會員密碼 = fact_password;
                    NewMember.f電子郵件 = p.txtEmail;
                    NewMember.f會員名稱 = p.txtNickname;
                    db.tMember.Add(NewMember);
                    db.SaveChanges();

                    send確認信(p.txtEmail, p.fActivationCode.ToString());
                    Message = "註冊已完成。確認連結已發送到您的信箱:"+p.txtEmail;

                    Status = true;
                }
            }
            else
            {
                Message = "Invalid Request";
            }
            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View();
        }


        [NonAction]
        public bool is信箱已存在(string emailID)
        {
            using (dbJoutaEntities db = new dbJoutaEntities())
            {
                var v = db.tMember.Where(a => a.f電子郵件 == emailID).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public void send確認信(string emailID,string c確認碼)
        {
            var verifyUrl = "/Home/VerifyAccount/" + c確認碼;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery,verifyUrl);
            var fromEmail = new MailAddress("agrwdcd1356@gmail.com", "確認");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "1234afcb";
            string subject = "註冊成功";

            string body = "<br/><br/>很高興通知您，會員註冊成功。請點以下連結確認帳號"
                + "<br/><br/><a href='" + link+"'>"+link+"</a>";

            //var smtp = new SmtpClient
            //{
            //    Host = "smtp.gmail.com",
            //    Port = 587,
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential(fromEmail.Address,fromEmailPassword)
            //};



            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential(
                    "agrwdcd1356@gmail.com", "1234afcb"
                    );
                client.EnableSsl = true;
                client.Send(message);
            }
            //smtp.Send(message);
        }

        //public ActionResult Save()
        //{
        //    tMember x = new tMember();
        //    x.f會員帳號 = Request.Form["txtAccount"];
        //    x.f會員密碼 = Request.Form["txtPassword"];
        //    x.f性別 = Request.Form["txtGender"];
        //    x.f暱稱 = Request.Form["txtNickname"];
        //    x.f電子郵件 = Request.Form["txtEmail"];

        //    dbJoutaEntities db = new dbJoutaEntities();
        //    db.tMember.Add(x);
        //    db.SaveChanges();
        //    return RedirectToAction("Login");
        //}

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}