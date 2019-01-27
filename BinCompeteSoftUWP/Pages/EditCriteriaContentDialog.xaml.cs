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
    public sealed partial class EditCriteriaContentDialog : ContentDialog
    {
        #region Class variables
        private Criteria criteriaToEdit;
        private bool EditingCriteria;
        #endregion

        #region Class constructors
        public EditCriteriaContentDialog(Criteria criteria)
        {
            this.InitializeComponent();

            if (criteria != null)
            {
                this.criteriaToEdit = criteria;

                EditingCriteria = true;
                CriteriaNameTextBox.Text = criteriaToEdit.Name;
                CriteriaDescriptionTextBox.Text = criteriaToEdit.Description;
            }
            else
            {
                this.criteriaToEdit = new Criteria();
            }
        }
        #endregion

        #region Class event handlers
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if everything is filled out.
            if (!string.IsNullOrWhiteSpace(CriteriaNameTextBox.Text) && !string.IsNullOrWhiteSpace(CriteriaDescriptionTextBox.Text))
            {
                ErrorTextBlock.Visibility = Visibility.Collapsed;
                ErrorInsertTextBlock.Visibility = Visibility.Collapsed;

                criteriaToEdit.Name = CriteriaNameTextBox.Text;
                criteriaToEdit.Description = CriteriaDescriptionTextBox.Text;

                if (EditingCriteria)
                {
                    if (await UpdateCriteria())
                    {
                        ContentDialog contentDialog = new ContentDialog
                        {
                            Title = "Success",
                            Content = "Updated criteria successfully.",
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
                    if (await InsertCriteria())
                    {
                        ContentDialog contentDialog = new ContentDialog
                        {
                            Title = "Success",
                            Content = "Created criteria successfully.",
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

        #region Class methods
        /// <summary>
        /// Update the criteria in the database.
        /// </summary>
        private async Task<bool> UpdateCriteria()
        {
            try
            {
                string query = "UPDATE criteria_table SET name = @name, description = @description " +
                    "WHERE id_criteria = @id_criteria";

                SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
                cmd.CommandText = query;

                cmd.Parameters.Add(new SqlParameter("name", criteriaToEdit.Name));

                cmd.Parameters.Add(new SqlParameter("description", criteriaToEdit.Description));

                cmd.Parameters.Add(new SqlParameter("id_criteria", criteriaToEdit.Id));

                await cmd.ExecuteNonQueryAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Insert a criteria in the database.
        /// </summary>
        private async Task<bool> InsertCriteria()
        {
            try
            {
                string query = "INSERT INTO criteria_table (name, description) " +
                    "VALUES (@name, @description)";

                SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
                cmd.CommandText = query;

                cmd.Parameters.Add(new SqlParameter("name", criteriaToEdit.Name));

                cmd.Parameters.Add(new SqlParameter("description", criteriaToEdit.Description));

                await cmd.ExecuteNonQueryAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
