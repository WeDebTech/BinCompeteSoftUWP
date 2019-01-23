using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP.Classes
{
    /// <summary>
    /// This class contains the criteria data.
    /// </summary>
    public class Criteria
    {
        #region Class variables
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        #endregion

        #region Class constructors
        /// <summary>
        /// Criteria constructor that takes all arguments.
        /// </summary>
        /// <param name="id">The criteria id.</param>
        /// <param name="name">The criteria name.</param>
        /// <param name="description">The criteria description.</param>
        public Criteria(int id, string name, string description)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
        }

        /// <summary>
        /// Criteria constructor that takes no arguments.
        /// Creates criteria with id = 0, name = "", description = "".
        /// </summary>
        public Criteria() : this(0, "", "") { }
        #endregion

        #region Class methods
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
