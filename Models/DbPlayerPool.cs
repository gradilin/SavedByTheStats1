using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoogleCloudSamples.Models
{
    public class DbPlayerPool : IPlayerPool
    {
        private readonly ApplicationDbContext _dbcontext;

        public DbPlayerPool(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        // [START create]
        public void Create(Player player)
        {
            var trackBook = _dbcontext.Players.Add(player);
            _dbcontext.SaveChanges();
            player.id = trackBook.id;
        }
        // [END create]
        public void Delete(long id)
        {
            Player player = _dbcontext.Players.Single(m => m.id == id);
            _dbcontext.Players.Remove(player);
            _dbcontext.SaveChanges();
        }

        // [START list]
        public PlayerList List(int pageSize, string nextPageToken)
        {
            IQueryable<Player> query = _dbcontext.Players.OrderBy(player => player.id);
            var players = query.ToArray();
            return new PlayerList()
            {
                Players = players,
            };
        }
        // [END list]

        public Player Read(long id)
        {
            return _dbcontext.Players.Single(m => m.id == id);
        }

        public void Update(Player player)
        {
            _dbcontext.Entry(player).State = System.Data.Entity.EntityState.Modified;
            _dbcontext.SaveChanges();
        }
    }
}