using Awesomium.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace angjwcf.Common
{
    /// <summary>
    /// A custom implementation of the `Awesomium.Windows.Forms.ResourceDataSourceProvider` functionality.
    /// This interceptor supports using the standard `http://` protocol scheme instead of Awesomium's use of the
    /// custom `asset://` scheme, which can cause problems with cross origin requests in Chromium, as well as
    /// third party services like Google APIs which authorize JavaScript clients based on the value of `window.location`
    /// in the browser.
    /// </summary>
    public class AngjWcfInterceptor : IResourceInterceptor
    {
        /// <summary>The base Uri to use as the protocol/scheme and server/domain for requesting embedded resources in the assembly manifest</summary>
        /// <remarks>Uses the template `http://[AssemblyName].local`</remarks>
        public static readonly Uri EmbeddedResourceDomain;
        public static readonly Uri LocalContentDomain;
        public static readonly Uri WcfServiceRequestDomain;

        /// <summary>A cached reference to the assembly containing the embedded resources</summary>
        private static readonly Assembly AppAssembly;

        /// <summary>The temporary folder on disk </summary>
        private static readonly string TempFolder;

        /// <summary>
        /// Static constructor to initialize the EmbeddedResourceDomain and TempFolder variables
        /// </summary>
        static AngjWcfInterceptor()
        {
            // cache a reference to the app's assembly to avoid looking up for every static file
            AppAssembly = typeof(AngjWcfInterceptor).Assembly;

            // the base Uri to use for all embedded resource requests
            EmbeddedResourceDomain = new Uri(String.Concat("http://", AppAssembly.GetName().Name, ".local"));
            LocalContentDomain = new Uri(String.Concat("http://localhost/Content/"));
            WcfServiceRequestDomain = new Uri("http://localhost:8890/");//string.Concat("net.pipe://localhost/", AppAssembly.GetName().Name, "Svc"));

            // let the framework create a unique directory path for us by using the method for creating a unique temp file
            TempFolder = Path.GetTempFileName();
            File.Delete(TempFolder);
            Directory.CreateDirectory(TempFolder);
        }

        /// <summary>
        /// Static finalizer to delete any temp folders/files created by this type
        /// </summary>
        ~AngjWcfInterceptor()
        {
            try
            {
                if (null != TempFolder && Directory.Exists(TempFolder))
                {
                    Directory.Delete(TempFolder, true);
                }
            }
            catch { }
        }

        /// <summary>
        /// Optionally blocks any web browser requests by returning true. Not used.
        /// </summary>
        /// <remarks>
        /// This method can implement a whitelist of allowed URLs here by
        /// returning true to block any whitelist misses
        /// </remarks>
        public virtual bool OnFilterNavigation(NavigationRequest request)
        {
            return false;
        }

        /// <summary>
        /// Intercepts any requests for the EmbeddedResourceDomain base Uri,
        /// and returns a response using the embedded resource in this app's assembly/DLL file
        /// </summary>
        public virtual ResourceResponse OnRequest(ResourceRequest request)
        {
            ResourceResponse response = null;

            // log the request to the debugger output
            System.Diagnostics.Debug.Print(String.Concat(request.Method, ' ', request.Url.ToString()));

            if (IsEmbeddedResource(request))
            {
                response = CreateResponseFromResource(request);
            }
            else if(IsLocalContentDomain(request))
            {
                response = CreateResponseFromLocalContent(request);
            }
            else if (IsWcfServiceRequestDomain(request))
            {
                response = CreateResponseFromWcfService(request);
            }

            return response;
        }

        private ResourceResponse CreateResponseFromLocalContent(ResourceRequest request)
        {
            //Uncomment this once above functionality is complete
            string resourceName;
            string filePath;

            // this project embeds static HTML/JS/CSS/PNG files as resources
            // by translating the resource's relative file path like Resources\foo/bar.html
            // to a logical name like /www/foo/bar.html
            //resourceName = String.Concat("Content", request.Url.AbsolutePath);
            resourceName = request.Url.AbsolutePath;
            resourceName = resourceName.Replace('/', Path.DirectorySeparatorChar);
            resourceName = (resourceName.StartsWith(Path.DirectorySeparatorChar.ToString())) ? resourceName.TrimStart(new char[] { Path.DirectorySeparatorChar }) : resourceName;
            filePath = Path.GetFullPath(Path.Combine(TempFolder, resourceName));

            // cache the resource to a temp file if
            if (!File.Exists(filePath))
            {
                ExtractResourceToFile(resourceName, filePath);
            }

            return ResourceResponse.Create(filePath);
        }

        private ResourceResponse CreateResponseFromWcfService(ResourceRequest request)
        {
            //Should convert and return response from Wcfservice by 
           // return ResourceResponse.Create()
            HttpWebRequest webreq = createWebRequest(request);

            //using (var streamWriter = new StreamWriter(webreq.GetRequestStream()))
            //{
            //    streamWriter.Write(_text);
            //    streamWriter.Flush();
            //    streamWriter.Close();
            //}


            return (readWebResponse(webreq));
        }

        private HttpWebRequest createWebRequest(ResourceRequest request)
        {
            HttpWebRequest webreq = null;

            webreq= HttpWebRequest.CreateHttp(request.Url);
            webreq.Method = request.Method;
            webreq.ContentType = "application/json; charset=utf-8";
            webreq.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //webreq.Accept = "application/json";
            //webreq.KeepAlive=
            if (!string.IsNullOrWhiteSpace(request.ExtraHeaders))
            {
                string[] hdrarr = request.ExtraHeaders.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string hdr in hdrarr)
                {
                    string[] hdrsplt= hdr.Split(new char[] { ':' });

                    if (hdrsplt.Length>1)
                    {
                        switch (hdrsplt[0].Trim())
                        {
                            case("Accept"):
                                webreq.Accept = hdrsplt[1];
                                break;
                            case("User-Agent"):
                                webreq.UserAgent = hdrsplt[1];
                                break;
                            case("Timeout"):
                                webreq.Timeout = int.Parse(hdrsplt[1]);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        webreq.Headers.Add(hdr);
                    }
                }
            }
            return (webreq);
        }

        private ResourceResponse readWebResponse(HttpWebRequest webreq)
        {
            HttpWebRequest.DefaultMaximumErrorResponseLength = 1048576;
            HttpWebResponse webresp = null;// = webreq.GetResponse() as HttpWebResponse;
            var memStream = new MemoryStream();
            Stream webStream;
            try
            {
                webresp = (HttpWebResponse)webreq.GetResponse();
                webStream = webresp.GetResponseStream();
                byte[] readBuffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = webStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                    memStream.Write(readBuffer, 0, bytesRead);
            }
            catch (WebException e)
            {
                var r = e.Response as HttpWebResponse;
                webStream = r.GetResponseStream();
                memStream = Read(webStream);
                var wrongLength = memStream.Length;
            }


            memStream.Position = 0;
            StreamReader sr = new StreamReader(memStream);
            string webStreamContent = sr.ReadToEnd();


            byte[] responseBuffer = Encoding.UTF8.GetBytes(webStreamContent);

            // Initialize unmanaged memory to hold the array.
            int responseSize = Marshal.SizeOf(responseBuffer[0]) * responseBuffer.Length;
            IntPtr pointer = Marshal.AllocHGlobal(responseSize);
            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(responseBuffer, 0, pointer, responseBuffer.Length);
                return ResourceResponse.Create((uint)responseBuffer.Length, pointer, webresp.ContentType);
            }
            finally
            {
                // Data is not owned by the ResourceResponse. A copy is made 
                // of the supplied buffer. We can safely free the unmanaged memory.
                Marshal.FreeHGlobal(pointer);
                webStream.Close();
            }

        }

        public static MemoryStream Read(Stream stream)
        {
            MemoryStream memStream = new MemoryStream();

            byte[] readBuffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                memStream.Write(readBuffer, 0, bytesRead);
            return memStream;
        }

        /// <summary>
        /// Determines if the request is for an embedded resource
        /// </summary>
        private bool IsEmbeddedResource(ResourceRequest request)
        {
            return EmbeddedResourceDomain.IsBaseOf(request.Url);
        }

        /// <summary>
        /// Determines if the request is for a local content resource
        /// </summary>
        private bool IsLocalContentDomain(ResourceRequest request)
        {
            return LocalContentDomain.IsBaseOf(request.Url);
        }

        /// <summary>
        /// Determines if the request is for a wcf service resource
        /// </summary>
        private bool IsWcfServiceRequestDomain(ResourceRequest request)
        {
            return WcfServiceRequestDomain.IsBaseOf(request.Url);
        }

        /// <summary>
        /// Creates a response using the contents of an embedded assembly resource.
        /// </summary>
        private ResourceResponse CreateResponseFromResource(ResourceRequest request)
        {
            string resourceName;
            string filePath;

            // this project embeds static HTML/JS/CSS/PNG files as resources
            // by translating the resource's relative file path like Resources\foo/bar.html
            // to a logical name like /www/foo/bar.html
            resourceName = String.Concat("www", request.Url.AbsolutePath);
            filePath = Path.GetFullPath(Path.Combine(TempFolder, resourceName.Replace('/', Path.DirectorySeparatorChar)));

            // cache the resource to a temp file if
            if (!File.Exists(filePath))
            {
                ExtractResourceToFile(resourceName, filePath);
            }

            return ResourceResponse.Create(filePath);
        }

        /// <summary>
        /// Extracts an assembly resource to a temporary file on disk.
        /// This avoids with pinning a managed byte array from GC reallocation in multi-threaded code.
        /// </summary>
        /// <remarks>
        /// While we could just use the `ResourceResponse.Create(uint NumBytes, IntPtr buffer, string mimeType)`
        /// overload to read a resource directly into memory, this could create headaches with needing to pin
        /// the byte array buffer in memory so that the GC can't move it.
        /// This can be challenging with value types like bytes in multi-threaded apps.
        ///
        /// It also eliminates the need to deal with determining the file type to mime type mapping
        /// in .Net 4.0 Client Profile installations.
        ///
        /// See http://www.hanselman.com/blog/BackToBasicsEveryoneRememberWhereWeParkedThatMemory.aspx
        /// </remarks>
        private void ExtractResourceToFile(string resourceName, string filePath)
        {
            string parentPath;

            // the embedded resources start with a '/' char in this project
            resourceName = angjwcf.Service.Entity.Utility.getAbsolutePath("",resourceName);

            parentPath = Directory.GetParent(filePath).FullName;
            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }

            using (Stream inputStream = new StreamReader(resourceName).BaseStream)
            using (FileStream outputStream = new FileStream(filePath, FileMode.Create))
            {
                inputStream.CopyTo(outputStream);
                outputStream.Close();
            }
        }
    }

}
