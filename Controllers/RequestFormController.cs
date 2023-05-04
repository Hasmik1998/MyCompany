using System;
using System.Collections.Generic;
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
    public class RequestFormController : Controller
    {
        /// <summary>
        /// DB context
        /// </summary>
        private readonly CompanyDbContext _context;

        /// <summary>
        /// Cunstructor
        /// </summary>
        /// <param name="context"></param>
        public RequestFormController(CompanyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Request index
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult RequestIndex()
        {
            ViewBag.ShowRequest = true;
            return View(_context.Requests.ToList());
        }

        /// <summary>
        /// Get request form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = ("User,Admin"))]
        public IActionResult MyForm()
        {
            return View();
        }

        /// <summary>
        /// Post request form
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = ("User,Admin"))]
        public async Task<IActionResult> MyForm(RequestModel model)
        {
            //if (!ModelState.IsValid)
            //    return View(model);
            try
            {
                var tempImage = System.Drawing.Image.FromStream(model.Avatar.OpenReadStream());
                var result = Utility.ResizeImage(tempImage, 308, 173);
                var base64 = string.Format("data:image/jpg; base64, {0}", Convert.ToBase64String(Utility.ImageToByte2(result)));

                var newRequest = new Request()
                {
                    Name = model.Name,
                    SurName = model.SurName,
                    Phone = model.Phone,
                    Email = User.Identity.Name,
                    About = model.About,
                    MyRequest = model.MyRequest,
                    BankAccount = model.BankAccount,
                    Avatar = base64,
                    Accept = false
                };

                await _context.Requests.AddAsync(newRequest);
                await _context.SaveChangesAsync();

                IEnumerable<Request> request = _context.Requests;
                if (newRequest.Accept == false)
                {
                    string strMsg = "Your application is under review. We will post it as soon as possible. ";
                    string script = "<script language=\"javascript\" type=\"text/javascript\">alert('" + strMsg + "'); window.location='RequestIndex'; </script>";
                    await Response.WriteAsync(script);

                    return RedirectToAction("RequestIndex");

                }
                else
                {
                    return RedirectToAction("RequestIndex");
                }
            }
            catch (Exception ex)
            {
                string strMsg = "Something Went Wrong or not all required fileds are filled try again";
                string script = "<script language=\"javascript\" type=\"text/javascript\">alert('" + strMsg + "'); window.location='MyForm'; </script>";
                await Response.WriteAsync(script);

                return View();
            }
        }

        /// <summary>
        /// Get choose Requests
        /// </summary>
        /// <returns></returns>
        [Route("~/RequestForm/ChoosRequests")]
        [HttpGet]
        public IActionResult ChoosRequests()
        {
            return View();
        }

        /// <summary>
        /// Get edit request
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/RequestForm/RequestIndex/Edit/{Id}")]
        [HttpGet]
        [Authorize(Roles = ("User,Admin"))]
        public IActionResult EditRequest(int Id)
        {
            var request = _context.Requests.FirstOrDefault(p => p.Id == Id);
            var model = new RequestModel()
            {
                Name = request.Name,
                SurName = request.SurName,
                Phone = request.Phone,
                Email = request.Email,
                About = request.About,
                MyRequest = request.MyRequest,
                UserImage = request.Avatar,
                BankAccount = request.BankAccount,
                Accept = true
            };
            return View(model);
        }

        /// <summary>
        /// Post Edit request
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/RequestForm/RequestIndex/Edit/{Id}")]
        [HttpPost]
        [Authorize(Roles = ("User,Admin"))]
        public IActionResult EditRequest(RequestModel model, int Id)
        {
            var base64 = "";
            if (model.Avatar != null)
            {
                var tempImage = System.Drawing.Image.FromStream(model.Avatar.OpenReadStream());
                var result = Utility.ResizeImage(tempImage, 308, 173);
                base64 = string.Format("data:image/jpg; base64, {0}", Convert.ToBase64String(Utility.ImageToByte2(result)));
                model.UserImage = base64;
            }
            var request = _context.Requests.FirstOrDefault(p => p.Id == Id);
            request.Name = model.Name;
            request.SurName = model.SurName;
            request.Phone = model.Phone;
            request.Email = string.IsNullOrEmpty(model.Email) ? string.Empty : model.Email;
            request.About = model.About;
            request.BankAccount = model.BankAccount;
            request.MyRequest = model.MyRequest;
            request.Accept = true;
            request.Avatar = string.IsNullOrEmpty(model.UserImage) ? request.Avatar : base64;
            _context.Requests.Update(request);
            _context.SaveChanges();
            return RedirectToAction("RequestIndex");
        }

        /// <summary>
        /// Delete request
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/RequestForm/RequestIndex/Delete/{Id}")]
        [Authorize(Roles = ("User,Admin"))]
        [HttpGet]
        public IActionResult DeleteRequest(int Id)
        {
            var room = _context.Requests.FirstOrDefault(p => p.Id == Id);
            _context.Requests.Remove(room);
            _context.SaveChanges();
            return RedirectToAction("RequestIndex");
        }

        /// <summary>
        /// Accept Request
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/RequestForm/RequestIndex/Accept/{Id}")]
        [Authorize(Roles = ("User,Admin"))]
        [HttpGet]
        public IActionResult AcceptRequest(int Id)
        {
            var room = _context.Requests.FirstOrDefault(p => p.Id == Id);

            room.Accept = true;

            _context.Requests.Update(room);
            _context.SaveChanges();
            return RedirectToAction("RequestIndex");
        }

        /// <summary>
        /// Mark request as done
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = ("User,Admin"))]
        public IActionResult MarkRequestAsDone(int Id)
        {
            var req = _context.Requests.FirstOrDefault(p => p.Id == Id);
            req.Done = true;
            _context.Requests.Update(req);
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
            return View(_context.Requests.ToList());
        }

    }
}