using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoft
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
        private List<JudgeEvaluation> judgeEvaluations;
        #endregion

        #region Class constructors
        /// <summary>
        /// ContestEvaluation constructor that takes all arguments.
        /// </summary>
        /// <param name="judgeEvaluations">The list of judge evaluations.</param>
        public ContestEvaluation(List<JudgeEvaluation> judgeEvaluations)
        {
            this.judgeEvaluations = judgeEvaluations;
        }

        /// <summary>
        /// ContestEvaluation constructor that takes no arguments.
        /// Initializes a ContestEvaluation object with an empty list of judge evaluations.
        /// </summary>
        public ContestEvaluation() : this(new List<JudgeEvaluation>()) { }
        #endregion

        #region Class Getters and Setterrs
        /// <summary>
        /// Gets or sets the list of judge evaluations.
        /// </summary>
        public List<JudgeEvaluation> JudgeEvaluations
        {
            get { return judgeEvaluations; }
            set { judgeEvaluations = value; }
        }
        #endregion
    }

    /// <summary>
    /// This class holds the evaluation for a criteria.
    /// </summary>
    public class JudgeEvaluation
    {
        #region Class variables
        private List<Evaluation> criteriaEvaluation;
        #endregion

        #region Class contructors
        /// <summary>
        /// JudgeEvaluation constructor that takes all arguments.
        /// </summary>
        /// <param name="criteriaEvaluation">The list of criteria evaluations.</param>
        public JudgeEvaluation(List<Evaluation> criteriaEvaluation)
        {
            this.criteriaEvaluation = criteriaEvaluation;
        }

        /// <summary>
        /// JudgeEvaluation constructor that takes no arguments.
        /// Initializes a JudgeEvaluation object with an empty list of criteria evaluations.
        /// </summary>
        public JudgeEvaluation() : this(new List<Evaluation>()) { }
        #endregion

        #region Class Getters and Setters
        /// <summary>
        /// Gets or sets the list of criteria evaluations.
        /// </summary>
        public List<Evaluation> CriteriaEvaluation
        {
            get { return criteriaEvaluation; }
            set { criteriaEvaluation = value; }
        }
        #endregion
    }

    public class ProjectEvaluation
    {
        #region Class variables
        private Project project;
        private double evaluation;
        #endregion

        #region Class constructors
        /// <summary>
        /// FinalEvaluation constructor that takes all arguments.
        /// </summary>
        /// <param name="project">The project this evaluation corresponds to.</param>
        /// <param name="evaluation">The project evaluation.</param>
        public ProjectEvaluation(Project project, double evaluation)
        {
            this.project = project;
            this.evaluation = evaluation;
        }

        /// <summary>
        /// ProjectEvaluation constructor that takes no arguments.
        /// Initializes a ProjectEvaluation object with empty project and evaluation 0.
        /// </summary>
        public ProjectEvaluation() : this(new Project(), 0f) { }
        #endregion

        #region Class Getters and Setters
        /// <summary>
        /// Gets or sets the project this evaluation applies to.
        /// </summary>
        public Project Project
        {
            get { return project; }
            set { project = value; }
        }

        /// <summary>
        /// Gets or sets the project evaluation.
        /// </summary>
        public double Evaluation
        {
            get { return evaluation; }
            set { evaluation = value; }
        }
        #endregion
    }
}
