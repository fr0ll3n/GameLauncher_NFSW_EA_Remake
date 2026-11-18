using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace GameLauncher;

[Serializable]
[CompilerGenerated]
private sealed class _003C_003Ec
{
	public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec ();

	public static EventHandler<CoreWebView2NewWindowRequestedEventArgs> _003C_003E9__134_0;

	public static CancelEventHandler _003C_003E9__195_0;

	public static WebBrowserNavigatingEventHandler _003C_003E9__195_1;

	internal void _003CInitPromoWebView2_003Eb__134_0 (object s, CoreWebView2NewWindowRequestedEventArgs e)
	{
		try {
			e.Handled = true;
			string text = e.Uri ?? "";
			string text2 = text.ToLowerInvariant ();
			if (!text2.Contains ("nightriderz") && !text2.Contains ("worldunited") && !text2.Contains ("sbrw") && !string.IsNullOrEmpty (text)) {
				Process.Start (text);
			}
		} catch {
		}
	}

	internal void _003CInitializeComponent_003Eb__195_0 (object s, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	internal void _003CInitializeComponent_003Eb__195_1 (object s, WebBrowserNavigatingEventArgs e)
	{
	}
}
