using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GameLauncher.Resources;

internal class FontWrapper
{
	internal static class UnsafeNativeMethods
	{
		[DllImport ("gdi32.dll")]
		public static extern IntPtr AddFontMemResourceEx (IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
	}

	private PrivateFontCollection mPrivateFontCollection;

	private Dictionary<string, int> mFontMapping;

	private static FontWrapper _instance = new FontWrapper ();

	public static FontWrapper Instance {
		[CompilerGenerated]
		get {
			return _instance;
		}
	}

	private FontWrapper ()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		mPrivateFontCollection = new PrivateFontCollection ();
		mFontMapping = new Dictionary<string, int> ();
	}

	public FontFamily GetFontFamily (string fontName)
	{
		if (mFontMapping.ContainsKey (fontName)) {
			return ((FontCollection)mPrivateFontCollection).Families [mFontMapping [fontName]];
		}
		int num = LoadEmbeddedFont (fontName);
		return ((FontCollection)mPrivateFontCollection).Families [num];
	}

	private int LoadEmbeddedFont (string fontName)
	{
		Stream manifestResourceStream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("GameLauncher.Fonts." + fontName);
		IntPtr intPtr = Marshal.AllocCoTaskMem ((int)manifestResourceStream.Length);
		byte[] array = new byte[manifestResourceStream.Length];
		manifestResourceStream.Read (array, 0, (int)manifestResourceStream.Length);
		Marshal.Copy (array, 0, intPtr, (int)manifestResourceStream.Length);
		uint pcFonts = 0u;
		UnsafeNativeMethods.AddFontMemResourceEx (intPtr, (uint)array.Length, IntPtr.Zero, ref pcFonts);
		mPrivateFontCollection.AddMemoryFont (intPtr, (int)manifestResourceStream.Length);
		manifestResourceStream.Close ();
		Marshal.FreeCoTaskMem (intPtr);
		mFontMapping.Add (fontName, ((FontCollection)mPrivateFontCollection).Families.Length - 1);
		return ((FontCollection)mPrivateFontCollection).Families.Length - 1;
	}
}
