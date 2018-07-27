using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GoogleCloudSamples
{
    public class NHLClasses
    {
        //Player Points Class
        public class PlayerPoints
        {
            public string PlayerName { get; set; }
            public int Goals { get; set; }
            public int Assists { get; set; }
            public int Wins { get; set; }
            public int Shutouts { get; set; }
            public int Points { get; set; }
        }

        public class TeamPointsTotals
        {
            public string PlayerName { get; set; }
            public int Goals { get; set; }
            public int Assists { get; set; }
            public int Wins { get; set; }
            public int Shutouts { get; set; }
            public int Points { get; set; }
        }

        // ex Get: https://statsapi.web.nhl.com/api/v1/teams/
        public class TimeZone
        {
            public string id { get; set; }
            public int offset { get; set; }
            public string tz { get; set; }
        }

        public class Venue
        {
            public string name { get; set; }
            public string link { get; set; }
            public string city { get; set; }
            public TimeZone timeZone { get; set; }
        }

        public class Division
        {
            public int id { get; set; }
            public string name { get; set; }
            public string link { get; set; }
        }

        public class Conference
        {
            public int id { get; set; }
            public string name { get; set; }
            public string link { get; set; }
        }

        public class Franchise
        {
            public int franchiseId { get; set; }
            public string teamName { get; set; }
            public string link { get; set; }
        }

        public class Team
        {
            public int id { get; set; }
            public string name { get; set; }
            public string link { get; set; }
            public Venue venue { get; set; }
            public string abbreviation { get; set; }
            public string teamName { get; set; }
            public string locationName { get; set; }
            public string firstYearOfPlay { get; set; }
            public Division division { get; set; }
            public Conference conference { get; set; }
            public Franchise franchise { get; set; }
            public string shortName { get; set; }
            public string officialSiteUrl { get; set; }
            public int franchiseId { get; set; }
            public bool active { get; set; }
        }

        public class Teams
        {
            public string copyright { get; set; }
            public List<Team> teams { get; set; }
        }

        public class Person
        {
            public int id { get; set; }
            public string fullName { get; set; }
            public string link { get; set; }
        }

        public class Position
        {
            public string code { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string abbreviation { get; set; }
        }

        public class Roster
        {
            public Person person { get; set; }
            public string jerseyNumber { get; set; }
            public Position position { get; set; }
        }

        public class RosterList
        {
            public string copyright { get; set; }
            public List<Roster> roster { get; set; }
            public string link { get; set; }
        }
    }
    // example GET: https://statsapi.web.nhl.com/api/v1/people/8475222?expand=person.stats&stats=statsSingleSeasonPlayoffs&season=20162017
    public class PConverter
    {

        public class CurrentTeam
        {
            public int id { get; set; }
            public string name { get; set; }
            public string link { get; set; }
        }

        public class PrimaryPosition
        {
            public string code { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string abbreviation { get; set; }
        }

        public class Type
        {
            public string displayName { get; set; }
        }

        public class Stat2
        {
            public string timeOnIce { get; set; }
            public int assists { get; set; }
            public int goals { get; set; }
            public int pim { get; set; }
            public int shots { get; set; }
            public int games { get; set; }
            public int hits { get; set; }
            public int powerPlayGoals { get; set; }
            public int powerPlayPoints { get; set; }
            public string powerPlayTimeOnIce { get; set; }
            public string evenTimeOnIce { get; set; }
            public string penaltyMinutes { get; set; }
            public double faceOffPct { get; set; }
            public double shotPct { get; set; }
            public int gameWinningGoals { get; set; }
            public int overTimeGoals { get; set; }
            public int shortHandedGoals { get; set; }
            public int shortHandedPoints { get; set; }
            public string shortHandedTimeOnIce { get; set; }
            public int blocked { get; set; }
            public int plusMinus { get; set; }
            public int points { get; set; }
            public int shifts { get; set; }
            public string timeOnIcePerGame { get; set; }
            public string evenTimeOnIcePerGame { get; set; }
            public string shortHandedTimeOnIcePerGame { get; set; }
            public string powerPlayTimeOnIcePerGame { get; set; }
        }

        public class Split
        {
            public string season { get; set; }
            public Stat2 stat { get; set; }
        }

        public class Stat
        {
            public Type type { get; set; }
            public List<Split> splits { get; set; }
        }

        public class Player
        {
            public int id { get; set; }
            public string fullName { get; set; }
            public string link { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string primaryNumber { get; set; }
            public string birthDate { get; set; }
            public int currentAge { get; set; }
            public string birthCity { get; set; }
            public string birthCountry { get; set; }
            public string nationality { get; set; }
            public string height { get; set; }
            public int weight { get; set; }
            public bool active { get; set; }
            public bool alternateCaptain { get; set; }
            public bool captain { get; set; }
            public bool rookie { get; set; }
            public string shootsCatches { get; set; }
            public string rosterStatus { get; set; }
            public CurrentTeam currentTeam { get; set; }
            public PrimaryPosition primaryPosition { get; set; }
            public List<Stat> stats { get; set; }
        }

        public class PlayerStats
        {
            public string copyright { get; set; }
            public List<Player> people { get; set; }
        }
    }
    // ex Get: https://statsapi.web.nhl.com/api/v1/teams/1/roster
    public class GoalieCoverter
    {
        public class CurrentTeam
        {
            public int id { get; set; }
            public string name { get; set; }
            public string link { get; set; }
        }

        public class PrimaryPosition
        {
            public string code { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string abbreviation { get; set; }
        }

        public class Type
        {
            public string displayName { get; set; }
        }

        public class Stat
        {
            public string timeOnIce { get; set; }
            public int shutouts { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int saves { get; set; }
            public int powerPlaySaves { get; set; }
            public int shortHandedSaves { get; set; }
            public int evenSaves { get; set; }
            public int shortHandedShots { get; set; }
            public int evenShots { get; set; }
            public int powerPlayShots { get; set; }
            public double savePercentage { get; set; }
            public double goalAgainstAverage { get; set; }
            public int games { get; set; }
            public int gamesStarted { get; set; }
            public int shotsAgainst { get; set; }
            public int goalsAgainst { get; set; }
            public string timeOnIcePerGame { get; set; }
            public double powerPlaySavePercentage { get; set; }
            public double shortHandedSavePercentage { get; set; }
            public double evenStrengthSavePercentage { get; set; }
        }

        public class Split
        {
            public string season { get; set; }
            public Stat stat { get; set; }
        }

        public class Stats
        {
            public Type type { get; set; }
            public IList<Split> splits { get; set; }
        }

        public class Person
        {
            public int id { get; set; }
            public string fullName { get; set; }
            public string link { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string primaryNumber { get; set; }
            public string birthDate { get; set; }
            public int currentAge { get; set; }
            public string birthCity { get; set; }
            public string birthCountry { get; set; }
            public string nationality { get; set; }
            public string height { get; set; }
            public int weight { get; set; }
            public bool active { get; set; }
            public bool alternateCaptain { get; set; }
            public bool captain { get; set; }
            public bool rookie { get; set; }
            public string shootsCatches { get; set; }
            public string rosterStatus { get; set; }
            public CurrentTeam currentTeam { get; set; }
            public PrimaryPosition primaryPosition { get; set; }
            public IList<Stats> stats { get; set; }
        }

        public class Example
        {
            public string copyright { get; set; }
            public IList<Person> people { get; set; }
        }

    }


    public static class PlayersConverter
    {
        // Separates users in the list.
        private const char PlayerSeparator = ';';

        public static string ConvertListToString(IEnumerable<string> teamPlayers)
        {
            var stringBuilder = new StringBuilder();

            // Build the users string.
            foreach (var player in teamPlayers)
            {
                stringBuilder.Append(player);
                stringBuilder.Append(PlayerSeparator);
            }

            // Remove trailing separator.
            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            return stringBuilder.ToString();
        }

        public static List<string> ParseStringToList(string playerString)
        {
            // Check that passed argument is not null.
            if (playerString == null) throw new ArgumentNullException("usersString");

            var players = playerString.Split(PlayerSeparator).ToList();

            return players;
        }
    }
}


