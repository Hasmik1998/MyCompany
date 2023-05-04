using Chess_Up.Data;
using Chess_Up.Models;
using Chess_Up.Services;
using HtmlAgilityPack;
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
using System.Net;
using System.Threading.Tasks;

namespace MyCompany.Controllers
{
    public class MLHelperController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private readonly CompanyDbContext _context;
        private static UsersModel FindedUsers = new UsersModel();
        private static bool Searched;
        private IRepository _repository;

        public MLHelperController(UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signInMgr, CompanyDbContext context, IRepository repository)
        {
            _userManager = userMgr;
            _signInManager = signInMgr;
            _context = context;
            _repository = repository;
        }
      
        [Route("~/RequestForm/ChessGuid")]
        [HttpGet]
        public IActionResult ChessGuid()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Retings(int pageNumber = 1)
        {
            const int pageSize = 50;

            var ratingsData = _context.RatingsData
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.PageNumber = pageNumber;

            return View(ratingsData);
        }

        [HttpGet]
        public JsonResult TrainerRatings(string sex, int year)
        {
            var players = _context.RatingsData.Where(p => p.Sex == sex).ToList();



            var trainers = players.GroupBy(p => p.Trainer)
            .Select(g => new TrainerViewModel
            {
                Name = g.Key,
                AverageRating = g.Average(p => GetRatingByYear(p, year))
            })
            .OrderByDescending(t => t.AverageRating)
            .ToList();

            return Json(trainers);
        }
        [HttpGet]
        public static List<string> FetchChessResults()
        {
            List<string> texts = new List<string>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://chess-results.com/fed.aspx?lan=1&fed=ARM");
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(responseText);

                    var tableRows = doc.DocumentNode.SelectNodes("//table[@class='CRs2']//tr[@class='CRg2 ARM']");
                    if (tableRows != null)
                    {
                        foreach (var row in tableRows)
                        {
                            var text = row.SelectSingleNode("./td[2]/a")?.InnerText?.Trim();
                            if (!string.IsNullOrEmpty(text))
                            {
                                texts.Add(text);
                            }
                        }
                    }
                }
            }

            return texts;
        }



        private double GetRatingByYear(RatingsData player, int year)
        {
            switch (year)
            {
                case 2016:
                    return player.rating_standard_2016;
                case 2017:
                    return player.rating_standard_2017;
                case 2018:
                    return player.rating_standard_2018;
                case 2019:
                    return player.rating_standard_2019;
                case 2020:
                    return player.rating_standard_2020;
                case 2021:
                    return player.rating_standard_2021;
                case 2022:
                    return player.rating_standard_2022;
                default:
                    return 0;
            }

        }

            [HttpGet]
        public IActionResult UpcomingGrowth()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UpcomingReting()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Tournament()
        {
            List<string> trainerNames = new List<string> { "Չարենցավան, 3-րդ կարգի որակավորման մրցաշար", "ՀՀ դպրոցականների 17-րդ օլիմպիադա.Մեղրի", "Դպրոցականների 17-րդ  օլիմպիադա, փուլ 2-րդ", "ՀՀ դպրոցականների 17-րդ օլիմպիադա Թալինի տարածաշրջան", "Դպրոցական օլիմպիադայի 2-րդ փուլ Բաղրամյանի տարածաշրջան", "2-րդ կարգի որակավորման մրցաշար- Արաբկիր", "ՀՀ շախմատի դպրոցականների 17-րդ օլիմպիադա Արարատի տարածաշրջան", "Մալաթիա ՇՄՄ  4-րդ կարգի  որակավորման մրցաշար (Ա խումբ մինչև 8տ.)", "ՀՀ շախմատի դպրոցականների 17-րդ օլիմպիադա Մասիսի տարածաշրջան", };
            return View(trainerNames);
            //var results = FetchChessResults();
            //return View(results);
            //string responseText = ChessResultsFetcher.FetchChessResults();
            ////return PartialView("_ChessResults", responseText);
            //return View((object)responseText);
        }
        [HttpGet]
        public IActionResult Triners()
        {
            return View();
        }

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
