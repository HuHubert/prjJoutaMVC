﻿using IIIProject_travel.Models;
using IIIProject_travel.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace IIIProject_travel.Services
{
    public class MembersService
    {
        //建立與資料庫的連接字串
        dbJoutaEntities db = new dbJoutaEntities();

        //註冊新會員
        public void Register(CRegister newMember)
        {
            //將密碼hash
            //newMember.txtPassword = HashPassword(newMember.txtPassword);
            //sql新增     isAdmin預設為0
            tMember t = db.tMember.FirstOrDefault(k=>k.f會員電子郵件 == newMember.txtEmail&&
            k.f會員名稱==newMember.txtNickname&&k.f會員密碼==newMember.txtPassword&&
            k.f會員大頭貼==newMember.txtFiles&&k.f驗證碼==newMember.fActivationCode);
            t.f會員電子郵件 = newMember.txtEmail;
            t.f會員名稱 = newMember.txtNickname;
            t.f會員密碼 = newMember.txtPassword;
            t.f會員大頭貼 = newMember.txtFiles;
            t.f驗證碼 = newMember.fActivationCode;
            t.isAdmin = false;
            db.tMember.Add(t);
            db.SaveChanges();
        }

        //密碼加密
        //private string HashPassword(string txtPassword)
        //{
        //    //宣告hash時添加的無意義亂數值
        //    string saltkey = "dbvgd3423gdrgg4353534";
        //    //將剛剛宣告的字串與密碼結合
        //    string saltAndPassword = String.Concat(txtPassword, saltkey);
        //    //定義SH256的hash物件
        //    SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
        //    //取得密碼轉換成byte資料
        //    byte[] passwordData = Encoding.Default.GetBytes(saltAndPassword);
        //    //取得hash後byte資料
        //    byte[] h = sha256Hasher.ComputeHash(passwordData);
        //    //將hash後byte資料轉換成string
        //    string hashResult = Convert.ToBase64String(h);
        //    //回傳結果
        //    return hashResult;
        //}

        //藉由信箱取得單筆資料(全部資料)
        private CRegister getAccount(string email)
        {
            CRegister c = new CRegister();
            tMember t = db.tMember.FirstOrDefault(k=>k.f會員電子郵件 == email);
            try
            {
                c.txtEmail =t.f會員電子郵件;
                c.txtNickname = t.f會員名稱;
                c.txtPassword = t.f會員密碼;
                c.fActivationCode =t.f驗證碼 ;
                c.isAdmin = Convert.ToBoolean(t.isAdmin);
            }
            catch (Exception e)
            {
                //查無資料
                c = null;
            }
            return c;
        }

        //確認信箱是否重複註冊
        public bool accountCheck(string email)
        {
            CRegister c = getAccount(email);
            //判斷是否查到資料
            bool result = (c == null);
            return result;
        }


        //取得公開資料
        public CRegister getAccount_openSource(string email)
        {
            CRegister c = new CRegister();
            tMember t = db.tMember.FirstOrDefault(k => k.f會員電子郵件 == email);
            try
            {
                c.txtEmail = t.f會員電子郵件;
                c.txtNickname = t.f會員名稱;
                c.txtFiles = t.f會員大頭貼;
            }
            catch (Exception e)
            {
                //查無資料
                c = null;
            }
            //finally
            //{
            //    conn.Close();
            //}
            return c;
        }

       
        //信箱驗證碼驗證
        public string emailValidation(string email, string authCode)
        {
            CRegister c = getAccount(email);
            //宣告驗證後訊息字串
            string validationStr = string.Empty;
            if (c != null)
            {
                if (c.fActivationCode == authCode)
                {
                    dbJoutaEntities db = new dbJoutaEntities();
                    tMember t = db.tMember.FirstOrDefault(k=>k.f會員電子郵件 == email&&k.f驗證碼==authCode);
                    t.f驗證碼 = "";
                    db.tMember.Add(t);
                    db.SaveChanges();
                    validationStr = "信箱驗證成功，現在可以登入囉~";
                }
                else {
                    validationStr = "驗證碼錯誤，請重新確認";
                }
            }
            return validationStr;
        }


        //密碼確認
        private bool passwordCheck(CRegister p, string password)
        {
            //判斷DB裡的密碼資料與傳入密碼資料Hash後是否一致
            //bool result = p.txtPassword.Equals(HashPassword(password));
            bool result = p.txtPassword.Equals(password);
            return result;
        }

        public string LoginCheck(string email, string password)
        {
            //取得傳入email的會員資料
            CRegister p = getAccount(email);
            //判斷是否有此人
            if (p != null)
            {
                //判斷是否經過信箱驗證，有經過驗證驗證碼欄位會被清空
                if (string.IsNullOrWhiteSpace(p.fActivationCode))
                {
                    //進行信箱密碼驗證
                    if (passwordCheck(p, password))
                    {
                        return "";
                    }
                    else
                    {
                        return "密碼輸入錯誤";
                    }
                }
                else {
                    return "此信箱尚未經過Email驗證，請去收信。";
                }
            }
            else
            {
                return "此信箱尚未註冊，請去註冊";
            }
        }

        //更改密碼
        public string ChangePassword(string email,string password,string newPassword)
        {
            CRegister p = getAccount(email);
            //確認舊密碼的正確性
            if (passwordCheck(p, password))
            {
                //p.txtPassword = HashPassword(newPassword);
                p.txtPassword = newPassword;
                tMember t = db.tMember.FirstOrDefault(k => k.f會員密碼 == p.txtPassword);
                db.tMember.Add(t);
                db.SaveChanges();
                return "密碼修改成功";
            }
            else
            {
                return "原密碼輸入錯誤";
            }
        }

        //會員權限角色管理
        public string getRole(string email)
        {
            //初始角色
            string Role = "User";
            CRegister p = getAccount(email);
            //判斷DB欄位，以確認是否為Admin
            if (p.isAdmin)
            {
                Role += "Admin";       //添加Admin
            }
            return Role;
        }


        //檢查圖片類型
        public bool CheckImg(string ContentType)
        {
            switch (ContentType)
            {
                case "image/jpg":
                case "image/jpeg":
                case "image/png":
                        return true;
            }
            return false;
        }

    }

}