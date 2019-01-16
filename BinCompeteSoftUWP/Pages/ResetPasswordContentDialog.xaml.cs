using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
    public sealed partial class ResetPasswordContentDialog : ContentDialog
    {
        #region Class variables
        private bool NewLogin;
        private int UserId;
        #endregion

        public ResetPasswordContentDialog(bool newLogin, int userId)
        {
            this.NewLogin = newLogin;
            this.UserId = userId;

            this.InitializeComponent();

            // Check if it's a new user.
            if (NewLogin)
            {
                ResetTextBlock.Text = "You are a new user or your password has been reset, please fill your desired password below.";
                CurrentPasswordTextBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                ResetTextBlock.Text = "Please input your current and desired new password below.";
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string CurrentPassword, NewPassword, NewPasswordRetype;

            CurrentPassword = CurrentPasswordTextBox.Password;
            NewPassword = NewPasswordTextBox.Password;
            NewPasswordRetype = NewPasswordRetypeTextBox.Password;

            // Check if all inputs are filled.
            if(!string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(NewPasswordRetype))
            {
                if (NewLogin)
                {
                    // Check if both inputs are the same.
                    if (NewPassword == NewPasswordRetype)
                    {
                        // Check if password meets our security standards.
                        Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})");
                        Match match = regex.Match(NewPassword);

                        if (match.Success)
                        {
                            // Hash the password.
                            string hashedPassword = DBSqlHelper.SHA512(NewPassword);

                            string query = "UPDATE user_table " +
                                "SET pw = @password, first_time_login = 0 " +
                                "WHERE id_user = @id_user";

                            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
                            cmd.CommandText = query;

                            SqlParameter sqlPassword = new SqlParameter("password", SqlDbType.NVarChar);
                            sqlPassword.Value = hashedPassword;
                            cmd.Parameters.Add(sqlPassword);

                            SqlParameter sqlUserId = new SqlParameter("id_user", DbType.Int32);
                            sqlUserId.Value = UserId;
                            cmd.Parameters.Add(sqlUserId);

                            cmd.ExecuteNonQuery();

                            ContentDialog successDialog = new ContentDialog
                            {
                                Title = "Success",
                                Content = "User details updated successfully!",
                                CloseButtonText = "OK"
                            };

                            App.ShowContentDialog(successDialog, null);
                        }
                        else
                        {
                            ResetTextBlock.Text = "The password must contain at least 1 lowercase, 1 uppercase, 1 numeric character, one special character, and be be 8 characters or longer!";
                            NewPasswordTextBox.Password = "";
                            NewPasswordRetypeTextBox.Password = "";

                            args.Cancel = true;
                        }
                    }
                    else
                    {
                        ResetTextBlock.Text = "Both fields must be the same!";
                        NewPasswordTextBox.Password = "";
                        NewPasswordRetypeTextBox.Password = "";

                        args.Cancel = true;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(CurrentPassword))
                    {
                        // Check if both inputs are the same.
                        if (NewPassword == NewPasswordRetype)
                        {
                            // Check if password meets our security standards.
                            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})");
                            Match match = regex.Match(NewPassword);

                            if (match.Success)
                            {
                                // Check if the current password is correct.
                                string hashedPassword = DBSqlHelper.SHA512(CurrentPassword);

                                string query = "SELECT COUNT(*) FROM user_table " +
                                    "WHERE pw = @password";

                                SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
                                cmd.CommandText = query;

                                SqlParameter sqlPassword = new SqlParameter("password", SqlDbType.NVarChar);
                                sqlPassword.Value = hashedPassword;
                                cmd.Parameters.Add(sqlPassword);

                                int count = (int)cmd.ExecuteScalar();

                                if (count >= 1)
                                {

                                    // Hash the password.
                                    hashedPassword = DBSqlHelper.SHA512(NewPassword);

                                    query = "UPDATE user_table " +
                                        "SET pw = @password, first_time_login = 0 " +
                                        "WHERE id_user = @id_user";

                                    cmd = DBSqlHelper.Connection.CreateCommand();
                                    cmd.CommandText = query;

                                    sqlPassword = new SqlParameter("password", SqlDbType.NVarChar);
                                    sqlPassword.Value = hashedPassword;
                                    cmd.Parameters.Add(sqlPassword);

                                    SqlParameter sqlUserId = new SqlParameter("id_user", DbType.Int32);
                                    sqlUserId.Value = Data.Instance.LoggedInUser.Id;
                                    cmd.Parameters.Add(sqlUserId);

                                    cmd.ExecuteNonQuery();

                                    ContentDialog successDialog = new ContentDialog
                                    {
                                        Title = "Success",
                                        Content = "User details updated successfully!",
                                        CloseButtonText = "OK"
                                    };

                                    App.ShowContentDialog(successDialog, null);

                                    this.Hide();
                                }
                                else
                                {
                                    ResetTextBlock.Text = "The password must contain at least 1 lowercase, 1 uppercase, 1 numeric character, 1 special character, and be be 8 characters or longer!";
                                    CurrentPasswordTextBox.Password = "";
                                    NewPasswordTextBox.Password = "";
                                    NewPasswordRetypeTextBox.Password = "";

                                    args.Cancel = true;
                                }
                            }
                            else
                            {
                                ResetTextBlock.Text = "Current password is incorrect!";
                                CurrentPasswordTextBox.Password = "";
                                NewPasswordTextBox.Password = "";
                                NewPasswordRetypeTextBox.Password = "";

                                args.Cancel = true;
                            }
                        }
                        else
                        {
                            ResetTextBlock.Text = "Both new password fields must be the same!";
                            NewPasswordTextBox.Password = "";
                            NewPasswordRetypeTextBox.Password = "";

                            args.Cancel = true;
                        }
                    }
                }
            }
        }

        private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
