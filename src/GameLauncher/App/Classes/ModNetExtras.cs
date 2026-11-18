using System;
using System.Diagnostics;
using System.IO;

namespace GameLauncher.App.Classes;

public static class ModNetExtras
{
	public static void FileANDFolder (string gamePath, Action<string> log = null)
	{
		if (string.IsNullOrEmpty (gamePath) || !Directory.Exists (gamePath)) {
			return;
		}
		string[] array = new string[1] { "modules" };
		string[] array2 = new string[1] { "scripts" };
		string[] array3 = new string[9] { "modules/udpcrc.soapbox.module", "modules/udpcrypt1.soapbox.module", "modules/udpcrypt2.soapbox.module", "modules/xmppsubject.soapbox.module", "scripts/global.ini", "ModManager.dat", "PocoFoundation.dll", "PocoNet.dll", "lightfx.dll" };
		string[] array4 = array;
		foreach (string path in array4) {
			string path2 = Path.Combine (gamePath, path);
			try {
				if (Directory.Exists (path2)) {
					Directory.Delete (path2, recursive: true);
				}
			} catch {
			}
		}
		array4 = array2;
		foreach (string path3 in array4) {
			try {
				Directory.CreateDirectory (Path.Combine (gamePath, path3));
			} catch {
			}
		}
		array4 = array3;
		foreach (string path4 in array4) {
			string path5 = Path.Combine (gamePath, path4);
			try {
				if (File.Exists (path5)) {
					File.Delete (path5);
				}
			} catch {
			}
		}
		log?.Invoke ("ModNet FileANDFolder cleanup done");
	}

	public static void CleanLinks (string gamePath, Action<string> log = null)
	{
		string path = Path.Combine (gamePath, ".links");
		if (!File.Exists (path)) {
			return;
		}
		int num = 0;
		try {
			foreach (string item in File.ReadLines (path)) {
				string[] array = item.Split (new char[1] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length != 2) {
					continue;
				}
				string path2 = array [0];
				if (!int.TryParse (array [1], out var result)) {
					continue;
				}
				string text = Path.Combine (gamePath, path2);
				try {
					if (result == 0) {
						string text2 = text + ".orig";
						if (IsReparse (text) && File.Exists (text)) {
							File.Delete (text);
							continue;
						}
						if (!File.Exists (text)) {
							num++;
							if (File.Exists (text2)) {
								try {
									File.Delete (text2);
								} catch {
								}
							}
							continue;
						}
						if (!File.Exists (text2)) {
							try {
								File.Delete (text);
							} catch {
							}
							continue;
						}
						try {
							if (File.Exists (text)) {
								File.Delete (text);
							}
							File.Move (text2, text);
						} catch {
							num++;
						}
						continue;
					}
					if (IsReparse (text) && Directory.Exists (text)) {
						Directory.Delete (text, recursive: true);
						continue;
					}
					if (!Directory.Exists (text)) {
						num++;
						continue;
					}
					string text3 = text + ".orig";
					if (Directory.Exists (text3)) {
						try {
							Directory.Delete (text, recursive: true);
							Directory.Move (text3, text);
						} catch {
							num++;
						}
					}
				} catch {
					num++;
				}
			}
			try {
				File.Delete (path);
			} catch {
			}
			log?.Invoke ("CleanLinks done (errors=" + num + ")");
		} catch (Exception ex) {
			log?.Invoke ("CleanLinks: " + ex.Message);
		}
	}

	private static bool IsReparse (string path)
	{
		try {
			if (File.Exists (path) || Directory.Exists (path)) {
				return (File.GetAttributes (path) & FileAttributes.ReparsePoint) != 0;
			}
		} catch {
		}
		return false;
	}

	public static void EnsureFirewallRules (string launcherExe, string gameDir, int proxyPort, Action<string> log = null)
	{
		try {
			string text = Path.Combine (gameDir ?? "", "nfsw.exe");
			RunNetsh ("advfirewall firewall delete rule name=\"ClassicEA Launcher\"");
			RunNetsh ("advfirewall firewall delete rule name=\"ClassicEA NFSW\"");
			RunNetsh ("advfirewall firewall delete rule name=\"ClassicEA Proxy\"");
			if (!string.IsNullOrEmpty (launcherExe) && File.Exists (launcherExe)) {
				RunNetsh ("advfirewall firewall add rule name=\"ClassicEA Launcher\" dir=in action=allow program=\"" + launcherExe + "\" enable=yes");
			}
			if (File.Exists (text)) {
				RunNetsh ("advfirewall firewall add rule name=\"ClassicEA NFSW\" dir=in action=allow program=\"" + text + "\" enable=yes");
			}
			if (proxyPort > 0) {
				RunNetsh ("advfirewall firewall add rule name=\"ClassicEA Proxy\" dir=in action=allow protocol=TCP localport=" + proxyPort + " enable=yes");
			}
			log?.Invoke ("Firewall rules applied");
		} catch (Exception ex) {
			log?.Invoke ("Firewall: " + ex.Message);
		}
	}

	private static void RunNetsh (string args)
	{
		try {
			Process.Start (new ProcessStartInfo {
				FileName = "netsh",
				Arguments = args,
				CreateNoWindow = true,
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Hidden
			})?.WaitForExit (5000);
		} catch {
		}
	}
}
