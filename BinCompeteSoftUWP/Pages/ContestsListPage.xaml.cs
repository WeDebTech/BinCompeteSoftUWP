using BinCompeteSoftUWP.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ContestsListPage : Page
    {
        #region Class variables

        #endregion

        #region Class constructors
        public ContestsListPage()
        {
            this.InitializeComponent();
        }
        #endregion

        #region Class event handlers
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if any item is selected in contest ListView.
            if (ContestsDataGrid.SelectedIndex != -1)
            {
                ContestDetails contestDetails = (ContestDetails)ContestsDataGrid.SelectedItem;

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

        private void ContestsDataGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Check if any item is selected in contest ListView.
            if (ContestsDataGrid.SelectedIndex != -1)
            {
                ContestDetails contestDetails = (ContestDetails)ContestsDataGrid.SelectedItem;

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

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }
        #endregion

        #region Class methods
        private async void RefreshData()
        {
            LoadingContestsProgressRing.IsActive = true;
            LoadingContestsTextBlock.Visibility = Visibility.Visible;

            await Data.Instance.RefreshContestsAsync();

            ContestsDataGrid.ItemsSource = Data.Instance.ContestDetails.OrderByDescending(contest => contest.LimitDate).ToList();

            // Cycle through all contests.
            foreach(ContestDetails contestDetails in Data.Instance.ContestDetails)
            {
                contestDetails.EveryoneVoted = Data.Instance.GetContestAllJudgesVoteStatus(contestDetails.Id);

                if (contestDetails.EveryoneVoted)
                {
                    contestDetails.Status = 2;
                }
                // Check if contest is in voting date.
                else if(contestDetails.LimitDate < DateTime.Now.Date && contestDetails.VotingDate > DateTime.Now.Date)
                {
                    contestDetails.Status = 1;
                }
            }

            LoadingContestsProgressRing.IsActive = false;
            LoadingContestsTextBlock.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}
