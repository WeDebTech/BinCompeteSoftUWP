using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP.Classes
{
    /// <summary>
    /// This class contains the judge member data.
    /// </summary>
    public class JudgeMember
    {
        #region Class variables
        public int Id { get; set; }
        public string Name { get; set; }
        #endregion

        #region Class constructors
        /// <summary>
        /// JudgeMember constructor that takes all arguments.
        /// </summary>
        /// <param name="id">The judge member id.</param>
        /// <param name="name">The judge member name.</param>
        public JudgeMember(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
        #endregion

        /// <summary>
        /// Returns the JudgeMember as a string.
        /// </summary>
        /// <returns>A string containing the JudgeMember's name.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
