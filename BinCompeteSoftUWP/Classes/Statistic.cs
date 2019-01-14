using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP.Classes
{
    /// <summary>
    /// This class contains statistics data.
    /// </summary>
    public class Statistic
    {
        #region Class variables
        public int Year { get; set; }
        public double ProjectPerCompetitionAvg { get; set; }
        public int TotalProjects { get; set; }
        public int TotalCompetitions { get; set; }
        public ObservableCollection<CategoryStatistics> CategoryStatistics { get; set; }
        public ObservableCollection<BestProjects> BestProjects { get; set; }
        #endregion

        #region Class constructors
        /// <summary>
        /// Statistic constructor that takes all arguments.
        /// </summary>
        /// <param name="year">The statistics year.</param>
        /// <param name="projectPerCompetitionAvg">The average of projects per competition.</param>
        /// <param name="totalProjects">The total of projects.</param>
        /// <param name="totalCompetitions">The total of competitions.</param>
        /// <param name="categoryStatistics">A list of category statistics.</param>
        /// <param name="bestProjects">A list of the best projects in this statistic.</param>
        public Statistic(int year, double projectPerCompetitionAvg, int totalProjects, int totalCompetitions, ObservableCollection<CategoryStatistics> categoryStatistics, ObservableCollection<BestProjects> bestProjects)
        {
            this.Year = year;
            this.ProjectPerCompetitionAvg = projectPerCompetitionAvg;
            this.TotalProjects = totalProjects;
            this.TotalCompetitions = totalCompetitions;
            this.CategoryStatistics = categoryStatistics;
            this.BestProjects = bestProjects;
        }

        /// <summary>
        /// Statistic constructor that takes no arguments.
        /// Creates a statistic with current year, average of 0, 0 projects, 0 competitions, and an empty list.
        /// </summary>
        public Statistic() : this(DateTime.Now.Year, 0, 0, 0, new ObservableCollection<CategoryStatistics>(), new ObservableCollection<BestProjects>()) { }
        #endregion
    }
}
