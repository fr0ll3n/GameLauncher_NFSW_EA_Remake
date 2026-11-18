using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace GameLauncher.Properties;

[GeneratedCode ("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable (EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager {
		get {
			if (resourceMan == null) {
				resourceMan = new ResourceManager ("GameLauncher.Properties.Resources", typeof(Resources).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable (EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture {
		get {
			return resourceCulture;
		}
		set {
			resourceCulture = value;
		}
	}

	internal static Bitmap aboutbg => (Bitmap)ResourceManager.GetObject ("aboutbg", resourceCulture);

	internal static Bitmap button_click => (Bitmap)ResourceManager.GetObject ("button_click", resourceCulture);

	internal static Bitmap button_disable => (Bitmap)ResourceManager.GetObject ("button_disable", resourceCulture);

	internal static Bitmap button_enable => (Bitmap)ResourceManager.GetObject ("button_enable", resourceCulture);

	internal static Bitmap button_hover => (Bitmap)ResourceManager.GetObject ("button_hover", resourceCulture);

	internal static Bitmap close => (Bitmap)ResourceManager.GetObject ("close", resourceCulture);

	internal static Bitmap close_click => (Bitmap)ResourceManager.GetObject ("close_click", resourceCulture);

	internal static Bitmap close_hover => (Bitmap)ResourceManager.GetObject ("close_hover", resourceCulture);

	internal static Bitmap loginbg => (Bitmap)ResourceManager.GetObject ("loginbg", resourceCulture);

	internal static Bitmap minimize => (Bitmap)ResourceManager.GetObject ("minimize", resourceCulture);

	internal static Bitmap minimize_click => (Bitmap)ResourceManager.GetObject ("minimize_click", resourceCulture);

	internal static Bitmap minimize_hover => (Bitmap)ResourceManager.GetObject ("minimize_hover", resourceCulture);

	internal static Bitmap playbg => (Bitmap)ResourceManager.GetObject ("playbg", resourceCulture);

	internal static Bitmap playButton_click => (Bitmap)ResourceManager.GetObject ("playButton_click", resourceCulture);

	internal static Bitmap playButton_disable => (Bitmap)ResourceManager.GetObject ("playButton_disable", resourceCulture);

	internal static Bitmap playButton_enable => (Bitmap)ResourceManager.GetObject ("playButton_enable", resourceCulture);

	internal static Bitmap playButton_hover => (Bitmap)ResourceManager.GetObject ("playButton_hover", resourceCulture);

	internal static Bitmap registerbg => (Bitmap)ResourceManager.GetObject ("registerbg", resourceCulture);

	internal static Bitmap server_offline => (Bitmap)ResourceManager.GetObject ("server_offline", resourceCulture);

	internal static Bitmap server_online => (Bitmap)ResourceManager.GetObject ("server_online", resourceCulture);

	internal static Bitmap settingsbg => (Bitmap)ResourceManager.GetObject ("settingsbg", resourceCulture);

	internal static Bitmap settingsbtn => (Bitmap)ResourceManager.GetObject ("settingsbtn", resourceCulture);

	internal static Bitmap settingsbtn_click => (Bitmap)ResourceManager.GetObject ("settingsbtn_click", resourceCulture);

	internal static Bitmap settingsbtn_hover => (Bitmap)ResourceManager.GetObject ("settingsbtn_hover", resourceCulture);

	internal Resources ()
	{
	}
}
