using Awesomium.Core;
using Awesomium.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace angjwcf.Common
{
    public enum ReadyState : ushort
    {
        UNSENT = 0,
        OPENED = 1,
        HEADERS_RECEIVED = 2,
        LOADING = 3,
        DONE = 4,
    }

    public sealed class UserScripts
    {
        internal static async void OnUserScriptHttpRequest(object sender, JavascriptMethodEventArgs e)
        {
            bool aborted = false;

            var webcontrol = sender as WebControl;
            var obj = ((JSObject)e.Arguments[0]).Clone();

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            CancellationTokenSource taskSource = null;
            CancellationToken token;

            //Create a remote object so we can bind to "abort" function
            JSObject uresponse = webcontrol.ExecuteJavascriptWithResult("new Object();");

            try
            {
                uresponse["status"] = (uint)404;
                uresponse["statusText"] = "Page not found";
                uresponse["readyState"] = (ushort)ReadyState.UNSENT;
                uresponse["finalUrl"] = obj["url"];
                if(obj.HasProperty("context"))
                    uresponse["context"]=obj["context"];
                

                if (obj.HasMethod("onreadystatechange"))
                    obj.Invoke("onreadystatechange", uresponse);

                request = HttpWebRequest.CreateHttp(obj["url"]);

                request.AllowAutoRedirect = true;
                request.Method = obj["method"];

                if (obj.HasProperty("timeout"))
                    request.Timeout = (int)obj["timeout"];

                if (obj.HasProperty("headers"))
                {
                    var headers = (JSObject)obj["headers"];

                    if (headers.HasProperty("Accept"))
                        request.Accept = headers["Accept"];

                    if (headers.HasProperty("User-Agent"))
                        request.UserAgent = headers["User-Agent"];
                }

                taskSource = new CancellationTokenSource(request.Timeout);
                token = taskSource.Token;

                uresponse.Bind("abort", false, (s, args) =>
                {
                    aborted = true;
                    taskSource.Cancel(true);
                });

                uresponse["readyState"] = (ushort)ReadyState.OPENED;
                uresponse["finalUrl"] = request.RequestUri.ToString();

                await Task.Run(async () =>
                {

                    await webcontrol.Dispatcher.InvokeAsync(() =>
                    {

                        if (obj.HasMethod("onreadystatechange"))
                            obj.Invoke("onreadystatechange", uresponse);
                    });

                    token.ThrowIfCancellationRequested();

                    if (request.Method.ToUpper() == "POST")
                    {
                        //TODO: Notify 'upload' property callbacks
                    }

                    response = (HttpWebResponse)await request.GetResponseAsync();

                    await webcontrol.Dispatcher.InvokeAsync(() =>
                    {
                        uresponse["status"] = response.StatusCode == HttpStatusCode.OK ?
                        (uint)response.StatusCode :
                        404;

                        uresponse["statusText"] = response.StatusCode == HttpStatusCode.OK ?
                        "OK" :
                        "Page not found";

                        uresponse["finalUrl"] = response.ResponseUri.ToString();
                    });

                    if (response.SupportsHeaders)
                    {

                        await webcontrol.Dispatcher.InvokeAsync(() =>
                        {

                            JSObject head = webcontrol.ExecuteJavascriptWithResult("new Object()");
                            JSObject json = webcontrol.ExecuteJavascriptWithResult("JSON");

                            response.Headers.AllKeys.ToList().ForEach((s) => head[s] = response.Headers[s]);

                            uresponse["readyState"] = (ushort)ReadyState.HEADERS_RECEIVED;
                            uresponse["responseHeaders"] = json.Invoke("stringify", head);

                            if (obj.HasMethod("onreadystatechange"))
                                obj.Invoke("onreadystatechange", uresponse);
                        });

                        token.ThrowIfCancellationRequested();
                    }

                    if (request.Method.ToUpper() != "HEAD")
                    {
                        var content = new StringBuilder();

                        var stream = response.GetResponseStream();
                        var memStream = new MemoryStream();

                        await webcontrol.Dispatcher.InvokeAsync(() =>
                        {

                            uresponse["readyState"] = (ushort)ReadyState.LOADING;

                            if (obj.HasMethod("onreadystatechange"))
                                obj.Invoke("onreadystatechange", uresponse);
                        });

                        token.ThrowIfCancellationRequested();

                        try
                        {
                            //create a copy of the reponse stream so we can get the proper length
                            //"ConnectStream" does not support "Seek" operations
                            stream.CopyTo(memStream);
                            stream.Close();

                            memStream.Position = 0;
                            byte[] temp = new byte[8192];

                            await webcontrol.Dispatcher.InvokeAsync(() =>
                            {
                                uresponse["loaded"] = (uint)0;
                                uresponse["total"] = (uint)memStream.Length;
                                uresponse["lengthComputable"] = true;
                            });

                            while (memStream.Position < memStream.Length)
                            {

                                if (token.IsCancellationRequested)
                                    break;

                                int remaining = (int)(memStream.Length - memStream.Position);
                                int read = memStream.Read(
                                temp,
                                0,
                                (temp.Length > remaining) ? remaining : temp.Length);

                                content.Append(Encoding.UTF8.GetString(temp, 0, read));

                                await webcontrol.Dispatcher.InvokeAsync(() =>
                                {
                                    uresponse["loaded"] = (uint)memStream.Position;

                                    if (obj.HasMethod("onprogress"))
                                        obj.Invoke("onprogress", uresponse);
                                });
                            }
                        }
                        finally { memStream.Dispose(); }

                        token.ThrowIfCancellationRequested();

                        await webcontrol.Dispatcher.InvokeAsync(() =>
                        {
                            uresponse.RemoveProperty("loaded");
                            uresponse.RemoveProperty("total");
                            uresponse.RemoveProperty("lengthComputable");

                            uresponse["responseText"] = content.ToString();
                        });
                    }

                    await webcontrol.Dispatcher.InvokeAsync(() =>
                    {
                        uresponse["readyState"] = (ushort)ReadyState.DONE;

                        if (obj.HasMethod("onreadystatechange"))
                            obj.Invoke("onreadystatechange", uresponse);
                    });

                    token.ThrowIfCancellationRequested();

                    await webcontrol.Dispatcher.InvokeAsync(() =>
                    {
                        if (obj.HasMethod("onload"))
                            obj.Invoke("onload", uresponse);
                    });
                },
                token);
            }
            catch (OperationCanceledException)
            {
                if (aborted && obj.HasMethod("onabort"))
                    obj.Invoke("onabort", uresponse);

                else if (!aborted && obj.HasMethod("ontimeout"))
                    obj.Invoke("ontimeout", uresponse);
            }
            catch (Exception)
            {
                if (response != null)
                {

                    uresponse["status"] = (uint)404;
                    uresponse["statusText"] = "Page not found";

                    if (obj.HasMethod("onerror"))
                        obj.Invoke("onerror", uresponse);
                }
            }
            finally
            {
                if (response != null)
                    response.Dispose();

                if (taskSource != null)
                    taskSource.Dispose();

                uresponse.Dispose();
                obj.Dispose();
            }
        }

        private static void ApplyRequestHeaders(HttpWebRequest request, JSObject headers)
        {

            string[] names = headers.GetPropertyNames();
            names.ToList().ForEach((s) =>
            {
                switch (s.ToLower())
                {
                    case "accept":
                        request.Accept = headers[s];
                        break;
                    case "connection":
                        string connection = ((string)headers[s]).ToLower();

                        if (connection == "keep-alive")
                            request.KeepAlive = true;

                        else if (connection == "close")
                            request.KeepAlive = false;

                        else request.Connection = (string)headers[s];
                        break;
                    case "content-length":
                        long length = 0;
                        if (long.TryParse(headers[s], out length))
                            request.ContentLength = length;
                        break;
                    case "content-type":
                        request.ContentType = headers[s];
                        break;
                    case "date":
                        {
                            DateTime time;
                            if (DateTime.TryParse(headers[s], null, DateTimeStyles.AssumeUniversal, out time))
                                request.Date = time;
                        }
                        break;
                    case "expect":
                        request.Expect = headers[s];
                        break;
                    case "if-modified-since":
                        {
                            DateTime time;
                            if (DateTime.TryParse(headers[s], null, DateTimeStyles.AssumeUniversal, out time))
                                request.IfModifiedSince = time;
                        }
                        break;
                    case "host":
                        request.Host = headers[s];
                        break;
                    case "referer":
                        request.Referer = headers[s];
                        break;
                    case "te":
                        request.TransferEncoding = headers[s];
                        break;
                    case "user-agent":
                        request.UserAgent = headers[s];
                        break;
                    default:
                        try { request.Headers[s] = headers[s]; }
                        catch { }
                        break;
                }
            });
        }

        private static JSObject CreateResponse(IWebView webView, String finalUri, JSObject context)
        {
            JSObject response = webView.ExecuteJavascriptWithResult("new Object()");

            response["status"] = (uint)404;
            response["statusText"] = "Page not found";
            response["readyState"] = (ushort)ReadyState.UNSENT;
            response["context"] =
            response["finalUrl"] = finalUri.ToString();

            return response;
        }
    }
}
