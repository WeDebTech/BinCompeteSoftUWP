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
    public sealed partial class EditCategoryContentDialog : ContentDialog
    {
        #region Class variables
        private Category categoryToEdit;
        private bool EditingCategory;
        #endregion

        #region Class constructors
        public EditCategoryContentDialog(Category category)
        {
            this.InitializeComponent();

            if (category != null)
            {
                this.categoryToEdit = category;

                EditingCategory = true;
                CategoryNameTextBox.Text = categoryToEdit.Name;
            }
            else
            {
                this.categoryToEdit = new Category();
            }
        }
        #endregion

        #region Class event handlers
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if everything is filled out.
            if (!string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                ErrorTextBlock.Visibility = Visibility.Collapsed;
                ErrorInsertTextBlock.Visibility = Visibility.Collapsed;

                categoryToEdit.Name = CategoryNameTextBox.Text;

                if (EditingCategory)
                {
                    if (await UpdateCategory())
                    {
                        ContentDialog contentDialog = new ContentDialog
                        {
                            Title = "Success",
                            Content = "Updated category successfully.",
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
                    if (await InsertCategory())
                    {
                        ContentDialog contentDialog = new ContentDialog
                        {
                            Title = "Success",
                            Content = "Created category successfully.",
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
        private async Task<bool> UpdateCategory()
        {
            try
            {
                string query = "UPDATE project_category SET category_name = @category_name" +
                    "WHERE id_category = @id_category";

                SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
                cmd.CommandText = query;

                cmd.Parameters.Add(new SqlParameter("category_name", categoryToEdit.Name));

                cmd.Parameters.Add(new SqlParameter("id_category", categoryToEdit.Id));

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
        private async Task<bool> InsertCategory()
        {
            try
            {
                string query = "INSERT INTO project_category (category_name) " +
                    "VALUES (@category_name)";

                SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
                cmd.CommandText = query;

                cmd.Parameters.Add(new SqlParameter("category_name", categoryToEdit.Name));

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
