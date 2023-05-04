using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCompany.Models;
using MyCompany.Utilities;
using MyCompany.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNet.SignalR;

namespace MyCompany.Controllers
{
    public class TrinerRequestFormController : Controller
    {
        private readonly CompanyDbContext _context;

        public TrinerRequestFormController(CompanyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Trainer Request Index
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult TrinerRequestIndex()
        {
            ViewBag.ShowTrinerRequest = true;
            return View(_context.TrinerRequests.ToList());
        }

        /// <summary>
        /// Get Form for request 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = ("User,Admin"))]
        public IActionResult MyForm()
        {
            return View();
        }

        /// <summary>
        /// Post form for request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = ("User,Admin"))]
        public async Task<IActionResult> MyForm(TrinerRequestModel model)
        {
            var tempImage = System.Drawing.Image.FromStream(model.Avatar.OpenReadStream());
            var result = Utility.ResizeImage(tempImage, 308, 173);
            var base64 = string.Format("data:image/jpg; base64, {0}", Convert.ToBase64String(Utility.ImageToByte2(result)));
            var newRequest = new TrinerRequest()
            {
                Name = model.Name,
                SurName = model.SurName,
                Phone = model.Phone,
                Email = string.IsNullOrEmpty(model.Email) ? string.Empty : model.Email,
                About = model.About,
                MyRequest = model.MyRequest,
                Avatar = base64,
                Accept = false
            };

            await _context.TrinerRequests.AddAsync(newRequest);
            await _context.SaveChangesAsync();

            if (newRequest.Accept == false)
            {
                string strMsg = "Your application is under review. We will post it as soon as possible. ";
                string script = "<script language=\"javascript\" type=\"text/javascript\">alert('" + strMsg + "'); window.location='TrinerRequestIndex'; </script>";
                await Response.WriteAsync(script);

                return RedirectToAction("TrinerRequestIndex");
            }
            else
            {
                return RedirectToAction("TrinerRequestIndex");
            }
        }

        /// <summary>
        /// Get Edit request, this for only admin
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/TrinerRequestForm/TrinerRequestIndex/Edit/{Id}")]
        [HttpGet]
        [Authorize(Roles = ("User,Admin"))]
        public IActionResult EditRequest(int Id)
        {
            var request = _context.TrinerRequests.FirstOrDefault(p => p.Id == Id);
            var model = new TrinerRequestModel()
            {
                Name = request.Name,
                SurName = request.SurName,
                Phone = request.Phone,
                Email = request.Email,
                About = request.About,
                MyRequest = request.MyRequest,
                UserImage = request.Avatar,
                Accept = true
            };
            return View(model);
        }

        /// <summary>
        /// Post Edit request, this for only admin
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/TrinerRequestForm/TrinerRequestIndex/Edit/{Id}")]
        [HttpPost]
        [Authorize(Roles = ("User,Admin"))]
        public IActionResult EditRequest(TrinerRequestModel model, int Id)
        {
            var base64 = "";
            if (model.Avatar != null)
            {
                var tempImage = System.Drawing.Image.FromStream(model.Avatar.OpenReadStream());
                var result = Utility.ResizeImage(tempImage, 308, 173);
                base64 = string.Format("data:image/jpg; base64, {0}", Convert.ToBase64String(Utility.ImageToByte2(result)));
                model.UserImage = base64;
            }
            var request = _context.TrinerRequests.FirstOrDefault(p => p.Id == Id);
            request.Name = model.Name;
            request.SurName = model.SurName;
            request.Phone = model.Phone;
            request.Email = string.IsNullOrEmpty(model.Email) ? string.Empty : model.Email;
            request.About = model.About;
            request.MyRequest = model.MyRequest;
            request.Accept = true;
            request.Avatar = string.IsNullOrEmpty(model.UserImage) ? request.Avatar : base64;
            _context.TrinerRequests.Update(request);
            _context.SaveChanges();
            return RedirectToAction("TrinerRequestIndex");
        }

        /// <summary>
        /// Delete Request
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/TrinerRequestForm/TrinerRequestIndex/Delete/{Id}")]
        [HttpGet]
        [Authorize(Roles = ("User,Admin"))]
        public IActionResult DeleteRequest(int Id)
        {
            var room = _context.TrinerRequests.FirstOrDefault(p => p.Id == Id);
            _context.TrinerRequests.Remove(room);
            _context.SaveChanges();
            return RedirectToAction("TrinerRequestIndex");
        }

        /// <summary>
        /// Accept Request
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/TrinerRequestForm/TrinerRequestIndex/Accept/{Id}")]
        [HttpGet]
        [Authorize(Roles = ("User,Admin"))]
        public IActionResult AcceptRequest(int Id)
        {
            var request = _context.TrinerRequests.FirstOrDefault(p => p.Id == Id);

            request.Accept = true;

            _context.TrinerRequests.Update(request);
            _context.SaveChanges();
            return RedirectToAction("TrinerRequestIndex");
        }

        /// <summary>
        /// Mark as done 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = ("User,Admin"))]
        public IActionResult MarkRequestAsDone(int Id)
        {
            var req = _context.TrinerRequests.FirstOrDefault(p => p.Id == Id);
            req.Done = true;
            _context.TrinerRequests.Update(req);
            _context.SaveChanges();
            return RedirectToAction("DoneRequests");
        }

        /// <summary>
        /// Completed Financial Requests
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DoneRequests()
        {
            return View(_context.TrinerRequests.ToList());
        }

    }
}
