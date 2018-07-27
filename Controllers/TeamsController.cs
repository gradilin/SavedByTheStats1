using GoogleCloudSamples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GoogleCloudSamples.Controllers
{
    public class TeamsController : Controller
    {
        private const int _pageSize = 20;

        private readonly ITeamList _list;
        private readonly IPlayerPool _pool;
        private ApplicationDbContext db = new ApplicationDbContext();

        public TeamsController(ITeamList list, IPlayerPool pool)
        {
            _list = list;
            _pool = pool;
        }

        // GET: Team
        public ActionResult Index(string nextPageToken)
        {
            var tList = _list.List(_pageSize, nextPageToken);
            foreach (var team in tList.Teams)
            {
                team.Players = PlayersConverter.ParseStringToList(team.PlayersConcat);
            }

            var sortedteams = tList.Teams.ToList().OrderBy(t => t.Points).Reverse();
            tList.Teams = sortedteams;

            return View(new ViewModels.Teams.Index()
            {
                TeamList = tList
            });
        }

        // GET: Teams/Edit/5
        public ActionResult UpdateTeams(long? id)
        {
            var tList = new List<Team>();
            if (id == null)
            {
                tList = _list.List(_pageSize, null).Teams.ToList();
            }
            else
                tList.Add(_list.Read((long)id));
                 
            foreach (var team in tList)
            {
                team.Players = PlayersConverter.ParseStringToList(team.PlayersConcat);
                int assists = 0;
                int goals = 0;
                int wins = 0;
                int so = 0;
                int points = 0;

                foreach (var player in team.Players)
                {
                    var pfdb = _pool.List(0, null).Players.Where(p => p.playerCode == player).First();
                    assists += pfdb.assists;
                    goals += pfdb.goals;
                    wins += pfdb.wins;
                    so += pfdb.shutouts;
                    points = (goals + assists + wins + so);
                }
                team.Assists = assists;
                team.Goals = goals;
                team.Wins = wins;
                team.Shutouts = so;
                team.Points = points;

                _list.Update(team);
            }
            return RedirectToAction("Index");
        }

        public ActionResult FixBensTeam(int id)
        {
            Team team = _list.Read((long)id);
            team.Players = PlayersConverter.ParseStringToList(team.PlayersConcat);
            string pr = "8471469";
            string ma = "8470594";
            //getting player's from db 
            List<string> pIds = team.Players;
            pIds.Remove(pr);
            pIds.Add(ma);
            team.Players = pIds;
            team.PlayersConcat = PlayersConverter.ConvertListToString(team.Players);
            _list.Update(team);
            return RedirectToAction("Index");
        }


        // GET: Teams/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Team team = _list.Read((long)id);
            team.Players = PlayersConverter.ParseStringToList(team.PlayersConcat);

            //getting player's from db 
            List<string> pIds = team.Players;
            var tPlayers = _pool.List(0, null).Players.Where(p => pIds.Contains(p.playerCode)).ToList();
            tPlayers.OrderBy(x => x.team); 
            //Processing Points 
            ViewBag.Players = tPlayers;
            if (team == null)
            {
                return HttpNotFound();
            }

            return View(team);
        }

        // [START create]
        // GET: Teams/Create
        public ActionResult Create()
        {
            var pList = _pool.List(0, null);
            var plistTiered = pList.Players.Where(m => !string.IsNullOrWhiteSpace(m.tierList));
            HashSet<string> tiers = new HashSet<string>();
            foreach (var player in plistTiered)
                tiers.Add(player.tierList);
            var tiersList = tiers.ToList();

            var SortedTiers = new List<string>();

            var temptiers = tiers.Where(m => m.Contains("EF")).ToList();
            temptiers.Sort();
            foreach (var tier in temptiers)
                SortedTiers.Add(tier);
            temptiers = tiers.Where(m => m.Contains("ED")).ToList();
            temptiers.Sort();
            foreach (var tier in temptiers)
                SortedTiers.Add(tier);
            temptiers = tiers.Where(m => m.Contains("EG")).ToList();
            temptiers.Sort();
            foreach (var tier in temptiers)
                SortedTiers.Add(tier);
            temptiers = tiers.Where(m => m.Contains("WF")).ToList();
            temptiers.Sort();
            foreach (var tier in temptiers)
                SortedTiers.Add(tier);
            temptiers = tiers.Where(m => m.Contains("WD")).ToList();
            temptiers.Sort();
            foreach (var tier in temptiers)
                SortedTiers.Add(tier);
            temptiers = tiers.Where(m => m.Contains("WG")).ToList();
            temptiers.Sort();
            foreach (var tier in temptiers)
                SortedTiers.Add(tier);

            List<SelectList> Fields = new List<SelectList>();
            foreach (var tier in SortedTiers)
            {
                Fields.Add(new SelectList(plistTiered.Where(m => m.tierList == tier).Select(p => new SelectListItem { Value = p.playerCode, Text = p.fullName }), "Value", "Text"));
            }

            ViewBag.Tiers = SortedTiers;
            ViewBag.SelectLists = Fields;
            return View();
        }

        // POST: Teams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Team team)
        {
            if (ModelState.IsValid)
            {
                team.PlayersConcat = PlayersConverter.ConvertListToString(team.Players);
                _list.Create(team);
                UpdateTeams(team.TeamId);
                return RedirectToAction("Details", new { id = team.TeamId });
            }
            return RedirectToAction("Index");
        }
        // [END create]

        /// <summary>
        /// Dispays the common form used for the Edit and Create pages.
        /// </summary>
        /// <param name="action">The string to display to the user.</param>
        /// <param name="team">The asp-action value.  Where will the form be submitted?</param>
        /// <returns>An IActionResult that displays the form.</returns>
        private ActionResult ViewForm(string action, string formAction, Team team = null)
        {
            var form = new ViewModels.Teams.Form()
            {
                Action = action,
                Team = team ?? new Team(),
                IsValid = ModelState.IsValid,
                FormAction = formAction
            };
            return View("/Views/Teams/Form.cshtml", form);
        }

        // GET: Teams/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Team team = _list.Read((long)id);
            team.Players = PlayersConverter.ParseStringToList(team.PlayersConcat);
            if (team == null)
            {
                return HttpNotFound();
            }
            return ViewForm("Edit", "Edit", team);
        }

        // POST: Teams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Team team, long id)
        {
            if (ModelState.IsValid)
            {
                team.TeamId = id;
                team.PlayersConcat = PlayersConverter.ConvertListToString(team.Players);
                _list.Update(team);
                return RedirectToAction("Details", new { id = team.TeamId });
            }
            return ViewForm("Edit", "Edit", team);
        }


        public ActionResult ClearDB()
        {
            var TeamDb = _list.List(0, null);
            foreach (var team in TeamDb.Teams)
            {
                _list.Delete(team.TeamId);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Banish(long id)
        {
            _list.Delete(id);
            return RedirectToAction("Index");
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            _list.Delete(id);
            return RedirectToAction("Index");
        }
    }
}