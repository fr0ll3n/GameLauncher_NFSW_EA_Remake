using System;
using System.Windows.Forms;

namespace GameLauncher.App.Classes;

internal class Delay
{
	public static void WaitSeconds (int sec)
	{
		if (sec >= 1) {
			DateTime dateTime = DateTime.Now.AddSeconds (sec);
			while (DateTime.Now < dateTime) {
				Application.DoEvents ();
			}
		}
	}
}
