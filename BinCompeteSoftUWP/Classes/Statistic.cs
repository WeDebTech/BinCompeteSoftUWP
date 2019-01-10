using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoft
{
    /// <summary>
    /// This class contains statistics data.
    /// </summary>
    public class Statistic
    {
        // Class variables.
        private int year;
        private double projectPerCompetitionAvg;
        private int totalProjects;
        private int totalCompetitions;
        private List<CategoryStatistics> categoryStatistics;
        private List<BestProjects> bestProjects;

        /// <summary>
        /// Statistic constructor that takes all arguments.
        /// </summary>
        /// <param name="year">The statistics year.</param>
        /// <param name="projectPerCompetitionAvg">The average of projects per competition.</param>
        /// <param name="totalProjects">The total of projects.</param>
        /// <param name="totalCompetitions">The total of competitions.</param>
        /// <param name="categoryStatistics">A list of category statistics.</param>
        /// <param name="bestProjects">A list of the best projects in this statistic.</param>
        public Statistic(int year, double projectPerCompetitionAvg, int totalProjects, int totalCompetitions, List<CategoryStatistics> categoryStatistics, List<BestProjects> bestProjects)
        {
            this.year = year;
            this.projectPerCompetitionAvg = projectPerCompetitionAvg;
            this.totalProjects = totalProjects;
            this.totalCompetitions = totalCompetitions;
            this.categoryStatistics = categoryStatistics;
            this.bestProjects = bestProjects;
        }

        /// <summary>
        /// Statistic constructor that takes no arguments.
        /// Creates a statistic with current year, average of 0, 0 projects, 0 competitions, and an empty list.
        /// </summary>
        public Statistic() : this(DateTime.Now.Year, 0, 0, 0, new List<CategoryStatistics>(), new List<BestProjects>()) { }

        /// <summary>
        /// Gets or sets the statistic year.
        /// </summary>
        public int Year
        {
            get { return year; }
            set { year = value; }
        }

        /// <summary>
        /// Gets or sets the average of projects per competition.
        /// </summary>
        public double ProjectPerCompetitionAvg
        {
            get { return projectPerCompetitionAvg; }
            set { projectPerCompetitionAvg = value; }
        }

        /// <summary>
        /// Gets or sets the total of projects of the statistic.
        /// </summary>
        public int TotalProjects
        {
            get { return totalProjects; }
            set { totalProjects = value; }
        }

        /// <summary>
        /// Gets or sets the totals of competitions of the statistic.
        /// </summary>
        public int TotalCompetitions
        {
            get { return totalCompetitions; }
            set { totalCompetitions = value; }
        }

        /// <summary>
        /// Gets or sets the list of category statistics of the statistic.
        /// </summary>
        public List<CategoryStatistics> CategoryStatistics
        {
            get { return categoryStatistics; }
            set { categoryStatistics = value; }
        }

        /// <summary>
        /// Gets or sets the list of best projects of the statistic.
        /// </summary>
        public List<BestProjects> BestProjects
        {
            get { return bestProjects; }
            set { bestProjects = value; }
        }
    }
}
