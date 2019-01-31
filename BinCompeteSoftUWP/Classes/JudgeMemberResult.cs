using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP.Classes
{
    public class JudgeMemberResult
    {
        #region Class variables
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Voted { get; set; }
        #endregion

        #region Class constructors
        /// <summary>
        /// JudgeMember constructor that takes all arguments.
        /// </summary>
        /// <param name="id">The judge member id.</param>
        /// <param name="name">The judge member name.</param>
        /// <param name="voted">Wether the judge has voted or not.</param>
        public JudgeMemberResult(int id, string name, bool voted)
        {
            this.Id = id;
            this.Name = name;
            this.Voted = voted;
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
