using MyCompany.Data;
using MyCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Repository
{
    public interface IRepository
    {
        #region Account
        /// <summary>
        /// Getting User info from database
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public RegistrationRequest GetUserInfoByUserName(string userName);

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Login(LoginViewModel user);

        /// <summary>
        /// Sign Out
        /// </summary>
        /// <returns></returns>
        public bool SignOut();

        /// <summary>
        /// Registration
        /// </summary>
        /// <param name="model"></param>
        public string Registration(RegistrationRequestModel model);

        /// <summary>
        /// Get User List
        /// </summary>
        /// <returns></returns>
        public List<RegistrationRequest> GetUsers();

        /// <summary>
        /// Delete User Registration Request
        /// </summary>
        /// <param name="id"></param>
        public void DeleteUserRequest(int id);

        /// <summary>
        /// Delete databaseaua
        /// </summary>
        /// <param name="id"></param>
        public void DeleteDatabase(int id);

        /// <summary>
        /// Get all users except logged in user
        /// </summary>
        /// <returns></returns>
        public List<RegistrationRequest> GetUsersExcpetLoggedInUser();

        /// <summary>
        /// Get logged in user model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RegistrationRequestModel GetLoggedInUserModel(int id);

        public void SaveEditedChanges(RegistrationRequestModel model, int id);
        #endregion
    }
}
