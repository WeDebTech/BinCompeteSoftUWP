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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BinCompeteSoftUWP.Pages
{
    public sealed partial class EditContestDescriptionContentDialog : ContentDialog
    {
        #region Class variables
        private string description;
        private ContestPage contestPage;
        #endregion

        #region Class constructors
        public EditContestDescriptionContentDialog(ContestPage contestPage, string description)
        {
            this.contestPage = contestPage;
            this.description = description;

            this.InitializeComponent();

            ProjectDescriptionTextBox.Text = this.description;
        }
        #endregion

        #region Class event handlers
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if the description is filled out.
            if (!string.IsNullOrWhiteSpace(ProjectDescriptionTextBox.Text))
            {
                ProjectDescriptionErrorText.Visibility = Visibility.Collapsed;

                description = ProjectDescriptionTextBox.Text;

                contestPage.EditDescription(description);
            }
            else
            {
                ProjectDescriptionErrorText.Visibility = Visibility.Visible;

                args.Cancel = true;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        #endregion
    }
}
