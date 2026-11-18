using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes;

namespace GameLauncher;

internal static class Program
{
	[STAThread]
	private static void Main ()
	{
		//Discarded unreachable code: IL_01c7
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		try {
			try {
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				ServicePointManager.Expect100Continue = false;
				ServicePointManager.DefaultConnectionLimit = 16;
			} catch {
			}
			try {
				string path = Path.Combine (Directory.GetCurrentDirectory (), "tempname.zip");
				if (File.Exists (path)) {
					File.Delete (path);
				}
			} catch {
			}
			try {
				if (!File.Exists ("LZMA.dll")) {
					byte[] array = ExtractResource.AsByte ("GameLauncher.LZMA.LZMA.dll");
					if (array != null && array.Length != 0) {
						File.WriteAllBytes ("LZMA.dll", array);
					}
				}
			} catch {
			}
			try {
				if (!File.Exists ("GameLauncherUpdater.exe")) {
					byte[] array2 = ExtractResource.AsByte ("GameLauncher.Updater.GameLauncherUpdater.exe");
					if (array2 != null && array2.Length != 0) {
						File.WriteAllBytes ("GameLauncherUpdater.exe", array2);
					}
				}
			} catch {
			}
			if (!new Mutex (initiallyOwned: false, "GameLauncherNFSW-ClassicEA").WaitOne (0, exitContext: false)) {
				MessageBox.Show ("There is a NFS World Launcher running!", "NFS World", (MessageBoxButtons)0, (MessageBoxIcon)64);
				return;
			}
			string[] obj5 = new string[4] { "Microsoft.WindowsAPICodePack.dll", "Microsoft.WindowsAPICodePack.Shell.dll", "Newtonsoft.Json.dll", "ICSharpCode.SharpZipLib.dll" };
			List<string> list = new List<string> ();
			string[] array3 = obj5;
			foreach (string text in array3) {
				if (!File.Exists (text)) {
					list.Add (text);
				}
			}
			if (list.Count > 0) {
				MessageBox.Show ("Cannot start. Missing files:\n\n• " + string.Join ("\n• ", list.ToArray ()) + "\n\nFolder: " + Directory.GetCurrentDirectory (), "GameLauncher", (MessageBoxButtons)0, (MessageBoxIcon)16);
				return;
			}
			try {
				if (Environment.OSVersion.Version.Major >= 6) {
					User32.SetProcessDPIAware ();
				}
			} catch {
			}
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);
			try {
				IniFile iniFile = new IniFile (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Settings.ini"));
				_ = iniFile.Read ("AcceptedTos") != "1";
			} catch {
			}
			Application.Run ((Form)new mainScreen ());
		} catch (Exception ex) {
			try {
				MessageBox.Show ("Failed to start GameLauncher.\n\n" + ex.Message + "\n\n" + ex.GetType ().FullName + "\n\n" + ((ex.InnerException != null) ? (ex.InnerException.Message + "\n\n") : string.Empty) + (ex.StackTrace ?? string.Empty), "GameLauncher – Startup Error", (MessageBoxButtons)0, (MessageBoxIcon)16);
			} catch {
			}
		}
	}
}
