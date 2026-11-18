using System;
using System.Runtime.InteropServices;

namespace GameLauncher.Resources;

internal static class UnsafeNativeMethods
{
	[DllImport ("gdi32.dll")]
	public static extern IntPtr AddFontMemResourceEx (IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
}
