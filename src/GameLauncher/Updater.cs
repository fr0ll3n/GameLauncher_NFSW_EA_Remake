using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using GameLauncherReborn;
using Newtonsoft.Json;

namespace GameLauncher;

internal class Updater
{
	internal static void checkForUpdate (object sender, EventArgs e)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		try {
			WebClientWithTimeout webClientWithTimeout = new WebClientWithTimeout ();
			Uri uri = new Uri ("http://nfsw.metonator.ct8.pl/checkUpdate.php?version=" + Application.ProductVersion);
			webClientWithTimeout.CancelAsync ();
			((WebClient)webClientWithTimeout).DownloadStringAsync (uri);
			webClientWithTimeout.DownloadStringCompleted += delegate(object obj5, DownloadStringCompletedEventArgs e2) {
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Invalid comparison between Unknown and I4
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				try {
					CheckVersion checkVersion = JsonConvert.DeserializeObject<CheckVersion> (e2.Result);
					if (checkVersion.update.info) {
						if ((int)MessageBox.Show ("An update is available. Do you wanna download it?\nYour version: " + Application.ProductVersion + "\nUpdated version: " + checkVersion.github_build, "GameLauncher", (MessageBoxButtons)4, (MessageBoxIcon)64) == 6) {
							new WebClientWithTimeout ().DownloadFile (checkVersion.update.download, "tempname.zip");
							Process.Start ("GameLauncherUpdater.exe", Process.GetCurrentProcess ().Id.ToString ());
						}
						return;
					}
					try {
						_ = ((Control)(Form)sender).Name == "mainScreen";
					} catch {
						MessageBox.Show ("Your launcher is up-to-date");
					}
				} catch {
					try {
						_ = ((Control)(Form)sender).Name == "mainScreen";
					} catch {
						MessageBox.Show ("Failed to check for update");
					}
				}
			};
		} catch {
			MessageBox.Show ("Failed to check for update");
		}
	}
}
