﻿using IIIProject_travel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IIIProject_travel.Controllers
{
    public class ProfileController : Controller
    {
        dbJoutaEntities db = new dbJoutaEntities();
        // GET: Profile
        
        public ActionResult ProfileIndex()
        {
            var target = (tMember)Session["member"];
            return View();
        }


    }
}