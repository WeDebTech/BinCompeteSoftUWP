using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoft
{
    /// <summary>
    /// This class contains the user data.
    /// </summary>
    public class User
    {
        #region Class variables
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool Administrator { get; set; }
        public bool FirstTimeLogin { get; set; }
        public bool Valid { get; set; }
        #endregion

        #region Class constructors
        /// <summary>
        /// User constructor that takes all arguments.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="name">The user name.</param>
        /// <param name="email">The user email.</param>
        /// <param name="username">The user username.</param>
        /// <param name="administrator">The user administrator status.</param>
        /// <param name="firstTimeLogin">The user first time login status.</param>
        /// <param name="valid">The user valid status.</param>
        public User(int id, string name, string email, string username, bool administrator, bool firstTimeLogin, bool valid)
        {
            this.Id = id;
            this.Name = name;
            this.Email = email;
            this.Username = username;
            this.Administrator = administrator;
            this.FirstTimeLogin = firstTimeLogin;
            this.Valid = valid;
        }
        #endregion
    }
}
