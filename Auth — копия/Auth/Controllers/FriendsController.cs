using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Auth.Models;
namespace Auth.Controllers
{
    public class FriendsController : Controller
    {
        TestingPlatformEntities context = new TestingPlatformEntities();
        // GET: Friends
        public ActionResult Index()
        {
            return View();
        }

        public void AddFriend()
        {
            int id = Convert.ToInt32(Request.Params["id"]);
            int myself = (Session["CurrentUser"] as User).id;

            Requests r = new Requests();
            r.from = myself;
            r.to = id;
            context.Requests.Add(r);
            context.SaveChanges();
            Response.Write(myself + " " + id);
        }
        public ActionResult Requests()
        {
            var user = Session["CurrentUser"] as User;
            user = context.User.Where(y => y.id == user.id).ToList().First();
            Session["CurrentUser"] = user;
            return View();
        }
        public void Refuse()
        {
            int me = (Session["CurrentUser"] as User).id;
            int from_id = Convert.ToInt32(Request.Params["id"]);
            Requests r = (from item in context.Requests
                          where item.@from == from_id &&
                          item.to == me
                          select item).ToList().First();
            context.Requests.Remove(r);
            context.SaveChanges();

            Response.Write("ok"); //ajax-in ktel (hushum a vor amen inch ok a, vorpeszi success@ ashxati)
        }

        public void Accept()
        {
            int me = (Session["CurrentUser"] as User).id;
            int from_id = Convert.ToInt32(Request.Params["id"]);
            Requests r = (from item in context.Requests
                          where item.@from == from_id &&
                          item.to == me
                          select item).ToList().First();
            context.Requests.Remove(r);
            Friends f = new Friends();
            f.from = me;
            f.to = from_id;
            context.Friends.Add(f);
            
            context.SaveChanges();
            Response.Write("ok");
           

        }

        public ActionResult dashboard()
        {
            User u = Session["CurrentUser"] as User;
            List<User> part1 = (from item in context.Friends where item.@from == u.id select item.User1).ToList();
            List<User> part2 = (from item in context.Friends where item.to == u.id select item.User).ToList();
            ViewBag.friends = part1.Union(part2).ToList<User>();
             return View();
        }
        public ActionResult messenger()
        {
            User u = Session["CurrentUser"] as User;
            List<User> part1 = (from item in context.Friends where item.@from == u.id select item.User1).ToList();
            List<User> part2 = (from item in context.Friends where item.to == u.id select item.User).ToList();
            ViewBag.friends = part1.Union(part2).ToList<User>();
            return View();
        }

        public void SendMessage()
        {
            string namak = Request.Params["namak"];
            int current_chat = int.Parse(Request.Params["current_chat"]);
            int me = (Session["CurrentUser"] as User).id;
            Messages m = new Messages();
            m.text = namak;
            m.to = current_chat;
            m.from = me;
            m.status = 1;
            m.time = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            context.Messages.Add(m);
            context.SaveChanges();

        }
        public void GetMessages()
        {
            int current_chat = int.Parse(Request.Params["current_chat"]);
            int me = (Session["CurrentUser"] as User).id;

            var data = (from item in context.Messages
                        where (item.to == current_chat && item.@from == me)
                        || (item.to == me && item.@from == current_chat)
                        select new
                        {
                            id = item.id,
                            text = item.text,
                            @from = item.@from,
                            to = item.to,
                            time = item.time

                        }).ToList();

           

            string json = new JavaScriptSerializer().Serialize(data);
            Response.Write(json);
        }
        public void Seen()
        {
            int current_chat = int.Parse(Request.Params["current_chat"]);
            int me = (Session["CurrentUser"] as User).id;

            var data = (from item in context.Messages
                        where item.to == me && item.@from == current_chat
                        select item).ToList();

            foreach(var item in data)
            {
                item.status = 0;
            }

            context.SaveChanges();
            
        }
        public void Remove()
        {
            int me = (Session["CurrentUser"] as User).id;
            int from_id = Convert.ToInt32(Request.Params["id"]);
            Friends f = (from item in context.Friends
                          where (item.@from == from_id &&
                          item.to == me) || (item.to == from_id &&
                          item.@from == me)
                          select item).ToList().First();
            context.Friends.Remove(f);
            context.SaveChanges();

            //Response.Write("ok"); 
        }
        public void Notifications()
        {
            User u = Session["CurrentUser"] as User;
            var data = (from item in context.Messages
                        where item.status == 1 && item.to == u.id select item.@from
                        ).ToList();


            string json = new JavaScriptSerializer().Serialize(data);
            Response.Write(json);

        }
    }
}