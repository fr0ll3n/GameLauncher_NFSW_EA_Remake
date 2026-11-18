using System;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace GameLauncher.App.Classes;

public static class LauncherUpdaterCheck
{
	public static void CheckAsync (Action<string> log = null)
	{
		ThreadPool.QueueUserWorkItem (delegate {
			try {
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				HttpWebRequest obj = (HttpWebRequest)WebRequest.Create ("https://api.github.com/repos/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest");
				obj.UserAgent = "ClassicEA-Launcher";
				obj.Timeout = 10000;
				HttpWebResponse httpWebResponse = (HttpWebResponse)obj.GetResponse ();
				try {
					using StreamReader streamReader = new StreamReader (((WebResponse)httpWebResponse).GetResponseStream ());
					dynamic val = JsonConvert.DeserializeObject (streamReader.ReadToEnd ());
					string text = val.tag_name;
					log?.Invoke ("Official SBRW latest release: " + text);
				} finally {
					((IDisposable)httpWebResponse)?.Dispose ();
				}
			} catch (Exception ex) {
				log?.Invoke ("Update check: " + ex.Message);
			}
		});
	}

	public static void EnableLargeAddressAware (string exePath, Action<string> log = null)
	{
		try {
			if (string.IsNullOrEmpty (exePath) || !File.Exists (exePath)) {
				return;
			}
			byte[] array = File.ReadAllBytes (exePath);
			if (array.Length < 64 || array [0] != 77 || array [1] != 90) {
				return;
			}
			int num = BitConverter.ToInt32 (array, 60);
			if (num <= 0 || num + 24 >= array.Length || array [num] != 80 || array [num + 1] != 69) {
				return;
			}
			int num2 = num + 22;
			ushort num3 = BitConverter.ToUInt16 (array, num2);
			if ((num3 & 0x20u) != 0) {
				log?.Invoke ("nfsw.exe already Large Address Aware");
				return;
			}
			num3 = (ushort)(num3 | 0x20u);
			array [num2] = (byte)(num3 & 0xFFu);
			array [num2 + 1] = (byte)((uint)(num3 >> 8) & 0xFFu);
			try {
				File.Copy (exePath, exePath + ".bak_laa", overwrite: false);
			} catch {
			}
			File.WriteAllBytes (exePath, array);
			log?.Invoke ("Enabled Large Address Aware on nfsw.exe");
		} catch (Exception ex) {
			log?.Invoke ("LAA patch: " + ex.Message);
		}
	}
}
