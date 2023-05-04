using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCompany.Data;
using MyCompany.Models;
using MyCompany.Utilities;

namespace MyCompany.Controllers
{
    public class ProblemController : Controller
    {
        private readonly CompanyDbContext _context;

        public ProblemController(CompanyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Problem index
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ProblemIndex()
        {
            ViewBag.ShowProblem = true;
            return View(_context.Problems.ToList()); //TODO
        }

        /// <summary>
        /// Get problem form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = ("User,Admin"))]
        public IActionResult ProblemForm()
        {
            return View();
        }

        /// <summary>
        /// Post problem form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = ("User,Admin"))]
        public async Task<IActionResult> ProblemForm(ProblemViewModel model)
        {
            var tempImage = System.Drawing.Image.FromStream(model.Avatar.OpenReadStream());
            var result = Utility.ResizeImage(tempImage, 245, 255);
            var base64 = string.Format("data:image/jpg; base64, {0}", Convert.ToBase64String(Utility.ImageToByte2(result)));
            var Problem = new Problem()
            {
                ProblemImage = base64,
                Description = model.Description,
                Solution = model.Solution,
                Level = model.Level
            };

            await _context.Problems.AddAsync(Problem);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProblemIndex"); //TODO

        }

        /// <summary>
        /// Delete problem
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/Problem/ProblemIndex/Delete/{Id}")]
        [Authorize(Roles = ("User,Admin"))]
        [HttpGet]
        public IActionResult DeleteProblem(int Id)
        {
            var problem = _context.Problems.FirstOrDefault(p => p.Id == Id);
            _context.Problems.Remove(problem);
            _context.SaveChanges();
            return RedirectToAction("ProblemIndex");
        }

        /// <summary>
        /// Delete quiz
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/Problem/ProblemIndex/DeleteQuiz/{Id}")]
        [Authorize(Roles = ("User,Admin"))]
        [HttpGet]
        public IActionResult DeleteQuiz(int Id)
        {
            var quiz = _context.QuizProblems.FirstOrDefault(p => p.Id == Id);
            _context.QuizProblems.Remove(quiz);
            _context.SaveChanges();
            return RedirectToAction("QuizIndex");
        }

        /// <summary>
        /// Quiz index
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult QuizIndex()
        {
            ViewBag.ShowQuiz = true;
            return View(_context.QuizProblems.ToList()); //TODO
        }

        /// <summary>
        /// Get Quiz form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = ("User,Admin"))]
        public IActionResult QuizForm()
        {
            return View(); //TODO
        }

        /// <summary>
        /// Post Quiz form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = ("User,Admin"))]
        public async Task<IActionResult> QuizForm(QuizViewModel model)
        {
            var QuizProblem = new Quiz()
            {
                OwnerName = model.OwnerName,
                QuizLink = model.QuizLink,
                Level = model.Level
            };

            await _context.QuizProblems.AddAsync(QuizProblem);
            await _context.SaveChangesAsync();
            return RedirectToAction("QuizIndex");
        }
    }
}