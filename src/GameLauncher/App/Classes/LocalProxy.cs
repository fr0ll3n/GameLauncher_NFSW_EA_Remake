using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace GameLauncher.App.Classes;

public static class LocalProxy
{
	private static HttpListener _listener;

	private static Thread _thread;

	private static string _realBase;

	private static volatile bool _running;

	public static string LastError = "";

	public static int Port { get; private set; }

	public static string GameUrl => "http://127.0.0.1:" + Port + "/nfsw/Engine.svc";

	public static bool Start (string realEngineSvcUrl)
	{
		Stop ();
		LastError = "";
		_realBase = (realEngineSvcUrl ?? "").TrimEnd (new char[1] { '/' });
		if (string.IsNullOrEmpty (_realBase)) {
			LastError = "Empty URL";
			return false;
		}
		try {
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ServicePointManager.ServerCertificateValidationCallback = (object _003Cp0_003E, X509Certificate _003Cp1_003E, X509Chain _003Cp2_003E, SslPolicyErrors _003Cp3_003E) => true;
			ServicePointManager.DefaultConnectionLimit = 100;
			ServicePointManager.Expect100Continue = false;
		} catch {
		}
		Random random = new Random ();
		for (int i = 0; i < 20; i++) {
			Port = 2017 + random.Next (0, 200);
			try {
				_listener = new HttpListener ();
				_listener.Prefixes.Add ("http://127.0.0.1:" + Port + "/");
				_listener.Start ();
				_running = true;
				_thread = new Thread (ListenLoop) {
					IsBackground = true,
					Name = "LocalProxy"
				};
				_thread.Start ();
				Log ("Proxy listening on " + GameUrl + " -> " + _realBase);
				return true;
			} catch (Exception ex) {
				LastError = ex.Message;
				try {
					if (_listener != null) {
						_listener.Close ();
					}
				} catch {
				}
				_listener = null;
			}
		}
		return false;
	}

	public static void Stop ()
	{
		_running = false;
		try {
			if (_listener != null) {
				_listener.Stop ();
				_listener.Close ();
			}
		} catch {
		}
		_listener = null;
	}

	private static void Log (string msg)
	{
		try {
			File.AppendAllText ("launcher.log", "[" + DateTime.Now.ToString ("HH:mm:ss") + "] [proxy] " + msg + Environment.NewLine);
		} catch {
		}
	}

	private static void ListenLoop ()
	{
		while (_running && _listener != null) {
			try {
				HttpListenerContext ctx = _listener.GetContext ();
				ThreadPool.QueueUserWorkItem (delegate {
					Handle (ctx);
				});
			} catch {
				if (!_running) {
					break;
				}
			}
		}
	}

	private static void Handle (HttpListenerContext ctx)
	{
		string text = "";
		try {
			string text2 = ctx.Request.Url.AbsolutePath ?? "";
			int num = text2.IndexOf ("/nfsw/Engine.svc", StringComparison.OrdinalIgnoreCase);
			string text3 = ((num >= 0) ? text2.Substring (num + "/nfsw/Engine.svc".Length) : ((!text2.StartsWith ("/Engine.svc", StringComparison.OrdinalIgnoreCase)) ? text2 : text2.Substring ("/Engine.svc".Length)));
			if (string.IsNullOrEmpty (text3)) {
				text3 = "/";
			}
			if (!text3.StartsWith ("/")) {
				text3 = "/" + text3;
			}
			text = _realBase + text3 + ctx.Request.Url.Query;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create (text);
			httpWebRequest.Method = ctx.Request.HttpMethod;
			httpWebRequest.Timeout = 120000;
			httpWebRequest.ReadWriteTimeout = 120000;
			httpWebRequest.AutomaticDecompression = DecompressionMethods.None;
			httpWebRequest.KeepAlive = false;
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.ProtocolVersion = HttpVersion.Version11;
			string host = new Uri (_realBase).Host;
			NameValueCollection headers = ctx.Request.Headers;
			string[] allKeys = headers.AllKeys;
			foreach (string text4 in allKeys) {
				if (string.IsNullOrEmpty (text4)) {
					continue;
				}
				switch (text4.ToLowerInvariant ()) {
				case "content-type":
					httpWebRequest.ContentType = headers [text4];
					break;
				case "user-agent":
					httpWebRequest.UserAgent = headers [text4];
					break;
				case "referer":
					try {
						httpWebRequest.Referer = headers [text4];
					} catch {
					}
					break;
				default:
					try {
						httpWebRequest.Headers [text4] = headers [text4];
					} catch {
					}
					break;
				case "connection":
				case "host":
				case "content-length":
				case "transfer-encoding":
				case "accept-encoding":
					break;
				}
			}
			try {
				httpWebRequest.Host = host;
			} catch {
			}
			if (string.IsNullOrEmpty (httpWebRequest.UserAgent)) {
				httpWebRequest.UserAgent = "Blackbox";
			}
			byte[] array = null;
			if (ctx.Request.HasEntityBody) {
				using MemoryStream memoryStream = new MemoryStream ();
				ctx.Request.InputStream.CopyTo (memoryStream);
				array = memoryStream.ToArray ();
			}
			if (array != null && array.Length != 0) {
				httpWebRequest.ContentLength = array.Length;
				using Stream stream = ((WebRequest)httpWebRequest).GetRequestStream ();
				stream.Write (array, 0, array.Length);
			} else if (ctx.Request.HttpMethod == "POST" || ctx.Request.HttpMethod == "PUT") {
				httpWebRequest.ContentLength = 0L;
			}
			Log (ctx.Request.HttpMethod + " " + text2 + " body=" + ((array != null) ? array.Length : 0) + " -> " + text);
			if (text2.IndexOf ("GetPermanentSession", StringComparison.OrdinalIgnoreCase) >= 0) {
				Log ("GetPermanentSession headers: " + (object)headers);
				if (array != null && array.Length != 0) {
					Log ("GetPermanentSession body: " + Encoding.UTF8.GetString (array, 0, Math.Min (array.Length, 400)));
				}
			}
			string contentType = "application/xml;charset=UTF-8";
			WebHeaderCollection webHeaderCollection = null;
			int statusCode;
			byte[] array2;
			try {
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
				try {
					statusCode = (int)httpWebResponse.StatusCode;
					if (!string.IsNullOrEmpty (httpWebResponse.ContentType)) {
						contentType = httpWebResponse.ContentType;
					}
					webHeaderCollection = httpWebResponse.Headers;
					using Stream stream2 = ((WebResponse)httpWebResponse).GetResponseStream ();
					using MemoryStream memoryStream2 = new MemoryStream ();
					stream2?.CopyTo (memoryStream2);
					array2 = memoryStream2.ToArray ();
				} finally {
					((IDisposable)httpWebResponse)?.Dispose ();
				}
			} catch (WebException ex) {
				if (ex.Response != null) {
					HttpWebResponse httpWebResponse2 = (HttpWebResponse)ex.Response;
					statusCode = (int)httpWebResponse2.StatusCode;
					if (!string.IsNullOrEmpty (httpWebResponse2.ContentType)) {
						contentType = httpWebResponse2.ContentType;
					}
					webHeaderCollection = httpWebResponse2.Headers;
					using (Stream stream3 = ((WebResponse)httpWebResponse2).GetResponseStream ()) {
						using MemoryStream memoryStream3 = new MemoryStream ();
						stream3?.CopyTo (memoryStream3);
						array2 = memoryStream3.ToArray ();
					}
					Log ("ERR " + statusCode + " " + text2 + " len=" + array2.Length);
					if (array2.Length != 0) {
						Log ("ERR body: " + Encoding.UTF8.GetString (array2, 0, Math.Min (array2.Length, 400)));
					}
				} else {
					statusCode = 502;
					array2 = Encoding.UTF8.GetBytes ("proxy error: " + ((Exception)(object)ex).Message);
					Log ("ERR " + text + " : " + ((Exception)(object)ex).Message);
				}
			}
			ctx.Response.StatusCode = statusCode;
			ctx.Response.ContentType = contentType;
			ctx.Response.SendChunked = false;
			ctx.Response.ContentLength64 = array2.Length;
			if (webHeaderCollection != null) {
				allKeys = webHeaderCollection.AllKeys;
				foreach (string text5 in allKeys) {
					switch (text5.ToLowerInvariant ()) {
					case "content-length":
					case "transfer-encoding":
					case "content-type":
					case "connection":
						continue;
					}
					try {
						ctx.Response.Headers [text5] = webHeaderCollection [text5];
					} catch {
					}
				}
			}
			if (array2.Length != 0) {
				ctx.Response.OutputStream.Write (array2, 0, array2.Length);
			}
			Log ("OK " + statusCode + " " + text2 + " len=" + array2.Length);
		} catch (Exception ex2) {
			Log ("ERR2: " + ex2.Message);
			try {
				byte[] bytes = Encoding.UTF8.GetBytes (ex2.Message);
				ctx.Response.StatusCode = 500;
				ctx.Response.SendChunked = false;
				ctx.Response.ContentLength64 = bytes.Length;
				ctx.Response.OutputStream.Write (bytes, 0, bytes.Length);
			} catch {
			}
		} finally {
			try {
				ctx.Response.OutputStream.Close ();
			} catch {
			}
		}
	}
}
