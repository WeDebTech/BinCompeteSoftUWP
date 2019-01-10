using BinCompeteSoft;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BinCompeteSoftUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();

            // Read config file so we can connect to the database.
            ReadConfigFile();
        }

        /// <summary>
        /// This class will load config.xml and place it's contents in the Settings.
        /// </summary>
        private async void ReadConfigFile()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                // Set the config.xml file path.
                string XMLFilePath = Path.Combine(Package.Current.InstalledLocation.Path, "config.xml");

                // Read the config.xml file contents.
                XDocument settingsData = XDocument.Load(XMLFilePath);

                // Create a new Settings objects with the data read from the file.
                var data = from query in settingsData.Descendants("settings")
                           select new Settings
                           {
                               Ip = (string)query.Element("ip"),
                               DBName = (string)query.Element("dbname"),
                               Security = (string)query.Element("security"),
                               UserId = (string)query.Element("user_id"),
                               Password = (string)query.Element("password")
                           };

                DBSqlHelper.Settings = data.First();

                // Check what data has been loaded.
                if (!string.IsNullOrWhiteSpace(DBSqlHelper.Settings.Ip))
                {
                    builder["Data Source"] = DBSqlHelper.Settings.Ip;
                }
                if (!string.IsNullOrWhiteSpace(DBSqlHelper.Settings.DBName))
                {
                    builder["Initial Catalog"] = DBSqlHelper.Settings.DBName;
                }
                if (!string.IsNullOrWhiteSpace(DBSqlHelper.Settings.Security))
                {
                    builder["Persist Security Info"] = DBSqlHelper.Settings.Security;
                }
                if (!string.IsNullOrWhiteSpace(DBSqlHelper.Settings.UserId))
                {
                    builder["User ID"] = DBSqlHelper.Settings.UserId;
                }
                if (!string.IsNullOrWhiteSpace(DBSqlHelper.Settings.Password))
                {
                    builder["Password"] = DBSqlHelper.Settings.Password;
                }

                // Build connection string and initialize connection with it.
                await DBSqlHelper.InitializeConnection(builder.ConnectionString);
            }
            catch(FileNotFoundException ex)
            {
                ContentDialog errorDialog = new ContentDialog()
                {
                    Title = "Error finding file",
                    Content = "Can't find config.xml, is it in the program root folder?",
                    CloseButtonText = "Ok"
                };

                // Show ContentDialog with error message.
                await errorDialog.ShowAsync();
            }
            catch(Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog()
                {
                    Title = "Error reading file",
                    Content = "Can't read file data, is everything properly named?",
                    CloseButtonText = "Ok"
                };

                // Show ContentDialog with error message.
                await errorDialog.ShowAsync();
            }
        }
    }
}
