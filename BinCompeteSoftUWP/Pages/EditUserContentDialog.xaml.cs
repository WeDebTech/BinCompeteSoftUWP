using BinCompeteSoftUWP.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BinCompeteSoftUWP.Pages
{
    public sealed partial class EditUserContentDialog : ContentDialog
    {
        #region Class variables
        private User userToEdit;
        private bool EditingUser = false;
        #endregion

        #region Class constructors
        public EditUserContentDialog(User user)
        {
            this.InitializeComponent();

            if (user != null)
            {
                this.userToEdit = user;

                EditingUser = true;
                UserNameTextBox.Text = userToEdit.Username;
                UserNickTextBox.Text = userToEdit.Name;
                UserAdminCheckBox.IsChecked = userToEdit.Administrator;
                UserValidCheckBox.IsChecked = userToEdit.Valid;
                UserPasswordResetCheckBox.IsChecked = userToEdit.FirstTimeLogin;
            }
            else
            {
                this.userToEdit = new User(-1, "", "", false, false, false);
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Update the user in the database.
        /// </summary>
        private async Task<bool> UpdateUser()
        {
            try
            {
                string query = "UPDATE user_table SET username = @username, fullname = @fullname, administrator = @administrator, first_time_login = @first_time_login, valid = @valid " +
                    "WHERE id_user = @id_user";

                SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
                cmd.CommandText = query;

                cmd.Parameters.Add(new SqlParameter("username", userToEdit.Name));

                cmd.Parameters.Add(new SqlParameter("fullname", userToEdit.Username));

                cmd.Parameters.Add(new SqlParameter("administrator", userToEdit.Administrator));

                cmd.Parameters.Add(new SqlParameter("first_time_login", userToEdit.FirstTimeLogin));

                cmd.Parameters.Add(new SqlParameter("valid", userToEdit.Valid));

                cmd.Parameters.Add(new SqlParameter("id_user", userToEdit.Id));

                await cmd.ExecuteNonQueryAsync();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Insert an user in the database.
        /// </summary>
        private async Task<bool> InsertUser()
        {
            try
            {
                string query = "INSERT INTO user_table (username, fullname, administrator, first_time_login, valid) " +
                    "VALUES (@username, @fullname,  @administrator, @first_time_login, @valid)";

                SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
                cmd.CommandText = query;

                cmd.Parameters.Add(new SqlParameter("username", userToEdit.Name));

                cmd.Parameters.Add(new SqlParameter("fullname", userToEdit.Username));

                cmd.Parameters.Add(new SqlParameter("administrator", userToEdit.Administrator));

                cmd.Parameters.Add(new SqlParameter("first_time_login", userToEdit.FirstTimeLogin));

                cmd.Parameters.Add(new SqlParameter("valid", userToEdit.Valid));

                await cmd.ExecuteNonQueryAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Class event handlers
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if everything is filled out.
            if(!string.IsNullOrWhiteSpace(UserNameTextBox.Text) && !string.IsNullOrWhiteSpace(UserNickTextBox.Text))
            {
                ErrorTextBlock.Visibility = Visibility.Collapsed;
                ErrorInsertTextBlock.Visibility = Visibility.Collapsed;

                userToEdit.Name = UserNickTextBox.Text;
                userToEdit.Username = UserNameTextBox.Text;
                userToEdit.Valid = (bool)UserValidCheckBox.IsChecked;
                userToEdit.Administrator = (bool)UserAdminCheckBox.IsChecked;
                userToEdit.FirstTimeLogin = (bool)UserPasswordResetCheckBox.IsChecked;

                if (EditingUser)
                {
                    if(await UpdateUser())
                    {
                        ContentDialog contentDialog = new ContentDialog
                        {
                            Title = "Success",
                            Content = "Updated user successfully.",
                            CloseButtonText = "OK"
                        };

                        App.ShowContentDialog(contentDialog, null);
                    }
                    else
                    {
                        ErrorInsertTextBlock.Visibility = Visibility.Visible;

                        args.Cancel = true;
                    }
                }
                else
                {
                    if (await InsertUser())
                    {
                        ContentDialog contentDialog = new ContentDialog
                        {
                            Title = "Success",
                            Content = "Created user successfully.",
                            CloseButtonText = "OK"
                        };

                        App.ShowContentDialog(contentDialog, null);
                    }
                    else
                    {
                        ErrorInsertTextBlock.Visibility = Visibility.Visible;

                        args.Cancel = true;
                    }
                }
            }
            else
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        #endregion
    }
}
