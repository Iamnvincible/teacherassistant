using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherAssistant.ViewModel
{
    public class IndexPageViewModel
    {
        public DateTime Now { get; set; }
        public IndexPageViewModel()
        {
            Now = DateTime.Now.ToLocalTime();
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["week"].Value = "155";
            string sss = ConfigurationManager.AppSettings["week"];
            cfa.Save();
        }
    }
}
