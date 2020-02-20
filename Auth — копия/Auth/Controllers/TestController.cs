using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Auth.Models;
using System.Web.Script.Serialization;

namespace Auth.Controllers
{
    public class TestController : Controller
    {
        
        private TestingPlatformEntities context = new TestingPlatformEntities();
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Add()
        {

            return View();
        }


        [HttpPost]
        public void save(Test t)
        {

            using(TestingPlatformEntities context = new TestingPlatformEntities())
            {

                User u = Session["CurrentUser"] as User;
                t.user_id = u.id;
                context.Test.Add(t);
                context.SaveChanges();
            }
        }


        public ActionResult All()
        {
            //using(TestingPlatformEntities context = new TestingPlatformEntities())
            //{
                User u = Session["CurrentUser"] as User;
                ViewBag.tests = context.Test.ToList();
                
                
           // }
            return View();
        }
        [HttpGet]
        public ActionResult pass(int id)
        {

            Test ts = context.Test.Where(m => m.id == id).ToList().First();
            if (ts.password != null && Session["done"]==null)
            {
                Session["CurrentTest"] = id;
                return Redirect("/Test/Secret");
            }

            Session["done"] = null;
            User u = Session["CurrentUser"] as User;
            int r = context.Results.Where(m => m.user_id == u.id && m.test_id == id).ToList().Count;
            //ViewBag.score = context.Results.Where(m => m.user_id == u.id && m.test_id ==id).ToList().First();

            if (r > 0)
            {
                return Redirect("/Test/All");
            }
            ViewBag.data = context.Test.Where(m => m.id == id).ToList().First();
            
            return View();
        }
        public ActionResult Secret()
        {
            return View();
        }
        public ActionResult checkpass()
        {
            int id = Convert.ToInt32(Session["CurrentTest"]);
            string password = Request.Params["pass"];
            Test t = context.Test.Where(m => m.id == id).ToList().First();
            if (t.password == password)
            {
                Session["done"] = 1;
                return Redirect("/Test/pass/"+id);
            }
            else
            {
                TempData["Error"] = "Password is wrong"; 
                return Redirect("/Test/Secret");
            }
        }
        public ActionResult Results()
        {
            return View();
        }
        [HttpPost]
        public void check(Test t)
        {
            int gnahatakan = 0;
            int test = -1;
            int count = 20/Convert.ToInt32(t.Question.Count);
            foreach(var item in t.Question)
            {
                int qonshac = item.Answers.ToList().First().id;
                Answers a = (from k in context.Answers where k.id == qonshac select k).ToList().First();
                test = Convert.ToInt32(a.Question.test_id);
                if (a.isRight == 1)
                {
                    gnahatakan+=count;
                }
            }

            User u = Session["CurrentUser"] as User;
            Results r = new Results();
            r.score = gnahatakan;
            r.test_id = test;
            r.user_id = u.id;
            context.Results.Add(r);
            context.SaveChanges();

            Response.Write(gnahatakan);

            
        }
      

    }
}