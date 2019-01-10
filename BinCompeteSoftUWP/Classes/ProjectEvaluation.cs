using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoft.Classes
{
    /// <summary>
    /// This class contains the project's evaluation.
    /// </summary>
    class ProjectEvaluation
    {
        // Class variables.
        private JudgeMember judge;
        private Contest contest;
        private Criteria criteria;
        private double[,] evaluation;

        /// <summary>
        /// ProjectEvaluation constructor that takes all arguments.
        /// </summary>
        /// <param name="judge">The project evaluation judge.</param>
        /// <param name="contest">The project evaluation contest.</param>
        /// <param name="criteria">The project evaluation criteria.</param>
        /// <param name="evaluation">The project evaluation matrix.</param>
        public ProjectEvaluation(JudgeMember judge, Contest contest, Criteria criteria, double[,] evaluation)
        {
            this.judge = judge;
            this.contest = contest;
            this.criteria = criteria;
            this.evaluation = evaluation;
        }

        /// <summary>
        /// Gets or sets the project evaluation judge.
        /// </summary>
        public JudgeMember Judge
        {
            get { return this.judge; }
            set { this.judge = value; }
        }

        /// <summary>
        /// Gets or sets the project evaluation contest.
        /// </summary>
        public Contest Contest
        {
            get { return this.contest; }
            set { this.contest = value; }
        }

        /// <summary>
        /// Gets or sets the project evaluation criteria.
        /// </summary>
        public Criteria Criteria
        {
            get { return this.criteria; }
            set { this.criteria = value; }
        }

        /// <summary>
        /// Gets or sets the project evaluation matrix.
        /// </summary>
        public double[,] Evaluation
        {
            get { return this.evaluation; }
            set { this.evaluation = value; }
        }
    }
}
