using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoft
{
    /// <summary>
    /// This class contains the judge member data.
    /// </summary>
    public class JudgeMember
    {
        // Class variables.
        private int id;
        private string name;
        private string email;

        /// <summary>
        /// JudgeMember constructor that takes all arguments.
        /// </summary>
        /// <param name="id">The judge member id.</param>
        /// <param name="name">The judge member name.</param>
        /// <param name="email">The judge member email.</param>
        public JudgeMember(int id, string name, string email)
        {
            this.id = id;
            this.name = name;
            this.email = email;
        }

        /// <summary>
        /// Gets or sets the judge member id.
        /// </summary>
        public int Id {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the judge member name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the judge member email.
        /// </summary>
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        /// <summary>
        /// Returns the JudgeMember as a string.
        /// </summary>
        /// <returns>A string containing the JudgeMember's name.</returns>
        public override string ToString()
        {
            return name;
        }
    }
}
