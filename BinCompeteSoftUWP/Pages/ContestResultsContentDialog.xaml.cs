using BinCompeteSoftUWP.Classes;
using DotNetMatrix;
using Net.Kniaz.AHP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
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
    public sealed partial class ContestResultsContentDialog : ContentDialog
    {
        #region Class variables
        private Contest contest;
        private ContestEvaluation contestEvaluation;
        private ObservableCollection<ProjectResult> contestResults = new ObservableCollection<ProjectResult>();
        private int criteriaCount = 0;
        private int projectCount = 0;
        #endregion

        #region Class constructors
        public ContestResultsContentDialog(Contest contest)
        {
            this.contest = contest;

            this.InitializeComponent();

            if (!CheckIfResultsAreCalculatedAndGetThem())
            {
                CalculateContestResults();

                InsertResultsIntoDB();
            }

            ResultsDataGrid.ItemsSource = contestResults;
        }
        #endregion

        #region Class methods
        /// <summary>
        /// This method checks in the database if the results have already been calculated,
        /// and if so, retrieves them.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfResultsAreCalculatedAndGetThem()
        {
            string query = "SELECT final_evaluation, id_project FROM final_result_table WHERE id_contest = @id_contest ORDER BY id_project ASC";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            cmd.Parameters.Add(new SqlParameter("@id_contest", contest.Id));

            List<double> resultList = new List<double>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                // Try to read from the database.
                while (reader.Read())
                {
                    int projectId = reader.GetInt32(1);

                    foreach (Project project in contest.Projects)
                    {
                        if (project.Id == projectId)
                        {
                            contestResults.Add(new ProjectResult {
                                Project = project,
                                Result = (double)reader.GetDecimal(0)
                            });

                            break;
                        }
                    }

                }

                contestResults = new ObservableCollection<ProjectResult>(contestResults.OrderByDescending(contest => contest.Result).ToList());

                // Check if there's anything returned.
                if (contestResults.Count <= 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// This method will calculate the contest results.
        /// </summary>
        private void CalculateContestResults()
        {
            GetContestVotes();
            
            double[][] criteriaValues;
            double[][] projectValues;
            
            List<double[][]> judgeProjectValues = new List<double[][]>();
            List<GeneralMatrix> projectResults = new List<GeneralMatrix>();

            // Cycle through all judge evaluations.
            foreach (JudgeEvaluation judgeEvaluation in contestEvaluation.JudgeEvaluations)
            {
                criteriaValues = new double[criteriaCount][];

                // Cycle through all criteria evaluations.
                int k = 0;
                for(int i = 0; i < criteriaCount; i++)
                {
                    criteriaValues[i] = new double[criteriaCount];
                    for(int j = i; j < criteriaCount; j++)
                    {
                        if (i != j)
                        {
                            double evaluation = judgeEvaluation.CriteriaEvaluations[k].Value;

                            if (evaluation > 0)
                            {
                                criteriaValues[i][j] = evaluation;
                            }
                            else
                            {
                                criteriaValues[i][j] = (1 / Math.Abs(evaluation));
                            }

                            k++;
                        }
                        else
                        {
                            criteriaValues[i][j] = 1;
                        }
                    }
                }

                int l = 0;

                judgeProjectValues = new List<double[][]>();

                // Cycle through all project evaluations.
                for (int i = 0; i < criteriaCount; i++)
                {
                    projectValues = new double[projectCount][];

                    for (int j = 0; j < projectCount; j++)
                    {
                        projectValues[j] = new double[projectCount];
                        for(k = j; k < projectCount; k++)
                        {
                            if(j != k)
                            {
                                double evaluation = judgeEvaluation.ProjectEvaluations[l].Value;

                                if(evaluation > 0)
                                {
                                    projectValues[j][k] = evaluation;
                                }
                                else
                                {
                                    projectValues[j][k] = (1 / Math.Abs(evaluation));
                                }

                                l++;
                            }
                            else
                            {
                                projectValues[j][k] = 1;
                            }
                        }
                    }

                    judgeProjectValues.Add(projectValues);
                }

                AHPModel model;

                model = new AHPModel(criteriaCount, projectCount);

                model.AddCriteria(criteriaValues);

                for (int i = 0; i < judgeProjectValues.Count; i++)
                {
                    model.AddCriterionRatedChoices(i, judgeProjectValues[i]);
                }

                model.CalculateModel();

                GeneralMatrix results = model.CalculatedChoices;

                projectResults.Add(results);
            }

            foreach(Project project in contest.Projects)
            {
                contestResults.Add(new ProjectResult
                {
                    Project = project,
                    Result = 0
                });
            }

            contestResults = new ObservableCollection<ProjectResult>(contestResults.OrderBy(contestResult => contestResult.Project.Id).ToList());

            for(int i = 0; i < projectResults.Count; i++)
            {
                for(int j = 0; j < contestResults.Count; j++)
                {
                    contestResults[j].Result += projectResults[i].Array[j][0];
                }
            }

            // Normalize the scores so they all sum together to 1.
            foreach(ProjectResult projectResult in contestResults)
            {
                projectResult.Result /= (double)projectResults.Count;
            }

            // Order the projects by their results.
            contestResults = new ObservableCollection<ProjectResult>(contestResults.OrderByDescending(contestResult => contestResult.Result).ToList());
        }

        /// <summary>
        /// This method retrieves all the judge votes from the database.
        /// </summary>
        private void GetContestVotes()
        {
            List<JudgeEvaluation> judgeEvaluations = new List<JudgeEvaluation>();

            string query = "SELECT id_user, id_criteria1, id_criteria2, value FROM contest_juri_criteria_table " +
                "WHERE id_contest = @id_contest " +
                "ORDER BY id_criteria1 ASC, id_criteria2 ASC";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            cmd.Parameters.Add(new SqlParameter("@id_contest", contest.Id));

            using(SqlDataReader reader = cmd.ExecuteReader())
            {
                int userId, userIndex;

                while (reader.Read())
                {
                    userIndex = -1;

                    userId = reader.GetInt32(0);

                    userIndex = judgeEvaluations.FindIndex(user => user.JudgeId == userId);

                    if(userIndex >= 0)
                    {
                        judgeEvaluations[userIndex].CriteriaEvaluations.Add(new CriteriaEvaluation
                        {
                            Criteria1 = new Criteria(reader.GetInt32(1), "", ""),
                            Criteria2 = new Criteria(reader.GetInt32(2), "", ""),
                            Value = reader.GetInt32(3)
                        });
                    }
                    else
                    {
                        judgeEvaluations.Add(new JudgeEvaluation
                        {
                            JudgeId = userId,
                            CriteriaEvaluations = new List<CriteriaEvaluation>()
                            {
                                new CriteriaEvaluation
                                {
                                    Criteria1 = new Criteria(reader.GetInt32(1), "", ""),
                                    Criteria2 = new Criteria(reader.GetInt32(2), "", ""),
                                    Value = reader.GetInt32(3)
                                }
                            }
                        });
                    }
                }
            }

            query = "SELECT id_user, id_project1, id_project2, id_criteria, value FROM evaluation_table " +
                "WHERE id_contest = @id_contest " +
                "ORDER BY id_criteria ASC, id_project1 ASC, id_project2 ASC";

            cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            cmd.Parameters.Add(new SqlParameter("@id_contest", contest.Id));

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                int userId, userIndex;

                while (reader.Read())
                {
                    userIndex = -1;

                    userId = reader.GetInt32(0);

                    userIndex = judgeEvaluations.FindIndex(user => user.JudgeId == userId);

                    if (userIndex >= 0)
                    {
                        judgeEvaluations[userIndex].ProjectEvaluations.Add(new ProjectEvaluation
                        {
                            Criteria = new Criteria(reader.GetInt32(3), "", ""),
                            Project1 = new Project(reader.GetInt32(1), "", "", new Category(), -1),
                            Project2 = new Project(reader.GetInt32(2), "", "", new Category(), -1),
                            Value = reader.GetInt32(4)
                        });
                    }
                    else
                    {
                        judgeEvaluations.Add(new JudgeEvaluation
                        {
                            JudgeId = userId,
                            ProjectEvaluations = new List<ProjectEvaluation>()
                            {
                                new ProjectEvaluation
                                {
                                    Criteria = new Criteria(reader.GetInt32(3), "", ""),
                                    Project1 = new Project(reader.GetInt32(1), "", "", new Category(), -1),
                                    Project2 = new Project(reader.GetInt32(2), "", "", new Category(), -1),
                                    Value = reader.GetInt32(4)
                                }
                            }
                        });
                    }
                }
            }

            // Now let's add all the judge evaluations to the contest evaluation.
            contestEvaluation = new ContestEvaluation
            {
                JudgeEvaluations = judgeEvaluations
            };

            // Get number of criterias from database.
            query = "SELECT COUNT(*) FROM contest_criteria_table WHERE id_contest = @id_contest";

            cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            cmd.Parameters.Add(new SqlParameter("@id_contest", contest.Id));

            using(SqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();

                criteriaCount = reader.GetInt32(0);
            }

            // Get number of projects from database.
            query = "SELECT COUNT(*) FROM project_table WHERE id_contest = @id_contest";

            cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            cmd.Parameters.Add(new SqlParameter("@id_contest", contest.Id));

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();

                projectCount = reader.GetInt32(0);
            }
        }

        /// <summary>
        /// This method inserts into the database the final project evaluations.
        /// </summary>
        private void InsertResultsIntoDB()
        {
            string query = "INSERT INTO final_result_table VALUES (@id_contest, @id_project, @final_evaluation)";

            SqlCommand cmd;

            // Cycle through all projects and add their final evaluation to the database.
            foreach (ProjectResult projectResult in contestResults)
            {
                cmd = DBSqlHelper.Connection.CreateCommand();
                cmd.CommandText = query;
                
                cmd.Parameters.Add(new SqlParameter("@id_contest", contest.Id));

                cmd.Parameters.Add(new SqlParameter("@id_project", projectResult.Project.Id));

                cmd.Parameters.Add(new SqlParameter("@final_evaluation", projectResult.Result));

                // Execute the query.
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// This method will write the results to and XML file.
        /// </summary>
        private async void WriteResultsToXML()
        {
            int count = 1;

            // Strip contest name of dots.
            string fileName = contest.ContestDetails.Name.Replace(".", String.Empty);

            // Ask the user where they want to save the file.
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            // Dropdown of which file types the user can save as.
            savePicker.FileTypeChoices.Add("XML File", new List<string>() { ".xml" });

            // Default file name if the user does not type one in or selecte a file to replace.
            savePicker.SuggestedFileName = fileName;

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            if(file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes to it.
                CachedFileManager.DeferUpdates(file);

                StringBuilder builder = new StringBuilder();
                using (StringWriter stringWriter = new StringWriter(builder))
                {
                    // Write to the file.
                    using (XmlWriter writer = XmlWriter.Create(stringWriter))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Results");

                        // Write the contest details.
                        writer.WriteStartElement("Contest");
                        writer.WriteElementString("Id", contest.Id.ToString());
                        writer.WriteElementString("Name", contest.ContestDetails.Name);
                        writer.WriteElementString("StartDate", contest.ContestDetails.StartDate.ToString());
                        writer.WriteElementString("LimitDate", contest.ContestDetails.LimitDate.ToString());
                        writer.WriteElementString("VotingDate", contest.ContestDetails.VotingDate.ToString());
                        writer.WriteEndElement();

                        // Write the criterias details.
                        writer.WriteStartElement("Criterias");

                        foreach (Criteria criteria in contest.Criterias)
                        {
                            writer.WriteStartElement("Criteria");
                            writer.WriteElementString("Id", criteria.Id.ToString());
                            writer.WriteElementString("Name", criteria.Name);
                            writer.WriteElementString("Description", criteria.Description);
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();

                        // Write the projects details.
                        writer.WriteStartElement("Projects");

                        foreach (ProjectResult projectResult in contestResults)
                        {
                            writer.WriteStartElement("Project");

                            writer.WriteElementString("Id", projectResult.Project.Id.ToString());
                            writer.WriteElementString("Name", projectResult.Project.Name);
                            writer.WriteElementString("Description", projectResult.Project.Description);
                            writer.WriteElementString("Score", projectResult.Result.ToString());
                            writer.WriteElementString("Position", count.ToString());

                            writer.WriteEndElement();

                            count++;
                        }

                        writer.WriteEndElement();

                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                }

                string stream = builder.ToString();

                // Save file.
                await FileIO.WriteTextAsync(file, stream);

                // Let Windows know that we're finished changing the file.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                
                if(status == FileUpdateStatus.Complete)
                {
                    ContentDialog contentDialog = new ContentDialog
                    {
                        Title = "Success",
                        Content = "Results successfully exported.",
                        CloseButtonText = "OK"
                    };

                    App.ShowContentDialog(contentDialog, null);
                }
                else
                {
                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Unable to export results.",
                        CloseButtonText = "OK"
                    };

                    App.ShowContentDialog(errorDialog, null);
                }
            }
            else
            {
                ContentDialog errorMsg = new ContentDialog
                {
                    Title = "Canceled",
                    Content = "Action canceled by the user.",
                    CloseButtonText = "OK"
                };

                App.ShowContentDialog(errorMsg, null);
            }
        }
        #endregion

        #region Class event handlers
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            WriteResultsToXML();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
        }
        #endregion
    }

    class ProjectResult
    {
        public Project Project { get; set; }
        public double Result { get; set; }
    }
}
