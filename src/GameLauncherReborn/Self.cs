using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace GameLauncherReborn;

internal class Self
{
	public static void runAsAdmin ()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		string[] commandLineArgs = Environment.GetCommandLineArgs ();
		ProcessStartInfo processStartInfo = new ProcessStartInfo {
			Verb = "runas",
			FileName = Application.ExecutablePath
		};
		if (commandLineArgs.Length != 0) {
			processStartInfo.Arguments = commandLineArgs [0];
		}
		try {
			Process.Start (processStartInfo);
		} catch (Exception ex) {
			MessageBox.Show ("Failed to self-run as admin: " + ex);
		}
	}

	public static void centerScreen (Form form)
	{
		form.StartPosition = (FormStartPosition)0;
		((Control)form).Top = (Screen.PrimaryScreen.Bounds.Height - ((Control)form).Height) / 2;
		((Control)form).Left = (Screen.PrimaryScreen.Bounds.Width - ((Control)form).Width) / 2;
	}
}
