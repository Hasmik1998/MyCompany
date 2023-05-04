using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chess_Up.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCompany.Data;
using MyCompany.Models;

namespace MyCompany.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// DB context
        /// </summary>
        private readonly CompanyDbContext _context;

        private static ForumModel FindedQuestions = new ForumModel();

        private static bool Searched;

        public HomeController(ILogger<HomeController> logger, CompanyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Main page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// About us view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AboutUs()
        {
            return View();
        }

        /// <summary>
        /// Get Contact us
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ContactUs()
        {
            ViewBag.ShowHeroImage = false;
            return View();
        }

        /// <summary>
        /// Contact us post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ContactUs(MailModel model)
        {
            model.Subject = "Contact Us";
            MailSenderService.SendMail(model);
            return RedirectToAction("ContactUs");
        }

        /// <summary>
        /// Privacy view
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Get review
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Review()
        {
            return View(_context.Reviews.ToList());
        }

        /// <summary>
        /// Get review form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ReviewForm()
        {
            return View();
        }

        /// <summary>
        /// Post review form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReviewForm(ReviewViewModel model)
        {
            Review review = new Review()
            {
                Name = string.IsNullOrEmpty(model.Name) ? "Unknown" : model.Name,
                ReviewText = model.Review
            };

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Review");
        }

        /// <summary>
        /// Delete Review
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/Home/Review/DeleteReview/{Id}")]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DeleteReview(int Id)
        {
            var review = _context.Reviews.FirstOrDefault(p => p.Id == Id);
            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return RedirectToAction("Review");
        }

        /// <summary>
        /// Questions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult StackOverflowQuestions()
        {
            ForumModel model = new ForumModel();
            if (Searched)
            {
                ViewBag.ShowAddQuestion = true;
                model = FindedQuestions;
                Searched = false;

                return View(model);
            }

            var items = _context.StackOverflowQuestions.ToList();
            model.Questions = items;
            //model.SearchText = "Search question";
            ViewBag.ShowAddQuestion = true;
            return View(model);
        }

        /// <summary>
        /// Answers to Questions
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/Home/StackOverflowAnswers/{Id}")]
        public IActionResult StackOverflowAnswers(int Id)
        {
            var question = _context.StackOverflowQuestions.FirstOrDefault(a => a.Id == Id);
            List<StackOverflowAnswer> result = _context.StackOverflowAnswers.ToList();
            var answers = result.FindAll(a => a.QuestionId == question.Id);
            (StackOverflowQuestion, List<StackOverflowAnswer>) tup = (question, answers);

            return View(tup);
        }

        /// <summary>
        /// Get Questions form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult StackOverflowQuestionForm()
        {
            return View();
        }

        /// <summary>
        /// Post Questions Form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> StackOverflowQuestionForm(StackOverflowQuestionModel model)
        {
            try
            {
                string userName = User.Identity.Name;
                StackOverflowQuestion question = new StackOverflowQuestion()
                {
                    UserName = userName,
                    Question = model.Question
                };

                await _context.StackOverflowQuestions.AddAsync(question);
                await _context.SaveChangesAsync();

                return RedirectToAction("StackOverflowQuestions");
            }
            catch (System.Exception)
            {
                return RedirectToAction("StackOverflowQuestions");
            }
        }

        /// <summary>
        /// Get Answers Form
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/Home/StackOverflowAnswerForm/{Id}")]
        [Authorize]
        public IActionResult StackOverflowAnswerForm(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }

        /// <summary>
        /// Post Question Answers
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("~/Home/StackOverflowAnswerForm/{Id}")]
        [Authorize(Roles = "User,SimpleUser")]
        public async Task<IActionResult> StackOverflowAnswerForm(int Id, StackOverflowAnswerModel model)
        {
            try
            {
                string userName = User.Identity.Name;
                StackOverflowAnswer answer = new StackOverflowAnswer()
                {
                    QuestionId = Id,
                    UserName = userName,
                    Answer = model.Answer
                };

                await _context.StackOverflowAnswers.AddAsync(answer);
                await _context.SaveChangesAsync();

                return RedirectToAction($"StackOverflowAnswers", new { Id = Id });
            }
            catch (Exception e)
            {
                return View();
            }
        }

        /// <summary>
        /// Delete question 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/Home/StackOverflowQuestions/DeleteStackOverflowQuestion/{Id}")]
        [Authorize]
        [HttpGet]
        public IActionResult DeleteStackOverflowQuestion(int Id)
        {
            var question = _context.StackOverflowQuestions.FirstOrDefault(p => p.Id == Id);
            _context.StackOverflowQuestions.Remove(question);
            _context.SaveChanges();

            return RedirectToAction("StackOverflowQuestions");
        }

        /// <summary>
        /// Get edited question
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/Home/StackOverflowQuestions/EditStackOverflowQuestionForm/{Id}")]
        [HttpGet]
        [Authorize]
        public IActionResult EditStackOverflowQuestionForm(int Id)
        {
            ViewBag.Id = Id;

            var question = _context.StackOverflowQuestions.FirstOrDefault(p => p.Id == Id);
            var model = new StackOverflowQuestionModel()
            {
                Question = question.Question
            };
            return View(model);
        }

        /// <summary>
        /// Post edited question
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/Home/StackOverflowQuestions/EditStackOverflowQuestionForm/{Id}")]
        [HttpPost]
        [Authorize]
        public IActionResult EditStackOverflowQuestionForm(StackOverflowQuestionModel model, int Id)
        {
            StackOverflowQuestion question = _context.StackOverflowQuestions.FirstOrDefault(a => a.Id == Id);
            question.Question = model.Question;

            _context.StackOverflowQuestions.Update(question);
            _context.SaveChanges();
            return RedirectToAction("StackOverflowQuestions");
        }

        /// <summary>
        /// Delete Answer
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/Home/StackOverflowAnswers/DeleteStackOverflowQuestion/{Id}")]
        [Authorize]
        [HttpGet]
        public IActionResult DeleteStackOverflowAnswer(int Id)
        {
            var question = _context.StackOverflowAnswers.FirstOrDefault(p => p.Id == Id);
            _context.StackOverflowAnswers.Remove(question);
            _context.SaveChanges();

            return RedirectToAction($"StackOverflowAnswers", new { Id = question.QuestionId });
        }

        /// <summary>
        /// Get request for Edit Answer Form
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/Home/StackOverflowQuestions/EditStackOverflowAnswerForm/{Id}")]
        [HttpGet]
        [Authorize]
        public IActionResult EditStackOverflowAnswerForm(int Id)
        {
            ViewBag.Id = Id;
            var answer = _context.StackOverflowAnswers.FirstOrDefault(a => a.Id == Id);

            StackOverflowAnswerModel model = new StackOverflowAnswerModel()
            {
                Answer = answer.Answer
            };

            return View(model);
        }

        /// <summary>
        /// Post Request for Edit Answer Form
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/Home/StackOverflowQuestions/EditStackOverflowAnswerForm/{Id}")]
        [HttpPost]
        [Authorize]
        public IActionResult EditStackOverflowAnswerForm(StackOverflowAnswerModel model, int Id)
        {
            StackOverflowAnswer answer = _context.StackOverflowAnswers.FirstOrDefault(a => a.Id == Id);
            answer.Answer = model.Answer;

            _context.StackOverflowAnswers.Update(answer);
            _context.SaveChanges();
            return RedirectToAction($"StackOverflowAnswers", new { Id = answer.QuestionId });
        }

        //[Route("~/Home/StackOverflowQuestions/Search/{SearchText}")]
        public IActionResult Search(string SearchText)
        {
            Searched = true;
            FindedQuestions.SearchText = SearchText;

            SearchText = SearchText.ToLower();
            List<StackOverflowQuestion> questions = _context.StackOverflowQuestions.ToList();
            List<StackOverflowQuestion> result = new List<StackOverflowQuestion>();

            if (FindedQuestions.Questions == null)
            {
                FindedQuestions.Questions = new List<StackOverflowQuestion>();
            }

            if (FindedQuestions.Questions.Count > 0)
            {
                FindedQuestions.Questions.Clear();
            }

            foreach (var item in questions)
            {
                if (item.Question.ToLower().Contains(SearchText))
                {
                    result.Add(item);
                }
            }

            FindedQuestions.Questions = result;

            return RedirectToAction("StackOverflowQuestions");
        }
    }
}
