using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP.Classes
{
    /// <summary>
    /// This class contains the contest details.
    /// </summary>
    public class ContestDetails
    {
        #region Class variables
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime LimitDate { get; set; }
        public DateTime VotingDate { get; set; }
        public bool HasVoted { get; set; }
        public bool HasResultsCalculated { get; set; }
        public bool HasBeenCreatedByCurrentUser { get; set; }
        public bool EveryoneVoted { get; set; }
        public int Status { get; set; }
        #endregion

        #region Class constructors
        /// <summary>
        /// Contest details constructor that takes all arguments.
        /// </summary>
        /// <param name="id">The contest id.</param>
        /// <param name="name">The contest name.</param>
        /// <param name="description">The contest description.</param>
        /// <param name="startDate">The contest start date.</param>
        /// <param name="limitDate">The contest limit date.</param>
        /// <param name="votingDate">The contest voting limit date.</param>
        /// <param name="hasVoted">If the contest has already been voted by the current user.</param>
        /// <param name="hasResultsCalculated">If the contest has already had it's results calculated.</param>
        /// <param name="hasBeenCreatedByCurrentUser">If the contest has been created by the currently logged on user.</param>
        public ContestDetails(int id, string name, string description, DateTime startDate, DateTime limitDate, DateTime votingDate, bool hasVoted, bool hasResultsCalculated, bool hasBeenCreatedByCurrentUser)
        {
            Id = id;
            Name = name;
            Description = description;
            StartDate = startDate;
            LimitDate = limitDate;
            VotingDate = votingDate;
            HasVoted = hasVoted;
            HasResultsCalculated = hasResultsCalculated;
            HasBeenCreatedByCurrentUser = hasBeenCreatedByCurrentUser;
            EveryoneVoted = false;
            Status = 0;
        }

        /// <summary>
        /// Contest details constructor that takes no arguments.
        /// Creates a contest details with id 0, no name, no description, and current day start, limit date, voting date and not yet voted.
        /// </summary>
        public ContestDetails() : this(0, "", "", DateTime.Now, DateTime.Now, DateTime.Now, false, false, false) { }
        #endregion
    }
}
