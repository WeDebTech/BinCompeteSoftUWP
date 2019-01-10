using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoft
{
    /// <summary>
    /// This class holds the project with their score data.
    /// </summary>
    public class BestProjects
    {
        // Class variables.
        private string projectName;
        private double score;

        /// <summary>
        /// BestProjects constructor that takes all arguments.
        /// </summary>
        /// <param name="projectName">The project name.</param>
        /// <param name="score">The project score.</param>
        public BestProjects(string projectName, double score)
        {
            this.projectName = projectName;
            this.score = score;
        }

        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
        }

        /// <summary>
        /// Gets or sets the project's score.
        /// </summary>
        public double Score
        {
            get { return score; }
            set { score = value; }
        }
    }
}
