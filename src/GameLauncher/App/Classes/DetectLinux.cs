using System;
using Microsoft.Win32;

namespace GameLauncher.App.Classes;

internal class DetectLinux
{
	public static bool WineDetected ()
	{
		bool result = false;
		try {
			if (!string.IsNullOrEmpty (((object)Registry.CurrentUser.OpenSubKey ("Software\\Wine")).ToString ())) {
				result = true;
			}
		} catch {
			result = false;
		}
		return result;
	}

	public static bool MonoDetected ()
	{
		int platform = (int)Environment.OSVersion.Platform;
		if (platform != 4 && platform != 6) {
			return platform == 128;
		}
		return true;
	}

	public static bool LinuxDetected ()
	{
		if (!WineDetected ()) {
			return MonoDetected ();
		}
		return true;
	}
}
