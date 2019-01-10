using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoft
{
    /// <summary>
    /// This class contains the criteria data.
    /// </summary>
    public class Criteria
    {
        // Class variables.
        private int id;
        private string name;
        private string description;

        /// <summary>
        /// Criteria constructor that takes all arguments.
        /// </summary>
        /// <param name="id">The criteria id.</param>
        /// <param name="name">The criteria name.</param>
        /// <param name="description">The criteria description.</param>
        public Criteria(int id, string name, string description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Criteria constructor that takes no arguments.
        /// Creates criteria with id = 0, name = "", description = "".
        /// </summary>
        public Criteria() : this(0, "", "") { }

        /// <summary>
        /// Gets or sets the criteria id.
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the criteria name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the criteria description.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}
