using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP.Classes
{
    /// <summary>
    /// This class contains the project's evaluation.
    /// </summary>
    public class ProjectContestEvaluation
    {
        #region Class variables
        public JudgeMember Judge { get; set; }
        public Contest Contest { get; set; }
        public Criteria Criteria { get; set; }
        public double[,] Evaluation { get; set; }
        #endregion

        #region Class constructors
        /// <summary>
        /// ProjectEvaluation constructor that takes all arguments.
        /// </summary>
        /// <param name="judge">The project evaluation judge.</param>
        /// <param name="contest">The project evaluation contest.</param>
        /// <param name="criteria">The project evaluation criteria.</param>
        /// <param name="evaluation">The project evaluation matrix.</param>
        public ProjectContestEvaluation(JudgeMember judge, Contest contest, Criteria criteria, double[,] evaluation)
        {
            Judge = judge;
            Contest = contest;
            Criteria = criteria;
            Evaluation = evaluation;
        }
        #endregion
    }
}
