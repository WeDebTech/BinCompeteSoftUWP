using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoft
{
    public static class AHP
    {
        /// <summary>
        /// Calculates the projects scores using the AHP method
        /// </summary>
        /// <param name="contestEvaluation">A ContestEvaluation object containing all evaluations for the contest. All values must be positive.</param>
        /// <param name="criteriaScores">A matrix containing the criteria scores. All values must be positive.</param>
        /// <returns>The projects final score.</returns>
        public static double[] CalculateAHP(ContestEvaluation contestEvaluation, double[,] criteriaScores)
        {
            // Check criteria values matrix for errors
            for (int i = 0; i < criteriaScores.GetLength(0); i++)
            {
                for (int j = 0; j < criteriaScores.GetLength(1); j++)
                {
                    if (criteriaScores[i, j] < 0 || criteriaScores[i, j] > 9)
                    {
                        throw new ArgumentOutOfRangeException("Criteria values must be between 1/9 and 9.");
                    }
                }
            }

            // Check project score values matrix for errors
            foreach (JudgeEvaluation judgeEvaluation in contestEvaluation.JudgeEvaluations)
            {
                foreach (Evaluation evaluation in judgeEvaluation.CriteriaEvaluation)
                {
                    for (int i = 0; i < evaluation.EvaluationMatrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < evaluation.EvaluationMatrix.GetLength(0); j++)
                        {
                            if (evaluation.EvaluationMatrix[i, j] < 0 || evaluation.EvaluationMatrix[i, j] > 9)
                            {
                                throw new ArgumentOutOfRangeException("Project values must be between 1/9 and 9.");
                            }
                        }
                    }
                }
            }

            // Calculate criterias step
            double[] criteriaScoresStep = CalculateStep(criteriaScores);

            // Calculate criteria ratio
            double[] criteriaScoresRatio = CalculateRatio(criteriaScoresStep);

            // Temporary matrix to hold a single judge criteria project values
            double[,] projectsScoresTemp;

            // Temporary array to hold criteria step values
            double[] projectsStepTemp;

            // Temporary matrix to hold the criteria ratio values
            double[,] projectsRatioTemp;

            // Temporary matrix to hold the criteria final values, size is (number of judges * number of projects)
            double[,] projectsFinalValues = new double[contestEvaluation.JudgeEvaluations.Count, contestEvaluation.JudgeEvaluations[0].CriteriaEvaluation[0].EvaluationMatrix.GetLength(0)];

            int m = 0;
            int n = 0;

            // Calculate projects step for each criteria and judge
            // Here we cycle through judges
            foreach(JudgeEvaluation judgeEvaluation in contestEvaluation.JudgeEvaluations)
            {
                // Final values matrix size is (number of criteria * number of projects).
                projectsRatioTemp = new double[judgeEvaluation.CriteriaEvaluation.Count, judgeEvaluation.CriteriaEvaluation[0].EvaluationMatrix.GetLength(0)];

                m = 0;

                // Here we cycle through criterias.
                foreach(Evaluation evaluation in judgeEvaluation.CriteriaEvaluation)
                {
                    projectsScoresTemp = new double[evaluation.EvaluationMatrix.GetLength(0), evaluation.EvaluationMatrix.GetLength(1)];

                    for(int i = 0; i < evaluation.EvaluationMatrix.GetLength(0); i++)
                    {
                        for(int j = 0; j < evaluation.EvaluationMatrix.GetLength(1); j++)
                        {
                            // Assign a single judg criteria values to a smaller matrix so it can be more easily evaluated.
                            projectsScoresTemp[i, j] = evaluation.EvaluationMatrix[i, j];
                        }
                    }

                    // Calculate criteria step.
                    projectsStepTemp = CalculateStep(projectsScoresTemp);

                    // Calculate criteria ratio.
                    double[] projectRatioArrayTemp = CalculateRatio(projectsStepTemp);

                    for(int k = 0; k < projectRatioArrayTemp.Length; k++)
                    {
                        projectsRatioTemp[m, k] = projectRatioArrayTemp[k];
                    }

                    m++;
                }

                // Calculate final values with all previous calculated final values for this judge.
                double[] projectFinalValuesTemp = CalculateFinalValues(projectsRatioTemp, criteriaScoresRatio);

                for(int l = 0; l < projectFinalValuesTemp.Length; l++)
                {
                    projectsFinalValues[n, l] = projectFinalValuesTemp[l];
                }

                n++;
            }

            // Array that will hold all the projects final values
            double[] contestFinalValues = new double[projectsFinalValues.GetLength(1)];

            // Finally calculate final values for all projects
            for(int i = 0; i < projectsFinalValues.GetLength(1); i++)
            {
                contestFinalValues[i] = 0;

                for(int j = 0; j < projectsFinalValues.GetLength(0); j++)
                {
                    contestFinalValues[i] += projectsFinalValues[j, i];
                }
            }

            return contestFinalValues;
        }

        /// <summary>
        /// Calculates the step of a criteria matrix.
        /// </summary>
        /// <param name="criteriaMatrix">The criteria matrix. All values must be positive.</param>
        /// <returns>Returns the calculated step array.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a value in the criteriaMatrix matrix
        /// is negative.</exception>
        public static double[] CalculateStep(double[,] criteriaMatrix)
        {
            // Create the step array with the length of the criteria matrix
            double[] stepArray = new double[criteriaMatrix.GetLength(0)];

            double temp;

            for(int i = 0; i < stepArray.Length; i++)
            {
                temp = 1;

                for(int j = 0; j < stepArray.Length; j++)
                {
                    // Check for errors
                    if(criteriaMatrix[i, j] < 0)
                    {
                        throw new ArgumentOutOfRangeException("Values cannot be negative.");
                    }

                    temp *= criteriaMatrix[i, j];
                }

                temp = Math.Pow(temp, 1f / (double) stepArray.Length);

                stepArray[i] = temp;
            }

            return stepArray;
        }

        /// <summary>
        /// Calculates the array of ratios.
        /// </summary>
        /// <param name="criteriaStep">Array of step values. All values must be positive.</param>
        /// <returns>The calculated ratios array.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a value in the criteriaStep array
        /// is negative.</exception>
        public static double[] CalculateRatio(double[] criteriaStep)
        {
            double sum = 0;

            double[] ratios = new double[criteriaStep.Length];

            for(int i = 0; i < criteriaStep.Length; i++)
            {
                // Check for errors
                if(criteriaStep[i] < 0)
                {
                    throw new ArgumentOutOfRangeException("Values cannot be negative.");
                }

                sum += criteriaStep[i];
            }

            for(int  i = 0; i < criteriaStep.Length; i++)
            {
                ratios[i] = criteriaStep[i] / sum;
            }

            return ratios;
        }

        /// <summary>
        /// Calculates the final priorities.
        /// </summary>
        /// <param name="criteriaRatios">The matrix of criteria ratios. All values must be positive</param>
        /// <param name="criteriaFinalRatios">The array of criteria final ratios. All values must be positive.</param>
        /// <returns>The calculated final values array.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a value in the criteriaRatios or criteriaFinalRatios
        /// array is negative.</exception>
        public static double[] CalculateFinalValues(double[,] criteriaRatios, double[] criteriaFinalRatios)
        {
            double[] priorities = new double[criteriaRatios.GetLength(1)];

            // Cycle through all projects
            for(int i = 0; i < criteriaRatios.GetLength(1); i++)
            {
                // Cycle through all criterias
                for(int j = 0; j < criteriaRatios.GetLength(0); j++)
                {
                    if(criteriaRatios[j, i] < 0 || criteriaFinalRatios[j] < 0)
                    {
                        throw new ArgumentOutOfRangeException("Values cannot be negative");
                    }

                    priorities[i] = priorities[i] + criteriaRatios[j, i] * criteriaFinalRatios[j];
                }
            }

            return priorities;
        }
    }
}