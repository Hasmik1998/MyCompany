using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MyCompany.Data;
using MyCompany.Models;
using MyCompany.Repository;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;

namespace MyCompany.Controllers
{
    public class MessengerController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Db Context
        /// </summary>
        private readonly CompanyDbContext _context;

        /// <summary>
        /// Controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        public MessengerController(ILogger<HomeController> logger, CompanyDbContext context, IRepository repository)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Get Chat
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "User,SimpleUser")]
        public IActionResult Chat()
        {
            List<RegistrationRequest> users = _context.RegistrationRequests.ToList();
            return View(users);
        }

        /// <summary>
        /// Create ChatRoom for users
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/Messenger/CreateChatRoom/{Id}")]
        [Authorize(Roles = "User,SimpleUser")]
        [HttpGet]
        public async Task<IActionResult> CreateChatRoom(int Id)
        {
            var id = _context.RegistrationRequests.FirstOrDefault(a => a.Email == User.Identity.Name).Id;
            var foundRoom = _context.CharRooms.FirstOrDefault(a => (a.CurrentUserId == id && a.ToUserId == Id) || (a.CurrentUserId == Id && a.ToUserId == id));
            if (foundRoom != null)
            {
                return RedirectToAction($"ChatRoom", new { Id = foundRoom.Id });
            }

            int NewChatRoomId = 0;
            if (_context.CharRooms.Any())
            {
                NewChatRoomId = _context.CharRooms.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                NewChatRoomId++;
            }
            else
            {
                NewChatRoomId = 1;
            }

            ChatRoom model = new ChatRoom()
            {
                CurrentUserId = id,
                ToUserId = Id,
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now
            };

            await _context.CharRooms.AddAsync(model);
            await _context.SaveChangesAsync();

            return RedirectToAction($"ChatRoom", new { Id = NewChatRoomId });
        }

        /// <summary>
        /// Get Chats
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "User,SimpleUser")]
        public IActionResult Chats()
        {
            return View(GetChatRooms());
        }

        /// <summary>
        /// GetChatRooms From 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "User,SimpleUser")]
        public List<ChatRoomsModel> GetChatRooms()
        {
            var id = _context.RegistrationRequests.FirstOrDefault(a => a.Email == User.Identity.Name).Id;

            List<ChatRoom> foundWithCurrentUserIdRoom = _context.CharRooms.Where(a => (a.CurrentUserId == id)).OrderByDescending(a => a.LastModifiedOn).ToList();
            List<ChatRoom> foundInToIdRoom = _context.CharRooms.Where(a => a.ToUserId == id).OrderByDescending(a => a.LastModifiedOn).ToList();

            List<ChatRoomsModel> result = new List<ChatRoomsModel>();

            foreach (var item in foundWithCurrentUserIdRoom)
            {
                var user = _context.RegistrationRequests.FirstOrDefault(a => a.Id == item.ToUserId);
                if (user == null)
                {
                    _context.CharRooms.Remove(item);
                    _context.SaveChanges();
                    continue;
                }

                ChatRoomsModel model = new ChatRoomsModel();
                model.ChatRoomId = item.Id;
                model.ToUserUserName = user.Email;
                model.Image = user.Image;
                model.ChatRoomData = item.LastModifiedOn;
                result.Insert(0, model);

            }

            foreach (var item in foundInToIdRoom)
            {
                var user = _context.RegistrationRequests.FirstOrDefault(a => a.Id == item.CurrentUserId);
  
                if (user == null) 
                {
                    _context.CharRooms.Remove(item);
                    _context.SaveChanges();
                    continue;
                }

                ChatRoomsModel model = new ChatRoomsModel();
                model.ChatRoomId = item.Id;
                model.ToUserUserName = user.Email;
                model.Image = user.Image;
                model.ChatRoomData = item.LastModifiedOn;

                result.Insert(0,model);
            }

            //return View(result);
            return result;
        }

        /// <summary>
        /// Get Chat Room
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("~/Messenger/ChatRoom/{Id}")]
        [Authorize(Roles = "User,SimpleUser")]
        [HttpGet]
        public IActionResult ChatRoom(int Id)
        {
            ViewBag.ChatRoomId = Id;
            ChatRoom room = _context.CharRooms.FirstOrDefault(a => a.Id == Id);

            RegistrationRequest user = _context.RegistrationRequests.Where(a => (a.Id == room.ToUserId && !a.Email.Equals(User.Identity.Name)) || (a.Id == room.CurrentUserId && !a.Email.Equals(User.Identity.Name))).ToList().First();

            UserDataModel model = new UserDataModel()
            {
                Image = user.Image,
                FullName = string.Join(" ", user.Name, user.SurName)
            };

            return View(model);
        }

        /// <summary>
        /// Save Messages
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "User,SimpleUser")]
        public JsonResult SaveMessage(int id, string message)
        {
            var msg = new Message()
            {
                ChatRoomId = id,
                UserName = User.Identity.Name,
                Content = message
            };
            _context.Messages.Add(msg);
            //_context.SaveChanges();

            var chatRoom = _context.CharRooms.FirstOrDefault(a => a.Id == id);
            chatRoom.LastModifiedOn = DateTime.Now;
            _context.CharRooms.Update(chatRoom);
            _context.SaveChanges();

            return Json("success");
        }

        /// <summary>
        /// Get messages
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "User,SimpleUser")]
        public JsonResult GetMessages(int id)
        {
            List<Message> messages = _context.Messages.Where(a => a.ChatRoomId == id).ToList();

            return Json(messages);
        }
    }
}
