using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoogleCloudSamples.Models
{
    /// <summary>
    /// An interface for storing teams.  Can be implemented by a database,
    /// Google Datastore, etc.
    /// </summary>
    public interface ITeamList
    {
        /// <summary>
        /// Creates a new team.  The Id of the team will be filled when the
        /// function returns.
        /// </summary>
        void Create(Team team);

        Team Read(long id);

        void Update(Team team);

        void Delete(long id);

        TeamList List(int pageSize, string nextPageToken);
    }

    /// <summary>
    /// Implements ITeamList with a database.
    /// </summary>
    public class DBTeamList : ITeamList
    {
        private readonly ApplicationDbContext _dbcontext;

        public DBTeamList(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        // [START create]
        public void Create(Team team)
        {
            var trackteam = _dbcontext.Teams.Add(team);
            _dbcontext.SaveChanges();
            team.TeamId = trackteam.TeamId;
        }
        // [END create]
        public void Delete(long id)
        {
            Team team = _dbcontext.Teams.Single(m => m.TeamId == id);
            _dbcontext.Teams.Remove(team);
            _dbcontext.SaveChanges();
        }

        // [START list]
        public TeamList List(int pageSize, string nextPageToken)
        {
            IQueryable<Team> query = _dbcontext.Teams.OrderBy(team => team.TeamId);
            var teams = query.ToArray();
            return new TeamList()
            {
                Teams = teams.ToList() 
            };
        }
        // [END list]

        public Team Read(long id)
        {
            return _dbcontext.Teams.Single(m => m.TeamId == id);
        }

        public void Update(Team team)
        {
            _dbcontext.Entry(team).State = System.Data.Entity.EntityState.Modified;
            _dbcontext.SaveChanges();
        }
    }
}