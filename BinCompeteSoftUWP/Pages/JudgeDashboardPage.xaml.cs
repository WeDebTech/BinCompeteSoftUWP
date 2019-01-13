using BinCompeteSoftUWP.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// A page where the most important data will be presented to the judge.
    /// </summary>
    public sealed partial class JudgeDashboardPage : Page
    {
        #region Class variables
        private ObservableCollection<NotificationListItem> NotificationsList = new ObservableCollection<NotificationListItem>();

        private ObservableCollection<ContestDetails> ContestDetailsList = new ObservableCollection<ContestDetails>();
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
            await PopulateContestsListAndGenerateNotificationsList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RefreshContestsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowContestDetailsButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Class methods
        /// <summary>
        /// Gets the most up to date contests list and updates the contests list.
        /// </summary>
        private async Task PopulateContestsListAndGenerateNotificationsList()
        {
            // Get current date.
            DateTime CurrentDate = DateTime.Now.Date;

            ContestDetailsList.Clear();

            bool success = await Data.Instance.RefreshContests();

            // Check if data has been loaded successfully.
            if (success)
            {
                // Cycle through all contests, and if they're after today's date, show them.
                // Also show contests that have finished, but haven't had their results calculated of which
                // the current user has created.
                foreach(ContestDetails contestDetails in Data.Instance.ContestDetails)
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
                    if(contestDetails.VotingDate > CurrentDate || (contestDetails.HasBeenCreatedByCurrentUser && !contestDetails.HasResultsCalculated))
                    {
                        // Add contest to DataGridView.
                        ContestDetailsList.Add(contestDetails);
                    }

                    // Check if contest is after the limit date and hasn't been voted yet,
                    // or hasn't had it's results calculated yet.
                    if(!contestDetails.HasBeenCreatedByCurrentUser && contestDetails.LimitDate < CurrentDate && contestDetails.VotingDate > CurrentDate && !contestDetails.HasVoted)
                    {
                        string NotificationEnd;

                        // Check if contest ends today.
                        if((contestDetails.VotingDate - CurrentDate).Days <= 1)
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
                    else if(contestDetails.HasBeenCreatedByCurrentUser && contestDetails.VotingDate < DateTime.Now && !contestDetails.HasResultsCalculated)
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
            }

            // Update the lists.
            ContestsListView.ItemsSource = ContestDetailsList;
            NotificationsListView.ItemsSource = NotificationsList;

            // Sort the DataGrid by ascending limit date.
            //ContestsDataGrid.
        }
        #endregion
    }
}
