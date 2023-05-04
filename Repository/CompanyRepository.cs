using Chess_Up.Services;
using Microsoft.AspNetCore.Identity;
using MyCompany.Controllers;
using MyCompany.Data;
using MyCompany.Models;
using MyCompany.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Repository
{
    public class CompanyRepository : IRepository
    {
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private readonly CompanyDbContext _context;

        public CompanyRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, CompanyDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        #region Account

        public RegistrationRequest GetUserInfoByUserName(string userName)
        {
            return _context.RegistrationRequests.FirstOrDefault(a => a.Email == userName);
        }

        public bool Login(LoginViewModel userModel)
        {
            throw new NotImplementedException();
        }

        public bool SignOut()
        {
            throw new NotImplementedException();
        }

        public string Registration(RegistrationRequestModel model)
        {
            string script = string.Empty;

            try
            {
                var tempImage = System.Drawing.Image.FromStream(model.Avatar.OpenReadStream());
                var result = Utility.ResizeImage(tempImage, 300, 300);
                var base64 = string.Format("data:image/jpg; base64, {0}", Convert.ToBase64String(Utility.ImageToByte2(result)));


                var newRequest = new RegistrationRequest()
                {
                    Name = model.Name,
                    SurName = model.SurName,
                    UserType = model.UserType == Utilities.UserType.TrainnerUser ? Utilities.UserType.User : model.UserType,
                    Phone = model.Phone,
                    Email = string.IsNullOrEmpty(model.Email) ? string.Empty : model.Email,
                    Password = model.Password,
                    Image = base64,
                    About = string.IsNullOrEmpty(model.About) ? string.Empty : model.About,
                    Accept = false
                };

                _context.RegistrationRequests.AddAsync(newRequest);
                _context.SaveChangesAsync();

                if (newRequest.Accept == false)
                {
                    string strMsg = "Your registration request is under review. We will look it as soon as possible. ";
                    script = "<script language=\"javascript\" type=\"text/javascript\">alert('" + strMsg + "'); window.location='Registration'; </script>";
                    return script;

                }
                else
                {
                    return script;
                }
            }
            catch (Exception ex)
            {
                string strMsg = "Something Went Wrong or not all required fileds are filled try again";
                script = "<script language=\"javascript\" type=\"text/javascript\">alert('" + strMsg + "'); window.location='Registration'; </script>";
                return script;
            }
        }

        public List<RegistrationRequest> GetUsers()
        {
            return _context.RegistrationRequests.ToList();
        }

        public void DeleteDatabase(int id)
        {
            var item = _context.RegistrationRequests.FirstOrDefault(p => p.Id == id);
            _context.RegistrationRequests.Remove(item);
            _context.SaveChanges();
        }

        public async void DeleteUserRequest(int id)
        {
            var model = _context.RegistrationRequests.FirstOrDefault(p => p.Id == id);
            IdentityUser user = await _userManager.FindByEmailAsync(model.Email);
            await _userManager.DeleteAsync(user);

            _context.RegistrationRequests.Remove(model);
            _context.SaveChanges();

            MailModel mail = new MailModel();
            mail.ToMail.Add(model.Email);
            mail.Subject = "User Deleted";
            mail.Body = "Your account has been deleted";

            MailSenderService.SendMail(mail);
        }

        public List<RegistrationRequest> GetUsersExcpetLoggedInUser()
        {
            throw new NotImplementedException();
        }

        public RegistrationRequestModel GetLoggedInUserModel(int id)
        {
            throw new NotImplementedException();
        }

        public void SaveEditedChanges(RegistrationRequestModel model, int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
