using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Auth.Models;
using Auth.lib;
using System.Web.Script.Serialization;
using System.Threading;
using System.IO;

namespace Auth.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        TestingPlatformEntities context = new TestingPlatformEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(User u)
        {
            string error = "";
            var users = context.User.Where(m => m.login == u.login).ToList();

            if (String.IsNullOrEmpty(u.name) || String.IsNullOrEmpty(u.surname) || String.IsNullOrEmpty(u.login) || String.IsNullOrEmpty(u.password))
            {
               error = "Lracreq Bolor Dashter@";
            }
            else if (u.password.Length < 6)
            {
                error = "password@ karch e";
            }
            else if (users.Count>0)
            {
                error = "mutqagreq urish login";
            }




            if (error == "")
            {
                u.password = SecurePasswordHasher.Hash(u.password);
                context.User.Add(u);
                context.SaveChanges();
            }
            else
            {
                //Session["msg"] = error;
                TempData["msg"] = error;
            }
            

            return Redirect("/User/Index");

        }
        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User u)
        {
            var us = context.User.Where(m => m.login == u.login).ToList();
            if (us.Count == 0)
            {
                TempData["msg"] = "Login sxal en";
                return Redirect("/User/Login");

            }else
            {
                User current = us.First();
                if (!SecurePasswordHasher.Verify(u.password, current.password))
                {
                    TempData["msg"] = "password sxal en";
                    return Redirect("/User/Login");
                }
            }

            Session["CurrentUser"] = us.First() ;
            return Redirect("/User/Profile");

        }

        public ActionResult Profile()
        {
            if (Session["CurrentUser"] == null)
            {
                TempData["msg"] = "#404";
                return Redirect("/User/Login");
            }
            return View();
        }
        [HttpGet]
        public ActionResult Edit()
        {
            if (Session["CurrentUser"] == null)
            {
                return Redirect("/User/Login");
            }
            ViewBag.skills = context.Skills.ToList();

            User u = Session["CurrentUser"] as User;
            Session["CurrentUser"] = context.User.Where(y => y.id == u.id).ToList().First();
            return View();
        }
        [HttpPost]
        public ActionResult Edit(User p)
        {
            p = Session["CurrentUser"] as User;
            string old = Request.Params["oldPassword"];
            string newPass = Request.Params["newPassword"];
            if (!SecurePasswordHasher.Verify(old,p.password))
            {
                TempData["msg"] = "sxal";
                return Redirect("/User/Edit");
            }
            else 
            {
                p = context.User.Where(m => m.id == p.id).ToList().First();
                p.password = SecurePasswordHasher.Hash(newPass);
                context.SaveChanges();
                return Redirect("/User/Profile");
            }
        }
        public ActionResult EditLogin(User p)
        {
            p = Session["CurrentUser"] as User;
            string curPass = Request.Params["curPassword"];
            string oldlog = Request.Params["oldLogin"];
            string newlog = Request.Params["newLogin"];
            if (!SecurePasswordHasher.Verify(curPass, p.password))
            {
                TempData["msg2"] = "sxal";
                return Redirect("/User/Edit");
            }
            else
            {
                var isLoginExist = context.User.Where(y => y.login == newlog).ToList();
                if(isLoginExist.Count > 0)
                {
                    TempData["msg2"] = "Sorry the login is busy";
                    return Redirect("/User/Edit");
                }
                p = context.User.Where(m => m.id == p.id).ToList().First();
                p.login = newlog;
                context.SaveChanges();
                return Redirect("/User/Profile");
            }
        }
        public ActionResult Logout()
        {
            Session["CurrentUser"] = null;
            return Redirect("/User/Login");
        }
        //ajax
        public void checkLogin()
        {
            string x = Request.Params["login"];
            var data = context.User.Where(m => m.login == x).ToList();

            Response.Write(data.Count);
        }
        public void checkPass()
        {
            string x = Request.Params["pas"];
            var data = context.User.Where(m => m.password == x).ToList();

            Response.Write(data.Count);
        }
        //ajax
        public void Search()
        {
            string x = Request.Params["text"];
            var data = (from item in context.User
                         where item.name.StartsWith(x)
                         select new {
                             id=item.id,
                             name=item.name,
                             surname=item.surname,
                             photo = item.photo

                         }).ToList();

            //JSON
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string result  = serializer.Serialize(data);
            Response.Write(result);

        }
      //  [ActionName("users")] //return RedirectToAction("users")
        public void ShowAllUsers()
        {
            
            var data = context.User.ToList();
            JavaScriptSerializer s = new JavaScriptSerializer();
            string result = s.Serialize(data);
            Response.Write(result);
        }

        public void GenerateLogin()
        {
            string t = Request.Params["tx"];
            List<string> logins = new List<string>();

            for(int i=0; i<5; i++)
            {
                Random r = new Random();
                Thread.Sleep(200);
                string tarberak = t + "_" + r.Next(1, 2100);
                var data = (from k in context.User where k.login == tarberak select k).ToList().Count;
                if (data == 0)
                {
                    logins.Add(tarberak);
                }


            }
            JavaScriptSerializer s = new JavaScriptSerializer();
            string r1 = s.Serialize(logins);
            Response.Write(r1);
        }

        public void GeneratePassword()
        {
            //string t = context.User.
            //Random num = new Random();
            //string rand = new string(t.ToCharArray().
            //                OrderBy(m => (num.Next(2) % 2) == 0).ToArray());
            User u = Session["CurrentUser"] as User;
            List<char> symbols = new List<char>() { '!', '@', '#', '$', '%', '^', '&', '*', '(', '?', };
            List<string> passwords = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                Random r = new Random();
                string temp = u.name.Substring(0, u.name.Length - 1) + u.surname.Substring(0, u.surname.Length - 3);
                string final = "";
                var tarer = temp.ToCharArray();
                for(int j = 0; j<tarer.Length; j++)
                {
                    Thread.Sleep(100);
                    int k = r.Next(0, tarer.Length);
                    char c = tarer[j];
                    tarer[j] = tarer[k];
                    tarer[k] = c;
                    final += tarer[j];
                    
                }

                passwords.Add(final);


            }
            JavaScriptSerializer s = new JavaScriptSerializer();
            string r1 = s.Serialize(passwords);
            Response.Write(r1);
        }

        public void AddSkill()
        {
            int id = int.Parse(Request.Params["cur"]);
            User us = Session["CurrentUser"] as User;
            us = context.User.Where(x => x.id == us.id).ToList().First();
            var data = context.UserSkils.Where(m => m.skills_id == id).ToList();
            if(data.Count == 0)
            {
                UserSkils y = new UserSkils();
                y.skills_id = id;
                y.user_id = us.id;
                //if (data.id == id)

                context.UserSkils.Add(y);
                context.SaveChanges();
                Response.Write(1);
            }else
            {
                Response.Write(0);
            }
           
            
            
           
        }

        public void DeleteSkill()
        {
            int x = int.Parse(Request.Params["id"]);
            var  data = context.UserSkils.Where(m => m.id==x).ToList().First();
            context.UserSkils.Remove(data);
            context.SaveChanges();
        }

        [HttpPost]
        public ActionResult uploadPhoto(HttpPostedFileBase file)
        {
            User u = Session["CurrentUser"] as User;
            u = context.User.Where(m => m.id == u.id).ToList().First();

            DateTime foo = DateTime.UtcNow;
            double unixTime = ((DateTimeOffset)foo).Millisecond*1000;

            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                string hash = SecurePasswordHasher.Hash(file.FileName);
                hash = hash.Replace("/", "").Substring(0,12);
                // extract only the filename
                var fileName = unixTime+ hash+ Path.GetFileName(file.FileName);
                u.photo = fileName;
                context.SaveChanges();
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                file.SaveAs(path);
            }
            // redirect back to the index action to show the form once again
            return Redirect("/User/Edit");
        }
        [HttpPost]
        public ActionResult uploadcoverPhoto(HttpPostedFileBase file)
        {
            User u = Session["CurrentUser"] as User;
            u = context.User.Where(m => m.id == u.id).ToList().First();

            DateTime foo = DateTime.UtcNow;
            double unixTime = ((DateTimeOffset)foo).Millisecond * 1000;

            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                string hash = SecurePasswordHasher.Hash(file.FileName);
                hash = hash.Replace("/", "").Substring(0, 12);
                // extract only the filename
                var fileName = unixTime + hash + Path.GetFileName(file.FileName);
                u.coverphoto = fileName;
                context.SaveChanges(); 
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                file.SaveAs(path);
            }
            // redirect back to the index action to show the form once again
            return Redirect("/User/Edit");
        }
       
        public ActionResult page(int id)
        {

            if (Session["CurrentUser"] == null)
            {
                return Redirect("/User/login");
            }
            
            User u = Session["CurrentUser"] as User;
            u = context.User.Where(y => y.id == u.id).ToList().First();
            Session["CurrentUser"] = u;
            
            var v = (from item in context.Requests
                     where item.@from == u.id && item.to == id
                     select item).ToList().Count;
            ViewBag.isRequestSent = v;
               // int id => um ejum enq
               // session current user id => es

            //ete et erku id-ner@ kan Requests -um, nshanakum a drujit em tvel es
            //hetevabar ViewBag.tvac = 1, hakarak depqwum VIewBag.tvac = 0
             ViewBag.prof = context.User.Where(m => m.id == id).ToList().First();
            
            return View();

        }
    }

}