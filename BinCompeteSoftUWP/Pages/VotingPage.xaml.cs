using BinCompeteSoftUWP.Classes;
using System;
using System.Collections.Generic;
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

        #endregion

        #region Class constructors
        public VotingPage()
        {
            this.InitializeComponent();

            RefreshCriterias();
        }
        #endregion

        #region Class methods
        private async void RefreshCriterias()
        {
            await Data.Instance.RefreshCriteriasAsync();

            LoadCriteriasIntoComboBox();
        }

        private void LoadCriteriasIntoComboBox()
        {
            Criteria1ComboBox.ItemsSource = Data.Instance.Criterias;
            Criteria2ComboBox.ItemsSource = Data.Instance.Criterias;

            Criteria1ComboBox.SelectedItem = Data.Instance.Criterias[0];
            Criteria2ComboBox.SelectedItem = Data.Instance.Criterias[1];
        }
        #endregion

        #region Class event handlers
        private void CriteriaImportanceSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

        }

        private void Criteria1ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if both ComboBoxes have the same item selected.
            if (e.AddedItems.FirstOrDefault() == Criteria2ComboBox.SelectedItem)
            {
                // Select back the previous selection.
                if (e.RemovedItems.FirstOrDefault() == null)
                {
                    Criteria1ComboBox.SelectedItem = null;
                }
                else
                {
                    Criteria1ComboBox.SelectedItem = e.RemovedItems[0];
                }
            }
            else
            {
                Criteria1DescriptionTextBlock.Text = ((Criteria)e.AddedItems[0]).Description;
            }
        }

        private void Criteria2ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if both ComboBoxes have the same item selected.
            if (e.AddedItems.FirstOrDefault() == Criteria1ComboBox.SelectedItem)
            {
                // Select back the previous selection.
                if (e.RemovedItems.FirstOrDefault() == null)
                {
                    Criteria2ComboBox.SelectedItem = null;
                }
                else
                {
                    Criteria2ComboBox.SelectedItem = e.RemovedItems[0];
                }
            }
            else
            {
                Criteria2DescriptionTextBlock.Text = ((Criteria)e.AddedItems[0]).Description;
            }
        }
        #endregion
    }
}
