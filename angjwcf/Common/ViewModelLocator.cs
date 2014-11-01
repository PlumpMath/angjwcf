using angjwcf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace angjwcf.Common
{
    class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel { get { return new MainWindowViewModel(); } }
        public HomeViewModel HomeViewModel { get { return new HomeViewModel(); } }
        public TodoViewModel TodoDetailsViewModel { get { return new TodoViewModel(); } }
    }
}
