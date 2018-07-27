using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoogleCloudSamples.Models
{
    [Bind(Include = "TeamName,TeamOwner,Players")]
    public class Team
    {
        [Key]
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamOwner { get; set; }
        public List<string> Players { get; set; }
        public string PlayersConcat { get; set; }
        public int Goals { get; set; }
        public int Wins{ get; set; }
        public int Assists{ get; set; }
        public int Shutouts { get; set; }
        public int Points { get; set; }
    }

    public class TeamList
    {
        public IEnumerable<Team> Teams { get; set; }
        public string NextPageToken { get; set; }
    }
}