﻿using BinCompeteSoftUWP.Classes;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContestPage : Page
    {
        #region Class variables
        private ContestDetails ContestToLoad;
        private Contest ContestToEdit;
        private bool EditingContest = false;
        private bool ContestStarted, ContestEnded, ContestEndedVoting;
        public ObservableCollection<JudgeMember> Judges { get; set; } = new ObservableCollection<JudgeMember>();
        public ObservableCollection<Project> Projects { get; set; } = new ObservableCollection<Project>();
        public ObservableCollection<Criteria> Criterias { get; set; } = new ObservableCollection<Criteria>();
        public ObservableCollection<JudgeMember> JudgesToAdd { get; set; } = new ObservableCollection<JudgeMember>();
        public ObservableCollection<Project> ProjectsToAdd { get; set; } = new ObservableCollection<Project>();
        public ObservableCollection<Criteria> CriteriasToAdd { get; set; } = new ObservableCollection<Criteria>();
        public ObservableCollection<Project> ProjectsToEdit { get; set; } = new ObservableCollection<Project>();
        public ObservableCollection<JudgeMember> JudgesToRemove { get; set; } = new ObservableCollection<JudgeMember>();
        public ObservableCollection<Project> ProjectsToRemove { get; set; } = new ObservableCollection<Project>();
        public ObservableCollection<Criteria> CriteriasToRemove { get; set; } = new ObservableCollection<Criteria>();

        private string ContestDescription = "";
        #endregion

        #region Class constructors
        public ContestPage()
        {
            this.InitializeComponent();

            // Set minimum dates for all CalendarDateTimePicker controls if it's a new contest.
            if (EditingContest)
            {
                StartDateCalendarDatePicker.MinDate = DateTime.Now.Date;
                LimitDateCalendarDatePicker.MinDate = DateTime.Now.Date;
                VotingLimitDateCalendarDatePicker.MinDate = DateTime.Now.Date;
            }
        }
        #endregion

        #region Class event handlers
        private void StartDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            LimitDateCalendarDatePicker.MinDate = args.NewDate.Value;
        }

        private void LimitDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            VotingLimitDateCalendarDatePicker.MinDate = args.NewDate.Value;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoadJudgesProgressRing.IsActive = true;
            LoadJudgesTextBlock.Visibility = Visibility.Visible;

            LoadProjectsProgressRing.IsActive = true;
            LoadProjectsTextBlock.Visibility = Visibility.Visible;

            LoadCriteriasProgressRing.IsActive = true;
            LoadCriteriasTextBlock.Visibility = Visibility.Visible;

            ContestToLoad = (ContestDetails)e.Parameter;

            if(ContestToLoad != null)
            {
                EditingContest = true;
                await LoadContestDetailsAsync();
            }
            else
            {
                ContestToEdit = new Contest(-1, ContestToLoad, new ObservableCollection<Project>(), new ObservableCollection<JudgeMember>(), new ObservableCollection<Criteria>(), new double[0, 0]);
                ContestToEdit.ContestDetails = new ContestDetails();
            }

            LoadJudgesProgressRing.IsActive = false;
            LoadJudgesTextBlock.Visibility = Visibility.Collapsed;

            LoadProjectsProgressRing.IsActive = false;
            LoadProjectsTextBlock.Visibility = Visibility.Collapsed;

            LoadCriteriasProgressRing.IsActive = false;
            LoadCriteriasTextBlock.Visibility = Visibility.Collapsed;

            Judges = ContestToEdit.JudgeMembers;
            Projects = ContestToEdit.Projects;
            Criterias = ContestToEdit.Criterias;

            JudgesListView.ItemsSource = Judges;
            ProjectsListView.ItemsSource = Projects;
            CriteriaListView.ItemsSource = Criterias;
        }

        private void AddJudgeButton_Click(object sender, RoutedEventArgs e)
        {
            AddJudgeContentDialog addJudgeContentDialog = new AddJudgeContentDialog(this);

            App.ShowContentDialog(addJudgeContentDialog, null);
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            EditProjectContentDialog editProjectContentDialog = new EditProjectContentDialog(this, null);

            App.ShowContentDialog(editProjectContentDialog, null);
        }

        private void AddCriteriaButton_Click(object sender, RoutedEventArgs e)
        {
            AddCriteriaContentDialog addCriteriaContentDialog = new AddCriteriaContentDialog(this);

            App.ShowContentDialog(addCriteriaContentDialog, null);
        }

        private void CancelContestButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog contentDialog = new ContentDialog
            {
                Title = "Cancel?",
                Content = "Do you really wish to cancel the creation of this contest?",
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

        private void ShowResultsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VotingPage));
        }

        private void CreateContestButton_Click(object sender, RoutedEventArgs e)
        {
            InsertContestToDB();
        }

        private void AddDescriptionButton_Click(object sender, RoutedEventArgs e)
        {
            EditContestDescriptionContentDialog editContestDescriptionContentDialog = new EditContestDescriptionContentDialog(this, ContestToEdit.ContestDetails.Description);

            App.ShowContentDialog(editContestDescriptionContentDialog, null);
        }

        private void RemoveJudgeButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which item was clicked.
            var item = ((FrameworkElement)sender).DataContext;

            // Remove judge from the list.
            Judges.Remove((JudgeMember)item);
            JudgesToRemove.Add((JudgeMember)item);
        }

        private void RemoveCriteriaButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which item was clicked.
            var item = ((FrameworkElement)sender).DataContext;

            // Remove judge from the list.
            Criterias.Remove((Criteria)item);
            CriteriasToRemove.Add((Criteria)item);
        }

        private void RemoveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which item was clicked.
            var item = ((FrameworkElement)sender).DataContext;

            // Remove judge from the list.
            Projects.Remove((Project)item);
            ProjectsToRemove.Add((Project)item);
        }

        private void EditProjectButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which item was clicked.
            var item = ((FrameworkElement)sender).DataContext;

            EditProjectContentDialog editProjectContentDialog = new EditProjectContentDialog(this, (Project)item);

            App.ShowContentDialog(editProjectContentDialog, null);
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
                ContestStarted = true;
                ContestEnded = true;
                ContestEndedVoting = true;

                DisableContestFields();
            }
            // Check if contest has already ended.
            else if(ContestToLoad.LimitDate < DateTime.Now.Date)
            {
                ContestStarted = true;
                ContestEnded = true;

                DisableContestFields();
            }
            // Check if contest has alreadt started.
            else if(ContestToLoad.StartDate < DateTime.Now.Date)
            {
                ContestStarted = true;

                DisableContestFields();
            }

            await LoadContestAsync();
        }

        /// <summary>
        /// Disables selected controls on the page according to the contest status.
        /// </summary>
        private void DisableContestFields()
        {
            ShowResultsButton.IsEnabled = false;

            if (ContestStarted)
            {
                StartDateCalendarDatePicker.IsEnabled = false;
            }
            if (ContestEnded)
            {
                ContestNameTextBox.IsEnabled = false;
                LimitDateCalendarDatePicker.IsEnabled = false;
                AddJudgeButton.IsEnabled = false;
                AddProjectButton.IsEnabled = false;
                AddCriteriaButton.IsEnabled = false;
            }
            if (ContestEndedVoting)
            {
                VotingLimitDateCalendarDatePicker.IsEnabled = false;
                ShowResultsButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Load the contest details from the database.
        /// </summary>
        /// <returns></returns>
        private async Task LoadContestAsync()
        {
            ContestToEdit = await Data.Instance.GetContest(ContestToLoad.Id);

            FillContestDetails();
        }

        /// <summary>
        /// Fill all fields with the contest details.
        /// </summary>
        private void FillContestDetails()
        {
            ContestNameTextBox.Text = ContestToEdit.ContestDetails.Name;
            StartDateCalendarDatePicker.Date = ContestToEdit.ContestDetails.StartDate.Date;
            LimitDateCalendarDatePicker.Date = ContestToEdit.ContestDetails.LimitDate.Date;
            VotingLimitDateCalendarDatePicker.Date = ContestToEdit.ContestDetails.VotingDate.Date;
            ContestDescription = ContestToEdit.ContestDetails.Description;
        }

        /// <summary>
        /// Add a judge to the contest.
        /// </summary>
        /// <param name="judgeMember">The judge to add</param>
        public void AddJudge(JudgeMember judgeMember)
        {
            Judges.Add(judgeMember);
            JudgesToAdd.Add(judgeMember);
        }

        /// <summary>
        /// Add a criteria to the contest.
        /// </summary>
        /// <param name="criteria">The criteria to add</param>
        public void AddCriteria(Criteria criteria)
        {
            Criterias.Add(criteria);
            CriteriasToAdd.Add(criteria);
        }

        /// <summary>
        /// Add a project to the contest.
        /// </summary>
        /// <param name="project">The project to add</param>
        public void AddProject(Project project)
        {
            Projects.Add(project);
            ProjectsToAdd.Add(project);
        }

        /// <summary>
        /// Edits an existing project in the contest.
        /// </summary>
        public void EditProject(Project project)
        {
            int index = Projects.IndexOf(project);
            if(index != -1)
            {
                Projects[index] = project;
                ProjectsToEdit.Add(project);
            }

            ProjectsListView.ItemsSource = Projects;
        }

        /// <summary>
        /// Edits the contest description.
        /// </summary>
        public void EditDescription(string description)
        {
            ContestToEdit.ContestDetails.Description = description;
        }

        private async void InsertContestToDB()
        {

        }
        #endregion
    }
}
