using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP.Classes
{
    /// <summary>
    /// This class contains the data used to evaluate projects by criteria.
    /// </summary>
    public class Evaluation
    {
        #region Class variables
        private Criteria criteria;
        private double[,] evaluationMatrix;
        private bool hasVoted;
        #endregion

        #region Class constructors
        /// <summary>
        /// Evaluation constructor that takes all arguments.
        /// </summary>
        /// <param name="criteria">The criteria that is being evaluated.</param>
        /// <param name="evaluationMatrix">The matrix of project evaluations.</param>
        /// <param name="hasVoted">Wether the evaluation has been already voted.</param>
        public Evaluation(Criteria criteria, double[,] evaluationMatrix, bool hasVoted)
        {
            this.criteria = criteria;
            this.evaluationMatrix = evaluationMatrix;
            this.hasVoted = hasVoted;
        }

        /// <summary>
        /// Evaluation constructor that takes no arguments.
        /// Initializes a Evaluation with empty criteria, 0x0 evaluation matrix and not yet voted.
        /// </summary>
        public Evaluation() : this(new Criteria(), new double[0, 0], false) { }
        #endregion

        #region Class methods
        /// <summary>
        /// Method to set the criteria values matrix with a json string.
        /// </summary>
        /// <param name="jsonCriteriaValues">The json string containing the criteria values.</param>
        public void SetEvaluationValuesFromJSON(string jsonEvaluationValues)
        {
            this.evaluationMatrix = JsonConvert.DeserializeObject<double[,]>(jsonEvaluationValues);
        }

        /// <summary>
        /// Method to construct a JSON string from the evaluation values.
        /// </summary>
        /// <returns>The evaluation values in a JSON string.</returns>
        public string GetEvaluationValuesJSON()
        {
            return JsonConvert.SerializeObject(evaluationMatrix);
        }
        #endregion

        #region Class Getters and Setters
        /// <summary>
        /// Gets or sets the criteria that is being evaluated.
        /// </summary>
        public Criteria Criteria
        {
            get { return criteria; }
            set { criteria = value; }
        }

        /// <summary>
        /// Gets or sets the matrix of project evaluations.
        /// </summary>
        public double[,] EvaluationMatrix
        {
            get { return evaluationMatrix; }
            set { evaluationMatrix = value; }
        }

        /// <summary>
        /// Gets or sets wether the evaluation has been already voted for.
        /// </summary>
        public bool HasVoted
        {
            get { return hasVoted; }
            set { hasVoted = value; }
        }
        #endregion
    }

    /// <summary>
    /// This class holds the evaluation for a contest.
    /// </summary>
    public class ContestEvaluation
    {
        #region Class variables
        public List<JudgeEvaluation> JudgeEvaluations { get; set; }
        #endregion
    }

    public class JudgeEvaluation
    {
        public int JudgeId { get; set; }
        public List<CriteriaEvaluation> CriteriaEvaluations { get; set; } = new List<CriteriaEvaluation>();
        public List<ProjectEvaluation> ProjectEvaluations { get; set; } = new List<ProjectEvaluation>();
    }

    public class CriteriaEvaluation
    {
        public Criteria Criteria1 { get; set; }
        public Criteria Criteria2 { get; set; }
        public int Value { get; set; }
    }

    public class ProjectEvaluation
    {
        public Criteria Criteria { get; set; }
        public Project Project1 { get; set; }
        public Project Project2 { get; set; }
        public int Value { get; set; }
    }
}
