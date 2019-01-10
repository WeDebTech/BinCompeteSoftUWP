using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoft
{
    /// <summary>
    /// This class contains the contest details.
    /// </summary>
    public class ContestDetails
    {
        // Class variables.
        private int id;
        private string name;
        private string description;
        private DateTime startDate;
        private DateTime limitDate;
        private DateTime votingDate;
        private bool hasVoted;
        private bool hasResultsCalculated;
        private bool hasBeenCreatedByCurrentUser;

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
            this.id = id;
            this.name = name;
            this.description = description;
            this.startDate = startDate;
            this.limitDate = limitDate;
            this.votingDate = votingDate;
            this.hasVoted = hasVoted;
            this.hasResultsCalculated = hasResultsCalculated;
            this.hasBeenCreatedByCurrentUser = hasBeenCreatedByCurrentUser;
        }

        /// <summary>
        /// Contest details constructor that takes no arguments.
        /// Creates a contest details with id 0, no name, no description, and current day start, limit date, voting date and not yet voted.
        /// </summary>
        public ContestDetails() : this(0, "", "", DateTime.Now, DateTime.Now, DateTime.Now, false, false, false) { }

        /// <summary>
        /// Gets or sets the contest id.
        /// </summary>
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// Gets or sets the contest name.
        /// </summary>
        [System.ComponentModel.DisplayName("Contest name")]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets or sets the contest description.
        /// </summary>
        [System.ComponentModel.DisplayName("Contest description")]
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// Gets or sets the contest start date.
        /// </summary>
        [System.ComponentModel.DisplayName("Start date")]
        public DateTime StartDate
        {
            get { return this.startDate; }
            set { this.startDate = value; }
        }

        /// <summary>
        /// Gets or sets the contest limit date.
        /// </summary>
        [System.ComponentModel.DisplayName("Limit date")]
        public DateTime LimitDate
        {
            get { return this.limitDate; }
            set { this.limitDate = value; }
        }

        /// <summary>
        /// Gets or sets the contest voting limit date.
        /// </summary>
        [System.ComponentModel.DisplayName("Voting date")]
        public DateTime VotingDate
        {
            get { return votingDate; }
            set { votingDate = value; }
        }

        /// <summary>
        /// Gets or sets if the contest has been voted by the current user.
        /// </summary>
        [System.ComponentModel.DisplayName("Has voted")]
        public bool HasVoted
        {
            get { return hasVoted; }
            set { hasVoted = value; }
        }

        /// <summary>
        /// Gets or sets if the contest has had it's results calculated.
        /// </summary>
        [System.ComponentModel.DisplayName("Results calculated")]
        public bool HasResultsCalculated
        {
            get { return hasResultsCalculated; }
            set { hasResultsCalculated = value; }
        }

        /// <summary>
        /// Gets or sets if the contest has been created by the currently logged on user.
        /// </summary>
        [System.ComponentModel.DisplayName("Created by user")]
        public bool HasBeenCreatedByCurrentUser
        {
            get { return hasBeenCreatedByCurrentUser; }
            set { hasBeenCreatedByCurrentUser = value; }
        }
    }
}
