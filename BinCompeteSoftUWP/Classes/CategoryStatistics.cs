using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP.Classes
{
    /// <summary>
    /// This class contains the statistics for a category.
    /// </summary>
    public class CategoryStatistics
    {
        #region Class variables
        public Category Category { get; set; }
        public int TimesUsed { get; set; }
        #endregion

        #region Class constructors
        /// <summary>
        /// Category statistics constructor that takes all arguments.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="timesUsed">The number of times the category was used.</param>
        public CategoryStatistics(Category category, int timesUsed)
        {
            this.Category = category;
            this.TimesUsed = timesUsed;
        }
        #endregion
    }
}
