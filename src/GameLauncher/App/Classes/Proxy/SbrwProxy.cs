using System;
using System.IO;
using Nancy.Hosting.Self;

namespace GameLauncher.App.Classes.Proxy;

public static class SbrwProxy
{
	private static NancyHost _host;

	public static string LastError = "";

	public static int ProxyPort { get; private set; }

	public static string ServerUrl { get; private set; }

	public static string GameUrl => "http://127.0.0.1:" + ProxyPort + "/nfsw/Engine.svc";

	public static bool Running => _host != null;

	public static bool Start (string realEngineSvcUrl, string serverName = "SBRW")
	{
		Stop ();
		LastError = "";
		ServerUrl = (realEngineSvcUrl ?? "").TrimEnd (new char[1] { '/' });
		if (string.IsNullOrEmpty (ServerUrl)) {
			LastError = "Empty server URL";
			return false;
		}
		Random random = new Random ();
		for (int i = 0; i < 15; i++) {
			ProxyPort = random.Next (2017, 2217);
			try {
				HostConfiguration configuration = new HostConfiguration {
					AllowChunkedEncoding = false,
					UrlReservations = new UrlReservations {
						CreateAutomatically = true
					}
				};
				_host = new NancyHost (new Uri ("http://127.0.0.1:" + ProxyPort), new ProxyBootstrapper (), configuration);
				_host.Start ();
				Log ("Nancy proxy on " + GameUrl + " -> " + ServerUrl);
				return true;
			} catch (Exception ex) {
				LastError = ex.Message;
				try {
					if (_host != null) {
						_host.Stop ();
						_host.Dispose ();
					}
				} catch {
				}
				_host = null;
			}
		}
		return false;
	}

	public static void Stop ()
	{
		try {
			if (_host != null) {
				_host.Stop ();
				_host.Dispose ();
			}
		} catch {
		}
		_host = null;
	}

	public static void Log (string msg)
	{
		try {
			File.AppendAllText ("launcher.log", "[" + DateTime.Now.ToString ("HH:mm:ss") + "] [proxy] " + msg + Environment.NewLine);
		} catch {
		}
	}
}
