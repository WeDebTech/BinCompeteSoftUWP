using BinCompeteSoftUWP.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BinCompeteSoftUWP.Pages
{
    /// <summary>
    /// A page where the most important data will be presented to the judge.
    /// </summary>
    public sealed partial class JudgeDashboardPage : Page
    {
        #region Class variables
        private List<int> StatisticsYears = new List<int>();
        private ObservableCollection<NotificationListItem> NotificationsList = new ObservableCollection<NotificationListItem>();
        private ObservableCollection<BestProjects> BestProjectsList = new ObservableCollection<BestProjects>();
        private ObservableCollection<ContestDetails> ContestDetailsList = new ObservableCollection<ContestDetails>();
        private int SelectedYear;
        #endregion

        #region Class constructors
        public JudgeDashboardPage()
        {
            this.InitializeComponent();
        }
        #endregion

        #region Class event handlers
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await RefreshData();

            PopulateContestsListAndGenerateNotificationsList();

            // Get current year.
            SelectedYear = DateTime.Now.Year;

            PopulateStatisticsList();
        }

        private async void RefreshContestsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingContestProgressRing.IsActive = true;
            LoadingContestsTextBlock.Visibility = Visibility.Visible;

            LoadingNotificationsProgressRing.IsActive = true;
            LoadingNotificationsTextBlock.Visibility = Visibility.Visible;

            await Data.Instance.RefreshContestsAsync();

            PopulateContestsListAndGenerateNotificationsList();
        }

        private void ShowContestDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if any item is selected in contest ListView.
            if(ContestsListView.SelectedIndex != -1)
            {
                ContestDetails contestDetails = (ContestDetails)ContestsListView.SelectedItem;

                // Check if the contest has been created by the current user.
                if (Data.Instance.GetIfContestIsCreatedByCurrentUser(contestDetails.Id))
                {
                    this.Frame.Navigate(typeof(ContestPage), contestDetails);
                }
                else
                {
                    this.Frame.Navigate(typeof(VotingPage), contestDetails);
                }
            }
            else
            {
                ContentDialog errorMsg = new ContentDialog
                {
                    Title = "Error",
                    Content = "You must select a contest to see it's details.",
                    CloseButtonText = "OK"
                };

                App.ShowContentDialog(errorMsg, null);
            }
        }

        private void PreviousYearButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if there's any statistics at all.
            if (StatisticsYears.Count > 0)
            {
                // Check if we're on the last item of the list.
                if (SelectedYear > 0)
                {
                    // Make the selected year the next year.
                    SelectedYear--;

                    ChangeStatisticsYear();
                }
            }
        }

        private void NextYearButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if there's any statistics at all.
            if(StatisticsYears.Count > 0)
            {
                // Check if we're on the last item of the list.
                if(SelectedYear < StatisticsYears.Count - 1)
                {
                    // Make the selected year the next year.
                    SelectedYear++;

                    ChangeStatisticsYear();
                }
            }
        }

        private void NotificationsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get selected notification id.
            int notificationId = ((NotificationListItem)NotificationsListView.SelectedItem).Id;

            // Get which contest that this notification corresponds to.
            ContestDetails contestToSelect = ContestDetailsList.Single(x => x.Id == notificationId);

            // Select the project.
            ContestsListView.SelectedItem = contestToSelect;
        }

        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Get which contest is associated with this grid.
            if(ContestsListView.SelectedItems.Count == 1)
            {
                ContestDetails contestDetails = (ContestDetails)ContestsListView.SelectedItems[0];

                // Check if the contest has been created by the current user.
                if (Data.Instance.GetIfContestIsCreatedByCurrentUser(contestDetails.Id))
                {
                    this.Frame.Navigate(typeof(ContestPage), contestDetails);
                }
                else
                {
                    this.Frame.Navigate(typeof(VotingPage), contestDetails);
                }
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Refreshes every data list.
        /// </summary>
        /// <returns></returns>
        private async Task RefreshData()
        {
            LoadingContestProgressRing.IsActive = true;
            LoadingContestsTextBlock.Visibility = Visibility.Visible;

            LoadingNotificationsProgressRing.IsActive = true;
            LoadingNotificationsTextBlock.Visibility = Visibility.Visible;

            LoadingStatisticsProgressRing.IsActive = true;
            LoadingStatisticsTextBlock.Visibility = Visibility.Visible;

            await Data.Instance.RefreshContestsAsync();

            await Data.Instance.RefreshCategoriesAsync();

            await Data.Instance.RefreshJudgesAsync();

            await Data.Instance.RefreshStatisticsAsync();

            await Data.Instance.RefreshCriteriasAsync();
        }

        /// <summary>
        /// Updates the statistics list.
        /// </summary>
        private void PopulateStatisticsList()
        {
            // Fill list with all statistic years.
            foreach(Statistic statistic in Data.Instance.Statistics)
            {
                // Check all years, and if it's not added yet, add it to the list.
                if(StatisticsYears.Count <= 0)
                {
                    StatisticsYears.Add(statistic.Year);
                }
                else
                {
                    foreach(int statsYears in StatisticsYears)
                    {
                        if(statsYears != statistic.Year)
                        {
                            StatisticsYears.Add(statistic.Year);
                            break;
                        }
                    }
                }
            }

            // Sort the list so all the years are ordered.
            StatisticsYears.Sort();

            // Make the current selected year the most recent one.
            SelectedYear = StatisticsYears.Count - 1;

            // Check if there's any statistic data.
            if (SelectedYear >= 0)
            {
                ChangeStatisticsYear();
            }
        }

        /// <summary>
        /// Updates the contests and notifications list.
        /// </summary>
        private void PopulateContestsListAndGenerateNotificationsList()
        {
            // Get current date.
            DateTime CurrentDate = DateTime.Now.Date;

            ContestDetailsList.Clear();
            NotificationsList.Clear();

            // Cycle through all contests, and if they're after today's date, show them.
            // Also show contests that have finished, but haven't had their results calculated of which
            // the current user has created.
            foreach (ContestDetails contestDetails in Data.Instance.ContestDetails)
            {
                // Check if the project is created by the current user.
                if (Data.Instance.GetIfContestIsCreatedByCurrentUser(contestDetails.Id))
                {
                    contestDetails.HasBeenCreatedByCurrentUser = true;

                    // Check if the contest has already had it's results calculated.
                    contestDetails.HasResultsCalculated = Data.Instance.GetContestResultsCalculatedStatus(contestDetails.Id);
                }
                else
                {
                    // Check if the contest has already been voted by the current user.
                    contestDetails.HasVoted = Data.Instance.GetContestVoteStatus(contestDetails.Id);
                }

                // Check if contest has already ended it's voting date, or if it's created by the current user.
                if (contestDetails.VotingDate > CurrentDate || (contestDetails.HasBeenCreatedByCurrentUser && !contestDetails.HasResultsCalculated))
                {
                    if (!contestDetails.HasVoted)
                    {
                        // Add contest to DataGridView.
                        ContestDetailsList.Add(contestDetails);
                    }
                }

                // Check if contest is after the limit date and hasn't been voted yet,
                // or hasn't had it's results calculated yet.
                if (!contestDetails.HasBeenCreatedByCurrentUser && contestDetails.LimitDate < CurrentDate && contestDetails.VotingDate > CurrentDate && !contestDetails.HasVoted)
                {
                    string NotificationEnd;

                    // Check if contest ends today.
                    if ((contestDetails.VotingDate - CurrentDate).Days <= 1)
                    {
                        NotificationEnd = "' will end in less than a day.";
                    }
                    else
                    {
                        NotificationEnd = "' will end in " + (contestDetails.VotingDate - CurrentDate).Days + " days.";
                    }

                    // Add a notification to the notification list.
                    NotificationsList.Add(new NotificationListItem
                    {
                        Id = contestDetails.Id,
                        Title = "Attention!",
                        Content = "Contest '" + contestDetails.Name + NotificationEnd
                    });
                }
                else if (contestDetails.HasBeenCreatedByCurrentUser && contestDetails.VotingDate < DateTime.Now && !contestDetails.HasResultsCalculated)
                {
                    // Add a notification to the notification list.
                    NotificationsList.Add(new NotificationListItem
                    {
                        Id = contestDetails.Id,
                        Title = "Attention!",
                        Content = "Contest '" + contestDetails.Name + "' has ended it's voting period, but hasn't had it's results calculated yet."
                    });
                }
            }

            // Sort the contests list by voting limit date ascending.
            ContestDetailsList.OrderBy(o => o.VotingDate).ToList();

            LoadingContestProgressRing.IsActive = false;
            LoadingContestsTextBlock.Visibility = Visibility.Collapsed;

            LoadingNotificationsProgressRing.IsActive = false;
            LoadingNotificationsTextBlock.Visibility = Visibility.Collapsed;

            // Update the lists.
            ContestsListView.ItemsSource = ContestDetailsList;
            NotificationsListView.ItemsSource = NotificationsList;
        }

        /// <summary>
        /// Fills the statistics chart with the given statistics year.
        /// </summary>
        /// <param name="year">The year to pull data from.</param>
        private void UpdateCategoryStatisticsChart(int year)
        {
            foreach(Statistic statistic in Data.Instance.Statistics)
            {
                // Check if the statistic is of the provided year.
                if(statistic.Year == year)
                {
                    (StatisticsPieChart.Series[0] as PieSeries).ItemsSource = statistic.CategoryStatistics;
                    break;
                }
            }
        }

        /// <summary>
        /// Fills the best projects DataGrid with the given statistics year.
        /// </summary>
        /// <param name="year">The year to pull data from.</param>
        private async void UpdateBestProjects(int year)
        {
            // Check if statistic is empty.
            if(Data.Instance.Statistics[SelectedYear] != null)
            {
                // Check if the best projects have already been loaded.
                if(Data.Instance.Statistics[SelectedYear].BestProjects.Count <= 0)
                {
                    await RefreshBestProjects(year);
                }

                BestProjectsList = Data.Instance.Statistics[SelectedYear].BestProjects;

                // Update DataGrid items source to the updated list.
                BestProjectsDataGrid.ItemsSource = BestProjectsList;
            }
            else
            {
                ContentDialog errorMsg = new ContentDialog
                {
                    Title = "Error fetching data",
                    Content = "Unable to find the selected year's best projects.",
                    CloseButtonText = "OK"
                };

                App.ShowContentDialog(errorMsg, null);
            }

            LoadingStatisticsProgressRing.IsActive = false;
            LoadingStatisticsTextBlock.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Retrieves the selected year's best projects list.
        /// </summary>
        /// <param name="year">The year to pull data from.</param>
        /// <returns>A list of BestProjects.</returns>
        private async Task RefreshBestProjects(int year)
        {
            // Get the best projects from the database.
            string query = "SELECT TOP 5 proj.name, eval.final_evaluation " +
                "FROM project_table proj " +
                "INNER JOIN final_result_table eval " +
                "ON proj.id_project = eval.id_project " +
                "WHERE proj.project_year = @selected_year " +
                "ORDER BY eval.final_evaluation DESC";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            SqlParameter sqlProjectYear = new SqlParameter("@selected_year", SqlDbType.Int);
            sqlProjectYear.Value = year;
            cmd.Parameters.Add(sqlProjectYear);

            using (DbDataReader reader = await cmd.ExecuteReaderAsync())
            {
                // Check if statistics exist.
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        Data.Instance.Statistics[SelectedYear].BestProjects.Add(new BestProjects(reader[0].ToString(), decimal.ToDouble((decimal)reader[1])));
                    }
                }
            }
        }

        /// <summary>
        /// Fills the given years other statistics.
        /// </summary>
        /// <param name="year">The year to pull data from.</param>
        private void UpdateOtherStatistics(int year)
        {
            // Check if there are any statistics for the selected year.
            if(Data.Instance.Statistics[SelectedYear] != null)
            {
                // Update the TextBlocks with the selected year's statistics.
                TotalCompetitionsTextBlock.Text = Data.Instance.Statistics[SelectedYear].TotalCompetitions.ToString();
                TotalProjectsTextBlock.Text = Data.Instance.Statistics[SelectedYear].TotalProjects.ToString();
                AverageProjectsCompetitionTextBlock.Text = (Data.Instance.Statistics[SelectedYear].TotalProjects / Data.Instance.Statistics[SelectedYear].TotalCompetitions).ToString();
            }
        }

        /// <summary>
        /// Changes the statistics year according to the SelectedYear index.
        /// </summary>
        private void ChangeStatisticsYear()
        {
            // Update the year label to show the current selected year.
            YearTextBlock.Text = StatisticsYears[SelectedYear].ToString();

            // Pass the selected year so it shows the most up-to-date statistics.
            UpdateCategoryStatisticsChart(StatisticsYears[SelectedYear]);
            UpdateBestProjects(StatisticsYears[SelectedYear]);
            UpdateOtherStatistics(StatisticsYears[SelectedYear]);
        }
        #endregion
    }
}