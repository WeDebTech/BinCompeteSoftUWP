using BinCompeteSoft;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP
{
    /// <summary>
    /// This class holds all the necessary data for the program to work.
    /// </summary>
    class Data
    {
        // Make it so the class is a singleton
        public static readonly Data _instance = new Data();

        public User loggedInUser { get; set; }

        /*public Form currentForm { get; set; }
        public Form loginform { get; set; }*/

        private List<JudgeMember> judgeMembers = new List<JudgeMember>();
        private List<Contest> contests = new List<Contest>();
        private List<ContestDetails> contestDetails = new List<ContestDetails>();
        private List<Project> projects = new List<Project>();
        private List<Category> categories = new List<Category>();
        private List<Statistic> statistics = new List<Statistic>();
        private List<Criteria> criterias = new List<Criteria>();

        private Data()
        {
            // Well, nothing to do here
        }

        /// <summary>
        /// Gets or sets the list of judge members.
        /// </summary>
        public List<JudgeMember> JudgeMembers
        {
            get { return judgeMembers; }
            set { judgeMembers = value; }
        }

        /// <summary>
        /// Gets or sets the list of contests.
        /// </summary>
        public List<Contest> Contests
        {
            get { return contests; }
            set { contests = value; }
        }

        /// <summary>
        /// Gets or sets the list of contest details.
        /// </summary>
        public List<ContestDetails> ContestDetails
        {
            get { return contestDetails; }
            set { contestDetails = value; }
        }

        /// <summary>
        /// Gets or sets the list of projects.
        /// </summary>
        public List<Project> Projects
        {
            get { return projects; }
            set { projects = value; }
        }

        /// <summary>
        /// Gets or sets the list of categories.
        /// </summary>
        public List<Category> Categories
        {
            get { return categories; }
            set { categories = value; }
        }

        /// <summary>
        /// Gets or sets the list of statistics.
        /// </summary>
        public List<Statistic> Statistics
        {
            get { return statistics; }
            set { statistics = value; }
        }

        /// <summary>
        /// Gets or sets the list of criterias.
        /// </summary>
        public List<Criteria> Criterias
        {
            get { return criterias; }
            set { criterias = value; }
        }

        /// <summary>
        /// Add a judge member to the judge members list.
        /// </summary>
        /// <param name="judgeMember"></param>
        public void AddJudgeMember(JudgeMember judgeMember)
        {
            judgeMembers.Add(judgeMember);
        }

        /// <summary>
        /// Add a contest to the contests list.
        /// </summary>
        /// <param name="contest"></param>
        public void AddContest(Contest contest)
        {
            contests.Add(contest);
        }

        /// <summary>
        /// Add a project to the projects list.
        /// </summary>
        /// <param name="project"></param>
        public void AddProject(Project project)
        {
            projects.Add(project);
        }

        /// <summary>
        /// This method retrieves the most up-to-date list of judges from the database.
        /// </summary>
        /// <returns>True if success, false otherwise.</returns>
        public bool RefreshJudges()
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
                    judgeMembers.Clear();

                    while (reader.Read())
                    {
                        // Construct user information from database
                        JudgeMember judge = new JudgeMember(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));

                        // Check if judge is not the current user
                        if (judge.Id != loggedInUser.Id)
                        {
                            // Add it to the list
                            judgeMembers.Add(judge);
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
        public bool RefreshCategories()
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
                    categories.Clear();

                    while (reader.Read())
                    {
                        Category category = new Category(reader.GetInt32(0), reader.GetString(1));
                        categories.Add(category);
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
        public bool RefreshContests()
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
            sqlUserId.Value = Data._instance.loggedInUser.Id;
            cmd.Parameters.Add(sqlUserId);

            // Execute query
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                // Check if contest exists
                if (reader.HasRows)
                {
                    contestDetails.Clear();

                    while (reader.Read())
                    {
                        ContestDetails contest = new ContestDetails(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4), reader.GetDateTime(5), false, false, false);
                        contestDetails.Add(contest);
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
        public bool GetContestResultsCalculatedStatus(int contestId)
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
        public bool GetContestVoteStatus(int contestId)
        {
            string query = "SELECT has_voted FROM contest_juri_table WHERE id_contest = @id_contest AND id_user = @id_user";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            SqlParameter sqlContestId = new SqlParameter("@id_contest", SqlDbType.Int);
            sqlContestId.Value = contestId;
            cmd.Parameters.Add(sqlContestId);

            SqlParameter sqlUserId = new SqlParameter("@id_user", SqlDbType.Int);
            sqlUserId.Value = loggedInUser.Id;
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
        public bool GetIfContestIsCreatedByCurrentUser(int contestId)
        {
            string query = "SELECT president FROM contest_juri_table WHERE id_contest = @id_contest AND id_user = @id_user";

            SqlCommand cmd = DBSqlHelper.Connection.CreateCommand();
            cmd.CommandText = query;

            SqlParameter sqlContestId = new SqlParameter("@id_contest", SqlDbType.Int);
            sqlContestId.Value = contestId;
            cmd.Parameters.Add(sqlContestId);

            SqlParameter sqlUserId = new SqlParameter("@id_user", SqlDbType.Int);
            sqlUserId.Value = loggedInUser.Id;
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
        public bool RefreshCriterias()
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
                    criterias.Clear();

                    while (reader.Read())
                    {
                        Criteria criteria = new Criteria(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));

                        criterias.Add(criteria);
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
        public Contest GetContest(int id)
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
            sqlUserId.Value = loggedInUser.Id;
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
                foreach (Category category in this.Categories)
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
                foreach (JudgeMember judge in this.JudgeMembers)
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
            this.RefreshCriterias();

            // Cycle through all criteria and get the full details.
            foreach (Criteria contestCriteria in contestCriterias)
            {
                foreach (Criteria criteria in this.criterias)
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
        public bool RefreshStatistics()
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
                    statistics.Clear();

                    while (reader.Read())
                    {
                        Statistic statistic = new Statistic(reader.GetInt32(0), (double)reader.GetDecimal(1), reader.GetInt32(2), reader.GetInt32(3), new List<CategoryStatistics>(), new List<BestProjects>());
                        statistics.Add(statistic);
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

                        foreach (Category tempCategory in Data._instance.Categories)
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
                        foreach (Statistic statistic in statistics)
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
        public void LogoutUser()
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
    }
}
