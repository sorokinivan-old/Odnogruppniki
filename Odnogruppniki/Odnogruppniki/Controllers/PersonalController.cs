using Microsoft.AspNet.Identity.Owin;
using Odnogruppniki.Core;
using Odnogruppniki.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Odnogruppniki.Controllers
{
    [Authorize]
    public class PersonalController : Controller
    {

        private DBContext _db;
        private UserManager _um;

        public PersonalController() { }
        public PersonalController(DBContext db, UserManager userManager)
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
        // GET: Personal
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> PersonalInfo(int? id)
        {
            var user = await GetCurrentUser();
            var personalInfo = new PersonalInfo();
            if (!id.HasValue)
            {
                personalInfo = await db.PersonalInfoes.FirstOrDefaultAsync(x => x.IdUser == user.Id);
            } else
            {
                personalInfo = await db.PersonalInfoes.FirstOrDefaultAsync(x => x.IdUser == id);
            }
            var username = GetCurrentUserName();
            ViewBag.RoleName = (from usr in db.Users
                                where usr.Login == username
                                join role in db.Roles
                                on usr.IdRole equals role.Id
                                select role.Name).FirstOrDefault();
            ViewBag.Photo = personalInfo.Photo;
            ViewBag.Name = personalInfo.Name;
            ViewBag.Faculty = (await db.Faculties.FirstOrDefaultAsync(x => x.Id == personalInfo.IdFaculty)).Name;
            ViewBag.Department = (await db.Departments.FirstOrDefaultAsync(x => x.Id == personalInfo.IdDepartment)).Name; ;
            ViewBag.City = personalInfo.City;
            ViewBag.Role = (await db.Roles.FirstOrDefaultAsync(x => x.Id == user.IdRole)).Name;
            ViewBag.AboutInfo = personalInfo.AboutInfo;
            ViewBag.UserId = user.Id;
            ViewBag.MyPage = true;
            ViewBag.Group = (await db.Groups.FirstOrDefaultAsync(x => x.Id == user.IdGroup)).Name;
            ViewBag.Roles = (await db.Roles.ToListAsync());
            return View();
        }

        [HttpPost]
        public async Task ChangeRole(int id, int role)
        {
            var user = await (from u in db.Users
                              where u.Id == id
                              select u).FirstOrDefaultAsync();
            user.IdRole = role;
            await db.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<ActionResult> Edit()
        {
            var username = GetCurrentUserName();
            ViewBag.RoleName = (from usr in db.Users
                                where usr.Login == username
                                join role in db.Roles
                                on usr.IdRole equals role.Id
                                select role.Name).FirstOrDefault();
            ViewBag.UserId = (await GetCurrentUser()).Id;
            ViewBag.Faculties = await db.Faculties.ToListAsync();
            ViewBag.Departments = await db.Departments.ToListAsync();
            ViewBag.Groups = await db.Groups.ToListAsync();
            var user = await GetCurrentUser();
            ViewBag.User = (await (from usr in db.Users
                                   join person in db.PersonalInfoes on usr.Id equals person.IdUser
                                   where usr.Id == user.Id
                                   select person).FirstOrDefaultAsync());
            return View();
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