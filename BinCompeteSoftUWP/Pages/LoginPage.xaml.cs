using BinCompeteSoftUWP.Classes;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BinCompeteSoftUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        #region Class variables
        private bool ConnectedSuccessfully = false;
        private bool ErroredConnecting = false;
        #endregion

        #region Class constructor
        public LoginPage()
        {
            this.InitializeComponent();
        }
        #endregion

        #region Class methods
        /// <summary>
        /// This class will load config.xml and place it's contents in the Settings.
        /// </summary>
        private void ReadConfigFile()
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

                DBSqlHelper.ConnectionString = builder.ConnectionString;
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
                App.ShowContentDialog(errorDialog, null);
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
                App.ShowContentDialog(errorDialog, null);
            }
        }

        /// <summary>
        /// This method will try to connect to the database.
        /// </summary>
        private async Task ConnectToDBAsync()
        {
            // Reset the values so we can start anew.
            ConnectedSuccessfully = false;
            ErroredConnecting = false;

            // Build connection string and initialize connection with it.
            Task<bool> result = DBSqlHelper.InitializeConnectionAsync();

            bool conected = await result;

            // Check if conection to database has been successfully established.
            if (conected)
            {
                ConnectedSuccessfully = true;
            }
            else
            {
                ErroredConnecting = true;
            }
        }

        /// <summary>
        /// This method will check if the app is connected to the database,
        /// if it errored out, or if the connection hasn't concluded yet.
        /// </summary>
        private async void VerifyIfIsConnectedToDB()
        {
            if (ConnectedSuccessfully)
            {
                await VerifyLoginData();
            }
            else if (ErroredConnecting)
            {
                ContentDialog errorDialog = new ContentDialog()
                {
                    Title = "Error connecting",
                    Content = "Couldn't connect to the database, try again?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No"
                };

                // Create callback to be called when ContentDialog closes.
                Action<ContentDialogResult> callback = async (result) =>
                {
                    if (result == ContentDialogResult.Primary)
                    {
                        await ConnectToDBAsync();
                    }
                };

                // Show ContentDialog with error message.
                App.ShowContentDialog(errorDialog, callback);
            }
            else
            {
                ContentDialog messageDialog = new ContentDialog()
                {
                    Title = "Connecting",
                    Content = "Connecting to database, please be patient.",
                    CloseButtonText = "Ok"
                };

                // Show ContentDialog with error message.
                App.ShowContentDialog(messageDialog, null);
            }
        }

        /// <summary>
        /// Check if everything is filled and check if credentials are correct.
        /// </summary>
        private async Task VerifyLoginData()
        {
            SigningInTextBlock.Visibility = Visibility.Visible;
            SigningInProgressRing.IsActive = true;

            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // Check if all values have been filled.
            if(!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                // Verify if user exists in the database.
                User loggedUser = await Data.Instance.GetUserDataFromDBAsync(username, password);

                // Check if any user was found.
                if(loggedUser != null)
                {
                    // Check if user has valid flag enabled.
                    if (loggedUser.Valid)
                    {
                        // Check if user is logging in for the first time or has had it's password reset.
                        if (loggedUser.FirstTimeLogin)
                        {
                            // Show reset password content dialog.
                            ContentDialog resetPassDialog = new ResetPasswordContentDialog(true, loggedUser.Id);

                            // Create callback to be called when ContentDialog closes.
                            Action<ContentDialogResult> callback = async (result) =>
                            {
                                if (result == ContentDialogResult.Primary)
                                {
                                    SigningInTextBlock.Visibility = Visibility.Collapsed;
                                    SigningInProgressRing.IsActive = false;

                                    LoginUser(loggedUser);
                                }
                                else
                                {
                                    ContentDialog errorMsg = new ContentDialog
                                    {
                                        Title = "Canceled",
                                        Content = "You must reset your password to continue.",
                                        CloseButtonText = "OK"
                                    };

                                    App.ShowContentDialog(errorMsg, null);

                                    SigningInTextBlock.Visibility = Visibility.Collapsed;
                                    SigningInProgressRing.IsActive = false;
                                }
                            };

                            App.ShowContentDialog(resetPassDialog, callback);
                        }
                        else
                        {
                            SigningInTextBlock.Visibility = Visibility.Collapsed;
                            SigningInProgressRing.IsActive = false;

                            LoginUser(loggedUser);
                        }
                    }
                    else
                    {
                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Disabled user",
                            Content = "User has it's account deactivated.\nPlease contact the administrator to reactivate your account.",
                            CloseButtonText = "Ok"
                        };

                        App.ShowContentDialog(errorDialog, null);

                        SigningInTextBlock.Visibility = Visibility.Collapsed;
                        SigningInProgressRing.IsActive = false;
                    }
                }
                else
                {
                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Incorrect credentials",
                        Content = "Login informations are incorrect.\nIf you have forgotten your login details, please contact your administrator to get new login details.",
                        CloseButtonText = "Ok"
                    };

                    App.ShowContentDialog(errorDialog, null);

                    UsernameTextBox.Text = "";
                    PasswordBox.Password = "";

                    SigningInTextBlock.Visibility = Visibility.Collapsed;
                    SigningInProgressRing.IsActive = false;
                }
            }
            else
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "All fields must be filled.",
                    CloseButtonText = "Ok"
                };

                App.ShowContentDialog(errorDialog, null);

                SigningInTextBlock.Visibility = Visibility.Collapsed;
                SigningInProgressRing.IsActive = false;
            }
        }

        /// <summary>
        /// Assigns the given user to the current logged user, and logs in.
        /// </summary>
        /// <param name="userToLogin">The user details to log in.</param>
        private void LoginUser(User userToLogin)
        {
            // Clear all textboxes in case the user logs out.
            UsernameTextBox.Text = "";
            PasswordBox.Password = "";

            // Set the current logged in user to the user that is trying to login.
            Data.Instance.LoggedInUser = userToLogin;

            // Navigate to next page.
            this.Frame.Navigate(typeof(MainPage));
        }
        #endregion

        #region Class event handlers
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Read config file so we can connect to the database.
            ReadConfigFile();

            // Try to connect to the database.
            await ConnectToDBAsync();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            VerifyIfIsConnectedToDB();
        }

        private void PasswordBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            // Check if the entered key is an enter.
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                VerifyIfIsConnectedToDB();
            }
        }

        private void UsernameTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            // Check if the entered key is an enter.
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                VerifyIfIsConnectedToDB();
            }
        }
        #endregion
    }
}
