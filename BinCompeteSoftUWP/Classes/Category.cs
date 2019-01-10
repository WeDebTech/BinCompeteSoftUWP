using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoft
{
    /// <summary>
    /// This class contains the category data.
    /// </summary>
    public class Category
    {
        // Class variables.
        private int id;
        private string name;

        /// <summary>
        /// Category constructor that takes all arguments.
        /// </summary>
        /// <param name="id">The category id.</param>
        /// <param name="name">The category name.</param>
        public Category(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// Category constructor with no arguments.
        /// Creates a constructor with id 0, no name.
        /// </summary>
        public Category() : this(0, "") { }

        /// <summary>
        /// Gets or sets the category id.
        /// </summary>
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Returns the category as a string.
        /// </summary>
        /// <returns>A string containing the category's name.</returns>
        public override string ToString()
        {
            return this.name;
        }
    }
}
