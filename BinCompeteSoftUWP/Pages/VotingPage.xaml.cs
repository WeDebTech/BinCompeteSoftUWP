using BinCompeteSoftUWP.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
