using System;
using System.Runtime.InteropServices;

namespace GameLauncher.App.Classes;

internal static class LZMA
{
	[DllImport ("LZMA.dll", CharSet = CharSet.Ansi)]
	public static extern int LzmaUncompress (byte[] dest, ref IntPtr destLen, byte[] src, ref IntPtr srcLen, byte[] outProps, IntPtr outPropsSize);

	[DllImport ("LZMA.dll", CharSet = CharSet.Ansi)]
	public static extern int LzmaUncompressBuf2File (string destFile, ref IntPtr destLen, byte[] src, ref IntPtr srcLen, byte[] outProps, IntPtr outPropsSize);
}
