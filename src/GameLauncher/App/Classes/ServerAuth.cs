using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameLauncher.App.Classes;

public static class ServerAuth
{
	public static string LastError = string.Empty;

	public static string LastRaw = string.Empty;

	private const string SbrwVersion = "2.2.4.0";

	private static readonly string UA = "SBRW Launcher 2.2.4.0 (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";

	private static readonly string XUA = "GameLauncherReborn 2.2.4.0 WinForms (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";

	private static string _launcherHash;

	private static string LauncherHash {
		get {
			if (_launcherHash != null) {
				return _launcherHash;
			}
			try {
				string location = Assembly.GetExecutingAssembly ().Location;
				if (!string.IsNullOrEmpty (location) && File.Exists (location)) {
					SHA1 sHA = SHA1.Create ();
					try {
						using FileStream fileStream = File.OpenRead (location);
						byte[] array = ((HashAlgorithm)sHA).ComputeHash ((Stream)fileStream);
						StringBuilder stringBuilder = new StringBuilder (array.Length * 2);
						for (int i = 0; i < array.Length; i++) {
							stringBuilder.Append (array [i].ToString ("x2"));
						}
						_launcherHash = stringBuilder.ToString ().ToUpperInvariant ();
						return _launcherHash;
					} finally {
						((IDisposable)sHA)?.Dispose ();
					}
				}
			} catch {
			}
			_launcherHash = "0000000000000000000000000000000000000000";
			return _launcherHash;
		}
	}

	private static void ApplySbrwHeaders (HttpWebRequest req)
	{
		req.UserAgent = UA;
		req.Headers ["X-UserAgent"] = XUA;
		req.Headers ["X-HWID"] = GetHwid ();
		req.Headers ["X-HiddenHWID"] = GetHwidAlt ();
		req.Headers ["X-GameLauncherHash"] = LauncherHash;
		req.Headers ["X-GameLauncherCertificate"] = "ClassicEA";
		req.Headers ["X-DiscordID"] = "0";
		req.Headers ["X-Electron"] = "false";
	}

	private static string GetHwid ()
	{
		try {
			string s = Environment.MachineName + "|" + Environment.UserName + "|" + Environment.ProcessorCount;
			SHA1 sHA = SHA1.Create ();
			try {
				byte[] array = sHA.ComputeHash (Encoding.UTF8.GetBytes (s));
				StringBuilder stringBuilder = new StringBuilder ();
				for (int i = 0; i < array.Length; i++) {
					stringBuilder.Append (array [i].ToString ("x2"));
				}
				return stringBuilder.ToString ().ToUpperInvariant ();
			} finally {
				((IDisposable)sHA)?.Dispose ();
			}
		} catch {
			return "UNKNOWNHWID";
		}
	}

	private static string GetHwidAlt ()
	{
		try {
			string s = "ALT|" + Environment.OSVersion?.ToString () + "|" + Environment.SystemDirectory;
			SHA1 sHA = SHA1.Create ();
			try {
				byte[] array = sHA.ComputeHash (Encoding.UTF8.GetBytes (s));
				StringBuilder stringBuilder = new StringBuilder ();
				for (int i = 0; i < array.Length; i++) {
					stringBuilder.Append (array [i].ToString ("x2"));
				}
				return stringBuilder.ToString ().ToUpperInvariant ();
			} finally {
				((IDisposable)sHA)?.Dispose ();
			}
		} catch {
			return "UNKNOWNALTHWID";
		}
	}

	public static string Authenticate (string serverEngineUrl, string email, string plainPassword)
	{
		LastError = string.Empty;
		LastRaw = string.Empty;
		try {
			if (string.IsNullOrEmpty (serverEngineUrl)) {
				return Fail ("No server URL");
			}
			string text = serverEngineUrl.TrimEnd (new char[1] { '/' });
			string text2 = plainPassword ?? "";
			string text3 = (email ?? "").Trim ();
			if (string.IsNullOrEmpty (text3) || string.IsNullOrEmpty (text2)) {
				return Fail ("Invalid E-mail or Password");
			}
			bool flag = text.ToLowerInvariant ().IndexOf ("nightriderz") >= 0;
			bool flag2 = text.ToLowerInvariant ().IndexOf ("worldunited") >= 0;
			Log ("Auth start -> " + text + " email=" + text3 + " passLen=" + text2.Length + " nrz=" + flag + " wu=" + flag2);
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ServicePointManager.ServerCertificateValidationCallback = (object a, X509Certificate b, X509Chain c, SslPolicyErrors d) => true;
			if (flag) {
				string text4 = TryModern (text, text3, text2, lowerKeys: true);
				if (IsOk (text4)) {
					Log ("Auth OK (NRZ lowercase)");
					return text4;
				}
				text4 = TryModern (text, text3, text2, lowerKeys: true, includeUpgrade: false);
				if (IsOk (text4)) {
					Log ("Auth OK (NRZ no upgrade)");
					return text4;
				}
				if (string.IsNullOrEmpty (LastError)) {
					LastError = "Invalid E-mail or Password";
				}
				Log ("Auth FAIL NRZ: " + LastError + " raw=" + Trunc (LastRaw));
				return Fail (LastError);
			}
			if (flag2) {
				try {
					string text5 = TryClassic (text, text3, Sha1 (text2));
					if (IsOk (text5)) {
						Log ("Auth OK (WU classic)");
						return text5;
					}
					if (IsWrongLauncher (LastRaw) || IsWrongLauncher (text5)) {
						LastError = "WorldUnited requires the official SBRW Launcher.\nDownload: https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases\n\nNightRiderz works with this Classic EA launcher.";
						Log ("Auth FAIL WU: wrong launcher blocked");
						return Fail (LastError);
					}
				} catch (Exception ex) {
					Log ("WU classic exception: " + ex.Message);
					if (IsWrongLauncher (LastRaw)) {
						LastError = "WorldUnited requires the official SBRW Launcher.\nDownload: https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases";
						return Fail (LastError);
					}
				}
				if (string.IsNullOrEmpty (LastError) || LastError.IndexOf ("401") >= 0) {
					LastError = "Invalid E-mail or Password";
				}
				Log ("Auth FAIL WU: " + LastError + " raw=" + Trunc (LastRaw));
				return Fail (LastError);
			}
			string text6 = TryModern (text, text3, text2, lowerKeys: true);
			if (IsOk (text6)) {
				Log ("Auth OK (lowercase)");
				return text6;
			}
			text6 = TryModern (text, text3, text2, lowerKeys: false);
			if (IsOk (text6)) {
				Log ("Auth OK (capital)");
				return text6;
			}
			try {
				text6 = TryClassic (text, text3, Sha1 (text2));
				if (IsOk (text6)) {
					Log ("Auth OK (classic)");
					return text6;
				}
			} catch (Exception ex2) {
				Log ("Classic exception: " + ex2.Message);
			}
			if (string.IsNullOrEmpty (LastError)) {
				LastError = "Invalid E-mail or Password";
			}
			Log ("Auth FAIL: " + LastError + " raw=" + Trunc (LastRaw));
			return Fail (LastError);
		} catch (Exception ex3) {
			Log ("Auth exception: " + ex3.Message);
			return Fail (ex3.Message);
		}
	}

	private static bool IsWrongLauncher (string body)
	{
		if (string.IsNullOrEmpty (body)) {
			return false;
		}
		if (body.IndexOf ("wrong launcher", StringComparison.OrdinalIgnoreCase) < 0) {
			return body.IndexOf ("git.io/Download_NFSW", StringComparison.OrdinalIgnoreCase) >= 0;
		}
		return true;
	}

	private static bool IsOk (string xmlOrJson)
	{
		if (string.IsNullOrEmpty (xmlOrJson)) {
			return false;
		}
		if (xmlOrJson.IndexOf ("<UserId>0</UserId>", StringComparison.OrdinalIgnoreCase) >= 0) {
			return false;
		}
		if (xmlOrJson.IndexOf ("\"userId\":0", StringComparison.OrdinalIgnoreCase) >= 0) {
			return false;
		}
		if (IsWrongLauncher (xmlOrJson)) {
			return false;
		}
		if (xmlOrJson.IndexOf ("<LoginToken>", StringComparison.OrdinalIgnoreCase) >= 0 && xmlOrJson.IndexOf ("<UserId>", StringComparison.OrdinalIgnoreCase) >= 0 && xmlOrJson.IndexOf ("<UserId>0</UserId>", StringComparison.OrdinalIgnoreCase) < 0) {
			return true;
		}
		try {
			if (xmlOrJson.TrimStart (Array.Empty<char> ()).StartsWith ("{")) {
				JObject jObject = JObject.Parse (xmlOrJson);
				string value = TokenToString (jObject ["token"] ?? jObject ["Token"]);
				string text = TokenToString (jObject ["userId"] ?? jObject ["UserId"]);
				return !string.IsNullOrEmpty (value) && !string.IsNullOrEmpty (text) && text != "0";
			}
		} catch {
		}
		return false;
	}

	private static string TryModern (string baseUrl, string email, string password, bool lowerKeys, bool includeUpgrade = true)
	{
		try {
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create (baseUrl + "/User/modernAuth");
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Timeout = 30000;
			ApplySbrwHeaders (httpWebRequest);
			string s = ((!lowerKeys) ? (includeUpgrade ? JsonConvert.SerializeObject (new {
				Email = email,
				Password = password,
				upgrade = true
			}) : JsonConvert.SerializeObject (new {
				Email = email,
				Password = password
			})) : (includeUpgrade ? JsonConvert.SerializeObject (new {
				email = email,
				password = password,
				upgrade = true
			}) : JsonConvert.SerializeObject (new { email, password })));
			byte[] bytes = Encoding.UTF8.GetBytes (s);
			httpWebRequest.ContentLength = bytes.Length;
			using (Stream stream = ((WebRequest)httpWebRequest).GetRequestStream ()) {
				stream.Write (bytes, 0, bytes.Length);
			}
			int statusCode;
			string text;
			try {
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
				try {
					using StreamReader streamReader = new StreamReader (((WebResponse)httpWebResponse).GetResponseStream ());
					statusCode = (int)httpWebResponse.StatusCode;
					text = streamReader.ReadToEnd ();
				} finally {
					((IDisposable)httpWebResponse)?.Dispose ();
				}
			} catch (WebException ex) {
				if (!(ex.Response is HttpWebResponse httpWebResponse2)) {
					Log ("TryModern exception: " + ((Exception)(object)ex).Message);
					return null;
				}
				statusCode = (int)httpWebResponse2.StatusCode;
				using StreamReader streamReader2 = new StreamReader (((WebResponse)httpWebResponse2).GetResponseStream ());
				text = streamReader2.ReadToEnd ();
			}
			LastRaw = text;
			Log ("modernAuth keys=" + (lowerKeys ? "lower" : "capital") + " upgrade=" + includeUpgrade + " status=" + statusCode + " body=" + Trunc (text));
			if (statusCode >= 200 && statusCode < 300) {
				string text2 = ParseModernBody (text);
				if (IsOk (text2)) {
					return text2;
				}
			}
			TryParseError (text);
			return text;
		} catch (Exception ex2) {
			Log ("TryModern exception: " + ex2.Message);
			return null;
		}
	}

	private static string ParseModernBody (string body)
	{
		try {
			JObject jObject = JObject.Parse (body);
			string text = TokenToString (jObject ["userId"] ?? jObject ["UserId"]);
			string text2 = TokenToString (jObject ["token"] ?? jObject ["Token"]);
			Log ("ParseModern userId=" + text + " tokenLen=" + (text2?.Length ?? 0));
			if (string.IsNullOrEmpty (text) || string.IsNullOrEmpty (text2)) {
				LastError = "Invalid E-mail or Password";
				return Fail (LastError);
			}
			return "<?xml version=\"1.0\"?><LoginStatusVO><UserId>" + XmlEsc (text) + "</UserId><LoginToken>" + XmlEsc (text2) + "</LoginToken><Description/></LoginStatusVO>";
		} catch {
			LastError = "Invalid E-mail or Password";
			return Fail (LastError);
		}
	}

	private static void TryParseError (string body)
	{
		try {
			if (string.IsNullOrEmpty (body)) {
				return;
			}
			if (IsWrongLauncher (body)) {
				LastError = "Wrong launcher blocked by server";
			} else if (body.TrimStart (Array.Empty<char> ()).StartsWith ("{")) {
				JObject jObject = JObject.Parse (body);
				string text = TokenToString (jObject ["error"] ?? jObject ["Error"] ?? jObject ["description"] ?? jObject ["Description"]);
				if (!string.IsNullOrEmpty (text)) {
					if (text.IndexOf ("email", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf ("password", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf ("invalid", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf ("unauthorized", StringComparison.OrdinalIgnoreCase) >= 0) {
						LastError = "Invalid E-mail or Password";
					} else {
						LastError = text;
					}
				}
			} else {
				if (body.IndexOf ("<Description>", StringComparison.OrdinalIgnoreCase) < 0) {
					return;
				}
				int num = body.IndexOf ("<Description>", StringComparison.OrdinalIgnoreCase) + 13;
				int num2 = body.IndexOf ("</Description>", StringComparison.OrdinalIgnoreCase);
				if (num2 > num) {
					string text2 = body.Substring (num, num2 - num).Trim ();
					if (IsWrongLauncher (text2)) {
						LastError = "Wrong launcher blocked by server";
					} else if (!string.IsNullOrEmpty (text2)) {
						LastError = text2;
					}
				}
			}
		} catch {
		}
	}

	private static string TryClassic (string baseUrl, string email, string sha1Password)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create (baseUrl + "/User/authenticateUser?email=" + Uri.EscapeDataString (email) + "&password=" + Uri.EscapeDataString (sha1Password));
		httpWebRequest.Method = "GET";
		httpWebRequest.Timeout = 30000;
		ApplySbrwHeaders (httpWebRequest);
		try {
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
			try {
				using StreamReader streamReader = new StreamReader (((WebResponse)httpWebResponse).GetResponseStream ());
				string text = (LastRaw = streamReader.ReadToEnd ());
				Log ("classic status=200 body=" + Trunc (text));
				return text;
			} finally {
				((IDisposable)httpWebResponse)?.Dispose ();
			}
		} catch (WebException ex) {
			if (ex.Response is HttpWebResponse httpWebResponse2) {
				using StreamReader streamReader2 = new StreamReader (((WebResponse)httpWebResponse2).GetResponseStream ());
				string text2 = (LastRaw = streamReader2.ReadToEnd ());
				Log ("classic status=" + (int)httpWebResponse2.StatusCode + " body=" + Trunc (text2));
				TryParseError (text2);
				return text2;
			}
			throw;
		}
	}

	private static string Sha1 (string s)
	{
		SHA1 sHA = SHA1.Create ();
		try {
			byte[] array = sHA.ComputeHash (Encoding.UTF8.GetBytes (s ?? ""));
			StringBuilder stringBuilder = new StringBuilder ();
			byte[] array2 = array;
			foreach (byte b in array2) {
				stringBuilder.Append (b.ToString ("x2"));
			}
			return stringBuilder.ToString ();
		} finally {
			((IDisposable)sHA)?.Dispose ();
		}
	}

	private static string TokenToString (JToken tok)
	{
		if (tok == null || tok.Type == JTokenType.Null) {
			return null;
		}
		string text = tok.ToString ();
		if (string.IsNullOrWhiteSpace (text) || text == "null") {
			return null;
		}
		return text.Trim ();
	}

	private static string XmlEsc (string s)
	{
		return (s ?? "").Replace ("&", "&amp;").Replace ("<", "&lt;").Replace (">", "&gt;");
	}

	private static string Trunc (string s)
	{
		if (string.IsNullOrEmpty (s)) {
			return "";
		}
		s = s.Replace ("\r", " ").Replace ("\n", " ");
		if (s.Length <= 300) {
			return s;
		}
		return s.Substring (0, 300) + "...";
	}

	private static void Log (string msg)
	{
		try {
			File.AppendAllText ("launcher.log", DateTime.Now.ToString ("HH:mm:ss") + " [auth] " + msg + Environment.NewLine);
		} catch {
		}
	}

	private static string Fail (string d)
	{
		LastError = d ?? "Invalid E-mail or Password";
		return "<?xml version=\"1.0\"?><LoginStatusVO><UserId>0</UserId><LoginToken></LoginToken><Description>" + XmlEsc (LastError) + "</Description></LoginStatusVO>";
	}
}
