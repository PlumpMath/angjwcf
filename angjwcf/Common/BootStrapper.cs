using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using angjwcf.Service;
using System.ServiceModel.Description;
using Awesomium.Core;
using System.ServiceModel.Web;

namespace angjwcf.Common
{
    class BootStrapper
    {
        private static BootStrapper _instance;
        private static ServiceHost _host;
        //private static WebServiceHost _host;
        private static AngjWcfInterceptor _resourceInterceptor;

        public AngjWcfInterceptor ResourceInterceptor { get { return (_resourceInterceptor); } }

        private static TodoServiceClient _todoServiceClient;

        public static BootStrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BootStrapper();
                }
                return (_instance);
            }
        }

        public TodoServiceClient todoServiceClient { get { return (_todoServiceClient); } }

        private BootStrapper()
        {
            _host = new ServiceHost(typeof(TodoService), new Uri("net.pipe://localhost/angjwcfSvc"));
            _host.AddServiceEndpoint(typeof(angjwcf.Service.ITodoService), new NetNamedPipeBinding(), "");

            //_host = new WebServiceHost(typeof(TodoService), new Uri("http://localhost:8890/angjwcfSvc"));//, new Uri("http://127.0.0.1:8890/angjwcfSvc")); //new WebServiceHost(typeof(TodoService), new Uri("http://localhost:8890/angjwcfSvc"));

            //var binding = new WSHttpBinding();
            //_host.AddServiceEndpoint(typeof(angjwcf.Service.ITodoService), binding, "angjwcfSvc");

            //ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            //smb.HttpGetEnabled = true;
            //smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            //_host.Description.Behaviors.Add(smb);
            //_host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "http://localhost:8890/angjwcfSvc/mex");


            _resourceInterceptor = new AngjWcfInterceptor();
            WebCore.Started += WebCore_Started;


            //In case we are hosting there service in this application. It will pick the settings from app.config
           // _host = new ServiceHost(typeof(angjwcf.Service.TodoService));                        
        }

        private void WebCore_Started(object sender, CoreStartEventArgs e)
        {
            WebCore.ResourceInterceptor = BootStrapper.Instance.ResourceInterceptor;
        }

        public void Bootstrap(App app, System.Windows.StartupEventArgs e)
        {


            //Start server in new thread
            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem(state =>
                {
                    _host.Open();
                });
            }
            catch (Exception exc)
            {
                _host.Abort();
                throw (exc);
            }

            //Start client
            _todoServiceClient = new TodoServiceClient("NetNamedPipeBinding_ITodoService");

            //try
            //{
            //    _todoServiceClient.Open();
            //}
            //catch (Exception ex)
            //{
            //    _todoServiceClient.Abort();
            //    throw(ex);
            //}

            //_todoServiceClient = new TodoServiceClient("WsHttpBinding_ITodoService");


        }

        public void ShutDown(App app, System.Windows.ExitEventArgs e)
        {
            ////Lets close the client first
            try
            {
                _todoServiceClient.Close();
            }
            catch (Exception exc1)
            {
                _todoServiceClient.Abort();
                throw (exc1);
            }

            //Close the server now
            try
            {
                _host.Close();
            }
            catch (Exception exc)
            {
                _host.Abort();
                throw (exc);
            }

        }
    }
}
