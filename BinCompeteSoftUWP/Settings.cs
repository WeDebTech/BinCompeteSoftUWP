using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BinCompeteSoftUWP
{
    public class Settings : INotifyPropertyChanged
    {
        #region Class variables
        public string Ip { get; set; }
        public string DBName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }

        private double _fontSizeSmaller = 16;
        public double FontSizeSmaller
        {
            get { return _fontSizeSmaller; }
            set { _fontSizeSmaller = value; OnPropertyChanged(); }
        }
        private double _fontSizeSmall = 18;
        public double FontSizeSmall
        {
            get { return _fontSizeSmall; }
            set { _fontSizeSmall = value; OnPropertyChanged(); }
        }
        private double _fontSizeNormal = 20;
        public double FontSizeNormal
        {
            get { return _fontSizeNormal; }
            set { _fontSizeNormal = value; OnPropertyChanged(); }
        }
        private double _fontSizeTitle = 24;
        public double FontSizeTitle
        {
            get { return _fontSizeTitle; }
            set { _fontSizeTitle = value; OnPropertyChanged(); }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
