using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Odnogruppniki.Core;
using Odnogruppniki.Models;
using Odnogruppniki.Models.DBModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TeleSharp.TL;
using TLSharp.Core;

namespace Odnogruppniki.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private DBContext _db;
        private UserManager _um;

        public HomeController() { }
        public HomeController(DBContext db, UserManager userManager)
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

        public static string hash = "";
        public TLUser user;
        public const string phone = "+79157566365";

        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            var user = await GetCurrentUser();
            if (user != null)
            {
                ViewBag.RoleName = (from usr in db.Users
                                    where usr.Login == user.Login
                                    join role in db.Roles
                                    on usr.IdRole equals role.Id
                                    select role.Name).FirstOrDefault();
            }
            else
            {
                ViewBag.RoleName = "Guest";
            }
                var facultyforreg = (await (from faculty in db.Faculties
                                            select faculty).ToListAsync());
                var depforreg = (await (from department in db.Departments
                                        select department).ToListAsync());
            var groupsforreg = (await (from groups in db.Groups
                                       select groups).ToListAsync());
            var interviews = (await (from inter in db.Interviews
                                    select inter).ToListAsync());
            var users = (await (from us in db.PersonalInfoes
                               select us).ToListAsync());
            /*var groupsforreg = new GroupsViewModel
            {
                department = 
            };*/
            ViewBag.Faculties = facultyforreg;
                ViewBag.Departments = depforreg;
                ViewBag.Groups = groupsforreg;
            ViewBag.Interviews = interviews;
            ViewBag.Users = users;
                return View("Index");
        }




        public async Task<ActionResult> ChangeList(int department)
        {
            var groupsforreg = (await(from groups in db.Groups
                                      where groups.IdDepartment == department
                                      select groups).ToListAsync());
            ViewBag.Groups = groupsforreg;
            return View("Index");
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View("Login");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(string login, string password)
        {
            if (await um.IsValid(login, password))
            {
                var ident = new ClaimsIdentity(
                  new[] { 
                      // adding following 2 claim just for supporting default antiforgery provider
                      new Claim(ClaimTypes.NameIdentifier, login),

                      new Claim(ClaimTypes.Name, login)

                      //// optionally you could add roles if any
                      //new Claim(ClaimTypes.Role, "RoleName"),
                      //new Claim(ClaimTypes.Role, "AnotherRole")
                  },
                DefaultAuthenticationTypes.ApplicationCookie);

                HttpContext.GetOwinContext().Authentication.SignIn(
                   new AuthenticationProperties { IsPersistent = false }, ident);
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false, Error = "Login or password are incorrect!" });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Register()
        {
            var facultyforreg = (await (from faculty in db.Faculties
                                        select faculty).ToListAsync());
            var depforreg = (await (from department in db.Departments
                                    select department).ToListAsync());
            var groupsforreg = (await (from groups in db.Groups
                                       select groups).ToListAsync());
            ViewBag.Faculties = facultyforreg;
            ViewBag.Departments = depforreg;
            ViewBag.Groups = groupsforreg;
            return View();
        }

        [HttpGet]
        public ActionResult Groups()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task SaveInfo(string photo, string aboutInfo)
        {
            var usr = await GetCurrentUser();
            var info = (await (from user in db.Users
                               where user.Id == usr.Id
                               join person in db.PersonalInfoes on user.Id equals person.IdUser
                               select person).FirstOrDefaultAsync());
            info.Photo = photo;
            info.AboutInfo = aboutInfo;
            await db.SaveChangesAsync();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Search()
        {
            var searchItems = await (from user in db.Users
                                     join person in db.PersonalInfoes
                                     on user.Id equals person.IdUser
                                     join faculty in db.Faculties
                                     on person.IdFaculty equals faculty.Id
                                     join department in db.Departments
                                     on person.IdDepartment equals department.Id
                                     join grup in db.Groups
                                     on person.IdGroup equals grup.Id
                                     select new SearchUserViewModel
                                     {
                                         id_user = user.Id,
                                         photo = person.Photo,
                                         name = person.Name,
                                         id_faculty = person.IdFaculty,
                                         faculty = faculty.Name,
                                         id_department = person.IdDepartment,
                                         department = department.Name,
                                         id_group = person.IdGroup,
                                         @group = grup.Name
                                     }).ToListAsync();
            ViewBag.SearchUsers = searchItems;
            return View("Search");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> SearchGroup()
        {
            var searchItems = await (from department in db.Departments
                                     join grup in db.Groups
                                     on department.Id equals grup.IdDepartment
                                     join faculty in db.Faculties
                                     on department.IdFaculty equals faculty.Id
                                     select new SearchGroupsViewModel
                                     {
                                         id_group = grup.Id,
                                         name = grup.Name,
                                         id_faculty = department.IdFaculty,
                                         faculty = faculty.Name,
                                         id_department = grup.IdDepartment,
                                         department = department.Name,
                                     }).ToListAsync();
            ViewBag.SearchGroups = searchItems;
            return View("SearchGroup");
        }

        public async Task<ActionResult> OpenProfile(int id)
        {
            var personalInfo = (await (from person in db.PersonalInfoes
                                       where person.IdUser == id
                                       select person).FirstOrDefaultAsync());
            var user = (await db.Users.FirstOrDefaultAsync(x => x.Id == id));
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
            ViewBag.UserId = id;
            ViewBag.MyPage = false;
            ViewBag.Roles = (await db.Roles.ToListAsync());
            ViewBag.Group = (await db.Groups.FirstOrDefaultAsync(x => x.Id == user.IdGroup)).Name;
            return View("/Views/Personal/PersonalInfo.cshtml");
        }

        [HttpGet]
        public ActionResult LogoutPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(
                    new AuthenticationProperties { IsPersistent = false }, User.Identity.AuthenticationType);
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false, Error = "User is not login!" });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Register(string login, string password, string fio, string city, string phone, int id_faculty, int id_department, int id_group)
        {
            if (await db.Users.FirstOrDefaultAsync(x => x.Login == login) == null)
            {
                var newUser = new User
                {
                    Login = login,
                    Password = PasswordHash.GetPasswordHash(password),
                    IdGroup = id_group,
                    IdRole = 2
                };
                db.Users.Add(newUser);
                await db.SaveChangesAsync();
                var user = (await (from users in db.Users
                                   where users.Login == login
                                   select users).FirstOrDefaultAsync());
                var newPersonalInfo = new PersonalInfo
                {
                    Name = fio,
                    IdFaculty = id_faculty,
                    IdDepartment = id_department,
                    IdRole = 2,
                    IdGroup = id_group,
                    IdUser = user.Id,
                    Phone = phone,
                    City = city,
                    AboutInfo = "О себе",
                    Photo = "/Content/defaultphoto.jpg"
                };
                db.PersonalInfoes.Add(newPersonalInfo);
                await db.SaveChangesAsync();
                return Json(new { Success = true });
            }
            return Json(new { Success = false, Error = "This user already exists!" });
        }

        [HttpPost]
        public async Task<ActionResult> AddFaculty(string name)
        {
            if (await db.Faculties.FirstOrDefaultAsync(x => x.Name == name) == null)
            {
                var newFaculty = new Faculty
                {
                    Name = name
                };
                db.Faculties.Add(newFaculty);
                await db.SaveChangesAsync();
                return Json(new { Success = true });
            }
            return Json(new { Success = false, Error = "This faculty already exists!" });
        }
        [HttpPost]
        public async Task<ActionResult> AddDepartment(string name, int id_faculty)
        {
            if (await db.Faculties.FirstOrDefaultAsync(x => x.Name == name) == null)
            {
                var newDepartment = new Department
                {
                    Name = name,
                    IdFaculty = id_faculty
                };
                db.Departments.Add(newDepartment);
                await db.SaveChangesAsync();
                return Json(new { Success = true });
            }
            return Json(new { Success = false, Error = "This department already exists!" });
        }

        [HttpPost]
        public async Task<ActionResult> AddGroup(string name, int id_department)
        {
            if ((await db.Groups.Where(x => x.Name == name && x.IdDepartment == id_department).ToListAsync()).Count == 0)
            {
                var newGroup = new Group
                {
                    Name = name,
                    IdDepartment = id_department
                };
                db.Groups.Add(newGroup);
                await db.SaveChangesAsync();
                return Json(new { Success = true });
            }
            return Json(new { Success = false, Error = "This group already exists!" });
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