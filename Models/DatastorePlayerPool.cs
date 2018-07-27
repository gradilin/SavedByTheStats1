using Google.Cloud.Datastore.V1;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoogleCloudSamples.Models
{
    public static class DatastorePlayerPoolExtensionMethods
    {
        /// <summary>
        /// Make a datastore key given a book's id.
        /// </summary>
        /// <param name="id">A book's id.</param>
        /// <returns>A datastore key.</returns>
        public static Key ToKey(this long id) =>
            new Key().WithElement("Player", id);

        /// <summary>
        /// Make a book id given a datastore key.
        /// </summary>
        /// <param name="key">A datastore key</param>
        /// <returns>A book id.</returns>
        public static long ToId(this Key key) => key.Path.First().Id;

        /// <summary>
        /// Create a datastore entity with the same values as book.
        /// </summary>
        /// <param name="book">The book to store in datastore.</param>
        /// <returns>A datastore entity.</returns>
        /// [START toentity]
        public static Entity ToEntity(this Player player) => new Entity()
        {
            Key = player.id.ToKey(),
            ["playerCode"] = player.playerCode,
            ["fullName"] = player.fullName,
            ["jerseyNumber"] = player.jerseyNumber,
            ["position"] = player.position,
            ["type"] = player.type,
            ["goals"] = player.goals,
            ["assists"] = player.assists,
            ["wins"] = player.wins,
            ["shutouts"] = player.shutouts,
            ["tierList"] = player.tierList
        };
        // [END toentity]

        /// <summary>
        /// Unpack a book from a datastore entity.
        /// </summary>
        /// <param name="entity">An entity retrieved from datastore.</param>
        /// <returns>A book.</returns>
        public static Player ToPlayer(this Entity entity) => new Player()
        {
            id = entity.Key.Path.First().Id,
            playerCode = (string)entity["playerCode"],
            fullName = (string)entity["fullName"],
            jerseyNumber = (string)entity["jerseyNumber"],
            position = (string)entity["position"],
            type = (string)entity["type"],
            goals = (int)entity["goals"],
            assists = (int)entity["assists"],
            wins = (int)entity["wins"],
            shutouts = (int)entity["shutouts"],
            tierList = (string)entity["tierList"]
        };
    }

    public class DatastorePlayerPool : IPlayerPool
    {
        private readonly string _projectId;
        private readonly DatastoreDb _db;

        /// <summary>
        /// Create a new datastore-backed bookstore.
        /// </summary>
        /// <param name="projectId">Your Google Cloud project id</param>
        public DatastorePlayerPool(string projectId)
        {
            _projectId = projectId;
            _db = DatastoreDb.Create(_projectId);
        }

        // [START create]
        public void Create(Player player)
        {
            var entity = player.ToEntity();
            entity.Key = _db.CreateKeyFactory("Player").CreateIncompleteKey();
            var keys = _db.Insert(new[] { entity });
            player.id = keys.First().Path.First().Id;
        }
        // [END create]

        public void Delete(long id)
        {
            _db.Delete(id.ToKey());
        }

        // [START list]
        public PlayerList List(int pageSize, string nextPageToken)
        {
            var query = new Query("Player");
            if (pageSize > 0)
                query.Limit = pageSize ;
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                query.StartCursor = ByteString.FromBase64(nextPageToken);
            var results = _db.RunQuery(query);
            return new PlayerList()
            {
                Players = results.Entities.Select(entity => entity.ToPlayer()),
                NextPageToken = results.Entities.Count == query.Limit ?
                    results.EndCursor.ToBase64() : null
            };
        }

        // [END list]
        public Player Read(long id)
        {
            return _db.Lookup(id.ToKey())?.ToPlayer();
        }

        public void Update(Player player)
        {
            _db.Update(player.ToEntity());
        }
    }
}