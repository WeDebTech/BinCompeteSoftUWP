using BinCompeteSoftUWP.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BinCompeteSoftUWP.Pages
{
    public sealed partial class AddCriteriaContentDialog : ContentDialog
    {
        #region Class variables
        private ObservableCollection<Criteria> criterias = new ObservableCollection<Criteria>();
        private ContestPage contestPage;
        #endregion

        #region Class constructors
        public AddCriteriaContentDialog(ContestPage contestPage)
        {
            this.contestPage = contestPage;

            this.InitializeComponent();

            LoadCriterias();
        }
        #endregion

        #region Class methods
        /// <summary>
        /// This method will create a list of judges that can be added.
        /// </summary>
        private void LoadCriterias()
        {
            criterias = Data.Instance.Criterias;

            ObservableCollection<Criteria> criteriasAdded = contestPage.Criterias;

            // Remove judges that are added from the judges that can be added.
            criterias = new ObservableCollection<Criteria>(criterias.Where(criteria => !criteriasAdded.Any(criteriaToRemove => criteriaToRemove.Id == criteria.Id)).ToList());

            CriteriasComboBox.ItemsSource = criterias;
        }
        #endregion

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if anything was selected.
            if (CriteriasComboBox.SelectedItem != null)
            {
                Criteria criteria = (Criteria)CriteriasComboBox.SelectedItem;

                contestPage.AddCriteria(criteria);
            }
            else
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void CriteriasComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CriteriaDescriptionTextBlock.Text = ((Criteria)e.AddedItems[0]).Description;
        }
    }
}
