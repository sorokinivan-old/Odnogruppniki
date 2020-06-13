using Microsoft.AspNet.Identity.Owin;
using Odnogruppniki.Core;
using Odnogruppniki.Models;
using Odnogruppniki.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TeleSharp.TL;
using TLSharp.Core;

namespace Odnogruppniki.Controllers
{
    public class GroupMessageController : Controller
    {
        private DBContext _db;
        private UserManager _um;


        public GroupMessageController() { }
        public GroupMessageController(DBContext db, UserManager userManager)
        {
            _db = db;
        }

        public DBContext db
        {
            get
            {
                return _db ?? HttpContext.GetOwinContext().Get<DBContext>();
            }
            private set
            {
                _db = value;
            }
        }

        public UserManager um
        {
            get
            {
                return _um ?? HttpContext.GetOwinContext().Get<UserManager>();
            }
            private set
            {
                _um = value;
            }
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var user = await GetCurrentUser();
            var grup = await (from gruup in db.Groups
                              join usr in db.Users
                              on gruup.Id equals usr.IdGroup
                              where gruup.Id == user.IdGroup
                              select new GroupMessageViewModel
                              {
                                  id = gruup.Id
                              }).FirstOrDefaultAsync();
            var date = DateTime.Now.AddDays(-1);
            var messages = await (from message in db.GroupMessages
                                  join group_in in db.Groups
                                  on message.IdIn equals group_in.Id
                                  join group_out in db.Groups
                                  on message.IdOut equals group_out.Id
                                  where message.IdIn == grup.id
                                  select new GroupMessageViewModel
                                  {
                                      id = message.Id,
                                      id_in = message.IdIn,
                                      id_out = message.IdOut,
                                      message = message.Message,
                                      date = message.Date,
                                      name = group_out.Name
                                  }).OrderByDescending(x => x.date).ToListAsync();
            messages.ForEach(x => x.dateString = date >= x.date ? string.Format("{0:dd/MM/yy}", x.date) : string.Format("{0:HH:mm:ss}", x.date));
            ViewBag.Messages = messages;
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> OpenMessages(int par)
        {
            var user = await GetCurrentUser();
            var grup = await (from gruup in db.Groups
                              join usr in db.Users
                              on gruup.Id equals usr.IdGroup
                              where gruup.Id == user.IdGroup
                              select new GroupMessageViewModel
                              {
                                  id = gruup.Id
                              }).FirstOrDefaultAsync();
            var date = DateTime.Now.AddDays(-1);
            var messages = new List<GroupMessageViewModel>();
            if (par == 1)
            {
                messages.AddRange(await (from message in db.GroupMessages
                                         join group_in in db.Groups
                                         on message.IdIn equals group_in.Id
                                         join group_out in db.Groups
                                         on message.IdOut equals group_out.Id
                                         where message.IdOut == grup.id
                                         select new GroupMessageViewModel
                                         {
                                             id = message.Id,
                                             id_in = message.IdIn,
                                             id_out = message.IdOut,
                                             message = message.Message,
                                             date = message.Date,
                                             name = group_out.Name
                                         }).OrderByDescending(x => x.date).ToListAsync());
            }
            else
            {
                messages.AddRange(await (from message in db.GroupMessages
                                         join group_in in db.Groups
                                         on message.IdIn equals group_in.Id
                                         join group_out in db.Groups
                                         on message.IdOut equals group_out.Id
                                         where message.IdIn == grup.id
                                         select new GroupMessageViewModel
                                         {
                                             id = message.Id,
                                             id_in = message.IdIn,
                                             id_out = message.IdOut,
                                             message = message.Message,
                                             date = message.Date,
                                             name = group_out.Name
                                         }).OrderByDescending(x => x.date).ToListAsync());
            }
            messages.ForEach(x => x.dateString = date >= x.date ? string.Format("{0:dd/MM/yy}", x.date) : string.Format("{0:HH:mm:ss}", x.date));
            ViewBag.Messages = messages;
            return View("Index");
        }

        [HttpGet]
        public async Task<ActionResult> OpenMessage(int id)
        {
            var username = GetCurrentUserName();
            ViewBag.RoleName = (from usr in db.Users
                                where usr.Login == username
                                join role in db.Roles
                                on usr.IdRole equals role.Id
                                select role.Name).FirstOrDefault();
            ViewBag.IsAnswer = true;
            var model = await (from message in db.GroupMessages
                               join group_in in db.Groups
                               on message.IdIn equals group_in.Id
                               join group_out in db.Groups
                               on message.IdOut equals group_out.Id
                               where message.Id == id
                               select new GroupMessageViewModel
                               {
                                   id = message.Id,
                                   id_in = message.IdIn,
                                   id_out = message.IdOut,
                                   message = message.Message,
                                   date = message.Date,
                                   name = group_out.Name
                               }).FirstOrDefaultAsync();
            model.dateString = string.Format("{0:dd/MM/yy HH:mm:ss}", model.date);
            ViewBag.Message = model;
            return View("GroupMessage");
        }

        [HttpPost]
        public async Task SendMessage(int id_out, string message)
        {
            var user = await GetCurrentUser();
            var grup = await (from gruup in db.Groups
                              join usr in db.Users
                              on gruup.Id equals usr.IdGroup
                              where gruup.Id == user.IdGroup
                              select new GroupMessageViewModel
                              {
                                  id = gruup.Id,
                                  name = gruup.Name
                              }).FirstOrDefaultAsync();
            var newMessage = new GroupMessage
            {
                IdIn = id_out,
                IdOut = grup.id,
                Message = message,
                Date = DateTime.UtcNow
            };
            db.GroupMessages.Add(newMessage);
            await db.SaveChangesAsync();
        }

        [HttpGet]
        public ActionResult NewMessage(int id)
        {
            var username = GetCurrentUserName();
            ViewBag.RoleName = (from usr in db.Users
                                where usr.Login == username
                                join role in db.Roles
                                on usr.IdRole equals role.Id
                                select role.Name).FirstOrDefault();
            ViewBag.IsAnswer = false;
            ViewBag.Message = new GroupMessageViewModel { id_out = id, name = "name" };
            return View("GroupMessage");
        }

        private string GetCurrentUserName()
        {
            return HttpContext.GetOwinContext().Authentication.User.Identity.Name;
        }

        private async Task<User> GetCurrentUser()
        {
            var name = GetCurrentUserName();
            return await db.Users.FirstOrDefaultAsync(x => x.Login == name);
        }
    }
}