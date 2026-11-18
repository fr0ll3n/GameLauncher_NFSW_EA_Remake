using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace GameLauncher.App.Classes;

public static class ServerMods
{
	public class GetModInfo
	{
		public string basePath { get; set; }

		public string serverID { get; set; }
	}

	public class ModEntry
	{
		public string Name { get; set; }

		public string Checksum { get; set; }

		public long Size { get; set; }
	}

	public class ModIndex
	{
		public List<ModEntry> entries { get; set; }
	}

	public static string LastStatus = string.Empty;

	public static int LastTotal;

	public static int LastDownloaded;

	public static int LastSkipped;

	public static int LastFailed;

	public static void CleanupBeforeLaunch (string gameDir, Action<string> log)
	{
		EnableTls ();
		RemoveStrayJunk (gameDir, log);
		try {
			Directory.CreateDirectory (Path.Combine (gameDir, "GLOBAL", "EventData"));
			Directory.CreateDirectory (Path.Combine (gameDir, "CARS"));
		} catch {
		}
	}

	public static bool PrepareMods (string serverEngineUrl, string gameDir, Action<string> log)
	{
		if (log == null) {
			log = delegate {
			};
		}
		EnableTls ();
		LastDownloaded = (LastSkipped = (LastFailed = (LastTotal = 0)));
		try {
			if (string.IsNullOrEmpty (gameDir) || !Directory.Exists (gameDir)) {
				LastStatus = "Game folder missing";
				log (LastStatus);
				return false;
			}
			RemoveStrayJunk (gameDir, log);
			try {
				InstallLauncherModules (gameDir, log);
			} catch (Exception ex) {
				log ("ModNet modules: " + ex.Message);
			}
			string text = NormalizeEngineUrl (serverEngineUrl);
			if (string.IsNullOrEmpty (text)) {
				LastStatus = "No server URL for server mods";
				log (LastStatus);
				return true;
			}
			log ("GetModInfo from " + text);
			string text2 = null;
			try {
				text2 = HttpGet (text + "/Modding/GetModInfo");
			} catch (Exception ex2) {
				LastStatus = "No ModNet on server: " + ex2.Message;
				log (LastStatus);
				WriteLog (gameDir, LastStatus);
				return true;
			}
			WriteLog (gameDir, "GetModInfo raw: " + (text2 ?? string.Empty));
			if (string.IsNullOrWhiteSpace (text2) || !text2.TrimStart (Array.Empty<char> ()).StartsWith ("{")) {
				LastStatus = "Empty mod info";
				log (LastStatus);
				return true;
			}
			GetModInfo getModInfo = JsonConvert.DeserializeObject<GetModInfo> (text2);
			if (getModInfo == null || string.IsNullOrEmpty (getModInfo.basePath)) {
				LastStatus = "No mod basePath";
				log (LastStatus);
				return true;
			}
			string text3 = ((!string.IsNullOrEmpty (getModInfo.serverID)) ? getModInfo.serverID : "default");
			string text4 = Md5Hex (text3);
			string text5 = getModInfo.basePath.TrimEnd (new char[1] { '/' });
			string text6 = Path.Combine (gameDir, "MODS");
			string text7 = Path.Combine (text6, text4);
			Directory.CreateDirectory (text6);
			try {
				string[] array = new string[4] {
					text3,
					text3.ToLowerInvariant (),
					text3.ToUpperInvariant (),
					Md5Hex (text3.ToLowerInvariant ())
				};
				foreach (string text8 in array) {
					if (string.IsNullOrEmpty (text8) || string.Equals (text8, text4, StringComparison.Ordinal)) {
						continue;
					}
					string text9 = Path.Combine (text6, text8);
					if (Directory.Exists (text9)) {
						if (!Directory.Exists (text7)) {
							Directory.Move (text9, text7);
							log ("Migrated MODS\\" + text8 + " -> MODS\\" + text4);
						}
						break;
					}
				}
			} catch (Exception ex3) {
				log ("Mod folder migrate: " + ex3.Message);
			}
			Directory.CreateDirectory (text7);
			log ("Mod CDN: " + text5 + " serverID=" + text3 + " folder=MODS\\" + text4 + " (MD5 serverID)");
			WriteLog (gameDir, "Downloading mods into " + text7);
			ModIndex modIndex = null;
			try {
				string value = HttpGet (text5 + "/index.json");
				log ("Got index.json (in-memory, not written to MODS folder)");
				modIndex = JsonConvert.DeserializeObject<ModIndex> (value);
			} catch (Exception ex4) {
				LastStatus = "index.json failed: " + ex4.Message;
				log (LastStatus);
				WriteLog (gameDir, LastStatus);
				return true;
			}
			if (modIndex == null || modIndex.entries == null || modIndex.entries.Count == 0) {
				LastStatus = "No mod entries";
				log (LastStatus);
				return true;
			}
			LastTotal = modIndex.entries.Count;
			int num = 0;
			foreach (ModEntry entry in modIndex.entries) {
				num++;
				if (entry == null || string.IsNullOrEmpty (entry.Name)) {
					continue;
				}
				string fileName = Path.GetFileName (entry.Name.Replace ('/', Path.DirectorySeparatorChar));
				if (string.Equals (fileName, "index.json", StringComparison.OrdinalIgnoreCase) || string.Equals (fileName, "cars.json", StringComparison.OrdinalIgnoreCase) || string.Equals (fileName, "events.json", StringComparison.OrdinalIgnoreCase) || string.Equals (fileName, ".links", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith (".tmp", StringComparison.OrdinalIgnoreCase)) {
					log ("Skip non-mod entry: " + entry.Name);
					continue;
				}
				string text10 = Path.Combine (text7, entry.Name.Replace ('/', Path.DirectorySeparatorChar));
				bool flag = true;
				if (File.Exists (text10)) {
					try {
						if (!string.IsNullOrEmpty (entry.Checksum) && string.Equals (Sha1File (text10), entry.Checksum, StringComparison.OrdinalIgnoreCase)) {
							flag = false;
						} else if (entry.Size > 0 && new FileInfo (text10).Length == entry.Size) {
							flag = false;
						}
					} catch {
						flag = true;
					}
				}
				if (!flag) {
					LastSkipped++;
					if (num <= 8 || num % 25 == 0) {
						log ("OK " + entry.Name);
					}
					continue;
				}
				log ($"Mods [{num}/{LastTotal}] GET {entry.Name}");
				try {
					HttpDownload (text5 + "/" + entry.Name.Replace ("\\", "/"), text10);
					LastDownloaded++;
				} catch (Exception ex5) {
					LastFailed++;
					log ("Fail " + entry.Name + ": " + ex5.Message);
					WriteLog (gameDir, "Fail " + entry.Name + ": " + ex5.Message);
				}
			}
			try {
				string[] array2 = new string[6] { "index.json", "cars.json", "events.json", "cars.json.tmp", "events.json.tmp", ".links" };
				string[] array = array2;
				foreach (string text11 in array) {
					string path = Path.Combine (text7, text11);
					if (File.Exists (path)) {
						try {
							File.Delete (path);
							log ("Removed unexpected " + text11 + " from MODS folder");
						} catch {
						}
					}
				}
				array = array2;
				foreach (string text12 in array) {
					TryDelete (Path.Combine (gameDir, text12), log);
					TryDelete (Path.Combine (gameDir, "Data", text12), log);
					TryDelete (Path.Combine (gameDir, "CARS", text12), log);
				}
			} catch {
			}
			LastStatus = $"Mods: {LastDownloaded} new, {LastSkipped} cached, {LastFailed} failed / {LastTotal} -> {text7}";
			log (LastStatus);
			WriteLog (gameDir, LastStatus);
			return true;
		} catch (Exception ex6) {
			LastStatus = "Mod error: " + ex6.Message;
			log (LastStatus);
			try {
				WriteLog (gameDir, LastStatus + "\n" + ex6);
			} catch {
			}
			return true;
		}
	}

	private static void RemoveStrayJunk (string gameDir, Action<string> log)
	{
		if (string.IsNullOrEmpty (gameDir) || !Directory.Exists (gameDir)) {
			return;
		}
		string[] files;
		try {
			files = Directory.GetFiles (gameDir, "index.json", SearchOption.AllDirectories);
			for (int i = 0; i < files.Length; i++) {
				TryDelete (files [i], log);
			}
			files = Directory.GetFiles (gameDir, "cars.json", SearchOption.AllDirectories);
			for (int j = 0; j < files.Length; j++) {
				TryDelete (files [j], log);
			}
			files = Directory.GetFiles (gameDir, "events.json", SearchOption.AllDirectories);
			for (int k = 0; k < files.Length; k++) {
				TryDelete (files [k], log);
			}
			files = Directory.GetFiles (gameDir, "cars.json.tmp", SearchOption.AllDirectories);
			for (int l = 0; l < files.Length; l++) {
				TryDelete (files [l], log);
			}
			files = Directory.GetFiles (gameDir, "events.json.tmp", SearchOption.AllDirectories);
			for (int m = 0; m < files.Length; m++) {
				TryDelete (files [m], log);
			}
			files = Directory.GetFiles (gameDir, ".links", SearchOption.AllDirectories);
			for (int n = 0; n < files.Length; n++) {
				TryDelete (files [n], log);
			}
		} catch {
		}
		files = new string[6] { "index.json", "cars.json", "cars.json.tmp", "events.json", "events.json.tmp", ".links" };
		string[] array = files;
		foreach (string text in array) {
			TryDelete (Path.Combine (gameDir, text), log);
			TryDelete (Path.Combine (gameDir, "Data", text), log);
			TryDelete (Path.Combine (gameDir, "CARS", text), log);
		}
		try {
			files = Directory.GetFiles (gameDir, "*.tmp", SearchOption.TopDirectoryOnly);
			for (int num2 = 0; num2 < files.Length; num2++) {
				TryDelete (files [num2], log);
			}
		} catch {
		}
	}

	private static string Md5Hex (string s)
	{
		if (s == null) {
			s = "";
		}
		MD5 mD = MD5.Create ();
		try {
			byte[] array = mD.ComputeHash (Encoding.UTF8.GetBytes (s));
			StringBuilder stringBuilder = new StringBuilder (array.Length * 2);
			for (int i = 0; i < array.Length; i++) {
				stringBuilder.Append (array [i].ToString ("x2"));
			}
			return stringBuilder.ToString ();
		} finally {
			((IDisposable)mD)?.Dispose ();
		}
	}

	private static void RestoreVanillaCars (string gameDir, Action<string> log)
	{
	}

	private static void InstallLauncherModules (string gameDir, Action<string> log)
	{
		Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>> (HttpGet ("https://cdn.soapboxrace.world/launcher-modules/modules.json"));
		if (dictionary == null) {
			return;
		}
		string text = Path.Combine (gameDir, "scripts");
		Directory.CreateDirectory (text);
		foreach (KeyValuePair<string, string> item in dictionary) {
			string text2 = ((!item.Key.Equals ("global.ini", StringComparison.OrdinalIgnoreCase)) ? Path.Combine (gameDir, item.Key) : Path.Combine (text, item.Key));
			bool flag = true;
			if (File.Exists (text2) && !string.IsNullOrEmpty (item.Value)) {
				try {
					if (string.Equals (Sha256File (text2), item.Value, StringComparison.OrdinalIgnoreCase)) {
						flag = false;
					}
				} catch {
					flag = true;
				}
			}
			if (flag) {
				log ("ModNet module: " + item.Key);
				try {
					HttpDownload ("https://cdn.soapboxrace.world/launcher-modules/" + item.Key, text2);
				} catch (Exception ex) {
					log ("Module fail " + item.Key + ": " + ex.Message);
				}
			}
		}
	}

	private static string NormalizeEngineUrl (string url)
	{
		if (string.IsNullOrWhiteSpace (url)) {
			return null;
		}
		string text = url.Trim ();
		if ((text.IndexOf ("127.0.0.1", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf ("localhost", StringComparison.OrdinalIgnoreCase) >= 0) && text.IndexOf ("nightriderz", StringComparison.OrdinalIgnoreCase) >= 0) {
			return "https://horizon.nightriderz.world/Engine.svc";
		}
		if (text.IndexOf ("nightriderz", StringComparison.OrdinalIgnoreCase) >= 0 && text.IndexOf ("Engine.svc", StringComparison.OrdinalIgnoreCase) < 0) {
			text = text.TrimEnd (new char[1] { '/' }) + "/Engine.svc";
		}
		return text.TrimEnd (new char[1] { '/' });
	}

	private static void EnableTls ()
	{
		try {
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ServicePointManager.Expect100Continue = false;
		} catch {
		}
	}

	private static string HttpGet (string url)
	{
		HttpWebRequest obj = (HttpWebRequest)WebRequest.Create (url);
		obj.Method = "GET";
		obj.Timeout = 60000;
		obj.UserAgent = "SBRW Launcher 2.2.4 (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
		obj.Headers ["X-UserAgent"] = "GameLauncherReborn 2.2.4 WinForms (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
		HttpWebResponse httpWebResponse = (HttpWebResponse)obj.GetResponse ();
		try {
			using Stream stream = ((WebResponse)httpWebResponse).GetResponseStream ();
			using StreamReader streamReader = new StreamReader (stream);
			return streamReader.ReadToEnd ();
		} finally {
			((IDisposable)httpWebResponse)?.Dispose ();
		}
	}

	private static void HttpDownload (string url, string dest)
	{
		string directoryName = Path.GetDirectoryName (dest);
		if (!string.IsNullOrEmpty (directoryName)) {
			Directory.CreateDirectory (directoryName);
		}
		string text = dest + ".part";
		try {
			Path.Combine (Path.GetTempPath (), "nfsw_" + Guid.NewGuid ().ToString ("N") + ".part");
			text = dest + "." + Guid.NewGuid ().ToString ("N") + ".part";
		} catch {
		}
		try {
			HttpWebRequest obj2 = (HttpWebRequest)WebRequest.Create (url);
			obj2.Method = "GET";
			obj2.Timeout = 600000;
			obj2.ReadWriteTimeout = 600000;
			obj2.UserAgent = "SBRW Launcher 2.2.4 (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
			obj2.AutomaticDecompression = DecompressionMethods.None;
			HttpWebResponse httpWebResponse = (HttpWebResponse)obj2.GetResponse ();
			try {
				using Stream stream = ((WebResponse)httpWebResponse).GetResponseStream ();
				using FileStream fileStream = new FileStream (text, FileMode.Create, FileAccess.Write, FileShare.None, 81920);
				byte[] array = new byte[81920];
				int count;
				while ((count = stream.Read (array, 0, array.Length)) > 0) {
					fileStream.Write (array, 0, count);
				}
				fileStream.Flush (flushToDisk: true);
			} finally {
				((IDisposable)httpWebResponse)?.Dispose ();
			}
			if (File.Exists (dest)) {
				try {
					File.Delete (dest);
				} catch {
				}
			}
			try {
				File.Move (text, dest);
			} catch {
				File.Copy (text, dest, overwrite: true);
				try {
					File.Delete (text);
				} catch {
				}
			}
		} catch {
			try {
				if (File.Exists (text)) {
					File.Delete (text);
				}
			} catch {
			}
			throw;
		}
	}

	private static void TryDelete (string path, Action<string> log)
	{
		try {
			if (!File.Exists (path)) {
				return;
			}
			try {
				FileAttributes attributes = File.GetAttributes (path);
				if ((attributes & FileAttributes.ReadOnly) != 0) {
					File.SetAttributes (path, attributes & ~FileAttributes.ReadOnly);
				}
			} catch {
			}
			File.Delete (path);
			log?.Invoke ("Removed " + path);
		} catch (Exception ex) {
			try {
				log?.Invoke ("Could not remove " + path + ": " + ex.Message);
			} catch {
			}
		}
	}

	private static void WriteLog (string gameDir, string msg)
	{
		try {
			File.AppendAllText (Path.Combine (gameDir, "modsync.log"), DateTime.Now.ToString ("HH:mm:ss") + " " + msg + Environment.NewLine);
		} catch {
		}
		try {
			File.AppendAllText ("launcher.log", "MODS " + msg + Environment.NewLine);
		} catch {
		}
	}

	private static string Md5 (string input)
	{
		byte[] array = MD5.Create ().ComputeHash (Encoding.UTF8.GetBytes (input ?? string.Empty));
		StringBuilder stringBuilder = new StringBuilder ();
		byte[] array2 = array;
		foreach (byte b in array2) {
			stringBuilder.Append (b.ToString ("x2"));
		}
		return stringBuilder.ToString ();
	}

	private static string Sha1File (string path)
	{
		FileStream fileStream = File.OpenRead (path);
		byte[] array = ((HashAlgorithm)SHA1.Create ()).ComputeHash ((Stream)fileStream);
		StringBuilder stringBuilder = new StringBuilder ();
		byte[] array2 = array;
		foreach (byte b in array2) {
			stringBuilder.Append (b.ToString ("x2"));
		}
		return stringBuilder.ToString ();
	}

	private static string Sha256File (string path)
	{
		FileStream fileStream = File.OpenRead (path);
		byte[] array = ((HashAlgorithm)SHA256.Create ()).ComputeHash ((Stream)fileStream);
		StringBuilder stringBuilder = new StringBuilder ();
		byte[] array2 = array;
		foreach (byte b in array2) {
			stringBuilder.Append (b.ToString ("x2"));
		}
		return stringBuilder.ToString ();
	}

	private static void LinkModFolders (string gameDir, Action<string> log)
	{
	}

	private static void MakeJunction (string sourceDir, string destDir, Action<string> log)
	{
		try {
			if (!Directory.Exists (sourceDir)) {
				return;
			}
			string directoryName = Path.GetDirectoryName (destDir);
			if (!string.IsNullOrEmpty (directoryName)) {
				Directory.CreateDirectory (directoryName);
			}
			if (Directory.Exists (destDir)) {
				if ((File.GetAttributes (destDir) & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint) {
					return;
				}
				string text = destDir + ".orig";
				if (Directory.Exists (text)) {
					return;
				}
				Directory.Move (destDir, text);
			}
			ProcessStartInfo processStartInfo = new ProcessStartInfo ();
			processStartInfo.FileName = "cmd.exe";
			processStartInfo.Arguments = "/c mklink /J \"" + destDir + "\" \"" + sourceDir + "\"";
			processStartInfo.CreateNoWindow = true;
			processStartInfo.UseShellExecute = false;
			processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			Process.Start (processStartInfo)?.WaitForExit (8000);
			log?.Invoke ("Junction " + destDir);
		} catch (Exception ex) {
			log?.Invoke ("Junction fail " + destDir + ": " + ex.Message);
		}
	}
}
