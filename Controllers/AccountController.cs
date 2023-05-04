using Chess_Up.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCompany.Data;
using MyCompany.Models;
using MyCompany.Repository;
using MyCompany.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private readonly CompanyDbContext _context;
        private static UsersModel FindedUsers = new UsersModel();
        private static bool Searched;
        private IRepository _repository;

        public AccountController(UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signInMgr, CompanyDbContext context, IRepository repository)
        {
            _userManager = userMgr;
            _signInManager = signInMgr;
            _context = context;
            _repository = repository;
        }

        /// <summary>
        /// Profile
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [Route("~/Account/Profile/{UserName}")]
        [HttpGet]
        //[Authorize(Roles = "User,SimpleUser")]
        public IActionResult Profile(string UserName)
        {
            var user = _repository.GetUserInfoByUserName(UserName);

            if (user != null)
            {
                ViewBag.Found = true;
            }
            else 
            {
                ViewBag.Found = false;
            }

            return View(user);
        }

        /// <summary>
        /// Get login view 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Post Login 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Name);

            if (user == null)
            {
                return RedirectToAction("Login");
            }
            if ((await _signInManager.PasswordSignInAsync(user, model.Password, false, false)).Succeeded)
            {
                return Redirect("/RequestForm/ChoosRequests");
            }

            return View(model);
        }

        /// <summary>
        /// LogOut
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public async Task<RedirectResult> SignOut(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        /// <summary>
        /// Get registration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        /// <summary>
        /// Post registration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationRequestModel model)
        {
            string result = _repository.Registration(model);
            if (string.IsNullOrEmpty(result))
            {
                return RedirectToAction("Registration");
            }
            else 
            {
                await Response.WriteAsync(result);
                return RedirectToAction("Registration");
            }
        }

        /// <summary>
        /// User request index
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult UserRequestIndex()
        {
            ViewBag.ShowRequest = true;

            return View(_repository.GetUsers());
        }

        /// <summary>
        /// Delete request
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRequest(int Id)
        {
            var model = _context.RegistrationRequests.FirstOrDefault(p => p.Id == Id);
            IdentityUser user = await _userManager.FindByEmailAsync(model.Email);
            await _userManager.DeleteAsync(user);

            _context.RegistrationRequests.Remove(model);
            _context.SaveChanges();

            MailModel mail = new MailModel();
            mail.ToMail = new System.Collections.Generic.List<string>();
            mail.ToMail.Add(model.Email);
            mail.Subject = "Account Ban";
            mail.Body = $"Dear {model.Name} {model.SurName}, This email is to notify you that your account has been deleted. If you want to know the specific reason of the denial feel free to give us a call with the number provided on our website";

            MailSenderService.SendMail(mail);

            return RedirectToAction("UserRequestIndex");
        }

        /// <summary>
        /// Delete from database
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteDatabase(int Id)
        {
            _repository.DeleteDatabase(Id);


            return RedirectToAction("UserRequestIndex");
        }

        /// <summary>
        /// Accept registration Request
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/Account/UserRequestIndex/AcceptRequest/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AcceptRequest(int Id)
        {
            var model = _context.RegistrationRequests.FirstOrDefault(p => p.Id == Id);
            IdentityUser user = new IdentityUser()
            {
                UserName = model.Email,
                Email = model.Email,
            };

            string role = "";

            if (model.UserType == Utilities.UserType.TrainnerUser)
            {
                role = "User";
            }
            else
            {
                role = "SimpleUser";
            }

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                return RedirectToAction("UserRequestIndex");
            }

            model.Accept = true;

            _context.RegistrationRequests.Update(model);
            _context.SaveChanges();

            MailModel mail = new MailModel();
            mail.ToMail = new System.Collections.Generic.List<string>();
            mail.ToMail.Add(model.Email);
            mail.Subject = "Account approval";
            mail.Body = $"Dear {model.Name} {model.SurName}, This email is to notify you that your account has been approved, you're free to login now and enjoy all the features of the website";

            MailSenderService.SendMail(mail);

            return RedirectToAction("UserRequestIndex");
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "User,SimpleUser")]
        public IActionResult Users()
        {
            UsersModel model = new UsersModel();
            List<RegistrationRequest> users = _context.RegistrationRequests.ToList();

            if (Searched)
            {
                model = FindedUsers;
                Searched = false;

                return View(model);
            }

            // remove current user
            users.Remove(users.FirstOrDefault(a => a.Email == User.Identity.Name));

            model.Users = users;
            return View(model);
        }


        [HttpGet]
        [Route("~/Account/Profile/EditProfile/{Id}")]
        [Authorize]
        public IActionResult EditProfile(int Id)
        {
            ViewBag.Id = Id;
            var user = _context.RegistrationRequests.FirstOrDefault(a => a.Id == Id);
            RegistrationRequestModel model = new RegistrationRequestModel()
            {
                Name = user.Name,
                SurName = user.SurName,
                Phone = user.Phone,
                Email = user.Email,
                About = user.About
            };

            return View(model);
        }

        [HttpPost]
        [Route("~/Account/Profile/EditProfile/{Id}")]
        [Authorize]
        public async Task<IActionResult> EditProfile(RegistrationRequestModel model, int Id)
        {
            try
            {
                bool mailChanged = false;
                var base64 = "";
                if (model.Avatar != null)
                {
                    var tempImage = System.Drawing.Image.FromStream(model.Avatar.OpenReadStream());
                    var result = Utility.ResizeImage(tempImage, 300, 300);
                    base64 = string.Format("data:image/jpg; base64, {0}", Convert.ToBase64String(Utility.ImageToByte2(result)));
                    model.UserImage = base64;
                }

                var user = _context.RegistrationRequests.FirstOrDefault(a => a.Id == Id);

                user.Name = string.IsNullOrEmpty(model.Name) ? user.Name : model.Name;
                user.SurName = string.IsNullOrEmpty(model.SurName) ? user.SurName : model.SurName;
                user.Phone = string.IsNullOrEmpty(model.Phone) ? user.Phone : model.Phone;
                user.About = string.IsNullOrEmpty(model.About) ? user.About : model.About;
                user.Image = string.IsNullOrEmpty(model.UserImage) ? user.Image : model.UserImage;

                IdentityUser currentUser = await _userManager.FindByEmailAsync(user.Email);

                if (!string.IsNullOrEmpty(model.Password))
                {
                    await _userManager.ChangePasswordAsync(currentUser, user.Password, model.Password);

                    user.Password = model.Password;
                }


                if (!model.Email.Equals(user.Email))
                {
                    mailChanged = true;
                    string token = await _userManager.GenerateChangeEmailTokenAsync(currentUser, model.Email);
                    await _userManager.ChangeEmailAsync(currentUser, model.Email, token);
                    await _userManager.SetUserNameAsync(currentUser, model.Email);

                    user.Email = model.Email;
                }

                _context.Update(user);
                await _context.SaveChangesAsync();

                if (mailChanged)
                {
                    return RedirectToAction("SignOut");
                }

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("UserRequestIndex");
                }
                return RedirectToAction("Profile", new { UserName = user.Email });
            }
            catch (Exception ex)
            {
                string strMsg = "Something Went Wrong or not all required fileds are filled try again";
                string script = "<script language=\"javascript\" type=\"text/javascript\">alert('" + strMsg + "'); window.location='EditProfile'; </script>";
                await Response.WriteAsync(script);

                return View(model);
            }
        }

        public IActionResult Search(string SearchText)
        {
            Searched = true;
            FindedUsers.SearchText = SearchText;

            SearchText = SearchText.ToLower();
            List<RegistrationRequest> users = _context.RegistrationRequests.ToList();
            List<RegistrationRequest> result = new List<RegistrationRequest>();

            if (FindedUsers.Users == null)
            {
                FindedUsers.Users = new List<RegistrationRequest>();
            }

            if (FindedUsers.Users.Count > 0)
            {
                FindedUsers.Users.Clear();
            }

            foreach (var item in users)
            {
                if (item.Name.ToLower().Contains(SearchText) || item.SurName.ToLower().Contains(SearchText))
                {
                    result.Add(item);
                }
            }

            FindedUsers.Users = result;

            return RedirectToAction("Users");
        }
    }
}
