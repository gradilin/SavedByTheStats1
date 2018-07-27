using Google.Cloud.Datastore.V1;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoogleCloudSamples.Models
{
    public static class DatastoreTeamListExtensionMethods
    {
        /// <summary>
        /// Make a datastore key given a team's id.
        /// </summary>
        /// <param name="id">A team's id.</param>
        /// <returns>A datastore key.</returns>
        public static Key ToKeyT(this long id) =>
            new Key().WithElement("Team", id);

        /// <summary>
        /// Make a team id given a datastore key.
        /// </summary>
        /// <param name="key">A datastore key</param>
        /// <returns>A team id.</returns>
        public static long ToId(this Key key) => key.Path.First().Id;

        /// <summary>
        /// Create a datastore entity with the same values as team.
        /// </summary>
        /// <param name="team">The team to store in datastore.</param>
        /// <returns>A datastore entity.</returns>
        /// [START toentity]
        public static Entity ToEntity(this Team team) => new Entity()
        {
            Key = team.TeamId.ToKeyT(),
            ["TeamName"] = team.TeamName,
            ["TeamOwner"] = team.TeamOwner,
            ["Players"] = null,
            ["PlayersConcat"] = team.PlayersConcat,
            ["Goals"] = team.Goals,
            ["Assists"] = team.Assists,
            ["Wins"] = team.Wins,
            ["Shutouts"] = team.Shutouts,
            ["Points"] = team.Points
        };
        // [END toentity]

        /// <summary>
        /// Unpack a team from a datastore entity.
        /// </summary>
        /// <param name="entity">An entity retrieved from datastore.</param>
        /// <returns>A team.</returns>
        public static Team ToTeam(this Entity entity) => new Team()
        {
            TeamId = entity.Key.Path.First().Id,
            TeamName = (string)entity["TeamName"],
            TeamOwner = (string)entity["TeamOwner"],
            Players = null,
            PlayersConcat = (string)entity["Players"],
            Goals = (int)entity["Goals"],
            Wins = (int)entity["Wins"],
            Assists = (int)entity["Assists"],
            Points= (int)entity["Points"],
            Shutouts = (int)entity["Shutouts"]
        };
    }

    public class DatastoreTeamList : ITeamList
    {
        private readonly string _projectId;
        private readonly DatastoreDb _db;

        /// <summary>
        /// Create a new datastore-backed TeamList.
        /// </summary>
        /// <param name="projectId">Your Google Cloud project id</param>
        public DatastoreTeamList(string projectId)
        {
            _projectId = projectId;
            _db = DatastoreDb.Create(_projectId);
        }

        // [START create]
        public void Create(Team team)
        {
            var entity = team.ToEntity();
            entity.Key = _db.CreateKeyFactory("Team").CreateIncompleteKey();
            var keys = _db.Insert(new[] { entity });
            team.TeamId = keys.First().Path.First().Id;
        }
        // [END create]

        public void Delete(long id)
        {
            _db.Delete(id.ToKey());
        }

        // [START list]
        public TeamList List(int pageSize, string nextPageToken)
        {
            var query = new Query("team") { Limit = pageSize };
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                query.StartCursor = ByteString.FromBase64(nextPageToken);
            var results = _db.RunQuery(query);
            return new TeamList()
            {
                Teams = results.Entities.Select(entity => entity.ToTeam()),
                NextPageToken = results.Entities.Count == query.Limit ?
                    results.EndCursor.ToBase64() : null
            };
        }
        // [END list]

        public Team Read(long id)
        {
            return _db.Lookup(id.ToKey())?.ToTeam();
        }

        public void Update(Team team)
        {
            _db.Update(team.ToEntity());
        }
    }
}