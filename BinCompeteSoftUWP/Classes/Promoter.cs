using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP.Classes
{
    public class Promoter
    {
        #region Class variables
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        #endregion

        #region Class constructors
        /// <summary>
        /// Promoter constructor that takes all arguments.
        /// </summary>
        /// <param name="id">The promoter id.</param>
        /// <param name="name">The promoter name.</param>
        /// <param name="dateOfBirth">The promoter's date of birth.</param>
        public Promoter(int id, string name, DateTime dateOfBirth)
        {
            Id = id;
            Name = name;
            DateOfBirth = dateOfBirth;
        }

        /// <summary>
        /// Promoter constructor that takes no arguments.
        /// </summary>
        public Promoter() : this(0, "", DateTime.Now.Date) { }
        #endregion

        #region Class methods
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
