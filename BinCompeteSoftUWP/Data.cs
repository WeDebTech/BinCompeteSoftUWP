using BinCompeteSoft;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BinCompeteSoftUWP
{
    /// <summary>
    /// This class holds all the necessary data for the program to work.
    /// </summary>
    public static class Data
    {
        #region Class variables
        public static User LoggedInUser { get; set; }

        /*public Form currentForm { get; set; }
        public Form loginform { get; set; }*/

        public static List<JudgeMember> JudgeMembers { get; set; } = new List<JudgeMember>();
        public static List<Contest> Contests { get; set; } = new List<Contest>();
        public static List<ContestDetails> ContestDetails { get; set; } = new List<ContestDetails>();
        public static List<Project> Projects { get; set; } = new List<Project>();
        public static List<Category> Categories { get; set; } = new List<Category>();
        public static List<Statistic> Statistics { get; set; } = new List<Statistic>();
        public static List<Criteria> Criterias { get; set; } = new List<Criteria>();
        #endregion

        #region Class methods
        /// <summary>
        /// Gets the user from the database with the inserted data.
        /// </summary>
        /// <param name="username">The User username.</param>
        /// <param name="password">The User password.</param>
        /// <returns>The User from the database.</returns>
        public static User GetUserDataFromDB(string username, string password)
        {
            SqlCommand cmd;
            SqlParameter sqlUsername;
            SqlParameter sqlPassword;

            try
            {
                // Hash the input password.
                string hashedPassword = DBSqlHelper.SHA512(password);

                // Select user if username and password are correct OR first_time_login is set.
                string query = "SELECT id_user, fullname, email, username, administrator, first_time_login, valid FROM user_table " +
                    "WHERE (username = @username OR email = @username) AND (pw = @password OR first_time_login = 1)";

                cmd = DBSqlHelper.Connection.CreateCommand();
                cmd.CommandText = query;

                sqlUsername = new SqlParameter("@username", SqlDbType.NVarChar);
                sqlUsername.Value = username;
                cmd.Parameters.Add(sqlUsername);

                sqlPassword = new SqlParameter("@password", SqlDbType.NVarChar);
                sqlPassword.Value = hashedPassword;
                cmd.Parameters.Add(sqlPassword);

                // Execute query.
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    // Check if user exists.
                    if (reader.HasRows)
                    {
                        // Construct user information from database.
                        reader.Read();

                        User loggedUser = new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetBoolean(4), reader.GetBoolean(5), reader.GetBoolean(6));

                        return loggedUser;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Error reading user data from database.\nError: " + ex.Message,
                    CloseButtonText = "Ok"
                };

                App.ShowContentDialog(errorDialog, null);

                return null;
            }
        }

        /// <summary>
        /// This method retrieves the most up-to-date list of judges from the database.
        /// </summary>
        /// <returns>True if success, false otherwise.</returns>
        public static bool RefreshJudges()
        {
            // Load the judges from the Database
            string query = "SELECT id_user, fullname, email FROM user_table WHERE valid = 1";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            // Execute query
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if user exists
                if (reader.HasRows)
                {
                    JudgeMembers.Clear();

                    while (reader.Read())
                    {
                        // Construct user information from database
                        JudgeMember judge = new JudgeMember(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));

                        // Check if judge is not the current user
                        if (judge.Id != LoggedInUser.Id)
                        {
                            // Add it to the list
                            JudgeMembers.Add(judge);
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// This method retrieves the most up-to-date list of categories from the database.
        /// </summary>
        /// <returns>True if success, false otherwise.</returns>
        public static bool RefreshCategories()
        {
            // Load the categories from the Database
            string query = "SELECT id_category, category_name FROM project_category";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            // Execute query
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if category exists
                if (reader.HasRows)
                {
                    Categories.Clear();

                    while (reader.Read())
                    {
                        Category category = new Category(reader.GetInt32(0), reader.GetString(1));
                        Categories.Add(category);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// This method retrieves the most up-to-date list of contests from the database.
        /// </summary>
        /// <returns>True if success, false otherwise.</returns>
        public static bool RefreshContests()
        {
            // Load the contest that the users has part in from the Database
            string query = "SELECT id_contest, contest_name, descript, start_date, limit_date, voting_limit_date " +
                "FROM contest_table " +
                "WHERE id_contest IN( " +
                "SELECT id_contest FROM contest_juri_table contest " +
                "WHERE id_user = @id_user)";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            SqlParameter sqlUserId = new SqlParameter("id_user", SqlDbType.Int);
            sqlUserId.Value = Data.LoggedInUser.Id;
            cmd.Parameters.Add(sqlUserId);

            // Execute query
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if contest exists
                if (reader.HasRows)
                {
                    ContestDetails.Clear();

                    while (reader.Read())
                    {
                        ContestDetails contest = new ContestDetails(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4), reader.GetDateTime(5), false, false, false);
                        ContestDetails.Add(contest);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// This method retrieves if the given contest has has it's results calculated already.
        /// </summary>
        /// <param name="contestId">The contest id to check.</param>
        /// <returns>True if results have been calculated, false otherwise.</returns>
        public static bool GetContestResultsCalculatedStatus(int contestId)
        {
            string query = "SELECT id_contest FROM final_result_table WHERE id_contest = @id_contest";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            SqlParameter sqlContestId = new SqlParameter("@id_contest", SqlDbType.Int);
            sqlContestId.Value = contestId;
            cmd.Parameters.Add(sqlContestId);

            // Execute query.
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {

                    // If it reads data it means that it has been calculated already.
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method retrieves if the given contest has been voted for already by the current user.
        /// </summary>
        /// <param name="contestId">The contest id to check.</param>
        /// <returns>True if is voted, false otherwise.</returns>
        public static bool GetContestVoteStatus(int contestId)
        {
            string query = "SELECT has_voted FROM contest_juri_table WHERE id_contest = @id_contest AND id_user = @id_user";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            SqlParameter sqlContestId = new SqlParameter("@id_contest", SqlDbType.Int);
            sqlContestId.Value = contestId;
            cmd.Parameters.Add(sqlContestId);

            SqlParameter sqlUserId = new SqlParameter("@id_user", SqlDbType.Int);
            sqlUserId.Value = LoggedInUser.Id;
            cmd.Parameters.Add(sqlUserId);

            // Execute query.
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if user exists in contest.
                if (reader.HasRows)
                {
                    reader.Read();

                    bool isVotedByCurrentUser = reader.GetBoolean(0);

                    return isVotedByCurrentUser;
                }
            }

            return false;
        }

        /// <summary>
        /// This method retrieves if the given contest has been created by the current user.
        /// </summary>
        /// <param name="contestId">The contest id to check.</param>
        /// <returns>True if is created by current user, false otherwise.</returns>
        public static bool GetIfContestIsCreatedByCurrentUser(int contestId)
        {
            string query = "SELECT president FROM contest_juri_table WHERE id_contest = @id_contest AND id_user = @id_user";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            SqlParameter sqlContestId = new SqlParameter("@id_contest", SqlDbType.Int);
            sqlContestId.Value = contestId;
            cmd.Parameters.Add(sqlContestId);

            SqlParameter sqlUserId = new SqlParameter("@id_user", SqlDbType.Int);
            sqlUserId.Value = LoggedInUser.Id;
            cmd.Parameters.Add(sqlUserId);

            // Execute query.
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if user exists in contest.
                if (reader.HasRows)
                {
                    reader.Read();

                    bool isContestCreatedByCurrentUser = reader.GetBoolean(0);

                    return isContestCreatedByCurrentUser;
                }
            }

            return false;
        }

        /// <summary>
        /// This method retrieves the most up-to-date list of criterias from the database.
        /// </summary>
        /// <returns>True if success, false otherwise.</returns>
        public static bool RefreshCriterias()
        {
            // Load the criterias from the Database
            string query = "SELECT id_criteria, criteria_name, descript FROM criteria_data_table";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            // Execute query
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if criterias exists
                if (reader.HasRows)
                {
                    Criterias.Clear();

                    while (reader.Read())
                    {
                        Criteria criteria = new Criteria(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));

                        Criterias.Add(criteria);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the contest with all details filled out from the database.
        /// </summary>
        /// <param name="id">The contest id to retrieve.</param>
        /// <returns>The contest object if it exists, null otherwise.</returns>
        public static Contest GetContest(int id)
        {
            // Varaiables declaration.
            Contest contest;

            SqlCommand cmd;

            SqlParameter sqlContestId, sqlUserId;

            string query;

            List<Project> contestProjects = new List<Project>();
            List<JudgeMember> contestJudges = new List<JudgeMember>();
            List<Criteria> contestCriterias = new List<Criteria>();

            // Get the contest details from the database.
            query = "SELECT * " +
                "FROM contest_table " +
                "WHERE id_contest = @id_contest";

            cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            sqlUserId = new SqlParameter("id_user", SqlDbType.Int);
            sqlUserId.Value = LoggedInUser.Id;
            cmd.Parameters.Add(sqlUserId);

            sqlContestId = new SqlParameter("id_contest", SqlDbType.Int);
            sqlContestId.Value = id;
            cmd.Parameters.Add(sqlContestId);

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if the contest exists.
                if (reader.HasRows)
                {
                    reader.Read();

                    ContestDetails contestDetails = new ContestDetails(id, reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4), reader.GetDateTime(5), false, false, false);

                    // Get the criteria values and convert them from a JSON string to a double matrix.
                    string criteriaValues = reader.GetString(6);

                    contest = new Contest(id, contestDetails, new List<Project>(), new List<JudgeMember>(), new List<Criteria>(), new double[0, 0]);

                    contest.SetCriteriaValuesFromJSON(criteriaValues);
                }
                else
                {
                    return null;
                }
            }

            // Get the contest project list.
            query = "SELECT id_project, project_name, descript, promoter_name, promoter_age, id_category, project_year FROM project_table WHERE id_contest = @id_contest ORDER BY id_project";

            cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            sqlContestId = new SqlParameter("id_contest", SqlDbType.Int);
            sqlContestId.Value = id;
            cmd.Parameters.Add(sqlContestId);

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if there's any projects in the contest.
                if (reader.HasRows)
                {
                    // Read every project, and store it in a list.
                    while (reader.Read())
                    {
                        Project project = new Project(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), new Category(reader.GetInt32(5), ""), reader.GetInt32(6));

                        contestProjects.Add(project);
                    }
                }
            }

            // Cycle through all projects and get the full category.
            foreach (Project project in contestProjects)
            {
                foreach (Category category in Data.Categories)
                {
                    // Check if the project category id is the same as the category id.
                    if (project.Category.Id == category.Id)
                    {
                        // Assign the category to the project category.
                        project.Category = category;
                    }
                }
            }

            contest.Projects = contestProjects;

            // Get the contest judge list.
            query = "SELECT id_user, president FROM contest_juri_table WHERE id_contest = @id_contest ORDER BY id_user";

            cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            sqlContestId = new SqlParameter("id_contest", SqlDbType.Int);
            sqlContestId.Value = id;
            cmd.Parameters.Add(sqlContestId);

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if there's any judges in the contest.
                if (reader.HasRows)
                {
                    // Read every judge, and store it in a list.
                    while (reader.Read())
                    {
                        // Check if it's not the president.
                        if (!reader.GetBoolean(1))
                        {
                            JudgeMember judge = new JudgeMember(reader.GetInt32(0), "", "");

                            contestJudges.Add(judge);
                        }
                    }
                }
            }

            // Cycle through all judges and get the full details.
            foreach (JudgeMember contestJudge in contestJudges)
            {
                foreach (JudgeMember judge in Data.JudgeMembers)
                {
                    // Check if the contest judge id is the same as the stored judge id.
                    if (contestJudge.Id == judge.Id)
                    {
                        // Assign the judge details to the contest judge.
                        contestJudge.Name = judge.Name;
                        contestJudge.Email = judge.Email;
                    }
                }
            }

            contest.JudgeMembers = contestJudges;

            // Get the contest criteria list.
            query = "SELECT id_criteria FROM contest_criteria_table WHERE id_contest = @id_contest ORDER BY id_criteria";

            cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            sqlContestId = new SqlParameter("id_contest", SqlDbType.Int);
            sqlContestId.Value = id;
            cmd.Parameters.Add(sqlContestId);

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if there's any criterias in the contest.
                if (reader.HasRows)
                {
                    // Read every criteria, and store it in a list.
                    while (reader.Read())
                    {
                        Criteria criteria = new Criteria(reader.GetInt32(0), "", "");

                        contestCriterias.Add(criteria);
                    }
                }
            }

            // Refresh the criteria list.
            Data.RefreshCriterias();

            // Cycle through all criteria and get the full details.
            foreach (Criteria contestCriteria in contestCriterias)
            {
                foreach (Criteria criteria in Data.Criterias)
                {
                    // Check if the criteria judge id is the same as the stored judge id.
                    if (contestCriteria.Id == criteria.Id)
                    {
                        // Assign the criteria details to the contest criteria.
                        contestCriteria.Name = criteria.Name;
                        contestCriteria.Description = criteria.Description;
                    }
                }
            }

            contest.Criterias = contestCriterias;

            return contest;
        }

        /// <summary>
        /// This method retrieves the most up-to-date list of statistics from the database.
        /// </summary>
        /// <returns>True if success, false otherwise.</returns>
        public static bool RefreshStatistics()
        {
            // Load the general statistics from the Database.
            string query = "SELECT * FROM general_statistics";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if statistics exist.
                if (reader.HasRows)
                {
                    Statistics.Clear();

                    while (reader.Read())
                    {
                        Statistic statistic = new Statistic(reader.GetInt32(0), (double)reader.GetDecimal(1), reader.GetInt32(2), reader.GetInt32(3), new List<CategoryStatistics>(), new List<BestProjects>());
                        Statistics.Add(statistic);
                    }
                }
                else
                {
                    return false;
                }
            }

            // Load the category statistics from the Database.
            query = "SELECT * FROM project_category_stats";

            cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            // Execute query.
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if statistics exists.
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // Get the year of the current statistic.
                        int year = reader.GetInt32(0);

                        // Get the category id.
                        int categoryId = reader.GetInt32(1);

                        // Get the corresponding category from the category list.
                        Category category = new Category();

                        foreach (Category tempCategory in Data.Categories)
                        {
                            // Check if it's the category we want.
                            if (tempCategory.Id == categoryId)
                            {
                                category = tempCategory;
                                break;
                            }
                        }

                        // Create the category statistic from all gathered data.
                        CategoryStatistics categoryStatistics = new CategoryStatistics(category, reader.GetInt32(2));

                        // Cycle through all statistics until we get to the appropriate year
                        // If no appropriate one is found, ignore it, although it shouldn't happen on the database side.
                        foreach (Statistic statistic in Statistics)
                        {
                            if (statistic.Year == year)
                            {
                                statistic.CategoryStatistics.Add(categoryStatistics);
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// This method show a messagebox asking the user if they really wanna logout, and if yes,
        /// it will clear all variables and revert back to the login form.
        /// </summary>
        public static void LogoutUser()
        {
            // Show MessageBox asking user for confirmation.
            /*DialogResult dialogResult = MessageBox.Show("Do you really want to logout?", "Prompt", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                // Clear all variables.
                loggedInUser = null;
                judgeMembers = new List<JudgeMember>();
                contests = new List<Contest>();
                contestDetails = new List<ContestDetails>();
                projects = new List<Project>();
                categories = new List<Category>();
                statistics = new List<Statistic>();

                // Close current form and go back to the login form.
                loginform.Show();
                loginform.MdiParent.Text = "Login";
                currentForm.Close();
            }*/
        }
        #endregion
    }
}
