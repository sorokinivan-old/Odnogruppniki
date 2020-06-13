using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Odnogruppniki.Core;
using Odnogruppniki.Models;
using Odnogruppniki.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Migrations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TeleSharp.TL;
using TLSharp.Core;

namespace Odnogruppniki.Controllers
{
    public class InterviewController : Controller
    {

        private DBContext _db;
        private UserManager _um;
        private bool QuestionNotNull = false;
        private static List<InterviewResult> resultList = new List<InterviewResult>();
        private static List<Interview> interviewList = new List<Interview>();
        private static List<InterviewCategory> categoryList = new List<InterviewCategory>();
        private static List<InterviewQuestion> questionList = new List<InterviewQuestion>();
        private static List<InterviewAnswer> answerList = new List<InterviewAnswer>();
        private static List<InterviewCategoriesOfInterview> categoriesOfInterviewList = new List<InterviewCategoriesOfInterview>();
        private static List<InterviewQuestionsOfInterview> questionsOfInterviewList = new List<InterviewQuestionsOfInterview>();
        private static List<Interview> interviewMainList = new List<Interview>();
        private static Interview interviewMain;
        private static int interviewId = 0;
        private static int count = 1;
        private static bool? saati;
        private static int idCategory = 0;

        public InterviewController() { }
        public InterviewController(DBContext db, UserManager userManager)
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
        // GET: Interview
        public async Task<ActionResult> Index()
        {
            interviewList.Clear();
            var count = 0;
            var user = await GetCurrentUser();
            if (user != null)
            {
                ViewBag.RoleName = (from usr in db.Users
                                    where usr.Login == user.Login
                                    join role in db.Roles
                                    on usr.IdRole equals role.Id
                                    select role.Name).FirstOrDefault();
                var idgroup = (from usr in db.Users
                               where usr.Login == user.Login
                               select usr.IdGroup).FirstOrDefault();
                var userInfo = (from usr in db.PersonalInfoes
                                where usr.IdUser == user.Id
                                select usr).FirstOrDefault();
                var dones = await (from don in db.InterviewDones
                                   where don.IdUser == user.Id
                                   select don.IdInterview).ToListAsync();
                //var interviews = (await (from interview in db.Interviews
                //                         where interview.IdGroup == idgroup
                //                         select interview).ToListAsync());
                var interviews = (await (from interview in db.Interviews
                                         select interview).ToListAsync());
                //if (dones != null)
                dones.Sort();
                List<int> list = new List<int>();
                for (int i = 0; i < interviews.Count(); i++)
                    interviewList.Add(interviews[i]);
                foreach (Interview inter in interviews)
                {
                    foreach (int don in dones)
                    {
                        if (inter.Id == don)
                        {
                            interviewList = interviewList.Where(x => x.Name != inter.Name).ToList();
                        }
                    }
                }
                //bool tmp = false;
                //foreach (int don in dones)
                //{
                //    foreach (Interview inter in interviews)
                //    {
                //        if (inter.Id != don)
                //        {
                //            tmp = true;
                //        }
                //        if (tmp == true)
                //        {
                //            interviewList.Add(inter);
                //            tmp = false;
                //            break;
                //        }
                //    }
                //}
                //ViewBag.DonesInterview = dones;
                interviews.Clear();
                foreach (var inter in interviewList)
                {
                    if ((inter.IdFaculty == userInfo.IdFaculty && inter.IdDepartment == null && inter.IdGroup == null) || (inter.IdDepartment == userInfo.IdDepartment && inter.IdGroup == null) || inter.IdGroup == user.IdGroup)
                    {
                        interviews.Add(inter);
                    }
                }
                var degree = await (from gr in db.Groups
                                    where gr.Id == idgroup
                                    select gr.IdDegree).FirstOrDefaultAsync();
                if (degree != 2)
                {
                    interviews.RemoveAll(x => x.Saati != null);
                }
                ViewBag.Interviews = interviews;
                //ViewBag.IdFaculty = (from usr in db.Users
                //                     where usr.Login == user.Login
                //                     join person in db.PersonalInfoes
                //                     on usr.Id equals person.IdUser
                //                     select person.IdFaculty).FirstOrDefault();
                //ViewBag.IdDepartment = (from usr in db.Users
                //                     where usr.Login == user.Login
                //                     join person in db.PersonalInfoes
                //                     on usr.Id equals person.IdUser
                //                     select person.IdDepartment).FirstOrDefault();
            }
            else
            {
                ViewBag.RoleName = "Guest";
            }
            return View("Interview");
        }

        public async Task<ActionResult> ResultsInterviews()
        {
            var count = 0;
            var user = await GetCurrentUser();
            if (user != null)
            {
                ViewBag.RoleName = (from usr in db.Users
                                    where usr.Login == user.Login
                                    join role in db.Roles
                                    on usr.IdRole equals role.Id
                                    select role.Name).FirstOrDefault();

                var interviews = (await (from interview in db.Interviews
                                         where interview.Saati == null
                                         select interview).ToListAsync());
                var saatiInterviews = (await (from interview in db.Interviews
                                              where interview.Saati != null
                                              select interview).ToListAsync());
                var idDepartment = (await (from pInfo in db.PersonalInfoes
                                           where pInfo.IdUser == user.Id
                                           select pInfo.IdDepartment).FirstOrDefaultAsync());
                //interviews.Add(new Interview
                //{
                //    /*Id = (from que in interviews
                //          select que.Id).LastOrDefault() + 1,*/
                //    Id = interviews.LastOrDefault() == null ? 0 : interviews.LastOrDefault().Id + 1,
                //    //IdGroup = 0,
                //    Name = "Ранжирование образовательных программ групп магистратуры кафедры " + (from dep in db.Departments
                //                                                                                  where dep.Id == idDepartment
                //                                                                                  select dep.Name).FirstOrDefault(),
                //    Saati = idDepartment
                //});

            foreach (var interview in saatiInterviews)
            {
                if (interview.Saati == idDepartment)
                {
                    interviews.Add(new Interview
                    {
                        /*Id = (from que in interviews
                              select que.Id).LastOrDefault() + 1,*/
                        Id = interview.Id,
                        //IdGroup = 0,
                        Name = "Ранжирование образовательных программ групп магистратуры кафедры " + (from dep in db.Departments
                                                                                                      where dep.Id == idDepartment
                                                                                                      select dep.Name).FirstOrDefault(),
                        Saati = interview.Saati
                    });
                        break;
                }
            }

            var interviewsFinal = interviews.OrderByDescending(x => x.Id);
                ViewBag.Interviews = interviewsFinal;
            }
            else
            {
                ViewBag.RoleName = "Guest";
            }
            return View("InterviewResults");
        }
        public void DeleteCategoryResults(int user, int Id)
        {
            //var user = GetCurrentUser();
            var questions = (from que in db.InterviewQuestions
                             join inter in db.InterviewQuestionsOfInterviews
                           on que.Id equals inter.IdQuestion
                             where inter.IdInterview == Id
                             select que).ToList();
            /*var categories = (from cat in db.InterviewCategories
                              where cat.IdInterview == Id
                              select cat).ToList();*/
            var categories = (from cat in db.InterviewCategories
                              join inter in db.InterviewCategoriesOfInterviews
                              on cat.Id equals inter.IdCategory
                              where inter.IdInterview == Id
                              select cat).ToList();
            /*if (categories.Count == 0)
            {
                categories = (from cat in db.InterviewCategories
                              where cat.IdInterview == null
                              select cat).ToList();
            }*/
            var answers = (from ans in db.InterviewAnswers
                           join que in db.InterviewQuestionsOfInterviews
                           on ans.IdQuestion equals que.IdQuestion
                           where que.IdInterview == Id
                           select ans).ToList();
            var answers2 = (from ans in db.InterviewAnswers
                            where ans.IdQuestion == 0
                            select ans).ToList();
            var allanswers = answers2.Union(answers);
            var results = (from res in db.InterviewResults
                           where res.IdInterview == Id && res.IdUser == user
                           select res).ToList();
            List<ResultCountViewModel> resCountStandart = new List<ResultCountViewModel>();
            List<ResultCountViewModel> resCountCustom = new List<ResultCountViewModel>();
            List<InterviewCategoryResult> categoryResult = new List<InterviewCategoryResult>();
            double temp = 0;
            double resultOfCategory = 0;
            foreach (var category in categories)
            {
                List<InterviewQuestion> questions2 = questions.Where(x => x.IdCategory == category.Id).ToList();
                foreach (var que in questions2)
                {
                    foreach (var res in results)
                    {
                        if (res.IdQuestion == que.Id)
                        {
                            temp = allanswers.Where(x => x.Id == res.IdAnswer).Select(x => x.Weight).FirstOrDefault();
                        }
                    }
                    resultOfCategory += temp;
                }
                double roundedResult = Math.Round(resultOfCategory, MidpointRounding.AwayFromZero);
                categoryResult.Add(new InterviewCategoryResult
                {
                    IdCategory = category.Id,
                    IdInterview = Id,
                    Result = (int)roundedResult
                });
                resultOfCategory = 0;
            }
            foreach (var category in categoryResult)
            {
                var result = db.InterviewCategoryResults.Where(x => x.IdInterview == Id && x.IdCategory == category.IdCategory).FirstOrDefault();
                result.Result = result.Result - category.Result;
            }
            db.SaveChanges();
        }
        public void SaveCategoryResults(int Id)
        {
            var questions = (from que in db.InterviewQuestions
                             join inter in db.InterviewQuestionsOfInterviews
                           on que.Id equals inter.IdQuestion
                             where inter.IdInterview == Id
                             select que).ToList();
            /*var categories = (from cat in db.InterviewCategories
                              where cat.IdInterview == Id
                              select cat).ToList();*/
            var categories = (from cat in db.InterviewCategories
                              join inter in db.InterviewCategoriesOfInterviews
                              on cat.Id equals inter.IdCategory
                              where inter.IdInterview == Id
                              select cat).ToList();
            /*if (categories.Count == 0)
            {
                categories = (from cat in db.InterviewCategories
                              where cat.IdInterview == null
                              select cat).ToList();
            }*/
            var answers = (from ans in db.InterviewAnswers
                           join que in db.InterviewQuestionsOfInterviews
                           on ans.IdQuestion equals que.IdQuestion
                           where que.IdInterview == Id
                           select ans).ToList();
            var answers2 = (from ans in db.InterviewAnswers
                            where ans.IdQuestion == 0
                            select ans).ToList();
            var allanswers = answers2.Union(answers);
            var results = (from res in db.InterviewResults
                           where res.IdInterview == Id
                           select res).ToList();
            List<ResultCountViewModel> resCountStandart = new List<ResultCountViewModel>();
            List<ResultCountViewModel> resCountCustom = new List<ResultCountViewModel>();
            var questions2 = new List<InterviewQuestion>();
            for (int i = 0; i < questions.Count(); i++)
                questions2.Add(questions[i]);
            foreach (InterviewQuestion question in questions)
            {
                var answerstemp = (from ans in allanswers
                                   where ans.IdQuestion == question.Id
                                   select ans).ToList();
                if (answerstemp != null)
                {
                    foreach (InterviewAnswer answer in answerstemp)
                    {
                        var temp = (from res in results
                                    where res.IdAnswer == answer.Id && res.IdQuestion == question.Id
                                    select res).Count();
                        resCountCustom.Add(new ResultCountViewModel
                        {
                            idAnswer = answer.Id,
                            idQuestion = question.Id,
                            count = temp
                        });
                        questions2 = questions2.Where(x => x.Id != question.Id).ToList();

                    }
                }

            }
            foreach (InterviewQuestion question in questions2)
            {
                var answerstemp = (from ans in allanswers
                                   where ans.IdQuestion == 0
                                   select ans).ToList();
                if (answerstemp != null)
                {
                    foreach (InterviewAnswer answer in answerstemp)
                    {
                        var temp = (from res in results
                                    where res.IdAnswer == answer.Id && res.IdQuestion == question.Id
                                    select res).Count();
                        resCountStandart.Add(new ResultCountViewModel
                        {
                            idAnswer = answer.Id,
                            idQuestion = question.Id,
                            count = temp
                        });

                    }
                }

            }
            var resCountFinal = resCountCustom.Union(resCountStandart).ToList();
            var sortedResCountFinal = resCountFinal.OrderBy(x => x.idQuestion);
            List<AverageAnswersViewModel> categoryResults = new List<AverageAnswersViewModel>();
            List<AverageAnswersViewModel> averageAnswers = new List<AverageAnswersViewModel>();
            double tempStorage = 0;
            double categoryResult = 0;
            foreach (var cat in categories)
            {
                foreach (var que in questions)
                {
                    if (que.IdCategory == cat.Id)
                    {
                        foreach (var resC in sortedResCountFinal)
                        {
                            if (resC.idQuestion == que.Id && resC.count != 0)
                            {
                                var temp = (from res in db.InterviewAnswers
                                            where res.Id == resC.idAnswer
                                            select res.Weight).FirstOrDefault();
                                temp = temp * resC.count;
                                tempStorage = tempStorage + temp;
                            }

                        }
                        averageAnswers.Add(new AverageAnswersViewModel
                        {
                            id = que.Id,
                            average = tempStorage
                        });
                        tempStorage = 0;
                        foreach (var ans in averageAnswers)
                        {
                            if (que.Id == ans.id)
                            {
                                categoryResult += ans.average;
                            }
                        }
                    }

                }
                categoryResults.Add(new AverageAnswersViewModel
                {
                    id = cat.Id,
                    average = Math.Round(categoryResult, MidpointRounding.AwayFromZero)
                });
                /*var idInterview = (from inter in db.Interviews
                                   join category in db.InterviewCategories
                                   on inter.Id equals category.IdInterview
                                   where category.Id == cat.Id
                                   select inter.Id).FirstOrDefault();*/
                /*if (idInterview == null)
                {
                    idInterview = (from inter in db.Interviews
                                   join category in db.InterviewCategories
                                   on inter.Id equals category.IdInterview
                                   where category.Id == cat.Id
                                   select inter.Id).FirstOrDefault();
                }*/
                var interviewCategoryResult = new InterviewCategoryResult
                {
                    /*IdInterview = (from inter in db.Interviews
                                   join category in db.InterviewCategories
                                   on inter.Id equals category.IdInterview
                                   where category.Id == cat.Id
                                   select inter.Id).FirstOrDefault(),*/
                    IdInterview = Id,
                    IdCategory = cat.Id,
                    Result = (int)categoryResult
                };
                var check = db.InterviewCategoryResults.Where(x => x.IdCategory == interviewCategoryResult.IdCategory && x.IdInterview == interviewCategoryResult.IdInterview).ToList();
                if (check.Count != 0)
                    foreach (var c in check)
                        db.InterviewCategoryResults.Remove(c);
                db.InterviewCategoryResults.Add(interviewCategoryResult);
                categoryResult = 0;
            }
            db.SaveChanges();
        }

        public async Task<ActionResult> SaveInterviewMainInformation(bool? isSaati, string interviewName, int? interviewRole, string interviewWho, int? interviewWhoId, string categoryName, string questionName, string[] answers, double?[] weights, int complete)
        {
            /*db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.InterviewQuestion ON");
            db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.InterviewCategory ON");
            db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Interview ON");*/
            /*db.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[Interview] ON");
            db.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[InterviewCategory] ON");
            db.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[InterviewQuestion] ON");*/
            if (complete == 0)
            {
                int IdInterview = 0;
                saati = isSaati;
                ViewBag.Saati = isSaati;
                ViewBag.DepartmentList = await (from deps in db.Departments
                                                select deps).ToListAsync();
                ViewBag.RoleList = await (from roles in db.Roles
                                          select roles).ToListAsync();
                int? faculty = null;
                int? department = null;
                int? group = null;
                switch (interviewWho)
                {
                    case "Faculty":
                        faculty = await (from facult in db.Faculties
                                         where facult.Id == interviewWhoId
                                         select facult.Id).FirstOrDefaultAsync();
                        break;
                    case "Department":
                        department = await (from dep in db.Departments
                                            where dep.Id == interviewWhoId
                                            select dep.Id).FirstOrDefaultAsync();
                        break;
                    case "Group":
                        group = await (from gr in db.Groups
                                       where gr.Id == interviewWhoId
                                       select gr.Id).FirstOrDefaultAsync();
                        break;
                    default:
                        department = await (from dep in db.Departments
                                            where dep.Id == interviewWhoId
                                            select dep.Id).FirstOrDefaultAsync();
                        break;
                }
                IdInterview = (from inter in db.Interviews
                               select inter.Id).ToList().LastOrDefault();
                if (isSaati == false)
                {
                    interviewMain = new Interview
                    {
                        Id = IdInterview + 1,
                        IdRole = interviewRole != null ? interviewRole : 2,
                        IdGroup = group,
                        IdDepartment = department,
                        IdFaculty = faculty,
                        Name = interviewName,
                        Saati = null
                    };
                    //db.Interviews.Add(interviewMain);
                }
                else
                {
                    int count = 1;
                    var groupList = await (from gr in db.Groups
                                           where gr.IdDepartment == department
                                           select gr.Id).ToListAsync();
                    foreach (var gr in groupList)
                    {
                        var tempInterview = new Interview
                        {
                            Id = IdInterview + count,
                            IdRole = 2,
                            IdGroup = gr,
                            IdDepartment = department,
                            IdFaculty = faculty,
                            Name = "Опрос об образовательной программе группы " + (from g in db.Groups
                                                                                   where g.Id == gr
                                                                                   select g.Name).FirstOrDefault() + " магистратуры",
                            Saati = department
                        };
                        interviewMainList.Add(tempInterview);
                        count++;
                    }
                    //foreach (var inter in interviewMainList)
                    //{
                    //    db.Interviews.Add(inter);
                    //}


                }


                //await db.SaveChangesAsync();
            }
            else if (complete > 0)
            {
                int idInterview = 0;
                int idQuestion = 0;
                //var check = await (from cat in db.InterviewCategories
                //                   where cat.Name == categoryName
                //                   select cat).FirstOrDefaultAsync();
                idInterview = (from inter in db.Interviews
                               select inter.Id).ToList().LastOrDefault() + 1;
                //if(check == null)
                //{
                var category = categoryList.LastOrDefault();
                if (category == null || category.Name != categoryName)
                {
                    var temp = (from cate in db.InterviewCategories
                                select cate).ToList().LastOrDefault();
                    idCategory = temp.Id;
                    idCategory += count;
                    categoryList.Add(new InterviewCategory
                    {
                        Id = idCategory,
                        Name = categoryName
                    });
                    count++;
                }
                else
                {
                    categoryName = categoryList.Last().Name;
                }
                //}

                if (questionList.Count == 0)
                {
                    idQuestion = (from que in db.InterviewQuestions
                                  select que.Id).ToList().LastOrDefault() + 1;
                }
                else
                {
                    idQuestion = questionList.LastOrDefault().Id + 1;
                }
                questionList.Add(new InterviewQuestion
                {
                    Id = idQuestion,
                    Question = questionName,
                    IdCategory = idCategory
                });
                if (weights == null || weights.Contains(null))
                {
                    for (int i = 0; i < answers.Length; i++)
                    {
                        answerList.Add(new InterviewAnswer
                        {
                            IdQuestion = idQuestion,
                            Answer = answers[i],
                            Weight = 0
                        });
                    }
                }
                else
                {
                    for (int i = 0; i < answers.Length; i++)
                    {
                        answerList.Add(new InterviewAnswer
                        {
                            IdQuestion = idQuestion,
                            Answer = answers[i],
                            Weight = (double)weights[i]
                        });
                    }
                }

                //foreach (var cat in categoryList)
                //    db.InterviewCategories.Add(cat);
                //foreach (var que in questionList)
                //    db.InterviewQuestions.Add(que);
                //foreach (var ans in answerList)
                //    db.InterviewAnswers.Add(ans);


                //db.InterviewCategoriesOfInterviews.Add(new InterviewCategoriesOfInterview
                //{
                //    IdCategory = check != null ? check.Id : idCategory,
                //    IdInterview = idInterview
                //});

                //db.InterviewQuestionsOfInterviews.Add(new InterviewQuestionsOfInterview
                //{
                //    IdInterview = idInterview,
                //    IdQuestion = idQuestion
                //});
                if (complete > 1)
                {
                    if (interviewMain != null)
                    {
                        db.Interviews.Add(interviewMain);
                    }
                    else
                    {
                        foreach (var inter in interviewMainList)
                        {
                            db.Interviews.Add(inter);
                        }
                    }
                    if (interviewMain != null)
                    {
                        foreach (var cat in categoryList)
                        {
                            categoriesOfInterviewList.Add(new InterviewCategoriesOfInterview
                            {
                                IdCategory = cat.Id,
                                IdInterview = interviewMain.Id
                            });
                        }
                        foreach (var que in questionList)
                        {
                            questionsOfInterviewList.Add(new InterviewQuestionsOfInterview
                            {
                                IdInterview = interviewMain.Id,
                                IdQuestion = que.Id
                            });
                        }
                    }
                    else
                    {
                        foreach (var inter in interviewMainList)
                        {
                            foreach (var cat in categoryList)
                            {
                                categoriesOfInterviewList.Add(new InterviewCategoriesOfInterview
                                {
                                    IdCategory = cat.Id,
                                    IdInterview = inter.Id
                                });
                            }
                            foreach (var que in questionList)
                            {
                                questionsOfInterviewList.Add(new InterviewQuestionsOfInterview
                                {
                                    IdInterview = inter.Id,
                                    IdQuestion = que.Id
                                });
                            }
                        }
                    }


                    foreach (var cat in categoriesOfInterviewList)
                        db.InterviewCategoriesOfInterviews.Add(cat);
                    foreach (var que in questionsOfInterviewList)
                        db.InterviewQuestionsOfInterviews.Add(que);
                    foreach (var cat in categoryList)
                        db.InterviewCategories.Add(cat);
                    foreach (var que in questionList)
                        db.InterviewQuestions.Add(que);
                    foreach (var ans in answerList)
                        db.InterviewAnswers.Add(ans);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        var r = e;
                    }

                    /*db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.InterviewQuestion OFF");
                    db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.InterviewCategory OFF");
                    db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.InterviewCategory OFF");*/
                    /*db.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[Interview] OFF");
                    db.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[InterviewCategory] OFF");
                    db.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[InterviewQuestion] OFF");*/
                    categoryList.Clear();
                    questionList.Clear();
                    answerList.Clear();
                    interviewMainList.Clear();
                    count = 1;
                }


            }

            return Json(new { Success = true });
        }

        public async Task<ActionResult> Creator(int str = 0)
        {
            switch (str)
            {
                case 0:
                    ViewBag.DepartmentList = await (from deps in db.Departments
                                                    select deps).ToListAsync();
                    ViewBag.RoleList = await (from roles in db.Roles
                                              select roles).ToListAsync();
                    ViewBag.FacultyList = await (from faculty in db.Faculties
                                                 select faculty).ToListAsync();
                    ViewBag.GroupList = await (from gr in db.Groups
                                               select gr).ToListAsync();
                    break;
                case 1:
                    ViewBag.DepartmentList = null;
                    if (saati == true)
                        ViewBag.Saati = true;
                    break;
                case 2:
                    ViewBag.DepartmentList = null;
                    ViewBag.Saati = true;
                    break;
                default:
                    ViewBag.Categories = categoryList;
                    ViewBag.Questions = questionList;
                    ViewBag.Answers = answerList;
                    ViewBag.Interview = interviewMainList;
                    return View("/Views/Interview/InterviewEndCreate.cshtml");
            }
            return View("/Views/Interview/Creator.cshtml");
        }
        public async Task<ActionResult> GetInterviewResults(int Id)
        {
            var check = (from inter in db.Interviews
                         where inter.Id == Id
                         select inter.Saati).FirstOrDefault();
            if (check == null)
            {
                /*var questions = await (from que in db.InterviewQuestions
                                       join inter in db.InterviewQuestionsOfInterviews
                                     on que.Id equals inter.IdQuestion
                                       where inter.IdInterview == interviewId
                                       select que).ToListAsync();*/
                var questions = await (from que in db.InterviewQuestions
                                       join inter in db.InterviewQuestionsOfInterviews
                                     on que.Id equals inter.IdQuestion
                                       where inter.IdInterview == Id
                                       select que).ToListAsync();
                ViewBag.Questions = questions;
                var categories = (from cat in db.InterviewCategories
                                  join inter in db.InterviewCategoriesOfInterviews
                                  on cat.Id equals inter.IdCategory
                                  where inter.IdInterview == Id
                                  select cat).ToList();

                ViewBag.Category = categories;
                /*var answers = await (from ans in db.InterviewAnswers
                                     join que in questions
                                     on ans.IdQuestion equals que.Id
                                     select ans).ToListAsync();*/
                var answers = await (from ans in db.InterviewAnswers
                                     join que in db.InterviewQuestionsOfInterviews
                                     on ans.IdQuestion equals que.IdQuestion
                                     where que.IdInterview == Id
                                     select ans).ToListAsync();
                var answers2 = await (from ans in db.InterviewAnswers
                                      where ans.IdQuestion == 0
                                      select ans).ToListAsync();
                var allanswers = answers2.Union(answers);
                ViewBag.Answers = allanswers;
                var results = await (from res in db.InterviewResults
                                     where res.IdInterview == Id
                                     select res).ToListAsync();
                List<ResultCountViewModel> resCountStandart = new List<ResultCountViewModel>();
                List<ResultCountViewModel> resCountCustom = new List<ResultCountViewModel>();
                var questions2 = new List<InterviewQuestion>();
                for (int i = 0; i < questions.Count(); i++)
                    questions2.Add(questions[i]);
                foreach (InterviewQuestion question in questions)
                {
                    var answerstemp = (from ans in allanswers
                                       where ans.IdQuestion == question.Id
                                       select ans).ToList();
                    if (answerstemp != null)
                    {
                        foreach (InterviewAnswer answer in answerstemp)
                        {
                            var temp = (from res in results
                                        where res.IdAnswer == answer.Id && res.IdQuestion == question.Id
                                        select res).Count();
                            resCountCustom.Add(new ResultCountViewModel
                            {
                                idAnswer = answer.Id,
                                idQuestion = question.Id,
                                count = temp
                            });
                            questions2 = questions2.Where(x => x.Id != question.Id).ToList();

                        }
                    }

                }
                foreach (InterviewQuestion question in questions2)
                {
                    var answerstemp = (from ans in allanswers
                                       where ans.IdQuestion == 0
                                       select ans).ToList();
                    if (answerstemp != null)
                    {
                        foreach (InterviewAnswer answer in answerstemp)
                        {
                            var temp = (from res in results
                                        where res.IdAnswer == answer.Id && res.IdQuestion == question.Id
                                        select res).Count();
                            resCountStandart.Add(new ResultCountViewModel
                            {
                                idAnswer = answer.Id,
                                idQuestion = question.Id,
                                count = temp
                            });

                        }
                    }

                }
                var resCountFinal = resCountCustom.Union(resCountStandart).ToList();
                var sortedResCountFinal = resCountFinal.OrderBy(x => x.idQuestion);
                ViewBag.ResultsCount = resCountFinal;
                List<AverageAnswersViewModel> categoryResults = new List<AverageAnswersViewModel>();

                var categoryResult = await (from res in db.InterviewCategoryResults
                                            where res.IdInterview == Id
                                            select res).ToListAsync();
                foreach (var category in categoryResult)
                {
                    categoryResults.Add(new AverageAnswersViewModel
                    {
                        //id = (from res in db.InterviewCategoryResults
                        //      where res.IdInterview == category.IdInterview
                        //      select res.IdInterview).FirstOrDefault(),
                        //average = (from res in db.InterviewCategoryResults
                        //           where res.IdInterview == category.IdInterview
                        //           select res.Result).FirstOrDefault()
                        id = category.IdCategory,
                        average = category.Result

                    });
                }
                ViewBag.AverageCategory = categoryResults;
                return View("/Views/Interview/InterviewAllResults.cshtml");
            }
            else
            {
                var department = await (from fac in db.Interviews
                                        where fac.Id == Id
                                        select fac.IdDepartment).FirstOrDefaultAsync();
                var allCategoryResultsForSaati = await (from res in db.InterviewCategoryResults
                                                        join inter in db.Interviews
                                                        on res.IdInterview equals inter.Id
                                                        where inter.Saati == check
                                                        select res).ToListAsync();
                var sortedAllCategoryResultsForSaati = allCategoryResultsForSaati.OrderBy(x => x.IdInterview);
                var categoriesForSaati = await (from cat in db.InterviewCategories
                                                join inter in db.InterviewCategoriesOfInterviews
                                                on cat.Id equals inter.IdCategory
                                                where inter.IdInterview == Id
                                                select cat).ToListAsync();
                ViewBag.Category = categoriesForSaati;
                var variantsCount = (from inter in db.Interviews
                                     where inter.Saati == check
                                     select inter).Count();
                var interviewsForSaati = await (from inter in db.Interviews
                                                where inter.Saati != null && inter.IdDepartment == department
                                                select inter).ToListAsync();
                double summArguments = 0;
                List<ListSaatiViewModel> listSaati = new List<ListSaatiViewModel>();
                foreach (var cat in categoriesForSaati)
                {
                    foreach (var inter in interviewsForSaati)
                    {
                        foreach (var inter2 in interviewsForSaati)
                        {
                            var firstArgument = sortedAllCategoryResultsForSaati.Where(x => x.IdInterview == inter2.Id && x.IdCategory == cat.Id).Select(x => x.Result).FirstOrDefault();
                            var secondArgument = sortedAllCategoryResultsForSaati.Where(x => x.IdInterview == inter.Id && x.IdCategory == cat.Id).Select(x => x.Result).FirstOrDefault();
                            double temp = 0;
                            if (firstArgument == secondArgument)
                            {
                                temp = 1;
                            }
                            else if (firstArgument == 0)
                            {
                                temp = (double)((double)1 / (double)secondArgument);
                            }
                            else if (secondArgument == 0)
                            {
                                temp = (double)firstArgument;
                            }
                            else
                            {
                                temp = (double)((double)firstArgument / (double)secondArgument);
                            }

                            summArguments += temp;

                        }
                        listSaati.Add(new ListSaatiViewModel
                        {
                            idInterview = inter.Id,
                            idCategory = cat.Id,
                            Result = summArguments
                        });
                        summArguments = 0;
                    }



                }
                var listSaati2 = new List<ListSaatiViewModel>();

                foreach (var cat in categoriesForSaati)
                {
                    foreach (var inter in interviewsForSaati)
                    {
                        foreach (var inter2 in interviewsForSaati)
                        {
                            var firstArgument = sortedAllCategoryResultsForSaati.Where(x => x.IdInterview == inter.Id && x.IdCategory == cat.Id).Select(x => x.Result).FirstOrDefault();
                            var secondArgument = sortedAllCategoryResultsForSaati.Where(x => x.IdInterview == inter2.Id && x.IdCategory == cat.Id).Select(x => x.Result).FirstOrDefault();
                            double temp = 0;
                            if (firstArgument == secondArgument)
                            {
                                temp = 1;
                            }
                            else if (firstArgument == 0)
                            {
                                temp = (double)((double)1 / (double)secondArgument);
                            }
                            else if (secondArgument == 0)
                            {
                                temp = (double)firstArgument;
                            }
                            else
                            {
                                temp = (double)((double)firstArgument / (double)secondArgument);
                            }
                            var divider = listSaati.Where(x => x.idInterview == inter2.Id && x.idCategory == cat.Id).Select(x => x.Result).FirstOrDefault();
                            listSaati2.Add(new ListSaatiViewModel
                            {
                                idInterview = inter.Id,
                                idCategory = cat.Id,
                                Result = temp / divider
                            });
                        }
                    }
                }
                double averageArgument1 = 0;
                double averageArgument2 = 0;
                double average = 0;
                var interviewTemp = 0;
                var listSaati3 = new List<ListSaatiViewModel>();
                foreach (var cat in categoriesForSaati)
                {
                    foreach (var inter in interviewsForSaati)
                    {
                        foreach (var list in listSaati2)
                        {
                            if (list.idCategory == cat.Id && list.idInterview == inter.Id)
                            {
                                averageArgument1 = list.Result;
                                averageArgument2 += averageArgument1;
                                interviewTemp = inter.Id;
                            }
                        }
                        average = averageArgument2 / interviewsForSaati.Count();
                        listSaati3.Add(new ListSaatiViewModel
                        {
                            idInterview = interviewTemp,
                            groupName = (from interview in db.Interviews
                                         where interview.Id == inter.Id
                                         join grp in db.Groups
                                         on inter.IdGroup equals grp.Id
                                         select grp.Name).FirstOrDefault(),
                            idCategory = cat.Id,
                            Result = average
                        });
                        averageArgument2 = 0;
                    }

                }
                ViewBag.Category1 = listSaati3.Where(x => x.idCategory == 1).ToList().OrderByDescending(x => x.Result);
                ViewBag.Category2 = listSaati3.Where(x => x.idCategory == 2).ToList().OrderByDescending(x => x.Result);
                ViewBag.Category3 = listSaati3.Where(x => x.idCategory == 3).ToList().OrderByDescending(x => x.Result);
                ViewBag.Category4 = listSaati3.Where(x => x.idCategory == 4).ToList().OrderByDescending(x => x.Result);
                var listOfCategories = new List<Double>() { 0.294, 0.579, 0.08, 0.047 };
                var resultSaati = new List<ListSaatiViewModel>();
                double summOfAverages = 0;
                foreach (var inter in interviewsForSaati)
                {
                    var tempList = listSaati3.Where(x => x.idInterview == inter.Id).Select(x => x.Result).ToList();
                    var count = 0;
                    foreach (var list in tempList)
                    {
                        var listCategoriesElement = listOfCategories.ElementAt(count);
                        var result = list * listCategoriesElement;
                        summOfAverages += result;

                    }
                    resultSaati.Add(new ListSaatiViewModel
                    {
                        idInterview = inter.Id,
                        //groupName = (from grp in db.Groups
                        //             join interview in db.Interviews
                        //             on inter.Id equals interview.Id
                        //             select grp.Name).FirstOrDefault(),
                        groupName = (from interview in db.Interviews
                                     where interview.Id == inter.Id
                                     join grp in db.Groups
                                     on inter.IdGroup equals grp.Id
                                     select grp.Name).FirstOrDefault(),
                        Result = summOfAverages
                    });
                    summOfAverages = 0;
                }
                ViewBag.SaatiResults = resultSaati.OrderByDescending(x => x.Result);
                return View("/Views/Interview/InterviewRanking.cshtml");
            }



            ////////////////////////////////////////////////////////////////////////



        }



        [HttpPost]
        public async Task<ActionResult> OpenInterviewToUser(int UserId, int InterviewId)
        {
            var done = await (from don in db.InterviewDones
                              where don.IdInterview == InterviewId && don.IdUser == UserId
                              select don).FirstOrDefaultAsync();
            var results = await (from res in db.InterviewResults
                                 where res.IdInterview == InterviewId && res.IdUser == UserId
                                 select res).ToListAsync();
            if (done != null && results.Count != 0)
            {
                /*foreach (InterviewResult result in results)
                {
                    db.InterviewResults.Remove(result);
                }*/
                //db.InterviewDones.Remove(done);
                DeleteCategoryResults(UserId, InterviewId);
                db.SaveChanges();
                return Json(new { Success = true });
            }
            else
                return Json(new { Success = false, Error = "This user not do this interview" });
        }

        public async Task<ActionResult> OpenInterviewToGroup(int? GroupId, int InterviewId)
        {
            if (GroupId != null)
            {
                var userList = await (from usr in db.Users
                                      where usr.IdGroup == GroupId
                                      select usr).ToListAsync();
                foreach (User us in userList)
                {
                    var done = await (from don in db.InterviewDones
                                      where don.IdInterview == InterviewId && don.IdUser == us.Id
                                      select don).FirstOrDefaultAsync();
                    var results = await (from res in db.InterviewResults
                                         where res.IdInterview == InterviewId && res.IdUser == us.Id
                                         select res).ToListAsync();
                    if (done != null && results.Count != 0)
                    {
                        foreach (InterviewResult result in results)
                        {
                            db.InterviewResults.Remove(result);
                        }
                        db.InterviewDones.Remove(done);
                        db.SaveChanges();
                    }


                }
                return Json(new { Success = true });
            }
            else
                return Json(new { Success = false, Error = "Type a GroupId" });
        }

        public Task<ActionResult> GetInterview(int Id)
        {
            interviewId = Id;
            var question = (from que in db.InterviewQuestions
                            join inter in db.InterviewQuestionsOfInterviews
                          on que.Id equals inter.IdQuestion
                            where inter.IdInterview == interviewId
                            select que.Id).FirstOrDefault();

            return GetQuestion(question);
        }
        [HttpPost]
        public async Task<ActionResult> SaveAnswer(int IdQuestion, int IdAnswer)
        {
            var user = await GetCurrentUser();
            if (IdAnswer != 0)
            {
                var newResult = new InterviewResult
                {
                    IdInterview = interviewId,
                    IdQuestion = IdQuestion - 1,
                    IdAnswer = IdAnswer,
                    IdUser = user.Id
                };
                resultList.Add(newResult);
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false, Error = "Error" });
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetQuestion(int Id)
        {
            var user = await GetCurrentUser();
            //var QuestionsCount = (from inter in db.Interviews
            //                      where inter.Id == interviewId
            //                      join que in db.InterviewQuestions
            //                      on inter.Id equals que.IdInterview
            //                      select que.Id).ToList().Last();


            var QuestionsCount = (from que in db.InterviewQuestions
                                  join inter in db.InterviewQuestionsOfInterviews
                                on que.Id equals inter.IdQuestion
                                  where inter.IdInterview == interviewId
                                  select que.Id).ToList().Last();
            if (QuestionsCount + 1 == Id)
            {
                foreach (InterviewResult result in resultList)
                {
                    db.InterviewResults.Add(result);
                    await db.SaveChangesAsync();
                }
                resultList.Clear();
                var interviewCheck = (from inter in db.Interviews
                                      where inter.Id == interviewId
                                      select inter.Saati).FirstOrDefault();
                if (interviewCheck != 0)
                {
                    SaveCategoryResults(interviewId);
                }

                return GetEndPage(user.Id);
            }
            while (QuestionNotNull == false)
            {
                /*var question = await (from inter in db.Interviews
                                      where inter.Id == interviewId
                                      join que in db.InterviewQuestions
                                      on inter.Id equals que.IdInterview
                                      where que.Id == Id
                                      select que).FirstOrDefaultAsync();*/
                var question = await (from que in db.InterviewQuestions
                                      where que.Id == Id
                                      select que).FirstOrDefaultAsync();
                //var question = await (from que in db.InterviewQuestions
                //                               where que.IdInterview == interviewId
                //                               select que).FirstOrDefaultAsync();

                //var questionId = await (from inter in db.Interviews
                //                        where inter.Id == interviewId
                //                        join que in db.InterviewQuestions
                //                        on inter.Id equals que.IdInterview
                //                        where que.Id == Id
                //                        select que.Id).FirstOrDefaultAsync();

                /*ViewBag.Category2 = await (from cat in db.InterviewCategories
                                          where cat.IdInterview == interviewId
                                          join que in db.InterviewQuestions
                                          on cat.Id equals que.IdCategory
                                          select cat).ToListAsync();*/


                ViewBag.AnswersOne = await (from ans in db.InterviewAnswers
                                            select ans).ToListAsync();
                if (question == null)
                    Id++;
                else
                {
                    QuestionNotNull = true;
                    var questionCategory = question.IdCategory;
                    /*var category = await (from que in db.InterviewQuestions
                                          where que.IdInterview == interviewId
                                          join cat in db.InterviewCategories
                                          on que.IdCategory equals cat.Id
                                          where que.Id == question.Id
                                          select cat).FirstOrDefaultAsync();*/
                    var category = await (from cat in db.InterviewCategories
                                          where cat.Id == questionCategory
                                          select cat).FirstOrDefaultAsync();
                    ViewBag.QuestionsText = question.Question;
                    ViewBag.QuestionsId = question.Id;
                    ViewBag.Category = category;
                }

            }
            return View("/Views/Interview/InterviewPage.cshtml");
        }

        public ActionResult GetEndPage(int user)
        {
            var check = (from inter in db.Interviews
                         where inter.Id == interviewId
                         select inter.Saati).FirstOrDefault();
            ViewBag.QuestionList = (from que in db.InterviewQuestions
                                    join inter in db.InterviewQuestionsOfInterviews
                                  on que.Id equals inter.IdQuestion
                                    where inter.IdInterview == interviewId
                                    select que).ToList();
            var category = (from cat in db.InterviewCategories
                            join inter in db.InterviewCategoriesOfInterviews
                            on cat.Id equals inter.IdCategory
                            where inter.IdInterview == interviewId
                            select cat).ToList();
            /*if (category.Count == 0 && check !=null)
            {
                category = (from cat in db.InterviewCategories
                            where cat.IdInterview == null
                            select cat).ToList();
            }*/
            ViewBag.Category = category;
            ViewBag.ResultList = (from res in db.InterviewResults
                                  where res.IdInterview == interviewId && res.IdUser == user
                                  join ans in db.InterviewAnswers
                                       on res.IdAnswer equals ans.Id
                                  select new ResultsViewModel
                                  {
                                      id_answer = res.IdAnswer,
                                      id_interview = res.IdInterview,
                                      id_question = res.IdQuestion,
                                      id_user = res.IdUser,
                                      answer = ans.Answer
                                  }).ToList();
            var newDone = new InterviewDone
            {
                IdInterview = interviewId,
                IdUser = user
            };
            db.InterviewDones.Add(newDone);
            db.SaveChanges();
            return View("/Views/Interview/EndPage.cshtml");
        }

        /*public async Task<ActionResult> OpenInterview(int Id)
        {
            ViewBag.Questions = await (from inter in db.Interviews
                                       where inter.Id == Id
                                       join que in db.InterviewQuestions
                                       on inter.Id equals que.IdInterview
                                       select que).ToListAsync();
            ViewBag.Answers = await (from ans in db.InterviewAnswers
                                     select ans).ToListAsync();
            return View("/Views/Interview/InterviewPage.cshtml");
        }*/

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