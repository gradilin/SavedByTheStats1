using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GoogleCloudSamples.Models;
using Newtonsoft.Json;

namespace GoogleCloudSamples.Controllers
{
    public class PlayersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //number of players per page 
        private const int _pageSize = 20;
        private readonly IPlayerPool _pool;
        public PlayersController(IPlayerPool pool)
        {
            _pool = pool;
        }

        // GET: Players
        public ActionResult Index(string nextPageToken, string SearchString)
        {
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                return View(new ViewModels.Players.Index()
                {
                    PlayerList = new PlayerList() { Players = _pool.List(0, null).Players.Where(m => m.fullName.ToUpper().Contains(SearchString.ToUpper())).OrderBy(p => p.points).Reverse(), NextPageToken = null }
                });
            }
            else
            {
                return View(new ViewModels.Players.Index()
                {
                    PlayerList = new PlayerList() { Players = _pool.List(0, null).Players.OrderBy(p => p.points).Reverse() }
                });
            }
        }

        // GET: Players/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = _pool.Read((long)id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // GET: Players/Create
        public ActionResult Create()
        {
            return ViewForm("Create", "Create");
        }

      

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Player player)
        {
            if (ModelState.IsValid)
            {
                _pool.Create(player);
                return RedirectToAction("Details", new { id = player.id });
            }
            return ViewForm("Create", "Create", player);
        }

        public ActionResult FillDB()
        {
            for (int i = 1; i < 55; i++)
            {
                string url = @"https://statsapi.web.nhl.com/api/v1/teams/" + i + "/roster";
                var request = WebRequest.Create(url);
                try
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        var text = sr.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            var content = JsonConvert.DeserializeObject<NHLClasses.RosterList>(text);
                            if (ModelState.IsValid)
                            {
                                foreach (var playerProfile in content.roster)
                                {
                                    var currPlayer = db.Players.Where(m => m.playerCode == playerProfile.person.id.ToString()).ToList();
                                    if (currPlayer == null || currPlayer.Count() < 1)
                                    {
                                        var newPlayer = new Player();
                                        newPlayer.playerCode = playerProfile.person.id.ToString();
                                        newPlayer.fullName = playerProfile.person.fullName;
                                        newPlayer.jerseyNumber = playerProfile.jerseyNumber;
                                        newPlayer.position = playerProfile.position.name;
                                        newPlayer.type = playerProfile.position.type;
                                        newPlayer.goals = 0;
                                        newPlayer.shutouts = 0;
                                        newPlayer.wins = 0;

                                        _pool.Create(newPlayer);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult ClearDB()
        {
            var PlayerDB = _pool.List(0, null);
            foreach (var player in PlayerDB.Players)
            {
                _pool.Delete(player.id);
            }
            return RedirectToAction("Index");
        }

        public ActionResult UpdatePlayers()
        {
            var pToUp = _pool.List(0, null).Players.Where(p => p.tierList != null).ToList();
            foreach (var player in pToUp)
            {
                string url = @"https://statsapi.web.nhl.com/api/v1/people/" + player.playerCode + "?expand=person.stats&stats=statsSingleSeasonPlayoffs&season=20172018";
                var request = WebRequest.Create(url);
                try
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        var text = sr.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            if (player.position != "Goalie")
                            {
                                var content = JsonConvert.DeserializeObject<PConverter.PlayerStats>(text);
                                if (ModelState.IsValid)
                                {
                                    var season = content.people[0].stats[0].splits[0].stat;
                                    player.goals = season.goals;
                                    player.assists = season.assists;
                                    player.team = content.people[0].currentTeam.name;
                                    player.wins = 0;
                                    player.shutouts = 0;
                                    player.points = (player.goals + player.assists);
                                    _pool.Update(player);
                                }
                            }
                            else
                            {
                                var content = JsonConvert.DeserializeObject<GoalieCoverter.Example>(text);
                                if (ModelState.IsValid)
                                {
                                    var season = content.people[0].stats[0].splits[0].stat;
                                    player.wins = season.wins;
                                    player.team = content.people[0].currentTeam.name;
                                    player.shutouts = season.shutouts;
                                    player.points = (player.wins + player.shutouts);
                                    _pool.Update(player);
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Dispays the common form used for the Edit and Create pages.
        /// </summary>
        /// <param name="action">The string to display to the user.</param>
        /// <param name="book">The asp-action value.  Where will the form be submitted?</param>
        /// <returns>An IActionResult that displays the form.</returns>
        private ActionResult ViewForm(string action, string formAction, Player player = null)
        {
            var form = new ViewModels.Players.Form()
            {
                Action = action,
                Player = player ?? new Player(),
                IsValid = ModelState.IsValid,
                FormAction = formAction
            };
            return View("/Views/Players/Form.cshtml", form);
        }

        // GET: Players/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return ViewForm("Edit", "Edit", player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Player player, long Id)
        {
            if (ModelState.IsValid)
            {
                player.id = Id;
                Player pOg = db.Players.Find(Id);
                player.playerCode = pOg.playerCode;
                player.type = pOg.type;
                player.goals = pOg.goals;
                player.assists = pOg.assists;
                player.wins = pOg.wins;
                player.shutouts = pOg.shutouts;
                _pool.Update(player);
                return RedirectToAction("Details", new { id = player.id });
            }
            return ViewForm("Edit", "Edit", player);
        }

        // GET: Players/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            _pool.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
