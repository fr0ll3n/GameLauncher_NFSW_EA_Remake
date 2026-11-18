using System;
using System.IO;

namespace GameLauncher.App.Classes;

public static class AntiCheat
{
	public static int process_id;

	public static bool ChecksEnabled;

	public static void EnableChecks ()
	{
		ChecksEnabled = true;
		try {
			File.AppendAllText ("launcher.log", "[" + DateTime.Now.ToString ("HH:mm:ss") + "] [anticheat] EnableChecks\n");
		} catch {
		}
	}

	public static void DisableChecks (bool external = false)
	{
		ChecksEnabled = false;
		try {
			File.AppendAllText ("launcher.log", "[" + DateTime.Now.ToString ("HH:mm:ss") + "] [anticheat] DisableChecks\n");
		} catch {
		}
	}

	public static int Get_Cheat_Status ()
	{
		return 0;
	}
}
