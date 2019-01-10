using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoft
{
    /// <summary>
    /// This class contains the statistics for a category.
    /// </summary>
    public class CategoryStatistics
    {
        // Class variables.
        private Category category;
        private int timesUsed;

        /// <summary>
        /// Category statistics constructor that takes all arguments.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="timesUsed">The number of times the category was used.</param>
        public CategoryStatistics(Category category, int timesUsed)
        {
            this.category = category;
            this.timesUsed = timesUsed;
        }

        /// <summary>
        /// Gets or sets the category of the statistic.
        /// </summary>
        public Category Category
        {
            get { return category; }
            set { category = value; }
        }

        /// <summary>
        /// Gets or sets the number of times the category was used.
        /// </summary>
        public int TimesUsed
        {
            get { return timesUsed; }
            set { timesUsed = value; }
        }
    }
}
