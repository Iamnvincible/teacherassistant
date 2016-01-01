using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherAssistant.DataBase;
using TeacherAssistant.Model;

namespace TeacherAssistant.ViewModel
{
    public class StatisticsPageViewModel:INotifyPropertyChanged
    {
        ObservableCollection<ClassDetail> cd;
        public List<int> countstus { get; set; } = new List<int>();

        public ObservableCollection<ClassDetail> Cd
        {
            get
            {
                return cd;
            }

            set
            {
                cd = value;
                Notyfiy("Cd");
            }
        }

        public StatisticsPageViewModel()
        {
            Cd = new ObservableCollection<ClassDetail> (App.classtable);
            //cd = (from n in ori group n by n.StudentListUrl into g select g.First()).ToList() ;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notyfiy(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

    }
}
