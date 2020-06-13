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
using TLSharp.Core;
using TLSharp.Core.Requests;
using TLSharp.Core.Utils;
using TLSharp.Core.Network;
using TeleSharp.TL;
using TLSharp.Core.MTProto;
using TeleSharp.TL.Messages;

namespace Odnogruppniki.Controllers
{
    [Authorize]
    public class PersonalMessageController : Controller
    {
        private DBContext _db;
        private UserManager _um;

        public PersonalMessageController() { }
        public PersonalMessageController(DBContext db, UserManager userManager)
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
            var username = GetCurrentUserName();
            ViewBag.RoleName = (from usr in db.Users
                                where usr.Login == username
                                join role in db.Roles
                                on usr.IdRole equals role.Id
                                select role.Name).FirstOrDefault();
            var user = await GetCurrentUser();
            var date = DateTime.Now.AddDays(-1);
            var messages = await (from message in db.PersonalMessages
                                  join user_in in db.Users
                                  on message.IdIn equals user_in.Id
                                  join user_out in db.Users
                                  on message.IdOut equals user_out.Id
                                  join person_in in db.PersonalInfoes
                                  on user_in.Id equals person_in.IdUser
                                  join person_out in db.PersonalInfoes
                                  on user_out.Id equals person_out.IdUser
                                  where message.IdIn == user.Id || message.IdOut == user.Id
                                  select new PersonalMessageViewModel
                                  {
                                      id = message.Id,
                                      id_in = message.IdIn,
                                      id_out = message.IdOut,
                                      message = message.Message,
                                      name_in = person_in.Name,
                                      name_out = person_out.Name,
                                      photo_in = person_in.Photo,
                                      photo_out = person_out.Photo,
                                      date = message.Date
                                  }).OrderByDescending(x => x.date).ToListAsync();
            messages.ForEach(x => x.dateString = date >= x.date ? string.Format("{0:dd/MM/yy}", x.date) : string.Format("{0:HH:mm:ss}", x.date));
            ViewBag.Messages = messages;
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> OpenMessages(int par)
        {
            var username = GetCurrentUserName();
            ViewBag.RoleName = (from usr in db.Users
                                where usr.Login == username
                                join role in db.Roles
                                on usr.IdRole equals role.Id
                                select role.Name).FirstOrDefault();
            var user = await GetCurrentUser();
            var date = DateTime.Now.AddDays(-1);
            var messages = new List<PersonalMessageViewModel>();
            if (par == 1)
            {
                messages.AddRange(await (from message in db.PersonalMessages
                                         join user_in in db.Users
                                         on message.IdIn equals user_in.Id
                                         join user_out in db.Users
                                         on message.IdOut equals user_out.Id
                                         join person_in in db.PersonalInfoes
                                         on user_in.Id equals person_in.IdUser
                                         join person_out in db.PersonalInfoes
                                         on user_out.Id equals person_out.IdUser
                                         where message.IdOut == user.Id
                                         select new PersonalMessageViewModel
                                         {
                                             id = message.Id,
                                             id_in = message.IdIn,
                                             id_out = message.IdOut,
                                             message = message.Message,
                                             name_in = person_in.Name,
                                             name_out = person_out.Name,
                                             photo_in = person_in.Photo,
                                             photo_out = person_out.Photo,
                                             date = message.Date
                                         }).OrderByDescending(x => x.date).ToListAsync());
            }
            else
            {
                messages.AddRange(await (from message in db.PersonalMessages
                                         join user_in in db.Users
                                         on message.IdIn equals user_in.Id
                                         join user_out in db.Users
                                         on message.IdOut equals user_out.Id
                                         join person_in in db.PersonalInfoes
                                         on user_in.Id equals person_in.IdUser
                                         join person_out in db.PersonalInfoes
                                         on user_out.Id equals person_out.IdUser
                                         where message.IdIn == user.Id
                                         select new PersonalMessageViewModel
                                         {
                                             id = message.Id,
                                             id_in = message.IdIn,
                                             id_out = message.IdOut,
                                             message = message.Message,
                                             name_in = person_in.Name,
                                             name_out = person_out.Name,
                                             photo_in = person_in.Photo,
                                             photo_out = person_out.Photo,
                                             date = message.Date
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
            var model = await (from message in db.PersonalMessages
                               join user_in in db.Users
                               on message.IdIn equals user_in.Id
                               join user_out in db.Users
                               on message.IdOut equals user_out.Id
                               join person_in in db.PersonalInfoes
                               on user_in.Id equals person_in.IdUser
                               join person_out in db.PersonalInfoes
                               on user_out.Id equals person_out.IdUser
                               where message.Id == id
                               select new PersonalMessageViewModel
                               {
                                   id = message.Id,
                                   id_in = message.IdIn,
                                   id_out = message.IdOut,
                                   message = message.Message,
                                   name_in = person_in.Name,
                                   name_out = person_out.Name,
                                   photo_in = person_in.Photo,
                                   photo_out = person_out.Photo,
                                   date = message.Date
                               }).FirstOrDefaultAsync();
            model.dateString = string.Format("{0:dd/MM/yy HH:mm:ss}", model.date);
            ViewBag.Message = model;
            var userName = GetCurrentUserName();
            ViewBag.User = await (from user in db.Users.Where(x => x.Login == userName)
                                  join person in db.PersonalInfoes on user.Id equals person.IdUser
                                  select person.Name).FirstOrDefaultAsync();
            ViewBag.IsAnswer = true;
            return View("PersonalMessage");
        }

        [HttpPost]
        public async Task SendMessage(int id_out, string message)
        {
            var user = await GetCurrentUser();
            ViewBag.RoleName = (from usr in db.Users
                                where usr.Login == user.Login
                                join role in db.Roles
                                on usr.IdRole equals role.Id
                                select role.Name).FirstOrDefault();
            var userPersonal = await (from u in db.Users
                                      join pi in db.PersonalInfoes
                                      on u.Id equals pi.IdUser
                                      where u.Id == user.Id
                                      select pi.Name).FirstOrDefaultAsync();
            if (user.Id != id_out)
            {
                var newMessage = new PersonalMessage
                {
                    IdIn = id_out,
                    IdOut = user.Id,
                    Message = message,
                    Date = DateTime.UtcNow
                };
                db.PersonalMessages.Add(newMessage);
                await db.SaveChangesAsync();
            }
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
            ViewBag.Message = new PersonalMessageViewModel { id_out = id, name_out = "Other user" };
            return View("PersonalMessage");
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