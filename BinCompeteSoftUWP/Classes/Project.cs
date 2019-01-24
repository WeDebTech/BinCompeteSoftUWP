using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP.Classes
{
    /// <summary>
    /// This class contains the project data.
    /// </summary>
    public class Project
    {
        #region Class variables
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ObservableCollection<Promoter> Promoters { get; set; }
        public Category Category { get; set; }
        public int Year { get; set; }
        #endregion

        #region Class constructors
        /// <summary>
        /// Project constructor that takes all arguments.
        /// </summary>
        /// <param name="id">The project id.</param>
        /// <param name="name">The project name.</param>
        /// <param name="description">The project description.</param>
        /// <param name="category">The project category.</param>
        /// <param name="year">The year the project was created.</param>
        public Project(int id, string name, string description, Category category, int year)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Category = category;
            this.Year = year;
        }

        /// <summary>
        /// Project constructor that takes no arguments.
        /// This creates a project with id = 0, name = "", description = "", category = 0.
        /// </summary
        public Project() : this(0, "", "", new Category(), DateTime.Now.Year) { }
        #endregion
    }
}
