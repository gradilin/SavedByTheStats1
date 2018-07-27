using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoogleCloudSamples.Models
{
    [Bind(Include = "fullName,jerseyNumber,position,type,tierList", Exclude = "playerCode")]
    public class Player
    {
        [Key]
        public long id { get; set; }
        public string playerCode { get; set; }
        [Display(Name = "Name")]
        public string fullName { get; set; }
        [Display(Name = "Number")]
        public string jerseyNumber { get; set; }
        [Display(Name = "Position")]
        public string position { get; set; }
        public string type { get; set; }
        [Display(Name = "Goals")]
        public int goals { get; set; }
        [Display(Name = "Assists")]
        public int assists { get; set; }
        [Display(Name = "Wins")]
        public int wins { get; set; }
        [Display(Name = "Shutouts")]
        public int shutouts { get; set; }
        [Display(Name = "Team")]
        public string team { get; set; }
        [Display(Name = "Points")]
        public int points { get; set; }
        public string tierList { get; set; }
    }

    public class PlayerList
    {
        public IEnumerable<Player> Players;
        public string NextPageToken;
    }

    // [START dbset]
    public class ApplicationDbContext : DbContext
    {
        // [START_EXCLUDE]
        private static readonly string s_mySqlServerBaseName = "LocalMySqlServer";
        private static readonly string s_sqlServerBaseName = "LocalSqlServer";
        // [END_EXCLUDE]
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        // [END dbset]

        /// <summary>
        /// Pulls connection string from Web.config.
        /// </summary>
        public ApplicationDbContext() : base("name=" + ((UnityConfig.ChoosePlayerPoolFromConfig() == PlayerPoolFlag.MySql) ? s_mySqlServerBaseName : s_sqlServerBaseName))
        {
        }
    }

    public interface IPlayerPool
    {
        //Creates new Book 
        void Create(Player player);

        Player Read(long id);

        void Update(Player player);

        void Delete(long id);

        PlayerList List(int pageSize, string nextPageToken);
    }
}