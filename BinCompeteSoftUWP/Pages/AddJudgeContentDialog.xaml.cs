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
    public sealed partial class AddJudgeContentDialog : ContentDialog
    {
        #region Class variables
        private ObservableCollection<JudgeMember> judgeMembers = new ObservableCollection<JudgeMember>();
        private ContestPage contestPage;
        #endregion

        #region Class constructors
        public AddJudgeContentDialog(ContestPage contestPage)
        {
            this.contestPage = contestPage;

            this.InitializeComponent();

            LoadJudgeMembers();
        }
        #endregion

        #region Class methods
        /// <summary>
        /// This method will create a list of judges that can be added.
        /// </summary>
        private void LoadJudgeMembers()
        {
            judgeMembers = Data.Instance.JudgeMembers;

            ObservableCollection<JudgeMember> judgeMembersAdded = contestPage.JudgesToAdd;

            // Remove judges that are added from the judges that can be added.
            judgeMembers = new ObservableCollection<JudgeMember>(judgeMembers.Where(judge => !judgeMembersAdded.Any(judgeToRemove => judgeToRemove.Id == judge.Id)).ToList());

            JudgesComboBox.ItemsSource = judgeMembers;
        }
        #endregion

        #region Class method handlers
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if anything was selected.
            if(JudgesComboBox.SelectedItem != null)
            {
                JudgeMember judgeMember = (JudgeMember)JudgesComboBox.SelectedItem;

                contestPage.AddJudge(judgeMember);
            }
            else
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        #endregion
    }
}
