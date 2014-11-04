using angjwcf.Common;
using Awesomium.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace angjwcf.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            webControl.NativeViewInitialized += OnNativeViewInitialized;
            webControl.DocumentReady += OnDocumentReady;
        }

        private void OnNativeViewInitialized(object sender, WebViewEventArgs e)
        {

            using (var s = (JSObject)webControl.CreateGlobalJavascriptObject("uScriptHelper"))
            {
                s.Bind("xmlHttpRequest", false, UserScripts.OnUserScriptHttpRequest);
            }
            using (var s = (JSObject)webControl.CreateGlobalJavascriptObject("NamedPipeXmlHttp"))
            {
                s.Bind("xmlHttpRequest", false, NamedPipeXmlHttp.OnUserScriptHttpRequest);
            }
        }
        // I added a file named UserScripts.js to my solution
        // and set it's BuildAction to EmbeddedResource
        private void OnDocumentReady(object s, UrlEventArgs e)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            try
            {
                using (var reader = new StreamReader(asm.GetManifestResourceStream("angjwcf.Javascript.UserScripts.js")))
                    webControl.ExecuteJavascriptWithResult(reader.ReadToEnd());
            }
            catch { }
        }
    }
}
