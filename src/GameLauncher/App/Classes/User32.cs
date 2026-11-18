using System;
using System.Runtime.InteropServices;

namespace GameLauncher.App.Classes;

internal static class User32
{
	[DllImport ("user32.dll")]
	public static extern IntPtr LoadCursorFromFile (string filename);

	[DllImport ("user32.dll")]
	public static extern bool SetProcessDPIAware ();
}
