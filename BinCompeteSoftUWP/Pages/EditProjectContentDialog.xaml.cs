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
    public sealed partial class EditProjectContentDialog : ContentDialog
    {
        #region Class variables
        private ContestPage contestPage;
        private Project project;
        private bool editingProject;
        private ObservableCollection<Promoter> promoters = new ObservableCollection<Promoter>();
        #endregion

        #region Class constructors
        public EditProjectContentDialog(ContestPage contestPage, Project project)
        {
            if (project == null)
            {
                editingProject = false;
                this.project = new Project();
            }
            else
            {
                editingProject = true;
                this.project = project;
            }

            this.contestPage = contestPage;

            this.InitializeComponent();

            PromotersListView.ItemsSource = promoters;
        }
        #endregion

        #region Class methods
        private void FillProjectDetails()
        {
            ProjectCategoryComboBox.SelectedItem = project.Category;

            ProjectNameTextBox.Text = project.Name;
            ProjectDescriptionTextBox.Text = project.Description;

            promoters = project.Promoters;
        }
        #endregion

        #region Class event handlers
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if everything has been filled.
            if(!string.IsNullOrWhiteSpace(ProjectNameTextBox.Text) && ProjectCategoryComboBox.SelectedItem != null && !string.IsNullOrWhiteSpace(ProjectDescriptionTextBox.Text) && promoters.Count > 0)
            {
                ProjectErrorTextBlock.Visibility = Visibility.Collapsed;

                project.Name = ProjectNameTextBox.Text;
                project.Description = ProjectDescriptionTextBox.Text;

                project.Promoters = promoters;

                contestPage.AddProject(project);
            }
            else
            {
                ProjectErrorTextBlock.Visibility = Visibility.Visible;
                args.Cancel = true;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ProjectCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            project.Category = (Category)ProjectCategoryComboBox.SelectedItem;
        }

        private void RemovePromoterButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which item was clicked.
            var item = ((FrameworkElement)sender).DataContext;

            // Remove judge from the list.
            promoters.Remove((Promoter)item);
        }

        private void AddPromoterButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if everything has been filled.
            // DateOfBirth must be before today's date.
            if(!string.IsNullOrWhiteSpace(PromoterNameTextBox.Text) && (PromoterDateOfBirthCalendarDatePicker.Date != null) && PromoterDateOfBirthCalendarDatePicker.Date < DateTime.Now.Date)
            {
                PromoterErrorTextBlock.Visibility = Visibility.Visible;

                DateTimeOffset tempDate = (DateTimeOffset)PromoterDateOfBirthCalendarDatePicker.Date;
                Promoter promoter = new Promoter(-1, PromoterNameTextBox.Text, tempDate.DateTime.Date);

                promoters.Add(promoter);

                // Clear the promoter fields.
                PromoterNameTextBox.Text = "";
                PromoterDateOfBirthCalendarDatePicker.Date = null;
            }
            else
            {
                PromoterErrorTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private async void ContentDialog_Loading(FrameworkElement sender, object args)
        {
            if (Data.Instance.Categories.Count <= 0)
            {
                await Data.Instance.RefreshCategoriesAsync();
            }

            ProjectCategoryComboBox.ItemsSource = Data.Instance.Categories;

            if (editingProject)
            {
                FillProjectDetails();
            }
        }
        #endregion
    }
}
