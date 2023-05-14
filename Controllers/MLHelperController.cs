using Chess_Up.Data;
using Chess_Up.Models;
using Chess_Up.Services;
using HtmlAgilityPack;
using MarkovSharp.TokenisationStrategies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCompany.Data;
using MyCompany.Models;
using MyCompany.Repository;
using MyCompany.Utilities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
        public async Task<IActionResult> UpcomingGrowth()
        {
            string name = User.Identity.Name;

            RegistrationRequest user = _context.RegistrationRequests.FirstOrDefault(a => a.Email == name);
            string[] split = user.SurName.Split(' ');
            string fullName = split[0] + " " + user.Name + " " + split[1];

            RatingsData data = _context.RatingsData.FirstOrDefault(a => a.Full_Name == fullName);

            using var client = new HttpClient();

            //Set the request URL and JSON data
            string url = "http://127.0.0.1:8000/predict";
            var jsonData = "{\"scores\":" + $"[{data.rating_standard_2018}, {data.rating_standard_2019}, {data.rating_standard_2020}, {data.rating_standard_2021}, {data.rating_standard_2022}" + "]}";

            // Create the HttpContent object with the JSON data
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Send the POST request with the JSON data in the request body
            var response = await client.PostAsync(url, content);

            // Read the response content as a string
            var responseContent = await response.Content.ReadAsStringAsync();

            string path = "";
            if (responseContent.Contains("0"))
            {
                path = @"C:\Users\hasmik.petrosyan\Desktop\Capstone\decreaseJson.json";
                ViewBag.predictedText = "Your score will decrease in the upcoming year";
            }
            else
            {
                ViewBag.predictedText = "Your score will increase in the upcoming year";
                path = @"C:\Users\hasmik.petrosyan\Desktop\Capstone\increaseJson.json";
            }

            var model = new StringMarkov();
            var readResult = System.IO.File.ReadAllText(path);
            IEnumerable<string> ExampleData = JsonConvert.DeserializeObject<string[]>(readResult);


            model.Learn(ExampleData);

            var generatedText = model.Walk();
            ViewBag.text = generatedText.First();

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
            string url = "https://chess-results.com/fed.aspx?lan=1&fed=ARM";

            // Create a new HtmlWeb instance
            HtmlWeb web = new HtmlWeb();

            // Load the HTML document from the URL
            HtmlDocument doc = web.Load(url);

            // Scrape the tournament name
            HtmlNode tableNode = doc.DocumentNode.SelectSingleNode("//table[@class='CRs2']");
            List<TournamentModel> tournaments = new List<TournamentModel>();
            if (tableNode != null)
            {
                // Get all the <tr> elements within the table
                IEnumerable<HtmlNode> trNodes = tableNode.SelectNodes(".//tr");
                if (trNodes != null)
                {
                    // Iterate over the <tr> elements and process them
                    foreach (HtmlNode trNode in trNodes)
                    {
                        TournamentModel currentTournament = new TournamentModel();
                        int i = 0;
                        // Process each <tr> element as needed
                        Console.WriteLine("Row:");
                        foreach (HtmlNode tdNode in trNode.ChildNodes)
                        {
                            string text = HttpUtility.HtmlDecode(tdNode.InnerText.Trim());
                            if (i == 0)
                            {
                                currentTournament.Id = text;
                            }
                            else if (i == 1)
                            {
                                currentTournament.TournamentName = text;
                            }
                            else if (i == 2)
                            {
                                currentTournament.LastUpdate = text;
                            }
                            i++;
                        }
                        tournaments.Add(currentTournament);
                    }
                }
            }
            return View(tournaments);
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
        public IActionResult PasswordSecurity()
        {
            var result = _context.PasswordSecurity.ToList();
            if (result.Count() == 0)
                return View();
            return View(result.LastOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> PasswordSecurity(PasswordSecurity model)
        {
            var records = _context.PasswordSecurity.ToList();
            var lastRecord = records.Count() > 0 ? records.LastOrDefault() : null;
            int id = lastRecord == null ? 1 : lastRecord.Id + 1;
            PasswordSecurity result = new PasswordSecurity()
            {
                Id = id,
                MinimumLength = model.MinimumLength,
                LowercaseNum = model.LowercaseNum,
                UpercaseNum = model.UpercaseNum,
                NumberCharacters = model.NumberCharacters,
                SpecialCharacters = model.SpecialCharacters
            };

            if (result == null)
            {
                return RedirectToAction("PasswordSecurity");
            }
            else
            {
                await _context.PasswordSecurity.AddAsync(result);
                await _context.SaveChangesAsync();
                return RedirectToAction("ChessGuid");
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
