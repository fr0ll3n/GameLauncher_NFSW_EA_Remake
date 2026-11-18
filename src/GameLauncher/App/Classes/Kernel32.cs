using System.Runtime.InteropServices;

namespace GameLauncher.App.Classes;

internal static class Kernel32
{
	[DllImport ("kernel32", CharSet = CharSet.Auto)]
	public static extern int GetDiskFreeSpaceEx (string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);
}
