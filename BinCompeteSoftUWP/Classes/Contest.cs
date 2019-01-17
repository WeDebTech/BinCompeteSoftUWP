using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP.Classes
{
    /// <summary>
    /// This class contains the contest data.
    /// </summary>
    public class Contest
    {
        #region Class variables
        public int Id { get; set; }
        public ContestDetails ContestDetails { get; set; }
        public ObservableCollection<Project> Projects { get; set; }
        public ObservableCollection<JudgeMember> JudgeMembers { get; set; }
        public ObservableCollection<Criteria> Criterias { get; set; }
        public double[,] CriteriaValues { get; set; }
        #endregion

        #region Class constructors
        /// <summary>
        /// Contest constructor that takes all arguments.
        /// </summary>
        /// <param name="id">The contest id.</param>
        /// <param name="contestDetails">Object that contains contest information like id, name, description, start date and limit date.</param>
        /// <param name="projects">The contest projects.</param>
        /// <param name="judgeMembers">The contest judge members.</param>
        /// <param name="criterias">The contest criterias.</param>
        /// <param name="criteriaValues">The contest criteria values for evaluation.</param>
        public Contest(int id, ContestDetails contestDetails, ObservableCollection<Project> projects, ObservableCollection<JudgeMember> judgeMembers, ObservableCollection<Criteria> criterias, double[,] criteriaValues)
        {
            Id = id;
            ContestDetails = contestDetails;
            Projects = projects;
            JudgeMembers = judgeMembers;
            Criterias = criterias;
            CriteriaValues = criteriaValues;
        }

        /// <summary>
        /// Contest constructor that takes no arguments.
        /// Creates contest with id 0, no contest details, no projects, no judges, no criterias and empty criteria values matrix.
        /// </summary>
        public Contest() : this(0, new ContestDetails(), new ObservableCollection<Project>(), new ObservableCollection<JudgeMember>(), new ObservableCollection<Criteria>(), new double[0, 0]) { }
        #endregion

        #region Class methods
        /// <summary>
        /// Method to set the criteria values matrix with a json string.
        /// </summary>
        /// <param name="jsonCriteriaValues">The json string containing the criteria values.</param>
        public void SetCriteriaValuesFromJSON(string jsonCriteriaValues)
        {
            this.CriteriaValues = JsonConvert.DeserializeObject<double[,]>(jsonCriteriaValues);
        }

        /// <summary>
        /// Method to construct a JSON string from the criteria values.
        /// </summary>
        /// <returns>The criteria values in a JSON string.</returns>
        public string GetCriteriaValuesJSON()
        {
            return JsonConvert.SerializeObject(CriteriaValues);
        }
        #endregion
    }
}
