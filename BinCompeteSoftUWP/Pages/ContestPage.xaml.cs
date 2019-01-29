using BinCompeteSoftUWP.Classes;
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
using Windows.System;
using Windows.UI.Core;
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

            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatch_AcceleratorKeyActivated;
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
                ContestToEdit = new Contest(-1, ContestToLoad, new ObservableCollection<Project>(), new ObservableCollection<JudgeMember>(), new ObservableCollection<Criteria>());
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
            ContestResultsContentDialog contestResultsContentDialog = new ContestResultsContentDialog(ContestToEdit);

            App.ShowContentDialog(contestResultsContentDialog, null);
        }

        private async void CreateContestButton_Click(object sender, RoutedEventArgs e)
        {
            bool errored = false;
            string errorList = "";

            // Check if everything has been filled out.
            if (string.IsNullOrWhiteSpace(ContestNameTextBox.Text))
            {
                errored = true;
                errorList += "Contest name\n";
            }
            if (StartDateCalendarDatePicker.Date == null || StartDateCalendarDatePicker.Date < DateTime.Now.Date)
            {
                errored = true;
                errorList += "Contest start date\n";
            }
            if (LimitDateCalendarDatePicker.Date== null || LimitDateCalendarDatePicker.Date < DateTime.Now.Date || LimitDateCalendarDatePicker.Date < StartDateCalendarDatePicker.Date)
            {
                errored = true;
                errorList += "Contest limit date\n";
            }
            if (VotingLimitDateCalendarDatePicker.Date == null || VotingLimitDateCalendarDatePicker.Date < DateTime.Now.Date || VotingLimitDateCalendarDatePicker.Date < LimitDateCalendarDatePicker.Date || VotingLimitDateCalendarDatePicker.Date < StartDateCalendarDatePicker.Date)
            {
                errored = true;
                errorList += "Contest voting limit date\n";
            }
            if (string.IsNullOrWhiteSpace(ContestToEdit.ContestDetails.Description))
            {
                errored = true;
                errorList += "Contest description\n";
            }
            if(Judges.Count <= 0)
            {
                errored = true;
                errorList += "Contest judges\n";
            }
            if(Criterias.Count <= 0)
            {
                errored = true;
                errorList += "Contest criterias\n";
            }
            if (errored)
            {
                ContentDialog errorMsg = new ContentDialog
                {
                    Title = "Error",
                    Content = "You must fill the missing fields to create this contest:\n" + errorList,
                    PrimaryButtonText = "OK"
                };

                App.ShowContentDialog(errorMsg, null);
            }
            else
            {
                // Update the contest with the provided details.
                ContestToEdit.ContestDetails.Name = ContestNameTextBox.Text;
                ContestToEdit.ContestDetails.StartDate = ((DateTimeOffset)StartDateCalendarDatePicker.Date).DateTime.Date;
                ContestToEdit.ContestDetails.LimitDate = ((DateTimeOffset)LimitDateCalendarDatePicker.Date).DateTime.Date;
                ContestToEdit.ContestDetails.VotingDate = ((DateTimeOffset)VotingLimitDateCalendarDatePicker.Date).DateTime.Date;
                ContestToEdit.JudgeMembers = Judges;
                ContestToEdit.Projects = Projects;
                ContestToEdit.Criterias = Criterias;

                OverlayRectangle.Visibility = Visibility.Visible;
                InsertContestProgressRing.IsActive = true;
                InsertContestTextBlock.Visibility = Visibility.Visible;

                if (EditingContest)
                {
                    if(await UpdateContestInDb())
                    {
                        ContentDialog contentMsg = new ContentDialog
                        {
                            Title = "Success",
                            Content = "Updated contest successfully.",
                            PrimaryButtonText = "OK"
                        };

                        App.ShowContentDialog(contentMsg, null);

                        if (Data.Instance.LoggedInUser.Administrator)
                        {
                            this.Frame.Navigate(typeof(AdministratorDashboardPage));
                        }
                        else
                        {
                            this.Frame.Navigate(typeof(JudgeDashboardPage));
                        }
                    }
                    else
                    {
                        OverlayRectangle.Visibility = Visibility.Collapsed;
                        InsertContestProgressRing.IsActive = false;
                        InsertContestTextBlock.Visibility = Visibility.Collapsed;

                        ContentDialog errorMsg = new ContentDialog
                        {
                            Title = "Error",
                            Content = "Error updating the contest, try again later.",
                            PrimaryButtonText = "OK"
                        };

                        App.ShowContentDialog(errorMsg, null);
                    }
                }
                else
                {
                    if(await InsertContestToDB())
                    {
                        ContentDialog contentMsg = new ContentDialog
                        {
                            Title = "Success",
                            Content = "Created contest successfully.",
                            PrimaryButtonText = "OK"
                        };

                        App.ShowContentDialog(contentMsg, null);

                        this.Frame.Navigate(typeof(JudgeDashboardPage));
                    }
                    else
                    {
                        OverlayRectangle.Visibility = Visibility.Collapsed;
                        InsertContestProgressRing.IsActive = false;
                        InsertContestTextBlock.Visibility = Visibility.Collapsed;

                        ContentDialog errorMsg = new ContentDialog
                        {
                            Title = "Error",
                            Content = "Error creating the contest, try again later.",
                            PrimaryButtonText = "OK"
                        };

                        App.ShowContentDialog(errorMsg, null);
                    }
                }
            }
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
            JudgesToAdd.Remove((JudgeMember)item);
            JudgesToRemove.Add((JudgeMember)item);
        }

        private void RemoveCriteriaButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which item was clicked.
            var item = ((FrameworkElement)sender).DataContext;

            // Remove judge from the list.
            Criterias.Remove((Criteria)item);
            CriteriasToAdd.Remove((Criteria)item);
            CriteriasToRemove.Add((Criteria)item);
        }

        private void RemoveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which item was clicked.
            var item = ((FrameworkElement)sender).DataContext;

            // Remove judge from the list.
            Projects.Remove((Project)item);
            ProjectsToAdd.Remove((Project)item);
            ProjectsToRemove.Add((Project)item);
        }

        private void EditProjectButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which item was clicked.
            var item = ((FrameworkElement)sender).DataContext;

            EditProjectContentDialog editProjectContentDialog = new EditProjectContentDialog(this, (Project)item);

            App.ShowContentDialog(editProjectContentDialog, null);
        }

        private void Dispatch_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType.ToString().Contains("Down"))
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
                {
                    switch (args.VirtualKey)
                    {
                        case VirtualKey.G:
                            FillContestDetailsShowcase();
                            break;
                    }
                }
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Fills a sample contest for showcase use.
        /// </summary>
        private void FillContestDetailsShowcase()
        {
            ContestNameTextBox.Text = "Cooking and Technology";

            EditDescription("This contest's main goal is to gather an idea for a project for the integration of new technologies in the food industry.");

            StartDateCalendarDatePicker.Date = new DateTime(2019, 2, 6);
            LimitDateCalendarDatePicker.Date = new DateTime(2019, 2, 12);
            VotingLimitDateCalendarDatePicker.Date = new DateTime(2019, 2, 20);

            AddJudge(new JudgeMember(4, "Tiago Fonseca"));
            AddJudge(new JudgeMember(3, "Nuno Miranda"));

            ObservableCollection<Promoter> promoters = new ObservableCollection<Promoter>() { new Promoter(-1, "João Rosário", new DateTime(1990, 1, 12)), new Promoter(-1, "André Neves", new DateTime(1989, 9, 15)) };
            ObservableCollection<Promoter> promoters2 = new ObservableCollection<Promoter>() { new Promoter(-1, "Maria Cunha", new DateTime(1975, 2, 20)), new Promoter(-1, "José Pinto", new DateTime(1976, 5, 21)) };
            ObservableCollection<Promoter> promoters3 = new ObservableCollection<Promoter>() { new Promoter(-1, "Sara Dias", new DateTime(1994, 3, 2)), new Promoter(-1, "Mafalda Oliveira", new DateTime(1994, 5, 2)) };

            AddProject(new Project(-1, "Fun cooking", "Cooking suited to the little ones.", new Category(1, "Social"), 2019) { Promoters = promoters });
            AddProject(new Project(-1, "SmartPot", "Helping people cook with fewer ingredients.", new Category(2, "Technology"), 2019) { Promoters = promoters2 });
            AddProject(new Project(-1, "Knifork", "Restaurant locator for mobile devices.", new Category(3, "Turism"), 2019) { Promoters = promoters3 });

            AddCriteria(new Criteria(2, "Cost", "How much the project development will cost."));
            AddCriteria(new Criteria(1, "Environmental Impact", "How much the project development affects the environment."));
            AddCriteria(new Criteria(3, "Innovation", "The change in the current paradigm that the project will bring."));
        }

        /// <summary>
        /// Set the correct bool variables indicating the contest status and loads the contest details.
        /// </summary>
        private async Task LoadContestDetailsAsync()
        {
            if (!Data.Instance.LoggedInUser.Administrator)
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
                else if (ContestToLoad.LimitDate < DateTime.Now.Date)
                {
                    ContestStarted = true;
                    ContestEnded = true;

                    DisableContestFields();
                }
                // Check if contest has alreadt started.
                else if (ContestToLoad.StartDate < DateTime.Now.Date)
                {
                    ContestStarted = true;

                    DisableContestFields();
                }
            }

            await LoadContestAsync();
        }

        /// <summary>
        /// Disables selected controls on the page according to the contest status.
        /// </summary>
        private void DisableContestFields()
        {
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
                CreateContestButton.IsEnabled = false;
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

        private async Task<bool> UpdateContestInDb()
        {
            try
            {
                string query = "UPDATE contest_table SET contest_name = @contest_name, descript = @descript, start_date = @start_date, limit_date = @limit_date, voting_limit_date = @voting_limit_date " +
                    "WHERE id_contest = @id_contest";

                SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
                cmd.CommandText = query;

                cmd.Parameters.Add(new SqlParameter("@id_contest", ContestToEdit.Id));

                cmd.Parameters.Add(new SqlParameter("@contest_name", ContestToEdit.ContestDetails.Name));

                cmd.Parameters.Add(new SqlParameter("@descript", ContestToEdit.ContestDetails.Description));

                cmd.Parameters.Add(new SqlParameter("@start_date", ContestToEdit.ContestDetails.StartDate));

                cmd.Parameters.Add(new SqlParameter("@limit_date", ContestToEdit.ContestDetails.LimitDate));

                cmd.Parameters.Add(new SqlParameter("@voting_limit_date", ContestToEdit.ContestDetails.VotingDate));

                // Execute query.
                await cmd.ExecuteNonQueryAsync();

                // Add all projects.
                foreach (Project project in ProjectsToAdd)
                {
                    query = "INSERT INTO project_table (id_contest, id_category, name, descript, project_year) " +
                        "VALUES (@id_contest, @id_category, @name, @descript, @project_year); " +
                        "SELECT CAST(scope_identity() as int)";

                    cmd = DBSqlHelper.Connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.Parameters.Add(new SqlParameter("@id_contest", ContestToEdit.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_category", project.Category.Id));

                    cmd.Parameters.Add(new SqlParameter("@name", project.Name));

                    cmd.Parameters.Add(new SqlParameter("@descript", project.Description));

                    cmd.Parameters.Add(new SqlParameter("@project_year", project.Year));

                    // Execute query.
                    int insertProjectId = (int)await cmd.ExecuteScalarAsync();

                    // Add all promoters.
                    foreach (Promoter promoter in project.Promoters)
                    {
                        query = "INSERT INTO promoter_table (id_project, name, date_of_birth) " +
                            "VALUES (@id_project, @name, @date_of_birth)";

                        cmd = DBSqlHelper.Connection.CreateCommand();
                        cmd.CommandText = query;

                        cmd.Parameters.Add(new SqlParameter("@id_project", insertProjectId));

                        cmd.Parameters.Add(new SqlParameter("@name", promoter.Name));

                        cmd.Parameters.Add(new SqlParameter("@date_of_birth", promoter.DateOfBirth));

                        // Execute query.
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // Remove removed projects.
                foreach(Project project in ProjectsToRemove)
                {
                    query = "DELETE FROM project_table WHERE id_project = @id_project";

                    cmd = DBSqlHelper.Connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.Parameters.Add(new SqlParameter("@id_project", project.Id));

                    await cmd.ExecuteNonQueryAsync();

                    foreach(Promoter promoter in project.Promoters)
                    {
                        query = "DELETE FROM promoter_table WHERE id_promoter = @id_promoter";

                        cmd = DBSqlHelper.Connection.CreateCommand();
                        cmd.CommandText = query;

                        cmd.Parameters.Add(new SqlParameter("@id_promoter", promoter.Id));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // Edit edited projects.
                foreach (Project project in ProjectsToEdit)
                {
                    query = "UPDATE project_table SET id_contest = @id_contest, id_category = @id_category, name = @name, descript = @descript, project_year = @project_year WHERE id_project = @id_project";

                    cmd = DBSqlHelper.Connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.Parameters.Add(new SqlParameter("@id_project", project.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_contest", ContestToEdit.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_category", project.Category.Id));

                    cmd.Parameters.Add(new SqlParameter("@name", project.Name));

                    cmd.Parameters.Add(new SqlParameter("@descript", project.Description));

                    cmd.Parameters.Add(new SqlParameter("@project_year", project.Year));

                    await cmd.ExecuteNonQueryAsync();

                    foreach (Promoter promoter in project.Promoters)
                    {
                        query = "DELETE FROM promoter_table WHERE id_promoter = @id_promoter";

                        cmd = DBSqlHelper.Connection.CreateCommand();
                        cmd.CommandText = query;

                        cmd.Parameters.Add(new SqlParameter("@id_promoter", promoter.Id));

                        await cmd.ExecuteNonQueryAsync();

                        query = "INSERT INTO promoter_table (id_project, name, date_of_birth) " +
                            "VALUES (@id_project, @name, @date_of_birth)";

                        cmd = DBSqlHelper.Connection.CreateCommand();
                        cmd.CommandText = query;

                        cmd.Parameters.Add(new SqlParameter("@id_project", project.Id));

                        cmd.Parameters.Add(new SqlParameter("@name", promoter.Name));

                        cmd.Parameters.Add(new SqlParameter("@date_of_birth", promoter.DateOfBirth));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // Add the remaining judges.
                foreach (JudgeMember judgeMember in JudgesToAdd)
                {
                    query = "INSERT INTO contest_juri_table (id_contest, id_user, has_voted, president) " +
                    "VALUES (@id_contest, @id_user, 0, 0)";

                    cmd = DBSqlHelper.Connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.Parameters.Add(new SqlParameter("@id_contest", ContestToEdit.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_user", judgeMember.Id));

                    await cmd.ExecuteNonQueryAsync();
                }

                // Remove the removed judges.
                foreach(JudgeMember judgeMember in JudgesToRemove)
                {
                    query = "DELETE FROM contest_juri_table WHERE id_contest = @id_contest AND id_user = @id_user";

                    cmd = DBSqlHelper.Connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.Parameters.Add(new SqlParameter("@id_contest", ContestToEdit.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_user", judgeMember.Id));

                    await cmd.ExecuteNonQueryAsync();
                }

                // Add all the criterias to the database.
                foreach (Criteria criteria in CriteriasToAdd)
                {
                    query = "INSERT INTO contest_criteria_table (id_criteria, id_contest) " +
                        "VALUES (@id_criteria, @id_contest)";

                    cmd = DBSqlHelper.Connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.Parameters.Add(new SqlParameter("@id_criteria", criteria.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_contest", ContestToEdit.Id));

                    await cmd.ExecuteNonQueryAsync();
                }

                // Remove removed criterias.
                foreach (Criteria criteria in CriteriasToRemove)
                {
                    query = "DELETE FROM contest_criteria_table " +
                        "WHERE id_criteria = @id_criteria AND id_contest = @id_contest";

                    cmd = DBSqlHelper.Connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.Parameters.Add(new SqlParameter("@id_criteria", criteria.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_contest", ContestToEdit.Id));

                    await cmd.ExecuteNonQueryAsync();
                }

                await Data.Instance.RefreshContestsAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> InsertContestToDB()
        {
            try
            {
                string query = "INSERT INTO contest_table (contest_name, descript, start_date, limit_date, voting_limit_date) " +
                    "VALUES (@contest_name, @descript, @start_date, @limit_date, @voting_limit_date); " +
                    "SELECT CAST(scope_identity() as int)";

                SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
                cmd.CommandText = query;

                cmd.Parameters.Add(new SqlParameter("@contest_name", ContestToEdit.ContestDetails.Name));

                cmd.Parameters.Add(new SqlParameter("@descript", ContestToEdit.ContestDetails.Description));

                cmd.Parameters.Add(new SqlParameter("@start_date", ContestToEdit.ContestDetails.StartDate));

                cmd.Parameters.Add(new SqlParameter("@limit_date", ContestToEdit.ContestDetails.LimitDate));

                cmd.Parameters.Add(new SqlParameter("@voting_limit_date", ContestToEdit.ContestDetails.VotingDate));

                // Execute query.
                int insertedId = (int)await cmd.ExecuteScalarAsync();

                // Add all projects.
                foreach(Project project in ProjectsToAdd)
                {
                    query = "INSERT INTO project_table (id_contest, id_category, name, descript, project_year) " +
                        "VALUES (@id_contest, @id_category, @name, @descript, @project_year); " +
                        "SELECT CAST(scope_identity() as int)";

                    cmd = DBSqlHelper.Connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.Parameters.Add(new SqlParameter("@id_contest", insertedId));

                    cmd.Parameters.Add(new SqlParameter("@id_category", project.Category.Id));

                    cmd.Parameters.Add(new SqlParameter("@name", project.Name));

                    cmd.Parameters.Add(new SqlParameter("@descript", project.Description));

                    cmd.Parameters.Add(new SqlParameter("@project_year", project.Year));

                    // Execute query.
                    int insertProjectId = (int)await cmd.ExecuteScalarAsync();

                    // Add all promoters.
                    foreach (Promoter promoter in project.Promoters)
                    {
                        query = "INSERT INTO promoter_table (id_project, name, date_of_birth) " +
                            "VALUES (@id_project, @name, @date_of_birth)";

                        cmd = DBSqlHelper.Connection.CreateCommand();
                        cmd.CommandText = query;

                        cmd.Parameters.Add(new SqlParameter("@id_project", insertProjectId));

                        cmd.Parameters.Add(new SqlParameter("@name", promoter.Name));

                        cmd.Parameters.Add(new SqlParameter("@date_of_birth", promoter.DateOfBirth));

                        // Execute query.
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // Add main judge to the database.
                query = "INSERT INTO contest_juri_table (id_contest, id_user, has_voted, president) " +
                    "VALUES (@id_contest, @id_user, 0, 1)";

                cmd = DBSqlHelper.Connection.CreateCommand();
                cmd.CommandText = query;

                cmd.Parameters.Add(new SqlParameter("@id_contest", insertedId));

                cmd.Parameters.Add(new SqlParameter("@id_user", Data.Instance.LoggedInUser.Id));

                await cmd.ExecuteNonQueryAsync();

                // Add the remaining judges.
                foreach (JudgeMember judgeMember in JudgesToAdd)
                {
                    query = "INSERT INTO contest_juri_table (id_contest, id_user, has_voted, president) " +
                    "VALUES (@id_contest, @id_user, 0, 0)";

                    cmd = DBSqlHelper.Connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.Parameters.Add(new SqlParameter("@id_contest", insertedId));

                    cmd.Parameters.Add(new SqlParameter("@id_user", judgeMember.Id));

                    await cmd.ExecuteNonQueryAsync();
                }

                // Add all the criterias to the database.
                foreach(Criteria criteria in CriteriasToAdd)
                {
                    query = "INSERT INTO contest_criteria_table (id_criteria, id_contest) " +
                        "VALUES (@id_criteria, @id_contest)";

                    cmd = DBSqlHelper.Connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.Parameters.Add(new SqlParameter("@id_criteria", criteria.Id));

                    cmd.Parameters.Add(new SqlParameter("@id_contest", insertedId));

                    await cmd.ExecuteNonQueryAsync();
                }

                await Data.Instance.RefreshContestsAsync();

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
