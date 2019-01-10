using BinCompeteSoftUWP;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BinCompeteSoft
{
    /// <summary>
    /// This class is a helper so all database connections pass through here.
    /// </summary>
    public static class DBSqlHelper
    {
        #region Class variables
        // The connection object.
        public static SqlConnection Connection { get; set; }
        public static string ConnectionString { get; set; }
        public static Settings Settings { get; set; }
        #endregion

        #region Class methods
        /// <summary>
        /// Initializes the connection to the database with the provided connection string.
        /// </summary>
        /// <param name="connectionString">The string to connect to the database.</param>
        /// <returns>True if the connection was successfull, false otherwise.</returns>
        public async static Task<bool> InitializeConnectionAsync()
        {
            try
            {
                // Opens a new connection with the provided conection string.
                Connection = new SqlConnection(ConnectionString);

                await Connection.OpenAsync();

                return true;
            }
            catch(Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog()
                {
                    Title = "Error connecting to database",
                    Content = "Couldn't connect to database\nError: " + ex.Message,
                    CloseButtonText = "Ok"
                };

                // Show ContentDialog with error message.
                App.ShowContentDialog(errorDialog, null);

                return false;
            }
        }

        /// <summary>
        /// Hashes a provided password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password.</returns>
        public static string SHA512(string password)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);

            using(var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach(var b in hashedInputBytes)
                {
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                }

                return hashedInputStringBuilder.ToString();
            }
        }
        #endregion
    }

    /// <summary>
    /// This class will store the settings loaded from config.xml.
    /// </summary>
    public class Settings
    {
        #region Class variables
        public string Ip { get; set; }
        public string DBName { get; set; }
        public string Security { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        #endregion
    }
}
