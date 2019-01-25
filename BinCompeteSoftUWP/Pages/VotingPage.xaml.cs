using BinCompeteSoftUWP.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BinCompeteSoftUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VotingPage : Page
    {
        #region Class variables
        private ContestDetails ContestToLoad;
        private Contest ContestToEdit;
        private ObservableCollection<ObservableCollection<string>> criteriaValues = new ObservableCollection<ObservableCollection<string>>();
        private List<CriteriaList> criteriaList = new List<CriteriaList>();
        private List<CriteriaProjectsList> criteriaProjectsList = new List<CriteriaProjectsList>();
        
        #endregion

        #region Class constructors
        public VotingPage()
        {
            this.InitializeComponent();
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Set the correct bool variables indicating the contest status and loads the contest details.
        /// </summary>
        private async Task LoadContestDetailsAsync()
        {
            // Check if the contest has already ended it's voting time.
            if (ContestToLoad.VotingDate < DateTime.Now.Date)
            {
                ContentDialog errorMsg = new ContentDialog
                {
                    Title = "Error",
                    Content = "Voting hasn't started yet for this contest.\nPlease come back when the voting period begins.",
                    PrimaryButtonText = "OK"
                };

                App.ShowContentDialog(errorMsg, null);

                this.Frame.GoBack();
            }
            else
            {
                ContestToEdit = await Data.Instance.GetContest(ContestToLoad.Id);

                // Fill the list of criterias to judge against each other.
                for(int i = 0; i < ContestToEdit.Criterias.Count; i++)
                {
                    for (int j = i + 1; j < ContestToEdit.Criterias.Count; j++)
                    {
                        criteriaList.Add(new CriteriaList {
                            Criteria1 = ContestToEdit.Criterias[i],
                            Criteria2 = ContestToEdit.Criterias[j],
                            Value = 1
                        });
                    }

                    // Fill the list of projects to judge against each other in each criteria.
                    for(int j = 0; j < ContestToEdit.Projects.Count; j++)
                    {
                        for(int k = j + 1; k < ContestToEdit.Projects.Count; k++)
                        {
                            criteriaProjectsList.Add(new CriteriaProjectsList
                            {
                                Criteria = ContestToEdit.Criterias[i],
                                Project1 = ContestToEdit.Projects[j],
                                Project2 = ContestToEdit.Projects[k],
                                Value = 1
                            });
                        }
                    }
                }

                CriteriasComboBox.ItemsSource = ContestToEdit.Criterias;

                CriteriasListView.ItemsSource = criteriaList;
            }
        }

        /// <summary>
        /// Inserts all the voting data for this contest in the database.
        /// </summary>
        private async Task<bool> InsertVotingDataIntoDB()
        {
            try
            {
                string query = "INSERT INTO contest_juri_criteria_table " +
                    "VALUES (@id_contest, @id_user, @id_criteria1, @id_criteria2, @value)";

                SqlCommand cmd;

                // Cycle through all criterias.
                foreach (CriteriaList criteria in criteriaList)
                { 
                    cmd = DBSqlHelper.Connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.Parameters.Add(new SqlParameter("@id_contest", ContestToEdit.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_user", Data.Instance.LoggedInUser.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_criteria1", criteria.Criteria1.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_criteria2", criteria.Criteria2.Id));

                    cmd.Parameters.Add(new SqlParameter("@value", criteria.Value));

                    await cmd.ExecuteNonQueryAsync();
                }

                query = "INSERT INTO evaluation_table " +
                    "VALUES (@id_user, @id_contest, @id_criteria, @id_project1, @id_project2, @value)";

                // Cycle through all projects.
                foreach(CriteriaProjectsList criteriaProjects in criteriaProjectsList)
                {
                    cmd = DBSqlHelper.Connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.Parameters.Add(new SqlParameter("@id_contest", ContestToEdit.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_user", Data.Instance.LoggedInUser.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_criteria", criteriaProjects.Criteria.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_project1", criteriaProjects.Project1.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_project2", criteriaProjects.Project2.Id));

                    cmd.Parameters.Add(new SqlParameter("@value", criteriaProjects.Value));

                    await cmd.ExecuteNonQueryAsync();
                }

                // Update the user voted status.
                query = "UPDATE contest_juri_table SET has_voted = 1 WHERE id_contest = @id_contest AND id_user = @id_user";

                cmd = DBSqlHelper.Connection.CreateCommand();
                cmd.CommandText = query;

                cmd.Parameters.Add(new SqlParameter("@id_contest", ContestToEdit.Id));

                cmd.Parameters.Add(new SqlParameter("@id_user", Data.Instance.LoggedInUser.Id));

                await cmd.ExecuteNonQueryAsync();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Class event handlers
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ContestToLoad = (ContestDetails)e.Parameter;

            await LoadContestDetailsAsync();
        }

        private void CriteriasComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Criteria selectedCriteria = (Criteria)e.AddedItems[0];

            List<CriteriaProjectsList> projectsList = criteriaProjectsList.FindAll(criteria => criteria.Criteria.Id == selectedCriteria.Id);

            ProjectsListView.ItemsSource = projectsList;
        }

        private async void SendVoteButton_Click(object sender, RoutedEventArgs e)
        {
            OverlayRectangle.Visibility = Visibility.Visible;
            InsertVoteProgressRing.IsActive = true;
            InsertVoteTextBlock.Visibility = Visibility.Visible;

            if(await InsertVotingDataIntoDB())
            {
                ContentDialog contentMsg = new ContentDialog
                {
                    Title = "Success",
                    Content = "Updated contest successfully.",
                    PrimaryButtonText = "OK"
                };

                App.ShowContentDialog(contentMsg, null);

                this.Frame.Navigate(typeof(JudgeDashboardPage));
            }
            else
            {
                OverlayRectangle.Visibility = Visibility.Collapsed;
                InsertVoteProgressRing.IsActive = false;
                InsertVoteTextBlock.Visibility = Visibility.Collapsed;

                ContentDialog errorMsg = new ContentDialog
                {
                    Title = "Error",
                    Content = "Error updating the contest, try again later.",
                    PrimaryButtonText = "OK"
                };

                App.ShowContentDialog(errorMsg, null);
            }
        }

        private void CancelVoteButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog contentDialog = new ContentDialog
            {
                Title = "Cancel?",
                Content = "Do you really wish to stop voting for this contest?",
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No"
            };

            // Create callback to be called when ContentDialog closes.
            Action<ContentDialogResult> callback = (result) =>
            {
                if (result == ContentDialogResult.Primary)
                {
                    Frame.GoBack();
                }
            };

            App.ShowContentDialog(contentDialog, callback);
        }

        private void ProjectImportanceSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            // Check if value is 0.
            if(e.NewValue == 0)
            {
                ((Slider)sender).Value = e.OldValue;
            }
        }

        private void CriteriaImportanceSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            // Check if value is 0.
            if (e.NewValue == 0)
            {
                ((Slider)sender).Value = e.OldValue;
            }
        }
        #endregion
    }

    class CriteriaList
    {
        public Criteria Criteria1 { get; set; }
        public Criteria Criteria2 { get; set; }
        public int Value { get; set; }
    }

    class CriteriaProjectsList
    {
        public Criteria Criteria { get; set; }
        public Project Project1 { get; set; }
        public Project Project2 { get; set; }
        public int Value { get; set; }
    }
}
