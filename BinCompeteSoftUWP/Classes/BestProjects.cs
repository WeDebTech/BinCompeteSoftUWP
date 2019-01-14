using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP.Classes
{
    /// <summary>
    /// This class holds the project with their score data.
    /// </summary>
    public class BestProjects
    {
        #region Class variables
        public string ProjectName { get; set; }
        public double Score { get; set; }
        #endregion
        
        #region Class constructors
        /// <summary>
        /// BestProjects constructor that takes all arguments.
        /// </summary>
        /// <param name="projectName">The project name.</param>
        /// <param name="score">The project score.</param>
        public BestProjects(string projectName, double score)
        {
            this.ProjectName = projectName;
            this.Score = score;
        }
        #endregion
    }
}
