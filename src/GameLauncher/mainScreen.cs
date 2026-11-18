using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Xml;
using GameLauncher.App.Classes;
using GameLauncher.App.Classes.Events;
using GameLauncher.App.Classes.Proxy;
using GameLauncher.Properties;
using GameLauncher.Resources;
using GameLauncherReborn;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoapBox.JsonScheme;

namespace GameLauncher;

public class mainScreen : Form
{
	private struct DevMode
	{
		[MarshalAs (UnmanagedType.ByValTStr, SizeConst = 32)]
		public string dmDeviceName;

		public short dmSpecVersion;

		public short dmDriverVersion;

		public short dmSize;

		public short dmDriverExtra;

		public int dmFields;

		public int dmPositionX;

		public int dmPositionY;

		public int dmDisplayOrientation;

		public int dmDisplayFixedOutput;

		public short dmColor;

		public short dmDuplex;

		public short dmYResolution;

		public short dmTTOption;

		public short dmCollate;

		[MarshalAs (UnmanagedType.ByValTStr, SizeConst = 32)]
		public string dmFormName;

		public short dmLogPixels;

		public int dmBitsPerPel;

		public int dmPelsWidth;

		public int dmPelsHeight;

		public int dmDisplayFlags;

		public int dmDisplayFrequency;

		public int dmICMMethod;

		public int dmICMIntent;

		public int dmMediaType;

		public int dmDitherType;

		public int dmReserved1;

		public int dmReserved2;

		public int dmPanningWidth;

		public int dmPanningHeight;
	}

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

	[Serializable]
	[CompilerGenerated]
	private sealed class <>c
	{
		public static readonly <>c <>9 = new <>c ();

		public static CancelEventHandler <>9__211_0;

		public static WebBrowserNavigatingEventHandler <>9__211_1;

		internal void <InitializeComponent>b__211_0 (object s, CancelEventArgs e)
		{
			e.Cancel = true;
		}

		internal void <InitializeComponent>b__211_1 (object <p0>, WebBrowserNavigatingEventArgs <p1>)
		{
		}
	}

	private Point mouseDownPoint = Point.Empty;

	private bool loginEnabled;

	private Image loginDisableImageGray;

	private bool serverEnabled;

	private bool builtinserver;

	private bool useSavedPassword;

	private bool skipServerTrigger;

	private bool ticketRequired;

	private bool serverlistloaded;

	private bool windowMoved;

	private bool playenabled;

	private static PrivateFontCollection eaPrivateFonts;

	private static FontFamily eaMyriadBold;

	private bool inPlayScreen;

	private Timer playBlinkTimer;

	private bool playBlinkOn;

	private bool playGlowPaused;

	private Image[] playGlowFrames;

	private int playGlowIndex;

	private int playGlowDir;

	private DateTime DownloadStartTime;

	private string LoginToken = string.Empty;

	private string UserId = string.Empty;

	private string plainPasswordMemory = string.Empty;

	private string serverIP = string.Empty;

	private string serverCacheKey = "18051995";

	private string langInfo;

	private float DPIDefaultScale = 96f;

	private IniFile SettingFile = new IniFile ("Settings.ini");

	private string UserSettings = Environment.ExpandEnvironmentVariables ("%AppData%\\Need for Speed World\\Settings\\UserSettings.xml");

	private IContainer components;

	private PictureBox closebtn;

	private PictureBox minimizebtn;

	private TextBox email;

	private TextBox password;

	private Label serverStatus;

	private PictureBox serverStatusImg;

	private ComboBox serverPick;

	private Label currentWindowInfo;

	private RichTextBox consoleLog;

	private PictureBox bottomBanner;

	private Label advertisementLabel;

	private CheckBox rememberMe;

	private Timer Timeout;

	private Label onlineCount;

	private Button loginButton;

	private LinkLabel registerText;

	private Label emailLabel;

	private Label passwordLabel;

	private Label troubleLabel;

	private LinkLabel githubLink;

	private Button registerButton;

	private Label selectServerLabel;

	private PictureBox settingsButton;

	private Panel moveWindow;

	private PictureBox verticalBanner;

	private Button settingsSave;

	private ComboBox settingsLanguage;

	private Label settingsLanguageText;

	private ComboBox settingsQuality;

	private Label settingsQualityText;

	private Label settingsLanguageDesc;

	private Label settingsQualityDesc;

	private TextBox registerEmail;

	private TextBox registerTicket;

	private TextBox registerPassword;

	private TextBox registerConfirmPassword;

	private Label registerEmailText;

	private Label registerPasswordText;

	private Label registerTicketText;

	private Label registerConfirmPasswordText;

	private CheckBox registerAgree;

	private Label playLoggedInAs;

	private Panel promoBrowser;

	private FlowLayoutPanel promoCards;

	private Label promoStatusLabel;

	private WebBrowser promoIeBrowser;

	private WebView2 promoWebView;

	private bool promoWebViewReady;

	private bool promoUseWebView2;

	private Button logoutButton;

	private ProgressBar playProgress;

	private Button playButton;

	private Image _playChromeComposed;

	private NotifyIcon Notification;

	private LinkLabel forgotPassword;

	internal Label playProgressText;

	internal Label playProgressTime;

	private const int ENUM_CURRENT_SETTINGS = -1;

	private const int CDS_UPDATEREGISTRY = 1;

	private const int CDS_FULLSCREEN = 4;

	private const int DM_PELSWIDTH = 524288;

	private const int DM_PELSHEIGHT = 1048576;

	private const int DM_DISPLAYFREQUENCY = 4194304;

	private const int DM_BITSPERPEL = 262144;

	private Image playEnabledBase;

	private Image[] playGlowCache;

	private int playGlowCacheIndex;

	private float playGlowPhase;

	private float playGlowRange = -8f;

	private static Image _playBgCache;

	private string _nrzEmailCache = "";

	private string _nrzPassCache = "";

	private bool promoAlreadyShown;

	public mainScreen ()
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Expected O, but got Unknown
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Expected O, but got Unknown
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Expected O, but got Unknown
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Expected O, but got Unknown
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Expected O, but got Unknown
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Expected O, but got Unknown
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Expected O, but got Unknown
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Expected O, but got Unknown
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Expected O, but got Unknown
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Expected O, but got Unknown
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Expected O, but got Unknown
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Expected O, but got Unknown
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Expected O, but got Unknown
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Expected O, but got Unknown
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Expected O, but got Unknown
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Expected O, but got Unknown
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Expected O, but got Unknown
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Expected O, but got Unknown
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Expected O, but got Unknown
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Expected O, but got Unknown
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Expected O, but got Unknown
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Expected O, but got Unknown
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Invalid comparison between Unknown and I4
		if (Environment.OSVersion.Version.Major <= 5) {
			if (!DetectLinux.WineDetected ()) {
				MessageBox.Show ((IWin32Window)null, "Sadly, the red background cannot be fixed on Windows XP and lower, sorry...", "Need for Speed World", (MessageBoxButtons)0, (MessageBoxIcon)64);
			}
		} else {
			((Control)this).Font = new Font (((Control)this).Font.Name, 8.25f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, ((Control)this).Font.Style, ((Control)this).Font.Unit, ((Control)this).Font.GdiCharSet, ((Control)this).Font.GdiVerticalFont);
		}
		InitializeComponent ();
		try {
			((Control)consoleLog).Visible = false;
			if (bottomBanner != null) {
				try {
					Image val = LoadEmbedded ("GameLauncher.Banner.bottom_banner.png");
					if (val != null) {
						bottomBanner.Image = val;
					}
				} catch {
				}
				((Control)bottomBanner).Visible = false;
				((Control)bottomBanner).BringToFront ();
			}
			if (advertisementLabel != null) {
				((Control)advertisementLabel).Visible = false;
				((Control)advertisementLabel).BringToFront ();
			}
		} catch {
		}
		((Control)this).SetStyle ((ControlStyles)2048, true);
		ApplyEmbeddedFonts ();
		SettingFile.KeyExists ("__disabled_LauncherPosX");
		Self.centerScreen ((Form)(object)this);
		((Form)this).MaximizeBox = false;
		((Control)this).SetStyle ((ControlStyles)204802, true);
		((Control)closebtn).MouseEnter += closebtn_MouseEnter;
		((Control)closebtn).MouseLeave += closebtn_MouseLeave;
		((Control)closebtn).Click += closebtn_Click;
		((Control)settingsButton).MouseEnter += settingsButton_MouseEnter;
		((Control)settingsButton).MouseLeave += settingsButton_MouseLeave;
		((Control)settingsButton).Click += settingsButton_Click;
		((Control)minimizebtn).MouseEnter += minimizebtn_MouseEnter;
		((Control)minimizebtn).MouseLeave += minimizebtn_MouseLeave;
		((Control)minimizebtn).Click += minimizebtn_Click;
		((Control)loginButton).MouseEnter += loginButton_MouseEnter;
		((Control)loginButton).MouseLeave += loginButton_MouseLeave;
		((Control)loginButton).Click += loginButton_Click;
		((Control)loginButton).MouseUp += new MouseEventHandler (loginButton_MouseUp);
		((Control)loginButton).MouseDown += new MouseEventHandler (loginButton_MouseDown);
		((Control)registerButton).MouseEnter += registerButton_MouseEnter;
		((Control)registerButton).MouseLeave += registerButton_MouseLeave;
		((Control)registerButton).MouseUp += new MouseEventHandler (registerButton_MouseUp);
		((Control)registerButton).MouseDown += new MouseEventHandler (registerButton_MouseDown);
		((Control)registerButton).Click += registerButton_Click;
		((Control)settingsSave).MouseEnter += settingsSave_MouseEnter;
		((Control)settingsSave).MouseLeave += settingsSave_MouseLeave;
		((Control)settingsSave).MouseUp += new MouseEventHandler (settingsSave_MouseUp);
		((Control)settingsSave).MouseDown += new MouseEventHandler (settingsSave_MouseDown);
		((Control)settingsSave).Click += settingsSave_Click;
		((Control)email).KeyUp += new KeyEventHandler (loginbuttonenabler);
		((Control)email).KeyDown += new KeyEventHandler (loginEnter);
		((Control)password).KeyUp += new KeyEventHandler (loginbuttonenabler);
		((Control)password).TextChanged += loginbuttonenabler;
		((Control)email).TextChanged += loginbuttonenabler;
		((Control)password).KeyDown += new KeyEventHandler (loginEnter);
		((Control)serverPick).TextChanged += serverPick_TextChanged;
		forgotPassword.LinkClicked += new LinkLabelLinkClickedEventHandler (forgotPassword_LinkClicked);
		githubLink.LinkClicked += new LinkLabelLinkClickedEventHandler (githubLink_LinkClicked);
		((Control)moveWindow).MouseDown += new MouseEventHandler (moveWindow_MouseDown);
		((Control)moveWindow).MouseMove += new MouseEventHandler (moveWindow_MouseMove);
		((Control)moveWindow).MouseUp += new MouseEventHandler (moveWindow_MouseUp);
		((Control)playButton).MouseEnter += playButton_MouseEnter;
		((Control)playButton).MouseLeave += playButton_MouseLeave;
		((Control)playButton).Click += playButton_Click;
		((Control)playButton).MouseUp += new MouseEventHandler (playButton_MouseUp);
		((Control)playButton).MouseDown += new MouseEventHandler (playButton_MouseDown);
		((Control)logoutButton).Click += logoutButton_Click;
		((Form)this).FormClosing += new FormClosingEventHandler (mainScreen_FormClosing);
		((Control)logoutButton).MouseEnter += logoutButton_MouseEnter;
		((Control)logoutButton).MouseLeave += logoutButton_MouseLeave;
		((Control)logoutButton).MouseDown += new MouseEventHandler (logoutButton_MouseDown);
		((Control)logoutButton).MouseUp += new MouseEventHandler (logoutButton_MouseUp);
		try {
			string path = Directory.GetCurrentDirectory () + "\\test.txt";
			File.WriteAllText (path, "test");
			File.Delete (path);
		} catch {
			Self.runAsAdmin ();
			Environment.Exit (Environment.ExitCode);
		}
		if (string.IsNullOrEmpty (SettingFile.Read ("InstallationDirectory"))) {
			if (Environment.OSVersion.Version.Major <= 5) {
				MessageBox.Show ((IWin32Window)null, "Click OK to select folder with NFSW.exe", "Need for Speed World", (MessageBoxButtons)0, (MessageBoxIcon)64);
				FolderBrowserDialog val2 = new FolderBrowserDialog ();
				if ((int)((CommonDialog)val2).ShowDialog () == 1) {
					SettingFile.Write ("InstallationDirectory", val2.SelectedPath);
				} else {
					Environment.Exit (Environment.ExitCode);
				}
			} else {
				CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog {
					InitialDirectory = string.Empty,
					IsFolderPicker = true,
					Title = "GameLauncher: Please pick up a directory where NFSW is located or has to be installed."
				};
				if (commonOpenFileDialog.ShowDialog () == CommonFileDialogResult.Ok) {
					SettingFile.Write ("InstallationDirectory", commonOpenFileDialog.FileName);
				} else {
					Environment.Exit (Environment.ExitCode);
				}
			}
		}
		if (File.Exists (SettingFile.Read ("InstallationDirectory") + "X:\\__no_nfsw_cursor__\\default.cur")) {
			Cursor val3 = new Cursor (Cursor.Current.Handle);
			IntPtr intPtr = User32.LoadCursorFromFile (SettingFile.Read ("InstallationDirectory") + "X:\\__no_nfsw_cursor__\\default.cur");
			((object)val3).GetType ().InvokeMember ("handle", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField, null, val3, new object[1] { intPtr });
			((Control)this).Cursor = val3;
		}
		if (!Directory.Exists ("Themes")) {
			Directory.CreateDirectory ("Themes");
			File.WriteAllText ("Themes/ReadMe.txt", "Put your GLS Files in this directory.");
		}
		((Control)registerText).Text = "DON'T HAVE AN ACCOUNT?\nCLICK HERE TO CREATE ONE NOW...";
		((Control)this).Text = "Need for Speed World";
	}

	private void moveWindow_MouseDown (object sender, MouseEventArgs e)
	{
		((Control)this).ContextMenu = null;
		((Control)this).ContextMenu = null;
		mouseDownPoint = new Point (e.X, e.Y);
	}

	private void moveWindow_MouseUp (object sender, MouseEventArgs e)
	{
		mouseDownPoint = Point.Empty;
		((Control)this).Refresh ();
		((Form)this).Opacity = 1.0;
	}

	private void moveWindow_MouseMove (object sender, MouseEventArgs e)
	{
		if (!mouseDownPoint.IsEmpty) {
			((Form)this).Location = new Point (((Form)this).Location.X + (e.X - mouseDownPoint.X), ((Form)this).Location.Y + (e.Y - mouseDownPoint.Y));
			windowMoved = true;
			((Form)this).Opacity = 1.0;
		}
	}

	public void ConsoleLog (string e, string type)
	{
		try {
			File.AppendAllText ("launcher.log", "[" + DateTime.Now.ToString ("HH:mm:ss") + "] [" + type + "] " + e + Environment.NewLine);
		} catch {
		}
	}

	private void mainScreen_Load (object sender, EventArgs e)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0df8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e17: Expected O, but got Unknown
		//IL_0e1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e29: Expected O, but got Unknown
		//IL_0e31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3b: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		try {
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
			((Control)loginButton).ForeColor = Color.FromArgb (153, 153, 153);
			loginEnabled = false;
			UpdateLoginButtonState ();
		} catch {
		}
		Updater.checkForUpdate (sender, e);
		((Control)this).ContextMenu = new ContextMenu ();
		((Menu)((Control)this).ContextMenu).MenuItems.Add (new MenuItem ("&Check for updates", (EventHandler)Updater.checkForUpdate));
		((Menu)((Control)this).ContextMenu).MenuItems.Add (new MenuItem ("&About", (EventHandler)About.showAbout));
		((Menu)((Control)this).ContextMenu).MenuItems.Add (new MenuItem ("&Settings", (EventHandler)settingsButton_Click));
		((Menu)((Control)this).ContextMenu).MenuItems.Add ("-");
		((Menu)((Control)this).ContextMenu).MenuItems.Add (new MenuItem ("&Close", (EventHandler)closebtn_Click));
		Notification.ContextMenu = ((Control)this).ContextMenu;
		Notification.Icon = new Icon (((Form)this).Icon, ((Form)this).Icon.Width, ((Form)this).Icon.Height);
		Notification.Text = "Need for Speed World";
		Notification.Visible = false;
		ConsoleLog ("Log initialized", "info");
		ConsoleLog ("GameLauncher initialized", "info");
		try {
			DiscordPresence.Init ();
		} catch {
		}
		try {
			LauncherUpdaterCheck.CheckAsync (delegate(string m) {
				ConsoleLog (m, "info");
			});
		} catch {
		}
		ConsoleLog ("Installation directory: " + Path.GetFullPath (SettingFile.Read ("InstallationDirectory")), "info");
		((Control)email).Text = SettingFile.Read ("AccountEmail");
		((Control)password).Text = "";
		loginEnabled = false;
		useSavedPassword = false;
		UpdateLoginButtonState ();
		UpdateLoginButtonState ();
		((Control)loginButton).Enabled = true;
		if (!string.IsNullOrEmpty (SettingFile.Read ("AccountEmail"))) {
			rememberMe.Checked = true;
		}
		string text = string.Empty;
		string[] array;
		try {
			WebClient webClient = new WebClientWithTimeout ();
			string[] obj4 = new string[2] { "", "" };
			string text2 = null;
			string text3 = null;
			array = obj4;
			foreach (string text4 in array) {
				try {
					ConsoleLog ("Fetching " + text4, "info");
					text2 = webClient.DownloadString (text4);
					text3 = text4;
				} catch (Exception ex) {
					ConsoleLog ("Failed " + text4 + ": " + ex.Message, "warning");
					continue;
				}
				break;
			}
			if (text2 == null) {
				throw new Exception ("All server list APIs failed");
			}
			StringBuilder stringBuilder = new StringBuilder ();
			try {
				foreach (JToken item in JArray.Parse (text2)) {
					string text5 = (string)(item ["name"] ?? item ["Name"] ?? ((JToken)"Unknown"));
					string text6 = (string)(item ["ip_address"] ?? item ["ipAddress"] ?? item ["IPAddress"] ?? ((JToken)string.Empty));
					if (!string.IsNullOrEmpty (text6)) {
						string text7 = (text5 + " " + text6).ToLowerInvariant ();
						bool num = text7.Contains ("nightriderz") && !text7.Contains ("lab") && !text7.Contains ("offline") && !text7.Contains ("localhost") && !text7.Contains ("127.0.0.1") && (text7.Contains ("horizon") || text6.ToLowerInvariant ().Contains ("horizon.nightriderz"));
						bool flag = (text7.Contains ("worldunited") || text6.ToLowerInvariant ().Contains ("worldunited.gg")) && text7.Contains ("") && !text7.Contains ("offline") && !text7.Contains ("lab");
						if (num || flag) {
							stringBuilder.AppendLine (text5 + ";" + text6);
						}
					}
				}
				if (stringBuilder.Length == 0) {
					stringBuilder.AppendLine ("CHICANE - EU;https://horizon.nightriderz.world/Engine.svc");
					stringBuilder.AppendLine ("CHICANE - US;http://game.worldunited.gg:8080/Engine.svc");
					stringBuilder.AppendLine ("");
				}
			} catch (Exception ex2) {
				ConsoleLog ("JSON parse error: " + ex2.Message, "error");
				stringBuilder.Append (text2);
			}
			text = stringBuilder.ToString ();
			ConsoleLog ("Loaded servers from " + text3, "success");
			serverlistloaded = true;
			try {
				FileStream fileStream = new FileStream ("ServerCache", FileMode.Create);
				DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider {
					Key = Encoding.ASCII.GetBytes (serverCacheKey),
					IV = Encoding.ASCII.GetBytes (serverCacheKey)
				};
				StreamWriter streamWriter = new StreamWriter ((Stream)(object)new CryptoStream ((Stream)fileStream, dESCryptoServiceProvider.CreateEncryptor (), CryptoStreamMode.Write));
				streamWriter.Write (text);
				streamWriter.Close ();
				ConsoleLog ("Successfully created ServerCache", "success");
			} catch (Exception ex3) {
				ConsoleLog ("Failed to create cached serverlist. " + ex3.Message, "error");
			}
		} catch (Exception ex4) {
			if (File.Exists ("ServerCache")) {
				FileStream fileStream2 = new FileStream ("ServerCache", FileMode.Open);
				DESCryptoServiceProvider dESCryptoServiceProvider2 = new DESCryptoServiceProvider {
					Key = Encoding.ASCII.GetBytes (serverCacheKey),
					IV = Encoding.ASCII.GetBytes (serverCacheKey)
				};
				text = new StreamReader ((Stream)(object)new CryptoStream ((Stream)fileStream2, dESCryptoServiceProvider2.CreateDecryptor (), CryptoStreamMode.Read)).ReadToEnd ();
				serverlistloaded = true;
				ConsoleLog ("Fetched Serverlist from Cache", "warning");
			} else {
				ConsoleLog ("Failed to fetch serverlist. " + ex4.Message, "error");
			}
		}
		((ListControl)serverPick).DisplayMember = "Text";
		((ListControl)serverPick).ValueMember = "Value";
		List<object> list = new List<object> ();
		List<object> list2 = new List<object> ();
		List<object> list3 = new List<object> ();
		if (text.IndexOf ("", StringComparison.OrdinalIgnoreCase) < 0) {
			text += "";
		}
		array = text.Split (new string[2] { "\r\n", "\n" }, StringSplitOptions.None);
		foreach (string text8 in array) {
			if (string.IsNullOrEmpty (text8)) {
				continue;
			}
			string[] array2 = text8.Split (new string[1] { ";" }, StringSplitOptions.None);
			if (array2.Length >= 2) {
				string text9 = array2 [0].ToLowerInvariant ();
				string text10 = array2 [1].ToLowerInvariant ();
				bool num2 = (text9.Contains ("nightriderz") || text10.Contains ("horizon.nightriderz")) && !text9.Contains ("lab") && !text9.Contains ("offline") && !text9.Contains ("built-in") && !text9.Contains ("builtin");
				bool flag2 = (text9.Contains ("worldunited") || text10.Contains ("worldunited.gg")) && text9.Contains ("") && !text9.Contains ("offline") && !text9.Contains ("lab");
				bool flag3 = text9.Contains ("") || text10.Contains ("");
				if (num2) {
					list.Add (new {
						Text = array2 [0],
						Value = array2 [1]
					});
				} else if (flag2) {
					list2.Add (new {
						Text = array2 [0],
						Value = array2 [1]
					});
				} else if (flag3) {
					list3.Add (new {
						Text = array2 [0],
						Value = array2 [1]
					});
				}
			}
		}
		List<object> list4 = new List<object> ();
		list4.AddRange (list);
		list4.AddRange (list2);
		list4.AddRange (list3);
		if (list4.Count == 0) {
			list4.Add (new {
				Text = "CHICANE - EU",
				Value = "https://horizon.nightriderz.world/Engine.svc"
			});
			list4.Add (new {
				Text = "CHICANE - US",
				Value = "http://game.worldunited.gg:8080/Engine.svc"
			});
		}
		bool flag4 = false;
		foreach (object item2 in list4) {
			try {
				if (item2.GetType ().GetProperty ("Text").GetValue (item2, null)
					.ToString ()
					.ToLowerInvariant ()
					.Contains ("")) {
					flag4 = true;
					break;
				}
			} catch {
			}
		}
		if (!flag4) {
			list4.Add (new {
				Text = "",
				Value = ""
			});
		}
		serverPick.DataSource = list4;
		if (list4.Count == 0) {
			SetServersCurrentlyDown ();
			loginEnabled = false;
			serverEnabled = false;
			UpdateLoginButtonState ();
			((Control)password).Text = "";
		} else {
			((ListControl)serverPick).SelectedIndex = 0;
			if (serverlistloaded) {
				if (!SettingFile.KeyExists ("Server")) {
					SettingFile.Write ("Server", ((ListControl)serverPick).SelectedValue.ToString ());
				}
				if (SettingFile.KeyExists ("Server")) {
					skipServerTrigger = true;
					if (text.Contains (SettingFile.Read ("Server"))) {
						((ListControl)serverPick).SelectedValue = SettingFile.Read ("Server");
					}
					if (((ListControl)serverPick).SelectedIndex == 0) {
						serverPick_TextChanged (sender, e);
					}
				}
			} else {
				serverPick_TextChanged (sender, e);
			}
			((Control)serverStatusImg).Location = new Point (-16, -16);
			loginEnabled = false;
			serverEnabled = true;
		}
		useSavedPassword = false;
		UpdateLoginButtonState ();
		UpdateLoginButtonState ();
		((Control)loginButton).Enabled = true;
		((Control)password).Text = "";
		((ListControl)settingsLanguage).DisplayMember = "Text";
		((ListControl)settingsLanguage).ValueMember = "Value";
		var dataSource = new[] {
			new {
				Text = "English",
				Value = "EN"
			},
			new {
				Text = "Deutsch",
				Value = "DE"
			},
			new {
				Text = "Español",
				Value = "ES"
			},
			new {
				Text = "Français",
				Value = "FR"
			},
			new {
				Text = "Polski",
				Value = "PL"
			},
			new {
				Text = "Русский",
				Value = "RU"
			},
			new {
				Text = "Português (Brasil)",
				Value = "PT"
			},
			new {
				Text = "繁體中文",
				Value = "TC"
			},
			new {
				Text = "简体中文",
				Value = "SC"
			},
			new {
				Text = "ภาษาไทย",
				Value = "TH"
			},
			new {
				Text = "Türkçe",
				Value = "TR"
			}
		};
		settingsLanguage.DataSource = dataSource;
		if (SettingFile.KeyExists ("Language")) {
			((ListControl)settingsLanguage).SelectedValue = SettingFile.Read ("Language");
		}
		((ListControl)settingsQuality).DisplayMember = "Text";
		((ListControl)settingsQuality).ValueMember = "Value";
		var dataSource2 = new[] {
			new {
				Text = "Standard",
				Value = "0"
			},
			new {
				Text = "Maximum",
				Value = "1"
			}
		};
		settingsQuality.DataSource = dataSource2;
		if (SettingFile.KeyExists ("TracksHigh")) {
			((ListControl)settingsQuality).SelectedValue = SettingFile.Read ("TracksHigh");
		}
		if (File.Exists (UserSettings)) {
			ConsoleLog ("Found Game Config under " + UserSettings + " file.", "success");
		}
		Directory.CreateDirectory (SettingFile.Read ("InstallationDirectory"));
		if (!File.Exists (SettingFile.Read ("InstallationDirectory") + "/lightfx.dll")) {
			File.WriteAllBytes (SettingFile.Read ("InstallationDirectory") + "/lightfx.dll", ExtractResource.AsByte ("GameLauncher.SoapBoxModules.lightfx.dll"));
			Directory.CreateDirectory (SettingFile.Read ("InstallationDirectory") + "/modules");
			File.WriteAllText (SettingFile.Read ("InstallationDirectory") + "/modules/udpcrc.soapbox.module", ExtractResource.AsString ("GameLauncher.SoapBoxModules.udpcrc.soapbox.module"));
			File.WriteAllText (SettingFile.Read ("InstallationDirectory") + "/modules/udpcrypt1.soapbox.module", ExtractResource.AsString ("GameLauncher.SoapBoxModules.udpcrypt1.soapbox.module"));
			File.WriteAllText (SettingFile.Read ("InstallationDirectory") + "/modules/udpcrypt2.soapbox.module", ExtractResource.AsString ("GameLauncher.SoapBoxModules.udpcrypt2.soapbox.module"));
			File.WriteAllText (SettingFile.Read ("InstallationDirectory") + "/modules/xmppsubject.soapbox.module", ExtractResource.AsString ("GameLauncher.SoapBoxModules.xmppsubject.soapbox.module"));
		}
		if (builtinserver) {
			((Control)loginButton).ForeColor = Color.White;
			UpdateLoginButtonState ();
		}
		RegisterFormElements (hideElements: false);
		SettingsFormElements (hideElements: false);
		DownloadFormElements (hideElements: false);
		try {
			if (bottomBanner != null) {
				((Control)bottomBanner).Visible = false;
				((Control)bottomBanner).BringToFront ();
			}
			if (advertisementLabel != null) {
				((Control)advertisementLabel).Visible = false;
				((Control)advertisementLabel).BringToFront ();
			}
		} catch {
		}
		if (Environment.GetCommandLineArgs ().Length == 2) {
			MessageBox.Show ("Your launcher has been updated.");
			((Control)this).ContextMenu = null;
			((Control)this).MouseDown += new MouseEventHandler (moveWindow_MouseDown);
			((Control)this).MouseMove += new MouseEventHandler (moveWindow_MouseMove);
			((Control)this).MouseUp += new MouseEventHandler (moveWindow_MouseUp);
			((Control)this).ContextMenu = null;
			((Control)this).Text = "Need for Speed World";
			Notification.Text = "Need for Speed World";
			ClearAllContextMenus ();
		}
	}

	private void closebtn_Click (object sender, EventArgs e)
	{
		((Control)closebtn).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.close_click;
		Notification.Visible = false;
		string text = ((DateTime.UtcNow.Ticks - DateTime.Parse ("01/01/1970 00:00:00").Ticks) / 10000000).ToString ();
		if (serverlistloaded) {
			SettingFile.Write ("Server", ((ListControl)serverPick).SelectedValue.ToString ());
		}
		if (windowMoved) {
			SettingFile.Write ("__disabled_LauncherPosX", ((Form)this).Location.X.ToString ());
			SettingFile.Write ("__disabled_LauncherPosY", ((Form)this).Location.Y.ToString ());
		}
		try {
			if (!Directory.Exists ("Logs")) {
				Directory.CreateDirectory ("Logs");
			}
			consoleLog.SaveFile ("Logs/" + text + ".log", (RichTextBoxStreamType)1);
		} catch {
		}
		SettingFile.Write ("InstallationDirectory", Path.GetFullPath (SettingFile.Read ("InstallationDirectory")));
		Process.GetProcessById (Process.GetCurrentProcess ().Id).Kill ();
	}

	private void closebtn_MouseEnter (object sender, EventArgs e)
	{
		((Control)closebtn).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.close_hover;
	}

	private void closebtn_MouseLeave (object sender, EventArgs e)
	{
		((Control)closebtn).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.close;
	}

	private void minimizebtn_Click (object sender, EventArgs e)
	{
		((Control)minimizebtn).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.minimize_click;
		((Form)this).WindowState = (FormWindowState)1;
	}

	private void minimizebtn_MouseEnter (object sender, EventArgs e)
	{
		((Control)minimizebtn).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.minimize_hover;
	}

	private void minimizebtn_MouseLeave (object sender, EventArgs e)
	{
		((Control)minimizebtn).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.minimize;
	}

	private void loginEnter (object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		if ((int)e.KeyCode == 13 && loginEnabled) {
			loginButton_Click (null, null);
			e.SuppressKeyPress = true;
		} else if ((int)e.Modifiers == 131072 && (int)e.KeyCode == 86) {
			e.SuppressKeyPress = true;
		}
	}

	private Image GetGrayLoginDisableImage ()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		if (loginDisableImageGray != null) {
			return loginDisableImageGray;
		}
		try {
			Image button_disable = (Image)(object)GameLauncher.Properties.Resources.button_disable;
			Bitmap val = new Bitmap (button_disable.Width, button_disable.Height, (PixelFormat)2498570);
			Graphics val2 = Graphics.FromImage ((Image)(object)val);
			try {
				val2.DrawImage (button_disable, 0, 0, button_disable.Width, button_disable.Height);
			} finally {
				((IDisposable)val2)?.Dispose ();
			}
			for (int i = 0; i < ((Image)val).Height; i++) {
				for (int j = 0; j < ((Image)val).Width; j++) {
					Color pixel = val.GetPixel (j, i);
					if (pixel.A >= 20 && pixel.R > 160 && pixel.G > 160 && pixel.B > 160) {
						int a = pixel.A;
						val.SetPixel (j, i, Color.FromArgb (a, 153, 153, 153));
					}
				}
			}
			loginDisableImageGray = (Image)(object)val;
			return loginDisableImageGray;
		} catch {
			return (Image)(object)GameLauncher.Properties.Resources.button_disable;
		}
	}

	private void loginbuttonenabler (object sender, EventArgs e)
	{
		string text = ((Control)email).Text;
		string text2 = ((Control)password).Text;
		string text3 = ((Control)serverStatus).Text;
		bool flag = (loginEnabled = ((text != null && !string.IsNullOrEmpty (text.Trim ()) && !string.IsNullOrEmpty (text2) && serverEnabled && text3 != null && text3.IndexOf ("up and running", StringComparison.OrdinalIgnoreCase) >= 0 && text3.IndexOf ("Retrieving", StringComparison.OrdinalIgnoreCase) < 0) ? true : false));
		((Control)loginButton).Enabled = true;
		((Control)loginButton).Text = "LOG IN";
		if (flag) {
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_enable;
			((Control)loginButton).ForeColor = Color.White;
		} else {
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
			((Control)loginButton).ForeColor = Color.FromArgb (153, 153, 153);
		}
		plainPasswordMemory = text2;
		useSavedPassword = false;
	}

	private void loginButton_MouseUp (object sender, EventArgs e)
	{
		if (!loginEnabled) {
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
			((Control)loginButton).ForeColor = Color.FromArgb (153, 153, 153);
			((Control)loginButton).Text = "LOG IN";
		} else {
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_hover;
			((Control)loginButton).ForeColor = Color.White;
		}
	}

	private void loginButton_MouseDown (object sender, EventArgs e)
	{
		if (!loginEnabled) {
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
			((Control)loginButton).ForeColor = Color.FromArgb (153, 153, 153);
			((Control)loginButton).Text = "LOG IN";
		} else {
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_click;
			((Control)loginButton).ForeColor = Color.White;
		}
	}

	private void UpdateLoginButtonState ()
	{
		try {
			loginbuttonenabler (null, EventArgs.Empty);
		} catch {
		}
	}

	private void loginButton_Click (object sender, EventArgs e)
	{
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		if (!loginEnabled || !((Control)loginButton).Enabled || ((!loginEnabled || !serverEnabled) && !builtinserver)) {
			return;
		}
		serverIP = ((ListControl)serverPick).SelectedValue.ToString ();
		((ListControl)serverPick).GetItemText (serverPick.SelectedItem);
		string value = ((Control)email).Text.ToString ();
		string empty = string.Empty;
		string empty2 = string.Empty;
		SHA1 sHA = SHA1.Create ();
		StringBuilder stringBuilder = new StringBuilder ();
		byte[] array = sHA.ComputeHash (Encoding.UTF8.GetBytes (((Control)password).Text.ToString ()));
		foreach (byte b in array) {
			stringBuilder.Append (b.ToString ("X2"));
		}
		empty = (useSavedPassword ? SettingFile.Read ("Password") : stringBuilder.ToString ());
		if (rememberMe.Checked) {
			SettingFile.Write ("AccountEmail", value);
			SettingFile.Write ("Password", empty.ToLower ());
		} else {
			SettingFile.DeleteKey ("AccountEmail");
			SettingFile.DeleteKey ("Password");
		}
		ConsoleLog ("Trying to login into " + ((ListControl)serverPick).GetItemText (serverPick.SelectedItem) + " (" + serverIP + ")", "info");
		if (builtinserver) {
			MessageBox.Show ((IWin32Window)null, "Careful: This built-in server is in alpha! Use it at your own risk.", "Need for Speed World", (MessageBoxButtons)0, (MessageBoxIcon)64);
		}
		try {
			string text = ((Control)password).Text;
			if (string.IsNullOrEmpty (text) && !string.IsNullOrEmpty (plainPasswordMemory)) {
				text = plainPasswordMemory;
			}
			if (string.IsNullOrEmpty (text)) {
				MessageBox.Show ("Please enter your password (saved password hash cannot be used for Modern Auth).", "Login");
				return;
			}
			plainPasswordMemory = text;
			empty2 = ServerAuth.Authenticate (serverIP, value, text);
		} catch (WebException ex) {
			if (ex.Status == WebExceptionStatus.ProtocolError) {
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
				empty2 = ((httpWebResponse.StatusCode != HttpStatusCode.InternalServerError) ? ((Exception)(object)ex).Message : new StreamReader (((WebResponse)httpWebResponse).GetResponseStream ()).ReadToEnd ());
			} else {
				empty2 = ((Exception)(object)ex).Message;
			}
		}
		XmlDocument val = new XmlDocument ();
		try {
			if (!builtinserver) {
				if (string.IsNullOrWhiteSpace (empty2) || !empty2.TrimStart (Array.Empty<char> ()).StartsWith ("<")) {
					empty2 = "<?xml version=\"1.0\"?><LoginStatusVO><UserId>0</UserId><LoginToken></LoginToken><Description>" + (string.IsNullOrEmpty (ServerAuth.LastError) ? "Invalid reply" : ServerAuth.LastError).Replace ("&", "&amp;").Replace ("<", "&lt;") + "</Description></LoginStatusVO>";
				}
				val.LoadXml (empty2);
			} else {
				val.LoadXml ("<LoginStatusVO><UserId>1</UserId><LoginToken>aaaaaaaa-aaaa-aaaa-aaaaaaaa</LoginToken><Description/></LoginStatusVO>");
			}
			XmlNode val2 = ((XmlNode)val).SelectSingleNode ("LoginStatusVO/Description");
			XmlNode val3 = ((XmlNode)val).SelectSingleNode ("LoginStatusVO/LoginToken");
			XmlNode val4 = ((XmlNode)val).SelectSingleNode ("LoginStatusVO/UserId");
			string text2 = ((val2 != null) ? (val2.InnerText ?? "") : "");
			string text3 = ((val3 != null) ? (val3.InnerText ?? "") : "");
			string text4 = ((val4 != null) ? (val4.InnerText ?? "") : "");
			if (string.IsNullOrEmpty (text2) && !string.IsNullOrEmpty (text3) && !string.IsNullOrEmpty (text4) && text4 != "0") {
				UserId = text4;
				LoginToken = text3;
				try {
					try {
						((ListControl)serverPick).GetItemText (serverPick.SelectedItem);
					} catch {
					}
					string text5 = ((Control)email).Text ?? "";
					string text6 = plainPasswordMemory;
					if (string.IsNullOrEmpty (text6)) {
						try {
							text6 = ((Control)password).Text ?? "";
						} catch {
						}
					}
					if (!string.IsNullOrWhiteSpace (text5) && !string.IsNullOrWhiteSpace (text6)) {
						_nrzEmailCache = text5.Trim ();
						_nrzPassCache = text6;
						plainPasswordMemory = text6;
						try {
							string warmEm = _nrzEmailCache;
							string warmPw = _nrzPassCache;
							ThreadPool.QueueUserWorkItem (delegate {
								try {
									bool flag = NrzChallenges.WarmSession (warmEm, warmPw);
									ConsoleLog (flag ? "NightRiderz challenges session ready" : "NightRiderz challenges session failed", flag ? "success" : "info");
								} catch (Exception ex4) {
									try {
										ConsoleLog ("Challenges session: " + ex4.Message, "error");
									} catch {
									}
								}
							});
						} catch (Exception ex2) {
							try {
								ConsoleLog ("Challenges session: " + ex2.Message, "error");
							} catch {
							}
						}
					}
				} catch {
				}
				if (bottomBanner != null) {
					((Control)bottomBanner).Visible = false;
				}
				if (advertisementLabel != null) {
					((Control)advertisementLabel).Visible = false;
				}
				if (verticalBanner != null) {
					((Control)verticalBanner).Visible = false;
				}
				HidePlayBanners ();
				((Control)this).BackgroundImage = LoadPlayBg ();
				((Control)this).BackgroundImageLayout = (ImageLayout)0;
				((Control)currentWindowInfo).Visible = false;
				if (!builtinserver) {
					((Control)playLoggedInAs).Text = "LOGGED IN AS " + ((Control)email).Text.ToUpper ();
				} else {
					((Control)playLoggedInAs).Text = "LOGGED IN AS LOCALHOST";
				}
				LoginFormElements ();
				DownloadFormElements ();
				try {
					ShowPromoPanel ();
				} catch {
				}
				((Control)selectServerLabel).Visible = true;
				((Control)serverPick).Visible = true;
				((Control)settingsButton).Visible = true;
				((Control)playLoggedInAs).Visible = false;
				((Control)logoutButton).Visible = false;
				launchNFSW ();
			} else {
				string text7 = ((!string.IsNullOrEmpty (text2)) ? text2 : (string.IsNullOrEmpty (ServerAuth.LastError) ? "Invalid E-mail or Password" : ServerAuth.LastError));
				ConsoleLog (text7, "error");
				MessageBox.Show (text7, "Login");
			}
		} catch (Exception ex3) {
			string text8 = (string.IsNullOrEmpty (ServerAuth.LastError) ? ("Failed to get token. " + ex3.Message) : ServerAuth.LastError);
			ConsoleLog (text8 + " | raw=" + ServerAuth.LastRaw, "error");
			MessageBox.Show (text8, "Login");
		}
	}

	private void loginButton_MouseEnter (object sender, EventArgs e)
	{
		if (!loginEnabled) {
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
			((Control)loginButton).ForeColor = Color.FromArgb (153, 153, 153);
		} else {
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_hover;
			((Control)loginButton).ForeColor = Color.White;
		}
	}

	private void loginButton_MouseLeave (object sender, EventArgs e)
	{
		if (!loginEnabled) {
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
			((Control)loginButton).ForeColor = Color.FromArgb (153, 153, 153);
			((Control)loginButton).Text = "LOG IN";
		} else {
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_enable;
			((Control)loginButton).ForeColor = Color.White;
		}
	}

	private void SetServersCurrentlyDown ()
	{
		serverEnabled = false;
		loginEnabled = false;
		((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
		try {
			if (serverStatusImg != null) {
				((Control)serverStatusImg).Location = new Point (20, 334);
				((Control)serverStatusImg).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.server_offline;
				((Control)serverStatusImg).Visible = true;
			}
			if (serverStatus != null) {
				((Control)serverStatus).ForeColor = Color.FromArgb (227, 88, 50);
				((Control)serverStatus).Text = "The servers are currently down. Thanks for your patience!";
				UpdateBannerFromState ();
				((Control)serverStatus).Location = new Point (40, 332);
				((Control)serverStatus).Visible = true;
			}
			if (onlineCount != null) {
				((Control)onlineCount).Text = string.Empty;
				((Control)onlineCount).Visible = false;
			}
			serverEnabled = false;
			loginEnabled = false;
			try {
				UpdateLoginButtonState ();
			} catch {
			}
		} catch {
		}
	}

	private void serverPick_TextChanged (object sender, EventArgs e)
	{
		if (inPlayScreen && !string.IsNullOrEmpty (UserId) && UserId != "0") {
			promoAlreadyShown = false;
			if (Timeout != null) {
				Timeout.Stop ();
			}
			if (promoBrowser != null) {
				((Control)promoBrowser).Visible = false;
			}
			if (promoWebView != null) {
				((Control)promoWebView).Visible = false;
			}
			logoutButton_Click (sender, e);
		}
		serverEnabled = false;
		loginEnabled = false;
		((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
		((Control)loginButton).ForeColor = Color.FromArgb (153, 153, 153);
		if (!skipServerTrigger) {
			UpdateBannerFromState ();
			return;
		}
		if (((ListControl)serverPick).SelectedValue == null || serverPick.SelectedItem == null) {
			SetServersCurrentlyDown ();
			UpdateBannerFromState ();
			return;
		}
		loginEnabled = false;
		serverEnabled = false;
		((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
		((Control)loginButton).ForeColor = Color.FromArgb (153, 153, 153);
		((Control)password).Text = string.Empty;
		string verticalImageUrl = string.Empty;
		verticalBanner.Image = null;
		((Control)verticalBanner).BackColor = Color.Transparent;
		string text = ((ListControl)serverPick).SelectedValue.ToString ();
		string serverName = ((ListControl)serverPick).GetItemText (serverPick.SelectedItem);
		((Control)serverStatusImg).Location = new Point (-16, -16);
		((Control)serverStatus).ForeColor = Color.White;
		((Control)serverStatus).Text = "Retrieving server status...";
		UpdateBannerFromState ();
		serverEnabled = false;
		loginEnabled = false;
		((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
		((Control)serverStatus).Location = new Point (40, 332);
		((Control)onlineCount).Text = string.Empty;
		UpdateBannerFromState ();
		if (((ListControl)serverPick).GetItemText (serverPick.SelectedItem) == "Offline Built-In Server") {
			builtinserver = true;
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
			((Control)loginButton).Text = "LAUNCH";
			((Control)loginButton).ForeColor = Color.FromArgb (153, 153, 153);
		} else {
			builtinserver = false;
			((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
			((Control)loginButton).Text = "LOG IN";
			((Control)loginButton).ForeColor = Color.FromArgb (153, 153, 153);
		}
		WebClientWithTimeout client = new WebClientWithTimeout ();
		client.Headers.Add ("user-agent", "GameLauncher (+https://github.com/Amaranese/GameLauncher_NFSW Classic EA)");
		Uri StringToUri;
		try {
			StringToUri = new Uri (text + "/GetServerInformation");
			client.CancelAsync ();
			((WebClient)client).DownloadStringAsync (StringToUri);
		} catch {
			SetServersCurrentlyDown ();
			return;
		}
		string numPlayers;
		client.DownloadStringCompleted += delegate(object obj4, DownloadStringCompletedEventArgs e2) {
			if (e2.Cancelled) {
				client.CancelAsync ();
			} else if (((AsyncCompletedEventArgs)e2).Error != null) {
				SetServersCurrentlyDown ();
			} else if (string.IsNullOrWhiteSpace (e2.Result)) {
				SetServersCurrentlyDown ();
			} else {
				((Control)serverStatusImg).Location = new Point (20, 323);
				((Control)serverStatusImg).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.server_online;
				((Control)serverStatus).ForeColor = Color.FromArgb (181, 255, 33);
				((Control)serverStatus).Text = "The servers are currently up and running.";
				EnsureAdBannerVisibleSafe ();
				((Control)serverStatus).Location = new Point (40, 332);
				if (serverName == "Offline Built-In Server") {
					numPlayers = "∞";
				} else if (Environment.OSVersion.Version.Major <= 5) {
					ticketRequired = true;
					verticalImageUrl = null;
					numPlayers = "Unknown";
				} else {
					GetServerInformation getServerInformation = null;
					try {
						getServerInformation = JsonConvert.DeserializeObject<GetServerInformation> (e2.Result);
					} catch {
						SetServersCurrentlyDown ();
						return;
					}
					if (getServerInformation == null) {
						SetServersCurrentlyDown ();
						return;
					}
					if (!string.IsNullOrEmpty (getServerInformation.bannerUrl)) {
						bool flag;
						try {
							flag = Uri.TryCreate (getServerInformation.bannerUrl, UriKind.Absolute, out Uri result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
						} catch {
							flag = false;
						}
						if (flag) {
							verticalImageUrl = getServerInformation.bannerUrl;
						} else {
							verticalImageUrl = null;
						}
					} else {
						verticalImageUrl = null;
					}
					if (string.IsNullOrEmpty (getServerInformation.requireTicket)) {
						ticketRequired = true;
					} else if (getServerInformation.requireTicket == "true") {
						ticketRequired = true;
					} else {
						ticketRequired = false;
					}
					numPlayers = getServerInformation.onlineNumber + " out of " + getServerInformation.numberOfRegistered;
				}
				((Control)onlineCount).Text = string.Empty;
				EnsureAdBannerVisibleSafe ();
				((Control)onlineCount).Visible = false;
				serverEnabled = true;
				if (!string.IsNullOrEmpty (verticalImageUrl)) {
					WebClientWithTimeout client2 = new WebClientWithTimeout ();
					Uri uri = new Uri (verticalImageUrl);
					((WebClient)client2).DownloadDataAsync (uri);
					client2.DownloadDataCompleted += delegate(object obj6, DownloadDataCompletedEventArgs e3) {
						if (e3.Cancelled) {
							client2.CancelAsync ();
						} else if (((AsyncCompletedEventArgs)e3).Error == null) {
							try {
								Image image = Image.FromStream ((Stream)new MemoryStream (e3.Result));
								verticalBanner.Image = image;
								((Control)verticalBanner).BackColor = Color.Black;
							} catch {
								verticalBanner.Image = null;
							}
						}
					};
				}
				if (!DetectLinux.WineDetected ()) {
					Ping ping = new Ping ();
					ping.SendAsync (StringToUri.Host, 1000, new byte[1], new PingOptions (64, dontFragment: true), new AutoResetEvent (initialState: false));
					ping.PingCompleted += delegate(object obj5, PingCompletedEventArgs e3) {
						if (e3.Reply.Status == IPStatus.Success && serverName != "Offline Built-In Server") {
							((Control)onlineCount).Visible = false;
						}
					};
				}
			}
		};
		UpdateBannerFromState ();
	}

	private void ApplyEmbeddedFonts ()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Expected O, but got Unknown
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Expected O, but got Unknown
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Expected O, but got Unknown
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Expected O, but got Unknown
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Expected O, but got Unknown
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Expected O, but got Unknown
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Expected O, but got Unknown
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Expected O, but got Unknown
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Expected O, but got Unknown
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Expected O, but got Unknown
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Expected O, but got Unknown
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Expected O, but got Unknown
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Expected O, but got Unknown
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Expected O, but got Unknown
		FontFamily fontFamily = FontWrapper.Instance.GetFontFamily ("Font_MyriadProSemiCondBold.ttf");
		FontFamily fontFamily2 = FontWrapper.Instance.GetFontFamily ("Font_Register.ttf");
		FontFamily fontFamily3 = FontWrapper.Instance.GetFontFamily ("Font_RegisterBoldItalic.ttf");
		FontFamily fontFamily4 = FontWrapper.Instance.GetFontFamily ("Font_RegisterDemiBold.ttf");
		FontWrapper.Instance.GetFontFamily ("Font_RegisterBold.ttf");
		FontWrapper.Instance.GetFontFamily ("Font_MyriadProSemiCond.ttf");
		((Control)currentWindowInfo).Font = new Font (fontFamily3, 12.75f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)2);
		((Control)rememberMe).Font = new Font (fontFamily, 9f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)1);
		((Control)loginButton).Font = new Font (fontFamily2, 15f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)3);
		((Control)registerButton).Font = new Font (fontFamily2, 15f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)3);
		((Control)settingsSave).Font = new Font (fontFamily2, 15f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)3);
		((Control)serverStatus).Font = new Font (fontFamily, 9.749999f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)1);
		((Control)onlineCount).Font = new Font (fontFamily, 9.749999f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)1);
		((Control)registerText).Font = new Font (fontFamily, 9.749999f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)1);
		((Control)emailLabel).Font = new Font (fontFamily4, 11f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX);
		((Control)passwordLabel).Font = new Font (fontFamily4, 11f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX);
		((Control)troubleLabel).Font = new Font (fontFamily, 9.749999f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)1);
		((Control)githubLink).Font = new Font (fontFamily, 9.749999f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)1);
		((Control)forgotPassword).Font = new Font (fontFamily, 9f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)1);
		((Control)selectServerLabel).Font = new Font (fontFamily, 9.749999f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)1);
		((Control)settingsLanguageText).Font = new Font (fontFamily, 9.749999f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)1);
		((Control)settingsLanguageDesc).Font = new Font (fontFamily, 9.749999f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)1);
		((Control)settingsQualityText).Font = new Font (fontFamily, 9.749999f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)1);
		((Control)settingsQualityDesc).Font = new Font (fontFamily, 9.749999f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)1);
		((Control)registerEmailText).Font = new Font (fontFamily4, 11f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX);
		((Control)registerPasswordText).Font = new Font (fontFamily4, 11f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX);
		((Control)registerConfirmPasswordText).Font = new Font (fontFamily4, 11f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX);
		((Control)registerTicketText).Font = new Font (fontFamily4, 11f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX);
		((Control)registerAgree).Font = new Font (fontFamily2, 9.749999f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)3);
		((Control)playButton).Font = new Font (fontFamily3, 18.75f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)3);
		((Control)playButton).ForeColor = Color.White;
		((ButtonBase)playButton).TextAlign = (ContentAlignment)32;
		((Control)playButton).Text = "PLAY";
	}

	private void registerText_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		try {
			string text = ((((ListControl)serverPick).SelectedValue != null) ? ((ListControl)serverPick).SelectedValue.ToString () : string.Empty);
			string text2 = (((serverPick.SelectedItem != null) ? ((ListControl)serverPick).GetItemText (serverPick.SelectedItem) : string.Empty) + " " + text).ToLowerInvariant ();
			string text3 = "https://nightriderz.world/";
			if (text2.Contains ("worldunited") || text2.Contains ("worldunited.gg")) {
				text3 = "https://worldunited.gg/";
			} else if (text2.Contains ("") || text2.Contains ("")) {
				text3 = "";
			}
			try {
				Process.Start (text3);
			} catch {
				MessageBox.Show ("Could not open " + text3, "Register");
			}
		} catch (Exception ex) {
			MessageBox.Show ("Could not open browser: " + ex.Message, "Register");
		}
	}

	private void githubLink_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e)
	{
		ConsoleLog ("Redirecting into GitHub Issue page", "info");
		Process.Start ("https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/issues");
	}

	private void forgotPassword_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start (((ListControl)serverPick).SelectedValue.ToString ().Replace ("Engine.svc", string.Empty) + "forgotPasswd.jsp");
	}

	private void LoginFormElements (bool hideElements = false)
	{
		try {
			if (promoBrowser != null) {
				((Control)promoBrowser).Visible = false;
			}
		} catch {
		}
		try {
			if (promoWebView != null) {
				((Control)promoWebView).Visible = false;
			}
		} catch {
		}
		((Control)rememberMe).Visible = hideElements;
		((Control)loginButton).Visible = hideElements;
		UpdateLoginButtonState ();
		((Control)serverStatus).Visible = hideElements;
		((Control)onlineCount).Visible = false;
		((Control)registerText).Visible = hideElements;
		((Control)serverPick).Visible = hideElements;
		((Control)serverStatusImg).Visible = hideElements;
		((Control)consoleLog).Visible = false;
		((Control)email).Visible = hideElements;
		((Control)password).Visible = hideElements;
		((Control)emailLabel).Visible = hideElements;
		((Control)passwordLabel).Visible = hideElements;
		((Control)troubleLabel).Visible = hideElements;
		((Control)githubLink).Visible = hideElements;
		((Control)forgotPassword).Visible = hideElements;
		((Control)selectServerLabel).Visible = hideElements;
		((Control)settingsButton).Visible = hideElements;
		((Control)verticalBanner).Visible = hideElements;
		try {
			if (hideElements) {
				EnsureAdBannerVisible ();
				return;
			}
			if (bottomBanner != null) {
				((Control)bottomBanner).Visible = false;
			}
			if (advertisementLabel != null) {
				((Control)advertisementLabel).Visible = false;
			}
		} catch {
		}
	}

	public bool validateEmail (string email)
	{
		string pattern = "^[\\w!#$%&'*+\\-/=?\\^_`{|}~]+(\\.[\\w!#$%&'*+\\-/=?\\^_`{|}~]+)*@((([\\-\\w]+\\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\\.){3}[0-9]{1,3}))$";
		return Regex.IsMatch (email, pattern);
	}

	private void RegisterFormElements (bool hideElements = true)
	{
		((Control)registerButton).Visible = hideElements;
		((Control)registerEmail).Visible = hideElements;
		((Control)registerEmailText).Visible = hideElements;
		((Control)registerPassword).Visible = hideElements;
		((Control)registerPasswordText).Visible = hideElements;
		((Control)registerConfirmPassword).Visible = hideElements;
		((Control)registerConfirmPasswordText).Visible = hideElements;
		((Control)registerAgree).Visible = hideElements;
		((Control)consoleLog).Visible = false;
		((Control)verticalBanner).Visible = true;
		if (ticketRequired) {
			((Control)registerTicket).Visible = hideElements;
			((Control)registerTicketText).Visible = hideElements;
		} else {
			((Control)registerTicket).Visible = false;
			((Control)registerTicketText).Visible = false;
		}
	}

	private void registerButton_MouseEnter (object sender, EventArgs e)
	{
		((ButtonBase)registerButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_hover;
	}

	private void registerButton_MouseLeave (object sender, EventArgs e)
	{
		((ButtonBase)registerButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_enable;
	}

	private void registerButton_MouseUp (object sender, EventArgs e)
	{
		((ButtonBase)registerButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_hover;
	}

	private void registerButton_MouseDown (object sender, EventArgs e)
	{
		((ButtonBase)registerButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_click;
	}

	private void registerButton_Click (object sender, EventArgs e)
	{
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa->IL02fa: Incompatible stack heights: 1 vs 2
		bool flag = true;
		ConsoleLog ("Registering... Please wait", "info");
		if (string.IsNullOrEmpty (((Control)registerEmail).Text)) {
			ConsoleLog ("Please enter your email", "error");
			flag = false;
		} else if (!validateEmail (((Control)registerEmail).Text)) {
			ConsoleLog ("Please enter your correct email", "error");
			flag = false;
		}
		if (string.IsNullOrEmpty (((Control)registerTicket).Text) && ticketRequired) {
			ConsoleLog ("Please enter your ticket", "error");
			flag = false;
		}
		if (string.IsNullOrEmpty (((Control)registerPassword).Text)) {
			ConsoleLog ("Please enter your password", "error");
			flag = false;
		}
		if (string.IsNullOrEmpty (((Control)registerConfirmPassword).Text)) {
			ConsoleLog ("Please confirm your password", "error");
			flag = false;
		}
		if (((Control)registerConfirmPassword).Text != ((Control)registerPassword).Text) {
			ConsoleLog ("Passwords doesn't match", "error");
		}
		if (!registerAgree.Checked) {
			ConsoleLog ("You have not agreed to the Terms of Service", "error");
			flag = false;
		}
		if (!flag) {
			return;
		}
		serverIP = ((ListControl)serverPick).SelectedValue.ToString ();
		((ListControl)serverPick).GetItemText (serverPick.SelectedItem);
		string empty = string.Empty;
		string empty2 = string.Empty;
		SHA1 sHA = SHA1.Create ();
		StringBuilder stringBuilder = new StringBuilder ();
		byte[] array = sHA.ComputeHash (Encoding.UTF8.GetBytes (((Control)registerPassword).Text.ToString ()));
		foreach (byte b in array) {
			stringBuilder.Append (b.ToString ("X2"));
		}
		empty = stringBuilder.ToString ();
		try {
			WebClient webClient = new WebClientWithTimeout ();
			string address = (ticketRequired ? (serverIP + "/User/createUser?email=" + ((Control)registerEmail).Text + "&password=" + empty.ToLower () + "&inviteTicket=" + ((Control)registerTicket).Text) : (serverIP + "/User/createUser?email=" + ((Control)registerEmail).Text + "&password=" + empty.ToLower ()));
			empty2 = webClient.DownloadString (address);
		} catch (WebException ex) {
			if (ex.Status == WebExceptionStatus.ProtocolError) {
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
				empty2 = ((httpWebResponse.StatusCode != HttpStatusCode.InternalServerError) ? ((Exception)(object)ex).Message : new StreamReader (((WebResponse)httpWebResponse).GetResponseStream ()).ReadToEnd ());
			} else {
				empty2 = ((Exception)(object)ex).Message;
			}
		}
		XmlDocument val = new XmlDocument ();
		object obj;
		if (string.IsNullOrWhiteSpace (empty2) || !empty2.TrimStart (Array.Empty<char> ()).StartsWith ("<")) {
			if (string.IsNullOrEmpty (ServerAuth.LastError)) {
				goto IL_02fa;
			}
			obj = ServerAuth.LastError;
			goto IL_02ff;
		}
		goto IL_0328;
		IL_02fa:
		obj = "Invalid reply";
		goto IL_02ff;
		IL_02ff:
		empty2 = "<?xml version=\"1.0\"?><LoginStatusVO><UserId>0</UserId><LoginToken></LoginToken><Description>" + ((string)obj).Replace ("&", "&amp;").Replace ("<", "&lt;") + "</Description></LoginStatusVO>";
		goto IL_0328;
		IL_0328:
		val.LoadXml (empty2);
		XmlNode val2 = ((XmlNode)val).SelectSingleNode ("LoginStatusVO/Description");
		XmlNode val3 = ((XmlNode)val).SelectSingleNode ("LoginStatusVO/LoginToken");
		XmlNode val4 = ((XmlNode)val).SelectSingleNode ("LoginStatusVO/UserId");
		if (!string.IsNullOrEmpty (val2.InnerText)) {
			/*Error near IL_02f9: ldarg 3 (out-of-bounds)*/;
			goto IL_02fa;
		}
		UserId = val4.InnerText;
		LoginToken = val3.InnerText;
		HidePlayBanners ();
		((Control)this).BackgroundImage = LoadPlayBg ();
		((Control)this).BackgroundImageLayout = (ImageLayout)0;
		((Control)currentWindowInfo).Visible = false;
		((Control)playLoggedInAs).Text = "REGISTERED IN AS " + ((Control)registerEmail).Text.ToUpper ();
		RegisterFormElements (hideElements: false);
		DownloadFormElements ();
		((Control)consoleLog).Hide ();
		launchNFSW ();
	}

	private void settingsButton_Click (object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)((Form)this).WindowState == 1) {
			((Form)this).WindowState = (FormWindowState)0;
		}
		bool num = inPlayScreen && !string.IsNullOrEmpty (UserId) && UserId != "0";
		((Control)settingsButton).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.settingsbtn_click;
		((Control)this).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.settingsbg;
		((Control)currentWindowInfo).Visible = true;
		((Control)currentWindowInfo).Text = "PLEASE SELECT YOUR GAME SETTINGS:";
		LoginFormElements ();
		DownloadFormElements (hideElements: false);
		if (num) {
			inPlayScreen = true;
			HidePlayBanners ();
		}
		try {
			if (promoBrowser != null) {
				((Control)promoBrowser).Visible = false;
			}
		} catch {
		}
		try {
			if (promoWebView != null) {
				((Control)promoWebView).Visible = false;
			}
		} catch {
		}
		((Control)playLoggedInAs).Visible = false;
		((Control)logoutButton).Visible = false;
		SettingsFormElements ();
		((Control)settingsButton).Visible = false;
		((Control)selectServerLabel).Visible = false;
		((Control)serverPick).Visible = false;
	}

	private void settingsButton_MouseEnter (object sender, EventArgs e)
	{
		((Control)settingsButton).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.settingsbtn_hover;
	}

	private void settingsButton_MouseLeave (object sender, EventArgs e)
	{
		((Control)settingsButton).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.settingsbtn;
	}

	private void settingsSave_MouseEnter (object sender, EventArgs e)
	{
		((ButtonBase)settingsSave).Image = (Image)(object)GameLauncher.Properties.Resources.button_hover;
	}

	private void settingsSave_MouseLeave (object sender, EventArgs e)
	{
		((ButtonBase)settingsSave).Image = (Image)(object)GameLauncher.Properties.Resources.button_enable;
	}

	private void settingsSave_MouseUp (object sender, EventArgs e)
	{
		((ButtonBase)settingsSave).Image = (Image)(object)GameLauncher.Properties.Resources.button_hover;
	}

	private void settingsSave_MouseDown (object sender, EventArgs e)
	{
		((ButtonBase)settingsSave).Image = (Image)(object)GameLauncher.Properties.Resources.button_click;
	}

	private void settingsSave_Click (object sender, EventArgs e)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		SettingFile.Write ("Language", ((ListControl)settingsLanguage).SelectedValue.ToString ());
		SettingFile.Write ("TracksHigh", ((ListControl)settingsQuality).SelectedValue.ToString ());
		XmlDocument val = new XmlDocument ();
		if (File.Exists (UserSettings)) {
			try {
				val.Load (UserSettings);
				((XmlNode)val).SelectSingleNode ("Settings/UI/Language").InnerText = ((ListControl)settingsLanguage).SelectedValue.ToString ();
			} catch {
				File.Delete (UserSettings);
				XmlNode val2 = ((XmlNode)val).AppendChild ((XmlNode)(object)val.CreateElement ("Settings"));
				XmlNode obj = val2.AppendChild ((XmlNode)(object)val.CreateElement ("PersistentValue")).AppendChild ((XmlNode)(object)val.CreateElement ("Chat"));
				XmlNode val3 = val2.AppendChild ((XmlNode)(object)val.CreateElement ("UI"));
				obj.InnerXml = "<DefaultChatGroup Type=\"string\">" + ((ListControl)settingsLanguage).SelectedValue.ToString () + "</DefaultChatGroup>";
				val3.InnerXml = "<Language Type=\"string\">" + ((ListControl)settingsLanguage).SelectedValue.ToString () + "</Language>";
				Directory.CreateDirectory (Path.GetDirectoryName (UserSettings));
			}
		} else {
			XmlNode val4 = ((XmlNode)val).AppendChild ((XmlNode)(object)val.CreateElement ("Settings"));
			XmlNode obj3 = val4.AppendChild ((XmlNode)(object)val.CreateElement ("PersistentValue")).AppendChild ((XmlNode)(object)val.CreateElement ("Chat"));
			XmlNode val5 = val4.AppendChild ((XmlNode)(object)val.CreateElement ("UI"));
			obj3.InnerXml = "<DefaultChatGroup Type=\"string\">" + ((ListControl)settingsLanguage).SelectedValue.ToString () + "</DefaultChatGroup>";
			val5.InnerXml = "<Language Type=\"string\">" + ((ListControl)settingsLanguage).SelectedValue.ToString () + "</Language>";
			Directory.CreateDirectory (Path.GetDirectoryName (UserSettings));
		}
		val.Save (UserSettings);
		SettingsFormElements (hideElements: false);
		if (inPlayScreen && !string.IsNullOrEmpty (UserId) && UserId != "0" && !string.IsNullOrEmpty (LoginToken)) {
			RestorePlayScreenAfterSettings ();
			return;
		}
		((Control)this).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.loginbg;
		((Control)currentWindowInfo).Visible = true;
		((Control)currentWindowInfo).Text = "ENTER YOUR ACCOUNT INFORMATION TO LOG IN:";
		LoginFormElements (hideElements: true);
		DownloadFormElements (hideElements: false);
		EnsureAdBannerVisible ();
		inPlayScreen = false;
	}

	private static void EnsureEaFonts ()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		if (eaMyriadBold != null) {
			return;
		}
		try {
			eaPrivateFonts = new PrivateFontCollection ();
			using Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("GameLauncher.Fonts.Font_MyriadProSemiCondBold.ttf");
			if (stream == null) {
				return;
			}
			byte[] array = new byte[stream.Length];
			stream.Read (array, 0, array.Length);
			GCHandle gCHandle = GCHandle.Alloc (array, GCHandleType.Pinned);
			try {
				eaPrivateFonts.AddMemoryFont (gCHandle.AddrOfPinnedObject (), array.Length);
				if (((FontCollection)eaPrivateFonts).Families.Length != 0) {
					eaMyriadBold = ((FontCollection)eaPrivateFonts).Families [0];
				}
			} finally {
				gCHandle.Free ();
			}
		} catch {
		}
	}

	private static Font GetAdvertisementFont ()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		EnsureEaFonts ();
		if (eaMyriadBold == null) {
			return new Font ("Microsoft Sans Serif", 8f, (FontStyle)1, (GraphicsUnit)3);
		}
		return new Font (eaMyriadBold, 8f, (FontStyle)1, (GraphicsUnit)3);
	}

	private void EnsureAdBannerVisible ()
	{
		if (inPlayScreen) {
			HidePlayBanners ();
			UpdateBannerFromState ();
			return;
		}
		try {
			if (bottomBanner != null) {
				if (bottomBanner.Image == null) {
					Image val = LoadEmbedded ("GameLauncher.Banner.bottom_banner.png");
					if (val != null) {
						bottomBanner.Image = val;
					}
				}
				((Control)bottomBanner).Visible = IsServerOnline ();
				((Control)bottomBanner).BringToFront ();
			}
			if (advertisementLabel != null) {
				((Control)advertisementLabel).Font = GetAdvertisementFont ();
				((Control)advertisementLabel).ForeColor = Color.FromArgb (69, 133, 193);
				((Control)advertisementLabel).Location = new Point (308, 375);
				((Control)advertisementLabel).Size = new Size (200, 13);
				((Control)advertisementLabel).Text = "ADVERTISEMENT";
				advertisementLabel.TextAlign = (ContentAlignment)32;
				((Control)advertisementLabel).Visible = true;
				((Control)advertisementLabel).BringToFront ();
			}
		} catch {
		}
	}

	private void RestorePlayScreenAfterSettings ()
	{
		HidePlayBanners ();
		try {
			inPlayScreen = true;
			HidePlayBanners ();
			HidePlayBanners ();
			((Control)this).BackgroundImage = LoadPlayBg ();
			((Control)this).BackgroundImageLayout = (ImageLayout)0;
			((Control)currentWindowInfo).Visible = false;
			((Control)rememberMe).Visible = false;
			((Control)loginButton).Visible = false;
			((Control)serverStatus).Visible = false;
			((Control)onlineCount).Visible = false;
			((Control)registerText).Visible = false;
			((Control)serverStatusImg).Visible = false;
			((Control)email).Visible = false;
			((Control)password).Visible = false;
			((Control)emailLabel).Visible = false;
			((Control)passwordLabel).Visible = false;
			((Control)troubleLabel).Visible = false;
			((Control)githubLink).Visible = false;
			((Control)forgotPassword).Visible = false;
			((Control)verticalBanner).Visible = false;
			((Control)consoleLog).Visible = false;
			if (bottomBanner != null) {
				((Control)bottomBanner).Visible = false;
			}
			if (advertisementLabel != null) {
				((Control)advertisementLabel).Visible = false;
			}
			SettingsFormElements (hideElements: false);
			((Control)playLoggedInAs).Visible = false;
			((Control)logoutButton).Visible = false;
			((Control)playProgress).Visible = true;
			((Control)playProgressText).Visible = true;
			((Control)playButton).Visible = true;
			((Control)playProgressTime).Visible = true;
			((Control)selectServerLabel).Visible = true;
			((Control)serverPick).Visible = true;
			((Control)settingsButton).Visible = true;
			if (promoWebView != null) {
				((Control)promoWebView).Visible = false;
			}
			if (promoBrowser != null) {
				((Control)promoBrowser).Visible = true;
				((Control)promoBrowser).Location = new Point (10, 95);
				((Control)promoBrowser).Size = new Size (771, 310);
				((Control)promoBrowser).BringToFront ();
				ShowPromoPanel ();
			}
			((Control)playProgress).BringToFront ();
			((Control)playProgressText).BringToFront ();
			((Control)playButton).BringToFront ();
			((Control)selectServerLabel).BringToFront ();
			((Control)serverPick).BringToFront ();
			((Control)settingsButton).BringToFront ();
			((Control)this).Invalidate (true);
			((Control)this).Update ();
		} catch (Exception ex) {
			try {
				ConsoleLog ("RestorePlayScreen: " + ex.Message, "error");
			} catch {
			}
		}
	}

	private void SettingsFormElements (bool hideElements = true)
	{
		((Control)settingsSave).Visible = hideElements;
		((Control)settingsLanguage).Visible = hideElements;
		((Control)settingsLanguageText).Visible = hideElements;
		((Control)settingsLanguageDesc).Visible = hideElements;
		((Control)settingsQuality).Visible = hideElements;
		((Control)settingsQualityText).Visible = hideElements;
		((Control)settingsQualityDesc).Visible = hideElements;
	}

	private async void InitPromoWebView2 ()
	{
		if (promoWebViewReady || !promoUseWebView2) {
			return;
		}
		try {
			if (promoWebView == null) {
				promoWebView = new WebView2 ();
				((Control)promoWebView).Name = "promoWebView";
				((Control)promoWebView).Location = new Point (10, 95);
				((Control)promoWebView).Size = new Size (771, 310);
				((Control)promoWebView).Visible = false;
				((Control)this).Controls.Add ((Control)promoWebView);
			}
			await promoWebView.EnsureCoreWebView2Async (null);
			promoWebViewReady = true;
			promoWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
			promoWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
			promoWebView.CoreWebView2.NavigationCompleted += delegate(object s, CoreWebView2NavigationCompletedEventArgs e) {
				try {
					if (e.IsSuccess) {
						((Control)promoWebView).Visible = true;
						((Control)promoWebView).BringToFront ();
						if (promoIeBrowser != null) {
							((Control)promoIeBrowser).Visible = false;
						}
						if (promoBrowser != null) {
							((Control)promoBrowser).Visible = false;
						}
					}
				} catch {
				}
			};
			promoWebView.CoreWebView2.NewWindowRequested += delegate(object s, CoreWebView2NewWindowRequestedEventArgs e) {
				try {
					e.Handled = true;
					string text = e.Uri ?? "";
					string text2 = text.ToLowerInvariant ();
					if (text2.Contains ("nightriderz") || text2.Contains ("worldunited") || text2.Contains ("sbrw")) {
						try {
							promoWebView.CoreWebView2.Navigate (text);
							return;
						} catch {
							return;
						}
					}
					if (!string.IsNullOrEmpty (text)) {
						Process.Start (text);
					}
				} catch {
				}
			};
			try {
				promoWebView.CoreWebView2.Navigate (GetPromoSiteUrl ());
			} catch {
			}
		} catch (Exception ex) {
			promoUseWebView2 = false;
			promoWebViewReady = false;
			try {
				if (promoWebView != null) {
					try {
						((Control)promoWebView).Visible = false;
					} catch {
					}
					try {
						((Control)this).Controls.Remove ((Control)promoWebView);
					} catch {
					}
					try {
						((Component)promoWebView).Dispose ();
					} catch {
					}
					promoWebView = null;
				}
			} catch {
			}
			try {
				ConsoleLog ("WebView2 skipped: " + ex.Message, "info");
			} catch {
			}
		}
	}

	private void UnloadPromoToSaveMemory ()
	{
		try {
			if (promoBrowser != null) {
				((Control)promoBrowser).Visible = false;
			}
		} catch {
		}
		try {
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			GC.Collect ();
		} catch {
		}
	}

	private void PromoIeBrowser_Navigating (object sender, WebBrowserNavigatingEventArgs e)
	{
		try {
			string text = ((e.Url != null) ? e.Url.ToString () : "");
			if (string.IsNullOrEmpty (text) || text.StartsWith ("about:", StringComparison.OrdinalIgnoreCase) || (!text.StartsWith ("http://", StringComparison.OrdinalIgnoreCase) && !text.StartsWith ("https://", StringComparison.OrdinalIgnoreCase))) {
				return;
			}
			((CancelEventArgs)(object)e).Cancel = true;
			try {
				Process.Start (text);
			} catch {
			}
		} catch {
		}
	}

	private string GetPromoSiteUrl ()
	{
		string text = string.Empty;
		try {
			if (serverPick != null && serverPick.SelectedItem != null) {
				text = ((ListControl)serverPick).GetItemText (serverPick.SelectedItem) ?? string.Empty;
			}
		} catch {
		}
		string text2 = (text + " ").ToLowerInvariant ();
		if (text2.Contains ("worldunited") || text2.Contains ("world united")) {
			return "https://worldunited.gg/";
		}
		return "https://nightriderz.world/";
	}

	private void LoadPromoWebkitHtml ()
	{
		try {
			LoadChallengesWebViewInPanel ();
		} catch {
		}
	}

	private void ShowChallengesFallbackMessage ()
	{
		SafePromoUI (delegate {
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Expected O, but got Unknown
			EnsurePromoHosts ();
			ShowNativePromo ();
			int num = 0;
			int num2 = 0;
			try {
				num = NrzChallenges.LastServerYear;
				num2 = NrzChallenges.LastServerWeek;
			} catch {
			}
			string text = "";
			try {
				text = NrzChallenges.LastError ?? "";
			} catch {
			}
			if (promoStatusLabel != null) {
				((Control)promoStatusLabel).Text = "  WEEKLY CHALLENGES  ·  LIVE";
			}
			if (promoCards != null) {
				((Control)promoCards).Controls.Clear ();
				Label val = new Label ();
				((Control)val).Size = new Size (740, 260);
				((Control)val).ForeColor = Color.FromArgb (200, 200, 200);
				((Control)val).BackColor = Color.FromArgb (26, 29, 36);
				((Control)val).Font = new Font ("Segoe UI", 9.5f);
				val.TextAlign = (ContentAlignment)32;
				((Control)val).Text = "Could not load challenge cards in-panel.\n" + ((num2 > 0) ? ("Server week: " + num + " W" + num2 + "\n") : "") + text + "\n\nInstall Microsoft Edge WebView2 Runtime, then restart the launcher.";
				((Control)promoCards).Controls.Add ((Control)(object)val);
			}
		});
	}

	private void SafePromoUI (Action action)
	{
		try {
			if (!((Control)this).IsDisposed) {
				if (((Control)this).InvokeRequired) {
					((Control)this).BeginInvoke ((Delegate)action);
				} else {
					action ();
				}
			}
		} catch {
		}
	}

	private void ShowNativePromo ()
	{
		try {
			try {
				if (promoWebView != null) {
					((Control)promoWebView).Visible = false;
					((Control)promoWebView).SendToBack ();
				}
			} catch {
			}
			try {
				if (promoIeBrowser != null) {
					((Control)promoIeBrowser).Visible = false;
					((Control)promoIeBrowser).SendToBack ();
				}
			} catch {
			}
			if (promoBrowser != null) {
				((Control)promoBrowser).BackColor = Color.FromArgb (26, 29, 36);
				((Control)promoBrowser).Visible = true;
				((Control)promoBrowser).BringToFront ();
			}
			if (promoCards != null) {
				((Control)promoCards).BackColor = Color.FromArgb (26, 29, 36);
				((Control)promoCards).Visible = true;
				((Control)promoCards).BringToFront ();
			}
			if (promoStatusLabel != null) {
				((Control)promoStatusLabel).BackColor = Color.FromArgb (26, 29, 36);
				((Control)promoStatusLabel).ForeColor = Color.FromArgb (180, 186, 192);
				((Control)promoStatusLabel).Visible = true;
				((Control)promoStatusLabel).BringToFront ();
			}
		} catch {
		}
	}

	private async void LoadChallengesWebViewInPanel ()
	{
		string site = "https://nightriderz.world/challenges";
		try {
			if (((Control)this).InvokeRequired) {
				((Control)this).BeginInvoke ((Delegate)(Action)delegate {
					LoadChallengesWebViewInPanel ();
				});
				return;
			}
			try {
				if ((GetPromoSiteUrl () ?? "").IndexOf ("worldunited", StringComparison.OrdinalIgnoreCase) >= 0) {
					site = "https://worldunited.gg/";
				}
			} catch {
			}
			string u = "";
			string p = "";
			try {
				u = NrzChallenges.SessionU ?? "";
			} catch {
			}
			try {
				p = NrzChallenges.SessionP ?? "";
			} catch {
			}
			if (promoWebView == null) {
				promoWebView = new WebView2 ();
				((Control)promoWebView).Name = "promoWebView";
				((Control)promoWebView).Location = new Point (10, 95);
				((Control)promoWebView).Size = new Size (771, 310);
				((Control)promoWebView).Visible = false;
				try {
					promoWebView.DefaultBackgroundColor = Color.FromArgb (26, 29, 36);
				} catch {
				}
				((Control)this).Controls.Add ((Control)promoWebView);
			}
			((Control)promoWebView).Location = new Point (10, 95);
			((Control)promoWebView).Size = new Size (771, 310);
			try {
				promoWebView.DefaultBackgroundColor = Color.FromArgb (26, 29, 36);
			} catch {
			}
			string text = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "NFSWLauncher", "WebView2");
			try {
				Directory.CreateDirectory (text);
			} catch {
			}
			if (!promoWebViewReady || promoWebView.CoreWebView2 == null) {
				CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync (null, text);
				await promoWebView.EnsureCoreWebView2Async (environment);
				promoWebViewReady = true;
				promoWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
				promoWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
				promoWebView.CoreWebView2.Settings.IsStatusBarEnabled = false;
				promoWebView.CoreWebView2.NewWindowRequested += delegate(object s, CoreWebView2NewWindowRequestedEventArgs e) {
					try {
						e.Handled = true;
						if (!string.IsNullOrEmpty (e.Uri)) {
							promoWebView.CoreWebView2.Navigate (e.Uri);
						}
					} catch {
					}
				};
				promoWebView.CoreWebView2.NavigationCompleted += delegate {
					try {
						((Control)promoWebView).Visible = true;
						((Control)promoWebView).BringToFront ();
						if (promoBrowser != null) {
							((Control)promoBrowser).Visible = false;
						}
						if (promoCards != null) {
							((Control)promoCards).Visible = false;
						}
						if (promoStatusLabel != null) {
							((Control)promoStatusLabel).Visible = false;
						}
					} catch {
					}
				};
			}
			if (!string.IsNullOrEmpty (u) && !string.IsNullOrEmpty (p) && site.IndexOf ("nightriderz", StringComparison.OrdinalIgnoreCase) >= 0) {
				try {
					string text2 = Uri.EscapeDataString (u);
					string text3 = Uri.EscapeDataString (p);
					string javaScript = "try{localStorage.setItem('easharpptr_u',decodeURIComponent('" + text2 + "'));localStorage.setItem('easharpptr_p',decodeURIComponent('" + text3 + "'));}catch(e){}";
					await promoWebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync (javaScript);
				} catch {
				}
			}
			try {
				CookieContainer sessionCookies = NrzChallenges.GetSessionCookies ();
				if (sessionCookies != null && promoWebView.CoreWebView2 != null) {
					Uri[] array = new Uri[2] {
						new Uri ("https://nightriderz.world/"),
						new Uri ("https://api.nightriderz.world/")
					};
					foreach (Uri uri in array) {
						foreach (Cookie cookie in sessionCookies.GetCookies (uri)) {
							try {
								CoreWebView2Cookie coreWebView2Cookie = promoWebView.CoreWebView2.CookieManager.CreateCookie (cookie.Name, cookie.Value, cookie.Domain.TrimStart (new char[1] { '.' }), string.IsNullOrEmpty (cookie.Path) ? "/" : cookie.Path);
								coreWebView2Cookie.IsHttpOnly = cookie.HttpOnly;
								coreWebView2Cookie.IsSecure = cookie.Secure;
								promoWebView.CoreWebView2.CookieManager.AddOrUpdateCookie (coreWebView2Cookie);
							} catch {
							}
						}
					}
				}
			} catch {
			}
			promoWebView.CoreWebView2.Navigate (site);
			((Control)promoWebView).Visible = true;
			((Control)promoWebView).BringToFront ();
		} catch (Exception ex) {
			string message = ex.Message;
			try {
				ConsoleLog ("Challenges WebView2: " + message, "error");
			} catch {
			}
			try {
				bool flag;
				try {
					if (promoIeBrowser == null) {
						promoIeBrowser = new WebBrowser ();
						((Control)promoIeBrowser).Name = "promoIeBrowser";
						((Control)promoIeBrowser).Location = new Point (10, 95);
						((Control)promoIeBrowser).Size = new Size (771, 310);
						promoIeBrowser.ScriptErrorsSuppressed = true;
						((Control)this).Controls.Add ((Control)promoIeBrowser);
					}
					((Control)promoIeBrowser).Location = new Point (10, 95);
					((Control)promoIeBrowser).Size = new Size (771, 310);
					promoIeBrowser.Navigate (site);
					((Control)promoIeBrowser).Visible = true;
					((Control)promoIeBrowser).BringToFront ();
					if (promoWebView != null) {
						((Control)promoWebView).Visible = false;
					}
					flag = true;
				} catch {
					flag = false;
				}
				if (flag) {
					return;
				}
				try {
					string nrzEmailCache = _nrzEmailCache;
					string nrzPassCache = _nrzPassCache;
					if (string.IsNullOrEmpty (nrzPassCache)) {
						nrzPassCache = plainPasswordMemory;
					}
					List<NrzChallenges.ChallengeInfo> list = NrzChallenges.FetchThisWeek (nrzEmailCache, nrzPassCache);
					EnsurePromoHosts ();
					if (promoStatusLabel != null) {
						((Control)promoStatusLabel).Visible = true;
						((Control)promoStatusLabel).Text = "  WEEKLY CHALLENGES";
					}
					if (promoCards != null) {
						((Control)promoCards).Visible = true;
						((Control)promoCards).Controls.Clear ();
						if (list != null && list.Count > 0) {
							foreach (NrzChallenges.ChallengeInfo item in list) {
								((Control)promoCards).Controls.Add ((Control)(object)BuildChallengeCard (item));
							}
						} else {
							Label val = new Label ();
							((Control)val).Size = new Size (740, 260);
							((Control)val).ForeColor = Color.FromArgb (200, 200, 200);
							((Control)val).BackColor = Color.FromArgb (26, 29, 36);
							((Control)val).Font = new Font ("Segoe UI", 9.5f);
							val.TextAlign = (ContentAlignment)32;
							((Control)val).Text = "Could not open the challenges page in-panel.\n\nInstall Microsoft Edge WebView2 Runtime (x86/32-bit):\nhttps://developer.microsoft.com/microsoft-edge/webview2/\n\n" + message;
							((Control)promoCards).Controls.Add ((Control)(object)val);
						}
					}
					if (promoBrowser != null) {
						((Control)promoBrowser).Visible = true;
						((Control)promoBrowser).BringToFront ();
					}
				} catch (Exception ex2) {
					try {
						ConsoleLog ("Challenges fallback: " + ex2.Message, "error");
					} catch {
					}
				}
			} catch {
			}
		}
	}

	private Panel BuildChallengeCard (NrzChallenges.ChallengeInfo c)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Expected O, but got Unknown
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Expected O, but got Unknown
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Expected O, but got Unknown
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Expected O, but got Unknown
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Expected O, but got Unknown
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Expected O, but got Unknown
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Expected O, but got Unknown
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Expected O, but got Unknown
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Expected O, but got Unknown
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Expected O, but got Unknown
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Expected O, but got Unknown
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Expected O, but got Unknown
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Expected O, but got Unknown
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Expected O, but got Unknown
		Panel val = new Panel ();
		((Control)val).Size = new Size (160, 300);
		((Control)val).BackColor = Color.FromArgb (36, 40, 50);
		((Control)val).Margin = new Padding (5);
		PictureBox pic = new PictureBox ();
		((Control)pic).Location = new Point (0, 0);
		((Control)pic).Size = new Size (160, 90);
		pic.SizeMode = (PictureBoxSizeMode)4;
		((Control)pic).BackColor = Color.FromArgb (20, 22, 28);
		if (!string.IsNullOrEmpty (c.ImageUrl)) {
			string imgUrl = c.ImageUrl;
			ThreadPool.QueueUserWorkItem (delegate {
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Expected O, but got Unknown
				try {
					WebClient webClient = new WebClient ();
					try {
						webClient.Headers [HttpRequestHeader.UserAgent] = "Mozilla/5.0";
						webClient.Headers.Add ("Referer", "https://nightriderz.world/challenges");
						using MemoryStream memoryStream = new MemoryStream (webClient.DownloadData (imgUrl));
						Image val11 = Image.FromStream ((Stream)memoryStream);
						Image clone = (Image)new Bitmap (val11);
						try {
							if (!((Control)pic).IsDisposed) {
								((Control)pic).BeginInvoke ((Delegate)(Action)delegate {
									try {
										pic.Image = clone;
									} catch {
										try {
											clone.Dispose ();
										} catch {
										}
									}
								});
							}
						} catch {
							try {
								clone.Dispose ();
							} catch {
							}
						}
					} finally {
						((IDisposable)webClient)?.Dispose ();
					}
				} catch {
				}
			});
		}
		((Control)val).Controls.Add ((Control)(object)pic);
		Label val2 = new Label ();
		((Control)val2).AutoSize = true;
		((Control)val2).Location = new Point (6, 6);
		((Control)val2).Font = new Font ("Segoe UI", 7f, (FontStyle)1);
		((Control)val2).ForeColor = Color.White;
		string text = (c.Difficulty ?? "MEDIUM").ToUpperInvariant ();
		((Control)val2).Text = " " + text + " ";
		if (text.Contains ("VERY HARD")) {
			((Control)val2).BackColor = Color.FromArgb (160, 30, 50);
		} else if (text.Contains ("HARD") && !text.Contains ("VERY")) {
			((Control)val2).BackColor = Color.FromArgb (200, 80, 30);
		} else if (text.Contains ("MEDIUM")) {
			((Control)val2).BackColor = Color.FromArgb (40, 100, 180);
		} else if (text.Contains ("VERY EASY")) {
			((Control)val2).BackColor = Color.FromArgb (40, 160, 90);
		} else {
			((Control)val2).BackColor = Color.FromArgb (50, 140, 80);
		}
		((Control)val).Controls.Add ((Control)(object)val2);
		((Control)val2).BringToFront ();
		Label val3 = new Label ();
		((Control)val3).AutoSize = true;
		((Control)val3).Location = new Point (6, 70);
		((Control)val3).Font = new Font ("Segoe UI", 7f);
		((Control)val3).ForeColor = Color.FromArgb (200, 200, 200);
		((Control)val3).BackColor = Color.FromArgb (0, 0, 0, 120);
		((Control)val3).Text = ((c.Id > 0) ? (" #" + c.Id + " ") : "");
		((Control)val).Controls.Add ((Control)(object)val3);
		((Control)val3).BringToFront ();
		int num = 96;
		Label val4 = new Label ();
		((Control)val4).Location = new Point (6, num);
		((Control)val4).Size = new Size (148, 16);
		((Control)val4).Font = new Font ("Segoe UI", 7f);
		((Control)val4).ForeColor = Color.FromArgb (160, 170, 180);
		((Control)val4).BackColor = Color.FromArgb (36, 40, 50);
		((Control)val4).Text = c.Players ?? "";
		((Control)val).Controls.Add ((Control)(object)val4);
		num += 18;
		Label val5 = new Label ();
		((Control)val5).Location = new Point (6, num);
		((Control)val5).Size = new Size (148, 32);
		((Control)val5).Font = new Font ("Segoe UI", 7.5f, (FontStyle)1);
		((Control)val5).ForeColor = Color.White;
		((Control)val5).BackColor = Color.FromArgb (36, 40, 50);
		string text2 = c.Mode ?? "TIME-ATTACK MODE";
		string text3 = c.Name ?? "";
		((Control)val5).Text = text2 + "\n" + text3;
		((Control)val).Controls.Add ((Control)(object)val5);
		num += 34;
		if (!string.IsNullOrEmpty (c.TargetTime)) {
			Label val6 = new Label ();
			((Control)val6).Location = new Point (6, num);
			((Control)val6).Size = new Size (148, 14);
			((Control)val6).Font = new Font ("Segoe UI", 7f);
			((Control)val6).ForeColor = Color.FromArgb (180, 190, 200);
			((Control)val6).BackColor = Color.FromArgb (36, 40, 50);
			((Control)val6).Text = c.TargetTime;
			((Control)val).Controls.Add ((Control)(object)val6);
			num += 16;
		}
		if (!string.IsNullOrEmpty (c.Car)) {
			Label val7 = new Label ();
			((Control)val7).Location = new Point (6, num);
			((Control)val7).Size = new Size (148, 14);
			((Control)val7).Font = new Font ("Segoe UI", 7f);
			((Control)val7).ForeColor = Color.FromArgb (180, 190, 200);
			((Control)val7).BackColor = Color.FromArgb (36, 40, 50);
			((Control)val7).Text = c.Car;
			((Control)val).Controls.Add ((Control)(object)val7);
			num += 16;
		}
		Label val8 = new Label ();
		((Control)val8).Location = new Point (6, num);
		((Control)val8).Size = new Size (148, 14);
		((Control)val8).Font = new Font ("Segoe UI", 7f);
		((Control)val8).ForeColor = Color.FromArgb (180, 190, 200);
		((Control)val8).BackColor = Color.FromArgb (36, 40, 50);
		((Control)val8).Text = (string.IsNullOrEmpty (c.Powerups) ? "POWERUPS NOT ALLOWED" : c.Powerups);
		((Control)val).Controls.Add ((Control)(object)val8);
		num += 18;
		Label val9 = new Label ();
		((Control)val9).Location = new Point (6, Math.Min (num, 250));
		((Control)val9).Size = new Size (148, 20);
		((Control)val9).Font = new Font ("Segoe UI", 8f, (FontStyle)1);
		((Control)val9).ForeColor = Color.White;
		((Control)val9).BackColor = Color.FromArgb (200, 140, 20);
		val9.TextAlign = (ContentAlignment)32;
		((Control)val9).Text = (string.IsNullOrEmpty (c.Reward) ? "" : c.Reward);
		((Control)val).Controls.Add ((Control)(object)val9);
		Label val10 = new Label ();
		((Control)val10).Location = new Point (0, 272);
		((Control)val10).Size = new Size (160, 28);
		((Control)val10).Font = new Font ("Segoe UI", 8f, (FontStyle)1);
		val10.TextAlign = (ContentAlignment)32;
		string text4 = c.Status ?? "NOT COMPLETED";
		if (c.IsCompleted || text4.Equals ("COMPLETED", StringComparison.OrdinalIgnoreCase)) {
			((Control)val10).Text = "COMPLETED";
			((Control)val10).ForeColor = Color.White;
			((Control)val10).BackColor = Color.FromArgb (30, 130, 70);
		} else if (text4.IndexOf ("CLAIM", StringComparison.OrdinalIgnoreCase) >= 0) {
			((Control)val10).Text = "CLAIM REWARD";
			((Control)val10).ForeColor = Color.White;
			((Control)val10).BackColor = Color.FromArgb (180, 120, 20);
		} else {
			((Control)val10).Text = "NOT COMPLETED";
			((Control)val10).ForeColor = Color.White;
			((Control)val10).BackColor = Color.FromArgb (180, 45, 55);
		}
		((Control)val).Controls.Add ((Control)(object)val10);
		return val;
	}

	private int AddCardRow (Panel card, int y, string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		Label val = new Label ();
		((Control)val).Location = new Point (6, y);
		((Control)val).Size = new Size (136, 28);
		((Control)val).ForeColor = Color.FromArgb (232, 234, 237);
		((Control)val).BackColor = Color.FromArgb (52, 59, 73);
		((Control)val).Font = new Font ("Segoe UI", 7f);
		((Control)val).Text = text ?? "";
		((Control)card).Controls.Add ((Control)(object)val);
		return y + 30;
	}

	private void ShowPromoPanel ()
	{
		try {
			try {
				if (promoCards != null) {
					((Control)promoCards).Visible = false;
				}
				if (promoStatusLabel != null) {
					((Control)promoStatusLabel).Visible = false;
				}
				if (promoIeBrowser != null) {
					((Control)promoIeBrowser).Visible = false;
				}
				if (promoBrowser != null) {
					((Control)promoBrowser).Visible = false;
				}
			} catch {
			}
			ThreadPool.QueueUserWorkItem (delegate {
				try {
					string nrzEmailCache = _nrzEmailCache;
					string nrzPassCache = _nrzPassCache;
					if (string.IsNullOrEmpty (nrzPassCache) && !string.IsNullOrEmpty (plainPasswordMemory)) {
						nrzPassCache = plainPasswordMemory;
					}
					if (!string.IsNullOrEmpty (nrzEmailCache) && !string.IsNullOrEmpty (nrzPassCache)) {
						try {
							NrzChallenges.WarmSession (nrzEmailCache, nrzPassCache);
						} catch {
						}
					}
				} catch {
				}
				try {
					if (!((Control)this).IsDisposed) {
						((Control)this).BeginInvoke ((Delegate)(Action)delegate {
							try {
								LoadChallengesWebViewInPanel ();
							} catch {
							}
						});
					}
				} catch {
				}
			});
		} catch {
		}
	}

	private void EnsurePromoHosts ()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		if (promoBrowser != null) {
			try {
				((Control)promoBrowser).BackColor = Color.FromArgb (26, 29, 36);
				((Control)promoBrowser).Location = new Point (10, 95);
				((Control)promoBrowser).Size = new Size (771, 310);
			} catch {
			}
			if (promoStatusLabel == null) {
				promoStatusLabel = new Label ();
				((Control)promoStatusLabel).Name = "promoStatusLabel";
				((Control)promoStatusLabel).Dock = (DockStyle)1;
				((Control)promoStatusLabel).Height = 24;
				((Control)promoStatusLabel).ForeColor = Color.FromArgb (180, 186, 192);
				((Control)promoStatusLabel).BackColor = Color.FromArgb (26, 29, 36);
				((Control)promoStatusLabel).Font = new Font ("Segoe UI", 8.25f, (FontStyle)1);
				promoStatusLabel.TextAlign = (ContentAlignment)16;
				((Control)promoBrowser).Controls.Add ((Control)(object)promoStatusLabel);
			}
			((Control)promoStatusLabel).BackColor = Color.FromArgb (26, 29, 36);
			((Control)promoStatusLabel).Visible = true;
			if (promoCards == null) {
				promoCards = new FlowLayoutPanel ();
				((Control)promoCards).Name = "promoCards";
				((Control)promoCards).Dock = (DockStyle)5;
				((ScrollableControl)promoCards).AutoScroll = true;
				promoCards.WrapContents = true;
				promoCards.FlowDirection = (FlowDirection)0;
				((Control)promoCards).BackColor = Color.FromArgb (26, 29, 36);
				((Control)promoCards).Padding = new Padding (8, 6, 8, 6);
				((Control)promoBrowser).Controls.Add ((Control)(object)promoCards);
			}
			((Control)promoCards).BackColor = Color.FromArgb (26, 29, 36);
			((Control)promoCards).Visible = true;
			((Control)promoStatusLabel).BringToFront ();
		}
	}

	private void DownloadFormElements (bool hideElements = true)
	{
		if (hideElements) {
			HidePlayBanners ();
		}
		((Control)playLoggedInAs).Visible = false;
		((Control)logoutButton).Visible = false;
		if (hideElements) {
			inPlayScreen = true;
			HidePlayBanners ();
			((Control)selectServerLabel).Visible = true;
			((Control)serverPick).Visible = true;
			((Control)settingsButton).Visible = true;
			try {
				((Control)selectServerLabel).BringToFront ();
				((Control)serverPick).BringToFront ();
				((Control)settingsButton).BringToFront ();
			} catch {
			}
		}
		((Control)playProgress).Visible = hideElements;
		((Control)playProgressText).Visible = hideElements;
		((Control)playButton).Visible = hideElements;
		((Control)playProgressTime).Visible = hideElements;
		try {
			if (hideElements) {
				PromoFirstShowDelayed ();
				if (promoBrowser != null) {
					((Control)promoBrowser).Visible = true;
					if (promoWebView != null) {
						((Control)promoWebView).Visible = false;
					}
					((Control)promoBrowser).BringToFront ();
				}
				((Control)playProgress).BringToFront ();
				((Control)playProgressText).BringToFront ();
				((Control)playButton).BringToFront ();
			} else {
				if (promoBrowser != null) {
					((Control)promoBrowser).Visible = false;
				}
				if (promoWebView != null) {
					((Control)promoWebView).Visible = false;
				}
			}
		} catch {
		}
		try {
			if (bottomBanner != null) {
				((Control)bottomBanner).Visible = false;
			}
			if (advertisementLabel != null) {
				((Control)advertisementLabel).Visible = false;
			}
		} catch {
		}
	}

	private void mainScreen_FormClosing (object sender, FormClosingEventArgs e)
	{
		try {
			LocalProxy.Stop ();
		} catch {
		}
		try {
			SbrwProxy.Stop ();
		} catch {
		}
		try {
			AntiCheat.DisableChecks ();
		} catch {
		}
		try {
			DiscordPresence.Clear ();
		} catch {
		}
		try {
			Process[] processesByName = Process.GetProcessesByName ("nfsw");
			foreach (Process process in processesByName) {
				try {
					process.Kill ();
				} catch {
				}
			}
		} catch {
		}
		try {
			Environment.Exit (0);
		} catch {
		}
	}

	private void logoutButton_Click (object sender, EventArgs e)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		try {
			StopPlayBlink ();
		} catch {
		}
		LoginToken = string.Empty;
		UserId = string.Empty;
		playenabled = false;
		inPlayScreen = false;
		plainPasswordMemory = string.Empty;
		((Control)this).BackgroundImage = (Image)GameLauncher.Properties.Resources.loginbg;
		((Control)currentWindowInfo).Visible = true;
		((Control)currentWindowInfo).Text = "ENTER YOUR ACCOUNT INFORMATION TO LOG IN:";
		RegisterFormElements (hideElements: false);
		SettingsFormElements (hideElements: false);
		DownloadFormElements (hideElements: false);
		LoginFormElements (hideElements: true);
		((Control)logoutButton).Visible = false;
		((Control)playLoggedInAs).Visible = false;
		ConsoleLog ("Logged out.", "info");
		UpdateBannerFromState ();
		EnsureAdBannerVisible ();
	}

	private void SetLogoutImage (string name)
	{
		try {
			string logicalName = "GameLauncher.Logout." + name.Replace (".png", "") + ".png";
			if (name.StartsWith ("logout")) {
				logicalName = "GameLauncher.Logout." + name;
			}
			Image val = LoadEmbedded (logicalName);
			if (val != null) {
				((ButtonBase)logoutButton).Image = val;
			}
		} catch {
		}
		((Control)logoutButton).ForeColor = Color.White;
		((Control)logoutButton).BackColor = Color.Transparent;
	}

	private void logoutButton_MouseEnter (object sender, EventArgs e)
	{
		SetLogoutImage ("logout1.png");
	}

	private void logoutButton_MouseLeave (object sender, EventArgs e)
	{
		SetLogoutImage ("logout0.png");
	}

	private void logoutButton_MouseDown (object sender, MouseEventArgs e)
	{
		SetLogoutImage ("logout2.png");
	}

	private void logoutButton_MouseUp (object sender, MouseEventArgs e)
	{
		SetLogoutImage ("logout1.png");
	}

	[DllImport ("user32.dll")]
	private static extern bool EnumDisplaySettings (string deviceName, int modeNum, ref DevMode devMode);

	[DllImport ("user32.dll")]
	private static extern int ChangeDisplaySettings (ref DevMode devMode, int flags);

	private void ApplyFullscreen240Hz ()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		try {
			string userSettings = UserSettings;
			string directoryName = Path.GetDirectoryName (userSettings);
			if (!string.IsNullOrEmpty (directoryName) && !Directory.Exists (directoryName)) {
				Directory.CreateDirectory (directoryName);
			}
			XmlDocument val = new XmlDocument ();
			if (File.Exists (userSettings)) {
				try {
					val.Load (userSettings);
				} catch {
					val = new XmlDocument ();
				}
			}
			XmlNode val2 = ((XmlNode)val).SelectSingleNode ("Settings");
			if (val2 == null) {
				((XmlNode)val).RemoveAll ();
				val2 = ((XmlNode)val).AppendChild ((XmlNode)(object)val.CreateElement ("Settings"));
			}
			XmlNode val3 = val2.SelectSingleNode ("VideoConfig");
			if (val3 == null) {
				val3 = val2.AppendChild ((XmlNode)(object)val.CreateElement ("VideoConfig"));
			}
			int result = 0;
			int result2 = 0;
			try {
				XmlNode val4 = val3.SelectSingleNode ("screenwidth");
				XmlNode val5 = val3.SelectSingleNode ("screenheight");
				if (val4 != null) {
					int.TryParse (val4.InnerText, out result);
				}
				if (val5 != null) {
					int.TryParse (val5.InnerText, out result2);
				}
			} catch {
			}
			if (result <= 0 || result2 <= 0) {
				result = Screen.PrimaryScreen.Bounds.Width;
				result2 = Screen.PrimaryScreen.Bounds.Height;
			}
			int result3 = 0;
			try {
				XmlNode val6 = val3.SelectSingleNode ("screenwindowed");
				if (val6 != null) {
					int.TryParse (val6.InnerText, out result3);
				}
			} catch {
			}
			if (result3 == 0) {
				try {
					ForceDisplayRefreshRate (240);
				} catch {
				}
			}
			SetVideoInt (val, val3, "screenwindowed", result3);
			SetVideoInt (val, val3, "screenrefresh", 240);
			SetVideoInt (val, val3, "vsyncon", 0);
			SetVideoInt (val, val3, "screenwidth", result);
			SetVideoInt (val, val3, "screenheight", result2);
			val.Save (userSettings);
			ConsoleLog ("Video: OS refresh forced + fullscreen + screenrefresh=240", "info");
		} catch (Exception ex) {
			try {
				ConsoleLog ("ApplyFullscreen240Hz: " + ex.Message, "warning");
			} catch {
			}
		}
	}

	private void ForceDisplayRefreshRate (int targetHz)
	{
		try {
			DevMode devMode = default(DevMode);
			devMode.dmSize = (short)Marshal.SizeOf (typeof(DevMode));
			DevMode devMode2 = devMode;
			if (!EnumDisplaySettings (null, -1, ref devMode2)) {
				ConsoleLog ("EnumDisplaySettings current failed", "warning");
				return;
			}
			int dmPelsWidth = devMode2.dmPelsWidth;
			int dmPelsHeight = devMode2.dmPelsHeight;
			int num = ((devMode2.dmBitsPerPel > 0) ? devMode2.dmBitsPerPel : 32);
			DevMode devMode3 = devMode2;
			int num2 = -1;
			devMode = default(DevMode);
			devMode.dmSize = (short)Marshal.SizeOf (typeof(DevMode));
			DevMode devMode4 = devMode;
			for (int i = 0; EnumDisplaySettings (null, i, ref devMode4); i++) {
				if (devMode4.dmPelsWidth != dmPelsWidth || devMode4.dmPelsHeight != dmPelsHeight || (devMode4.dmBitsPerPel != 0 && num != 0 && devMode4.dmBitsPerPel != num)) {
					continue;
				}
				int dmDisplayFrequency = devMode4.dmDisplayFrequency;
				if (dmDisplayFrequency >= 1) {
					int num3 = ((dmDisplayFrequency == targetHz) ? 100000 : ((dmDisplayFrequency < targetHz) ? dmDisplayFrequency : (50000 - (dmDisplayFrequency - targetHz))));
					if (num3 > num2) {
						num2 = num3;
						devMode3 = devMode4;
					}
				}
			}
			if (devMode3.dmDisplayFrequency == devMode2.dmDisplayFrequency && devMode3.dmPelsWidth == dmPelsWidth) {
				ConsoleLog ("Display already at " + devMode2.dmDisplayFrequency + " Hz (" + dmPelsWidth + "x" + dmPelsHeight + ")", "info");
			}
			devMode3.dmSize = (short)Marshal.SizeOf (typeof(DevMode));
			devMode3.dmFields = 6029312;
			if (ChangeDisplaySettings (ref devMode3, 1) == 0) {
				ConsoleLog ("Display set to " + devMode3.dmPelsWidth + "x" + devMode3.dmPelsHeight + " @ " + devMode3.dmDisplayFrequency + " Hz", "success");
			} else {
				int num4 = ChangeDisplaySettings (ref devMode3, 0);
				ConsoleLog ("ChangeDisplaySettings result=" + num4 + " Hz=" + devMode3.dmDisplayFrequency, (num4 == 0) ? "success" : "warning");
			}
		} catch (Exception ex) {
			ConsoleLog ("ForceDisplayRefreshRate: " + ex.Message, "warning");
		}
	}

	private static void SetVideoInt (XmlDocument doc, XmlNode video, string name, int value)
	{
		XmlNode val = video.SelectSingleNode (name);
		if (val == null) {
			val = (XmlNode)(object)doc.CreateElement (name);
			XmlAttribute val2 = doc.CreateAttribute ("Type");
			((XmlNode)val2).Value = "int";
			val.Attributes.Append (val2);
			video.AppendChild (val);
		}
		val.InnerText = value.ToString ();
		XmlAttribute val3 = val.Attributes ["Type"];
		if (val3 == null) {
			val3 = doc.CreateAttribute ("Type");
			val.Attributes.Append (val3);
		}
		((XmlNode)val3).Value = "int";
	}

	private void LaunchGame (string UserId, string LoginToken, string ServerIP)
	{
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		string text = SettingFile.Read ("InstallationDirectory");
		string path = Path.Combine (text, "nfsw.exe");
		if (!File.Exists (path)) {
			MessageBox.Show ("Cannot find nfsw.exe in: " + text, "Launch");
			return;
		}
		try {
			string text2 = ((Control)password).Text;
			if (string.IsNullOrEmpty (text2) && !string.IsNullOrEmpty (plainPasswordMemory)) {
				text2 = plainPasswordMemory;
			}
			string text3 = ((Control)email).Text;
			if (!string.IsNullOrEmpty (text3) && !string.IsNullOrEmpty (text2) && !string.IsNullOrEmpty (ServerIP)) {
				string text4 = ServerAuth.Authenticate (ServerIP, text3, text2);
				if (!string.IsNullOrEmpty (text4) && text4.Contains ("<LoginToken>")) {
					try {
						XmlDocument val = new XmlDocument ();
						val.LoadXml (text4);
						XmlNode val2 = ((XmlNode)val).SelectSingleNode ("//LoginToken");
						XmlNode val3 = ((XmlNode)val).SelectSingleNode ("//UserId");
						if (val2 != null && !string.IsNullOrEmpty (val2.InnerText) && val3 != null && val3.InnerText != "0") {
							LoginToken = val2.InnerText;
							UserId = val3.InnerText;
							ConsoleLog ("Refreshed login token before launch", "info");
						}
					} catch {
					}
				}
			}
		} catch (Exception ex) {
			ConsoleLog ("Token refresh: " + ex.Message, "info");
		}
		if (string.IsNullOrEmpty (LoginToken) || string.IsNullOrEmpty (UserId) || UserId == "0") {
			MessageBox.Show ("Invalid login token. Please log in again.", "Launch");
			return;
		}
		string text5 = "US";
		string text6 = ServerIP ?? string.Empty;
		string text7 = text6.ToLowerInvariant ();
		if (text7.Contains ("nightriderz")) {
			text5 = "NIGHTRIDERZ";
		} else if (text7.Contains ("worldunited")) {
			text5 = "WUGG";
		}
		string text8 = text6;
		if (text6.StartsWith ("https://", StringComparison.OrdinalIgnoreCase)) {
			if (!SbrwProxy.Start (text6, text5)) {
				if (!LocalProxy.Start (text6)) {
					MessageBox.Show ("Could not start the local proxy (" + SbrwProxy.LastError + " / " + LocalProxy.LastError + ").\nRun as Administrator.", "Proxy");
					return;
				}
				text8 = LocalProxy.GameUrl;
			} else {
				text8 = SbrwProxy.GameUrl;
			}
			ConsoleLog ("Proxy " + text8 + " -> " + text6 + " (SBRW)", "info");
			try {
				ModNetExtras.EnsureFirewallRules (Application.ExecutablePath, text, (SbrwProxy.ProxyPort > 0) ? SbrwProxy.ProxyPort : LocalProxy.Port, delegate(string m) {
					ConsoleLog (m, "info");
				});
			} catch {
			}
		} else {
			text8 = text6;
		}
		try {
			try {
				ModNetExtras.FileANDFolder (text, delegate(string m) {
					ConsoleLog (m, "info");
				});
			} catch {
			}
			ServerMods.CleanupBeforeLaunch (text, delegate(string msg) {
				ConsoleLog (msg, "info");
			});
		} catch {
		}
		string text9 = text5 + " " + text8 + " " + LoginToken + " " + UserId;
		ConsoleLog ("Starting nfsw.exe " + text9, "info");
		try {
			File.AppendAllText ("launcher.log", "LAUNCH ARGS: " + text9 + Environment.NewLine);
		} catch {
		}
		try {
			UnloadPromoToSaveMemory ();
			try {
				Thread.Sleep (500);
			} catch {
			}
			string fullPath = Path.GetFullPath (text);
			string fullPath2 = Path.GetFullPath (path);
			try {
				LauncherUpdaterCheck.EnableLargeAddressAware (fullPath2, delegate(string m) {
					ConsoleLog (m, "info");
				});
			} catch {
			}
			ApplyFullscreen240Hz ();
			ProcessStartInfo processStartInfo = new ProcessStartInfo ();
			processStartInfo.FileName = fullPath2;
			processStartInfo.Arguments = text9;
			processStartInfo.WorkingDirectory = fullPath;
			processStartInfo.UseShellExecute = true;
			Process proc = Process.Start (processStartInfo);
			if (proc == null) {
				MessageBox.Show ("Failed to start nfsw.exe.", "Launch");
				return;
			}
			try {
				proc.PriorityClass = ProcessPriorityClass.AboveNormal;
			} catch {
			}
			try {
				int num = Math.Min (Math.Max (1, Environment.ProcessorCount), 8);
				int num2 = 0;
				for (int i = 0; i < num; i++) {
					num2 |= 1 << i;
				}
				proc.ProcessorAffinity = (IntPtr)num2;
			} catch {
			}
			try {
				AntiCheat.process_id = proc.Id;
				AntiCheat.EnableChecks ();
			} catch {
			}
			try {
				DiscordPresence.SetInGame (text5);
			} catch {
			}
			try {
				proc.PriorityClass = ProcessPriorityClass.AboveNormal;
			} catch {
			}
			if (proc == null) {
				MessageBox.Show ("Failed to start nfsw.exe.", "Launch");
				return;
			}
			((Form)this).ShowInTaskbar = false;
			((Form)this).WindowState = (FormWindowState)1;
			((Control)this).Hide ();
			((Control)this).Visible = false;
			proc.EnableRaisingEvents = true;
			proc.Exited += delegate {
				int num3 = -1;
				try {
					num3 = proc.ExitCode;
				} catch {
				}
				ConsoleLog ("Game exited code=" + num3, "info");
				try {
					LocalProxy.Stop ();
					try {
						SbrwProxy.Stop ();
					} catch {
					}
					try {
						AntiCheat.DisableChecks ();
					} catch {
					}
					try {
						DiscordPresence.Clear ();
					} catch {
					}
					try {
						ModNetExtras.CleanLinks (text, delegate(string m) {
							ConsoleLog (m, "info");
						});
					} catch {
					}
				} catch {
				}
				try {
					((Control)this).BeginInvoke ((Delegate)(Action)delegate {
						ConsoleLog ("Game closed — exiting launcher", "info");
						try {
							Environment.Exit (0);
						} catch {
						}
						try {
							Application.Exit ();
						} catch {
						}
					});
				} catch {
				}
			};
		} catch (Exception ex2) {
			MessageBox.Show ("Launch error: " + ex2.Message, "Launch");
			try {
				LocalProxy.Stop ();
				try {
					SbrwProxy.Stop ();
				} catch {
				}
				try {
					AntiCheat.DisableChecks ();
				} catch {
				}
				try {
					DiscordPresence.Clear ();
				} catch {
				}
				try {
					ModNetExtras.CleanLinks (text, delegate(string m) {
						ConsoleLog (m, "info");
					});
				} catch {
				}
			} catch {
			}
		}
	}

	private void playButton_Click (object sender, EventArgs e)
	{
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		try {
			UnloadPromoToSaveMemory ();
		} catch {
		}
		StopPlayBlink ();
		if (!playenabled) {
			return;
		}
		SHA1 sHA = SHA1.Create ();
		StringBuilder stringBuilder = new StringBuilder ();
		byte[] array = sHA.ComputeHash (Encoding.UTF8.GetBytes (((Control)password).Text.ToString ()));
		foreach (byte b in array) {
			stringBuilder.Append (b.ToString ("X2"));
		}
		if (useSavedPassword) {
			SettingFile.Read ("Password");
		} else {
			stringBuilder.ToString ();
		}
		string text2;
		try {
			string text = ((Control)password).Text;
			if (string.IsNullOrEmpty (text) && !string.IsNullOrEmpty (plainPasswordMemory)) {
				text = plainPasswordMemory;
			}
			if (string.IsNullOrEmpty (text)) {
				MessageBox.Show ("Please enter your password again before PLAY (Modern Auth needs the real password).", "Play");
				return;
			}
			plainPasswordMemory = text;
			text2 = ServerAuth.Authenticate (serverIP, ((Control)email).Text.ToString (), text);
		} catch (WebException ex) {
			if (ex.Status == WebExceptionStatus.ProtocolError) {
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
				text2 = ((httpWebResponse.StatusCode != HttpStatusCode.InternalServerError) ? ((Exception)(object)ex).Message : new StreamReader (((WebResponse)httpWebResponse).GetResponseStream ()).ReadToEnd ());
			} else {
				text2 = ((Exception)(object)ex).Message;
			}
		}
		XmlDocument val = new XmlDocument ();
		if (!builtinserver) {
			if (string.IsNullOrWhiteSpace (text2) || !text2.TrimStart (Array.Empty<char> ()).StartsWith ("<")) {
				text2 = "<?xml version=\"1.0\"?><LoginStatusVO><UserId>0</UserId><LoginToken></LoginToken><Description>" + (string.IsNullOrEmpty (ServerAuth.LastError) ? "Invalid reply" : ServerAuth.LastError).Replace ("&", "&amp;").Replace ("<", "&lt;") + "</Description></LoginStatusVO>";
			}
			val.LoadXml (text2);
		} else {
			val.LoadXml ("<LoginStatusVO><UserId>1</UserId><LoginToken>aaaaaaaa-aaaa-aaaa-aaaaaaaa</LoginToken><Description/></LoginStatusVO>");
		}
		try {
			XmlNode val2 = ((XmlNode)val).SelectSingleNode ("LoginStatusVO/Description");
			XmlNode val3 = ((XmlNode)val).SelectSingleNode ("LoginStatusVO/LoginToken");
			XmlNode val4 = ((XmlNode)val).SelectSingleNode ("LoginStatusVO/UserId");
			if (string.IsNullOrEmpty (val2.InnerText)) {
				UserId = val4.InnerText;
				LoginToken = val3.InnerText;
			}
		} catch {
			MessageBox.Show ("Failed to update token, server is probably offline.");
		}
		SetPlayChrome ((Image)(object)GameLauncher.Properties.Resources.playButton_enable);
		try {
			LaunchGame (UserId, LoginToken, serverIP);
		} catch {
			MessageBox.Show ("Failed to launch game. Cannot find NFSW.exe");
		}
		if (builtinserver) {
			((Control)playProgressText).Text = "SOAPBOX SERVER LAUNCHED. WAITING FOR QUERIES";
			return;
		}
		((Control)playProgressText).Text = "GAME LAUNCHED — KEEP THIS LAUNCHER OPEN";
		((Form)this).WindowState = (FormWindowState)1;
		((Form)this).ShowInTaskbar = false;
		((Control)this).Visible = false;
		HideTrayIcon ();
	}

	private void playButton_MouseUp (object sender, EventArgs e)
	{
		if (playenabled) {
			playGlowPaused = true;
			SetPlayChrome ((Image)(object)GameLauncher.Properties.Resources.playButton_hover);
		}
	}

	private void playButton_MouseDown (object sender, EventArgs e)
	{
		if (playenabled) {
			playGlowPaused = true;
			SetPlayChrome ((Image)(object)GameLauncher.Properties.Resources.playButton_click);
		}
	}

	private void playButton_MouseEnter (object sender, EventArgs e)
	{
		if (playenabled) {
			playGlowPaused = true;
			SetPlayChrome ((Image)(object)GameLauncher.Properties.Resources.playButton_hover);
		}
	}

	private void playButton_MouseLeave (object sender, EventArgs e)
	{
		if (!playenabled) {
			return;
		}
		playGlowPaused = false;
		((Control)playButton).ForeColor = Color.White;
		((Control)playButton).Text = "PLAY";
		if (playBlinkOn) {
			EnsurePlayGlowFrames ();
			if (playGlowCache != null && playGlowCache.Length != 0) {
				SetPlayChrome (playGlowCache [playGlowCacheIndex % playGlowCache.Length]);
			} else if (playEnabledBase != null) {
				SetPlayChrome (playEnabledBase);
			}
		} else {
			SetPlayChrome ((Image)(object)GameLauncher.Properties.Resources.playButton_enable);
		}
	}

	private string GetGameCDN ()
	{
		return "https://cdn.worldunited.gg";
	}

	private void launchNFSW ()
	{
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		SetPlayChrome ((Image)(object)GameLauncher.Properties.Resources.playButton_disable, enabled: false);
		((Control)playProgressText).Text = "DOWNLOAD WAITING...";
		((Control)playProgressTime).Text = string.Empty;
		Delay.WaitSeconds (1);
		try {
			if (File.Exists (Path.Combine (SettingFile.Read ("InstallationDirectory"), "nfsw.exe"))) {
				((Control)playProgressText).Text = "VERIFICATION COMPLETED! SYNCING MODS...";
				((Control)this).Refresh ();
				OnDownloadFinished ();
				return;
			}
		} catch {
		}
		string text;
		try {
			if (string.IsNullOrEmpty (SettingFile.Read ("Language"))) {
				text = "en";
			} else {
				new WebClientWithTimeout ().DownloadString (GetGameCDN () + "/" + SettingFile.Read ("Language").ToLower () + "/index.xml");
				text = SettingFile.Read ("Language").ToLower ();
			}
		} catch (Exception) {
			text = "en";
		}
		if (!File.Exists (SettingFile.Read ("InstallationDirectory") + "\\Sound\\Speech\\copspeechhdr_" + text + ".big")) {
			MessageBox.Show ((IWin32Window)null, "This downloader is in alpha. Please report every issue you will notice.\nThere's also a known issue about 'ESET Smart Security' cutting downloader from reaching chunks files.\nPlease, disable your antivirus.", "Need for Speed World", (MessageBoxButtons)0, (MessageBoxIcon)48);
			((Control)playProgressText).Text = "DOWNLOAD WAITING...";
			DownloadCoreFiles ();
		} else {
			OnDownloadFinished ();
		}
	}

	public void DownloadCoreFiles ()
	{
		((Control)playProgressText).Text = "VERIFYING CORE FILES...";
		((Control)playProgressTime).Text = string.Empty;
		if (!File.Exists (SettingFile.Read ("InstallationDirectory") + "\\nfsw.exe")) {
			DownloadStartTime = DateTime.Now;
			Downloader downloader = new Downloader ((ISynchronizeInvoke)(object)this, 3, 2, 16);
			downloader.ProgressUpdated = OnDownloadProgress;
			downloader.DownloadFinished = DownloadTracksFiles;
			downloader.DownloadFailed = OnDownloadFailed;
			downloader.ShowMessage = OnShowMessage;
			downloader.StartDownload (GetGameCDN (), string.Empty, SettingFile.Read ("InstallationDirectory"), calculateHashes: false, useIndexCache: false, 1130632198uL);
		} else {
			DownloadTracksFiles ();
		}
	}

	public void DownloadTracksFiles ()
	{
		((Control)playProgressText).Text = "VERIFYING TRACKS FILES...";
		((Control)playProgressTime).Text = string.Empty;
		if (!File.Exists (SettingFile.Read ("InstallationDirectory") + "\\Tracks\\STREAML5RA_98.BUN")) {
			DownloadStartTime = DateTime.Now;
			Downloader downloader = new Downloader ((ISynchronizeInvoke)(object)this, 3, 2, 16);
			downloader.ProgressUpdated = OnDownloadProgress;
			downloader.DownloadFinished = DownloadSpeechFiles;
			downloader.DownloadFailed = OnDownloadFailed;
			downloader.ShowMessage = OnShowMessage;
			downloader.StartDownload (GetGameCDN (), "Tracks", SettingFile.Read ("InstallationDirectory"), calculateHashes: false, useIndexCache: false, 615494528uL);
		} else {
			DownloadSpeechFiles ();
		}
	}

	public void DownloadSpeechFiles ()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		((Control)playProgressText).Text = "VERIFYING SPEECH FILES...";
		((Control)playProgressTime).Text = string.Empty;
		string text;
		ulong downloadSize;
		try {
			if (string.IsNullOrEmpty (SettingFile.Read ("Language"))) {
				text = "en";
				downloadSize = 141805935uL;
				langInfo = "ENGLISH";
			} else {
				string text2 = new WebClientWithTimeout ().DownloadString (GetGameCDN () + "/" + SettingFile.Read ("Language").ToLower () + "/index.xml");
				text2 = text2.Substring (3, text2.Length - 3);
				XmlDocument val = new XmlDocument ();
				val.LoadXml (text2);
				XmlNode obj = ((XmlNode)val).SelectSingleNode ("index/header/compressed");
				text = SettingFile.Read ("Language").ToLower ();
				downloadSize = Convert.ToUInt64 (obj.InnerText);
				langInfo = ((ListControl)settingsLanguage).GetItemText (settingsLanguage.SelectedItem).ToUpper ();
			}
		} catch (Exception) {
			text = "en";
			downloadSize = 141805935uL;
			langInfo = "ENGLISH";
		}
		((Control)playProgressText).Text = "VERIFYING " + langInfo + " SPEECH FILES...";
		if (!File.Exists (SettingFile.Read ("InstallationDirectory") + "\\Sound\\Speech\\copspeechsth_" + text + ".big")) {
			DownloadStartTime = DateTime.Now;
			Downloader downloader = new Downloader ((ISynchronizeInvoke)(object)this, 3, 2, 16);
			downloader.ProgressUpdated = OnDownloadProgress;
			downloader.DownloadFinished = DownloadTracksHighFiles;
			downloader.DownloadFailed = OnDownloadFailed;
			downloader.ShowMessage = OnShowMessage;
			downloader.StartDownload (GetGameCDN (), text, SettingFile.Read ("InstallationDirectory"), calculateHashes: false, useIndexCache: false, downloadSize);
		} else {
			DownloadTracksHighFiles ();
		}
	}

	public void DownloadTracksHighFiles ()
	{
		((Control)playProgressText).Text = "VERIFYING TRACKSHIGH FILES...";
		((Control)playProgressTime).Text = string.Empty;
		if (SettingFile.Read ("TracksHigh") == "1" && !File.Exists (SettingFile.Read ("InstallationDirectory") + "\\TracksHigh\\STREAML5RA_98.BUN")) {
			DownloadStartTime = DateTime.Now;
			Downloader downloader = new Downloader ((ISynchronizeInvoke)(object)this, 3, 2, 16);
			downloader.ProgressUpdated = OnDownloadProgress;
			downloader.DownloadFinished = OnDownloadFinished;
			downloader.DownloadFailed = OnDownloadFailed;
			downloader.ShowMessage = OnShowMessage;
			downloader.StartDownload (GetGameCDN (), "TracksHigh", SettingFile.Read ("InstallationDirectory"), calculateHashes: false, useIndexCache: false, 278397707uL);
		} else {
			OnDownloadFinished ();
		}
	}

	private string FormatFileSize (long byteCount)
	{
		double[] array = new double[4] { 1073741824.0, 1048576.0, 1024.0, 0.0 };
		string[] array2 = new string[4] { "GB", "MB", "KB", "Bytes" };
		for (int i = 0; i < array.Length; i++) {
			if ((double)byteCount >= array [i]) {
				return $"{(double)byteCount / array [i]:0.00}" + array2 [i];
			}
		}
		return "0 Bytes";
	}

	private string EstimateFinishTime (long current, long total)
	{
		double num = (double)current / (double)total;
		if (num < 0.01) {
			return "ESTIMATING...";
		}
		TimeSpan timeSpan = DateTime.Now - DownloadStartTime;
		TimeSpan timeSpan2 = TimeSpan.FromTicks ((long)((double)timeSpan.Ticks / num)) - timeSpan;
		object arg = timeSpan2.Hours;
		string arg2 = timeSpan2.Minutes.ToString ("D02");
		return string.Format ("{0}:{1}:{2}", arg, arg2, timeSpan2.Seconds.ToString ("D02"));
	}

	private void OnDownloadProgress (long downloadLength, long downloadCurrent, long compressedLength, string filename)
	{
		if (downloadCurrent < compressedLength) {
			_ = ((Control)playProgressText).Width;
			string text = filename.Replace (SettingFile.Read ("InstallationDirectory") + "/", string.Empty).ToUpper ();
			((Control)playProgressText).Text = "DOWNLOADING (" + FormatFileSize (downloadCurrent) + "/" + FormatFileSize (compressedLength) + ") - TIME REMAINING: " + EstimateFinishTime (downloadCurrent, compressedLength);
			((Control)playProgressTime).Text = "";
		}
		playProgress.Value = (int)(100 * downloadCurrent / compressedLength);
	}

	private void OnDownloadFinished ()
	{
		try {
			((Control)playProgressText).Text = "SYNCING SERVER MODS...";
			playProgress.Value = 95;
		} catch {
		}
		string ip = serverIP;
		string installDir = "";
		try {
			if (string.IsNullOrEmpty (ip) && serverPick != null && ((ListControl)serverPick).SelectedValue != null) {
				ip = ((ListControl)serverPick).SelectedValue.ToString ();
			}
		} catch {
		}
		try {
			installDir = SettingFile.Read ("InstallationDirectory");
		} catch {
		}
		ThreadPool.QueueUserWorkItem (delegate {
			try {
				ServerMods.PrepareMods (ip, installDir, delegate(string msg) {
					try {
						if (!((Control)this).IsDisposed) {
							((Control)this).BeginInvoke ((Delegate)(Action)delegate {
								try {
									((Control)playProgressText).Text = msg;
								} catch {
								}
							});
						}
					} catch {
					}
				});
			} catch {
			}
			try {
				if (!((Control)this).IsDisposed) {
					((Control)this).BeginInvoke ((Delegate)(Action)delegate {
						try {
							FinishAfterMods ();
						} catch {
						}
					});
				}
			} catch {
			}
		});
	}

	private void FinishAfterMods ()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		try {
			if (ServerMods.LastTotal > 0 && ServerMods.LastFailed >= ServerMods.LastTotal && ServerMods.LastDownloaded == 0 && ServerMods.LastSkipped == 0) {
				playenabled = false;
				playProgress.Value = 0;
				((Control)playProgressText).Text = "MODS FAILED — CHECK launcher.log (do not PLAY)";
				MessageBox.Show ("All server mods failed to download.\nFix the errors in launcher.log before playing.\nPlaying without mods often causes Out of Memory.", "Mods failed");
			} else {
				playenabled = true;
				playProgress.Value = 0;
				SetPlayChrome ((Image)(object)GameLauncher.Properties.Resources.playButton_enable);
				((Control)playButton).ForeColor = Color.White;
				((Control)playProgressText).Text = "DOWNLOAD COMPLETED";
				((Control)playProgressTime).Text = string.Empty;
				StartPlayBlink ();
			}
		} catch {
		}
	}

	private static Image LoadPlayBg ()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		if (_playBgCache != null) {
			return _playBgCache;
		}
		try {
			using Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("GameLauncher.Background.playbg.png");
			if (stream != null) {
				Image val = Image.FromStream (stream);
				try {
					_playBgCache = (Image)new Bitmap (val);
				} finally {
					((IDisposable)val)?.Dispose ();
				}
				return _playBgCache;
			}
		} catch {
		}
		try {
			_playBgCache = (Image)(object)GameLauncher.Properties.Resources.loginbg;
		} catch {
		}
		return _playBgCache;
	}

	private static Image LoadEmbedded (string logicalName)
	{
		try {
			using Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream (logicalName);
			if (stream == null) {
				return null;
			}
			return Image.FromStream (stream);
		} catch {
			return null;
		}
	}

	private static Image SetImgOpacity (Image imgPic, float imgOpac)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		if (imgPic == null) {
			return null;
		}
		Bitmap val = new Bitmap (imgPic.Width, imgPic.Height, (PixelFormat)2498570);
		Graphics val2 = Graphics.FromImage ((Image)(object)val);
		try {
			val2.Clear (Color.Transparent);
			ColorMatrix val3 = new ColorMatrix ();
			val3.Matrix33 = Math.Max (0f, Math.Min (1f, imgOpac));
			ImageAttributes val4 = new ImageAttributes ();
			val4.SetColorMatrix (val3, (ColorMatrixFlag)0, (ColorAdjustType)1);
			val2.DrawImage (imgPic, new Rectangle (0, 0, ((Image)val).Width, ((Image)val).Height), 0, 0, imgPic.Width, imgPic.Height, (GraphicsUnit)2, val4);
			return (Image)(object)val;
		} finally {
			((IDisposable)val2)?.Dispose ();
		}
	}

	private Image ComposePlayChrome (Image chrome, bool enabled)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		if (chrome == null) {
			return null;
		}
		try {
			Bitmap val = new Bitmap (165, 58, (PixelFormat)2498570);
			Graphics val2 = Graphics.FromImage ((Image)(object)val);
			try {
				val2.Clear (Color.Transparent);
				val2.DrawImage (chrome, 0, 0, 165, 58);
				val2.TextRenderingHint = (TextRenderingHint)3;
				Color color = (enabled ? Color.FromArgb (255, 255, 255) : Color.FromArgb (153, 153, 153));
				Font val3 = null;
				try {
					val3 = new Font (FontWrapper.Instance.GetFontFamily ("Font_RegisterBoldItalic.ttf"), 18.75f, (FontStyle)3, (GraphicsUnit)3);
				} catch {
					val3 = new Font ("Arial", 18.75f, (FontStyle)3, (GraphicsUnit)3);
				}
				Font val4 = val3;
				try {
					SolidBrush val5 = new SolidBrush (color);
					try {
						StringFormat val6 = new StringFormat ();
						try {
							val6.Alignment = (StringAlignment)1;
							val6.LineAlignment = (StringAlignment)1;
							val2.DrawString ("PLAY", val3, (Brush)(object)val5, new RectangleF (0f, 0f, 165f, 58f), val6);
						} finally {
							((IDisposable)val6)?.Dispose ();
						}
					} finally {
						((IDisposable)val5)?.Dispose ();
					}
				} finally {
					((IDisposable)val4)?.Dispose ();
				}
			} finally {
				((IDisposable)val2)?.Dispose ();
			}
			return (Image)(object)val;
		} catch {
			return chrome;
		}
	}

	private Image ScaleForDpi (Image src)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		if (src == null) {
			return null;
		}
		try {
			Graphics val = ((Control)this).CreateGraphics ();
			float dpiX;
			try {
				dpiX = val.DpiX;
			} finally {
				((IDisposable)val)?.Dispose ();
			}
			if (dpiX <= 96f) {
				return src;
			}
			float num = dpiX / 96f;
			int num2 = Math.Max (1, (int)((float)src.Width * num));
			int num3 = Math.Max (1, (int)((float)src.Height * num));
			Bitmap val2 = new Bitmap (num2, num3);
			Graphics val3 = Graphics.FromImage ((Image)val2);
			try {
				val3.InterpolationMode = (InterpolationMode)7;
				val3.DrawImage (src, 0, 0, num2, num3);
			} finally {
				((IDisposable)val3)?.Dispose ();
			}
			return (Image)val2;
		} catch {
			return src;
		}
	}

	private void SetPlayChrome (Image img, bool enabled = true)
	{
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Expected O, but got Unknown
		try {
			if (img == null) {
				return;
			}
			Image val = ScaleForDpi (img);
			((ButtonBase)playButton).FlatStyle = (FlatStyle)0;
			((ButtonBase)playButton).FlatAppearance.BorderSize = 0;
			((ButtonBase)playButton).FlatAppearance.MouseOverBackColor = Color.Transparent;
			((ButtonBase)playButton).FlatAppearance.MouseDownBackColor = Color.Transparent;
			((Control)playButton).BackColor = Color.Transparent;
			((Control)playButton).Padding = new Padding (0);
			((ButtonBase)playButton).Image = null;
			((Control)playButton).BackgroundImage = val;
			((Control)playButton).BackgroundImageLayout = (ImageLayout)0;
			int width = ((val != null && val.Width >= 100) ? val.Width : 165);
			int height = ((val != null && val.Height >= 40) ? val.Height : 58);
			((Control)playButton).Size = new Size (width, height);
			((Control)playButton).Location = new Point (618, 424);
			((Control)playButton).Text = "PLAY";
			((ButtonBase)playButton).TextAlign = (ContentAlignment)32;
			((Control)playButton).ForeColor = (enabled ? Color.FromArgb (255, 255, 255) : Color.FromArgb (153, 153, 153));
			((ButtonBase)playButton).UseCompatibleTextRendering = false;
			((Control)playButton).Enabled = true;
			try {
				Graphics val2 = ((Control)this).CreateGraphics ();
				float dpiX;
				try {
					dpiX = val2.DpiX;
				} finally {
					((IDisposable)val2)?.Dispose ();
				}
				float num = 18.75f * ((dpiX <= 0f) ? 1f : (dpiX / 96f));
				FontFamily fontFamily = FontWrapper.Instance.GetFontFamily ("Font_RegisterBoldItalic.ttf");
				((Control)playButton).Font = new Font (fontFamily, num, (FontStyle)3, (GraphicsUnit)3);
			} catch {
				((Control)playButton).Font = new Font ("Segoe UI", 18.75f, (FontStyle)3, (GraphicsUnit)3);
			}
		} catch {
		}
	}

	private void SetPlayDisabled ()
	{
		try {
			StopPlayBlink ();
		} catch {
		}
		SetPlayChrome ((Image)(object)GameLauncher.Properties.Resources.playButton_disable, enabled: false);
		playenabled = false;
	}

	private void EnsurePlayGlowFrames ()
	{
		if (playGlowCache != null && playGlowCache.Length != 0 && playEnabledBase != null) {
			return;
		}
		try {
			playEnabledBase = (Image)(object)GameLauncher.Properties.Resources.playButton_enable;
			playGlowFrames = (Image[])(object)new Image[4];
			playGlowFrames [0] = (Image)(object)GameLauncher.Properties.Resources.playButton_disable;
			playGlowFrames [1] = (Image)(object)GameLauncher.Properties.Resources.playButton_enable;
			playGlowFrames [2] = (Image)(object)GameLauncher.Properties.Resources.playButton_hover;
			playGlowFrames [3] = (Image)(object)GameLauncher.Properties.Resources.playButton_click;
			List<Image> list = new List<Image> ();
			for (float num = -8f; num <= 8.001f; num += 0.8f) {
				float num2 = EaGauss (num, 1f, 0f, 0.4f, 0f, inverted: false);
				if (num2 < 0f) {
					num2 = 0f;
				}
				if (num2 > 1f) {
					num2 = 1f;
				}
				list.Add (SetImgOpacity (playEnabledBase, num2));
			}
			playGlowCache = list.ToArray ();
			playGlowCacheIndex = 0;
			playGlowRange = -8f;
		} catch {
		}
	}

	private static float EaGauss (float x, float a, float b, float c, float floor, bool inverted)
	{
		if (c == 0f) {
			c = 1f;
		}
		a = 0.3989f / c;
		float num = a * (float)Math.Pow (Math.E, (0.0 - Math.Pow (x - b, 2.0)) / 2.0 * (double)c * (double)c);
		if (!inverted) {
			return num + floor;
		}
		return 0f - num + floor;
	}

	private void StartPlayBlink ()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		try {
			EnsurePlayGlowFrames ();
			if (playBlinkTimer == null) {
				playBlinkTimer = new Timer ();
				playBlinkTimer.Interval = 100;
				playBlinkTimer.Tick += playBlinkTimer_Tick;
			}
			playGlowCacheIndex = 0;
			playGlowRange = -8f;
			playBlinkOn = true;
			try {
				FontFamily fontFamily = FontWrapper.Instance.GetFontFamily ("Font_RegisterBoldItalic.ttf");
				if (fontFamily != null) {
					((Control)playButton).Font = new Font (fontFamily, 18.75f * DPIDefaultScale / ((Control)this).CreateGraphics ().DpiX, (FontStyle)3);
				}
			} catch {
			}
			((Control)playButton).ForeColor = Color.White;
			((Control)playButton).Text = "PLAY";
			((ButtonBase)playButton).TextAlign = (ContentAlignment)32;
			if (playGlowCache != null && playGlowCache.Length != 0) {
				SetPlayChrome (playGlowCache [0]);
			} else if (playEnabledBase != null) {
				SetPlayChrome (playEnabledBase);
			}
			playBlinkTimer.Start ();
		} catch {
		}
	}

	private void StopPlayBlink ()
	{
		try {
			if (playBlinkTimer != null) {
				playBlinkTimer.Stop ();
			}
			playBlinkOn = false;
			playGlowRange = -8f;
			playGlowCacheIndex = 0;
			if (playenabled) {
				((Control)playButton).ForeColor = Color.White;
				((Control)playButton).Text = "PLAY";
				EnsurePlayGlowFrames ();
				if (playEnabledBase != null) {
					SetPlayChrome (playEnabledBase);
				}
			}
		} catch {
		}
	}

	private void playBlinkTimer_Tick (object sender, EventArgs e)
	{
		if (!playenabled) {
			StopPlayBlink ();
			return;
		}
		try {
			if (playGlowPaused) {
				return;
			}
			EnsurePlayGlowFrames ();
			if (playGlowCache != null && playGlowCache.Length != 0) {
				Image val = playGlowCache [playGlowCacheIndex];
				if (val != null) {
					SetPlayChrome (val);
				}
				playGlowCacheIndex = (playGlowCacheIndex + 1) % playGlowCache.Length;
			}
		} catch {
		}
	}

	private void OnDownloadFailed (Exception ex)
	{
		playProgress.Value = 0;
		((Control)playProgressText).Text = "DOWNLOAD FAILED!";
	}

	private void OnShowMessage (string message, string header)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MessageBox.Show (message, header);
	}

	protected override void Dispose (bool disposing)
	{
		if (disposing && components != null) {
			((IDisposable)components).Dispose ();
		}
		((Form)this).Dispose (disposing);
	}

	private void InitializeComponent ()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Expected O, but got Unknown
		//IL_1a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a96: Expected O, but got Unknown
		//IL_1bba: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc4: Expected O, but got Unknown
		//IL_1f44: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f4e: Expected O, but got Unknown
		//IL_2008: Unknown result type (might be due to invalid IL or missing references)
		//IL_2012: Expected O, but got Unknown
		//IL_20cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_20d6: Expected O, but got Unknown
		//IL_218d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2197: Expected O, but got Unknown
		//IL_233e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2348: Expected O, but got Unknown
		//IL_247f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2490: Unknown result type (might be due to invalid IL or missing references)
		//IL_257e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2588: Expected O, but got Unknown
		//IL_25ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_26b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_26c2: Expected O, but got Unknown
		//IL_2a7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a84: Expected O, but got Unknown
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Expected O, but got Unknown
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Expected O, but got Unknown
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Expected O, but got Unknown
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Expected O, but got Unknown
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Expected O, but got Unknown
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Expected O, but got Unknown
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Expected O, but got Unknown
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Expected O, but got Unknown
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Expected O, but got Unknown
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Expected O, but got Unknown
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Expected O, but got Unknown
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Expected O, but got Unknown
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Expected O, but got Unknown
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Expected O, but got Unknown
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Expected O, but got Unknown
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Expected O, but got Unknown
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Expected O, but got Unknown
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Expected O, but got Unknown
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Expected O, but got Unknown
		//IL_0729: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Expected O, but got Unknown
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Expected O, but got Unknown
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Expected O, but got Unknown
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Expected O, but got Unknown
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Expected O, but got Unknown
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Expected O, but got Unknown
		//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab8: Expected O, but got Unknown
		//IL_0b41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4b: Expected O, but got Unknown
		//IL_0dea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df4: Expected O, but got Unknown
		//IL_0e25: Unknown result type (might be due to invalid IL or missing references)
		//IL_105b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1065: Expected O, but got Unknown
		//IL_1111: Unknown result type (might be due to invalid IL or missing references)
		//IL_111b: Expected O, but got Unknown
		//IL_114e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1158: Expected O, but got Unknown
		//IL_1212: Unknown result type (might be due to invalid IL or missing references)
		//IL_121c: Expected O, but got Unknown
		//IL_12d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e0: Expected O, but got Unknown
		//IL_13b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c0: Expected O, but got Unknown
		//IL_1583: Unknown result type (might be due to invalid IL or missing references)
		//IL_158d: Expected O, but got Unknown
		//IL_1765: Unknown result type (might be due to invalid IL or missing references)
		//IL_176f: Expected O, but got Unknown
		//IL_184c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1856: Expected O, but got Unknown
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Expected O, but got Unknown
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_0632: Expected O, but got Unknown
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Expected O, but got Unknown
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Expected O, but got Unknown
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Expected O, but got Unknown
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Expected O, but got Unknown
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Expected O, but got Unknown
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Expected O, but got Unknown
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Expected O, but got Unknown
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Expected O, but got Unknown
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Expected O, but got Unknown
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Expected O, but got Unknown
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Expected O, but got Unknown
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Expected O, but got Unknown
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Expected O, but got Unknown
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Expected O, but got Unknown
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Expected O, but got Unknown
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Expected O, but got Unknown
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Expected O, but got Unknown
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Expected O, but got Unknown
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Expected O, but got Unknown
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Expected O, but got Unknown
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Expected O, but got Unknown
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Expected O, but got Unknown
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Expected O, but got Unknown
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Expected O, but got Unknown
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Expected O, but got Unknown
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Expected O, but got Unknown
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Expected O, but got Unknown
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Expected O, but got Unknown
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Expected O, but got Unknown
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Expected O, but got Unknown
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Expected O, but got Unknown
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Expected O, but got Unknown
		components = (IContainer)(object)new Container ();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager (typeof(mainScreen));
		closebtn = new PictureBox ();
		minimizebtn = new PictureBox ();
		email = new TextBox ();
		password = new TextBox ();
		serverStatus = new Label ();
		serverStatusImg = new PictureBox ();
		serverPick = new ComboBox ();
		currentWindowInfo = new Label ();
		consoleLog = new RichTextBox ();
		bottomBanner = new PictureBox ();
		advertisementLabel = new Label ();
		rememberMe = new CheckBox ();
		Timeout = new Timer (components);
		onlineCount = new Label ();
		loginButton = new Button ();
		registerText = new LinkLabel ();
		emailLabel = new Label ();
		passwordLabel = new Label ();
		troubleLabel = new Label ();
		githubLink = new LinkLabel ();
		registerButton = new Button ();
		selectServerLabel = new Label ();
		settingsButton = new PictureBox ();
		moveWindow = new Panel ();
		playLoggedInAs = new Label ();
		promoUseWebView2 = false;
		promoWebViewReady = false;
		promoWebView = null;
		promoBrowser = new Panel ();
		((Control)promoBrowser).Name = "promoBrowser";
		((Control)promoBrowser).Location = new Point (10, 95);
		((Control)promoBrowser).Size = new Size (771, 310);
		((Control)promoBrowser).BackColor = Color.FromArgb (11, 21, 32);
		((Control)promoBrowser).Visible = false;
		promoIeBrowser = new WebBrowser ();
		((Control)promoIeBrowser).Name = "promoIeBrowser";
		((Control)promoIeBrowser).Dock = (DockStyle)5;
		promoIeBrowser.AllowWebBrowserDrop = false;
		promoIeBrowser.IsWebBrowserContextMenuEnabled = false;
		promoIeBrowser.ScriptErrorsSuppressed = true;
		promoIeBrowser.ScrollBarsEnabled = false;
		promoIeBrowser.WebBrowserShortcutsEnabled = false;
		promoIeBrowser.NewWindow += delegate(object s, CancelEventArgs e) {
			e.Cancel = true;
		};
		WebBrowser obj = promoIeBrowser;
		object obj2 = _003C_003Ec._003C_003E9__195_1;
		if (obj2 == null) {
			object obj3 = _003C_003Ec._003C_003E9__195_1;
			if (obj3 == null) {
				object obj4 = _003C_003Ec._003C_003E9__195_1;
				if (obj4 == null) {
					object obj5 = _003C_003Ec._003C_003E9__195_1;
					if (obj5 == null) {
						object obj6 = _003C_003Ec._003C_003E9__195_1;
						if (obj6 == null) {
							object obj7 = _003C_003Ec._003C_003E9__195_1;
							if (obj7 == null) {
								object obj8 = _003C_003Ec._003C_003E9__195_1;
								if (obj8 == null) {
									object obj9 = _003C_003Ec._003C_003E9__195_1;
									if (obj9 == null) {
										object obj10 = _003C_003Ec._003C_003E9__195_1;
										if (obj10 == null) {
											object obj11 = _003C_003Ec._003C_003E9__195_1;
											if (obj11 == null) {
												object obj12 = _003C_003Ec._003C_003E9__195_1;
												if (obj12 == null) {
													object obj13 = _003C_003Ec._003C_003E9__195_1;
													if (obj13 == null) {
														object obj14 = _003C_003Ec._003C_003E9__195_1;
														if (obj14 == null) {
															object obj15 = _003C_003Ec._003C_003E9__195_1;
															if (obj15 == null) {
																object obj16 = _003C_003Ec._003C_003E9__195_1;
																if (obj16 == null) {
																	object obj17 = _003C_003Ec._003C_003E9__195_1;
																	if (obj17 == null) {
																		object obj18 = _003C_003Ec._003C_003E9__195_1;
																		if (obj18 == null) {
																			object obj19 = _003C_003Ec._003C_003E9__195_1;
																			if (obj19 == null) {
																				object obj20 = _003C_003Ec._003C_003E9__195_1;
																				if (obj20 == null) {
																					object obj21 = _003C_003Ec._003C_003E9__195_1;
																					if (obj21 == null) {
																						object obj22 = _003C_003Ec._003C_003E9__195_1;
																						if (obj22 == null) {
																							object obj23 = _003C_003Ec._003C_003E9__195_1;
																							if (obj23 == null) {
																								object obj24 = _003C_003Ec._003C_003E9__195_1;
																								if (obj24 == null) {
																									object obj25 = _003C_003Ec._003C_003E9__195_1;
																									if (obj25 == null) {
																										object obj26 = _003C_003Ec._003C_003E9__195_1;
																										if (obj26 == null) {
																											object obj27 = _003C_003Ec._003C_003E9__195_1;
																											if (obj27 == null) {
																												object obj28 = _003C_003Ec._003C_003E9__195_1;
																												if (obj28 == null) {
																													object obj29 = _003C_003Ec._003C_003E9__195_1;
																													if (obj29 == null) {
																														object obj30 = _003C_003Ec._003C_003E9__195_1;
																														if (obj30 == null) {
																															object obj31 = _003C_003Ec._003C_003E9__195_1;
																															if (obj31 == null) {
																																object obj32 = _003C_003Ec._003C_003E9__195_1;
																																if (obj32 == null) {
																																	object obj33 = _003C_003Ec._003C_003E9__195_1;
																																	if (obj33 == null) {
																																		object obj34 = _003C_003Ec._003C_003E9__195_1;
																																		if (obj34 == null) {
																																			object obj35 = _003C_003Ec._003C_003E9__195_1;
																																			if (obj35 == null) {
																																				object obj36 = <>c.<>9__211_1;
																																				if (obj36 == null) {
																																					WebBrowserNavigatingEventHandler val = delegate {
																																					};
																																					<>c.<>9__211_1 = val;
																																					obj36 = (object)val;
																																				}
																																				_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj36;
																																				obj35 = obj36;
																																			}
																																			_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj35;
																																			obj34 = obj35;
																																		}
																																		_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj34;
																																		obj33 = obj34;
																																	}
																																	_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj33;
																																	obj32 = obj33;
																																}
																																_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj32;
																																obj31 = obj32;
																															}
																															_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj31;
																															obj30 = obj31;
																														}
																														_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj30;
																														obj29 = obj30;
																													}
																													_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj29;
																													obj28 = obj29;
																												}
																												_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj28;
																												obj27 = obj28;
																											}
																											_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj27;
																											obj26 = obj27;
																										}
																										_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj26;
																										obj25 = obj26;
																									}
																									_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj25;
																									obj24 = obj25;
																								}
																								_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj24;
																								obj23 = obj24;
																							}
																							_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj23;
																							obj22 = obj23;
																						}
																						_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj22;
																						obj21 = obj22;
																					}
																					_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj21;
																					obj20 = obj21;
																				}
																				_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj20;
																				obj19 = obj20;
																			}
																			_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj19;
																			obj18 = obj19;
																		}
																		_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj18;
																		obj17 = obj18;
																	}
																	_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj17;
																	obj16 = obj17;
																}
																_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj16;
																obj15 = obj16;
															}
															_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj15;
															obj14 = obj15;
														}
														_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj14;
														obj13 = obj14;
													}
													_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj13;
													obj12 = obj13;
												}
												_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj12;
												obj11 = obj12;
											}
											_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj11;
											obj10 = obj11;
										}
										_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj10;
										obj9 = obj10;
									}
									_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj9;
									obj8 = obj9;
								}
								_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj8;
								obj7 = obj8;
							}
							_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj7;
							obj6 = obj7;
						}
						_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj6;
						obj5 = obj6;
					}
					_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj5;
					obj4 = obj5;
				}
				_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj4;
				obj3 = obj4;
			}
			_003C_003Ec._003C_003E9__195_1 = (WebBrowserNavigatingEventHandler)obj3;
			obj2 = obj3;
		}
		obj.Navigating += (WebBrowserNavigatingEventHandler)obj2;
		((Control)promoBrowser).Controls.Add ((Control)(object)promoIeBrowser);
		logoutButton = new Button ();
		verticalBanner = new PictureBox ();
		settingsSave = new Button ();
		settingsLanguage = new ComboBox ();
		settingsLanguageText = new Label ();
		settingsQuality = new ComboBox ();
		settingsQualityText = new Label ();
		settingsLanguageDesc = new Label ();
		settingsQualityDesc = new Label ();
		registerEmail = new TextBox ();
		registerTicket = new TextBox ();
		registerPassword = new TextBox ();
		registerConfirmPassword = new TextBox ();
		registerEmailText = new Label ();
		registerPasswordText = new Label ();
		registerTicketText = new Label ();
		registerConfirmPasswordText = new Label ();
		registerAgree = new CheckBox ();
		playProgress = new ProgressBar ();
		playProgressText = new Label ();
		playButton = new Button ();
		Notification = new NotifyIcon (components);
		forgotPassword = new LinkLabel ();
		playProgressTime = new Label ();
		((ISupportInitialize)closebtn).BeginInit ();
		((ISupportInitialize)minimizebtn).BeginInit ();
		((ISupportInitialize)serverStatusImg).BeginInit ();
		((ISupportInitialize)settingsButton).BeginInit ();
		((Control)moveWindow).SuspendLayout ();
		((ISupportInitialize)verticalBanner).BeginInit ();
		((ISupportInitialize)bottomBanner).BeginInit ();
		((Control)this).SuspendLayout ();
		((Control)closebtn).BackColor = Color.Transparent;
		((Control)closebtn).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.close;
		((Control)closebtn).Location = new Point (742, 1);
		((Control)closebtn).Name = "closebtn";
		((Control)closebtn).Size = new Size (33, 26);
		closebtn.TabIndex = 0;
		closebtn.TabStop = false;
		((Control)closebtn).Click += closebtn_Click;
		((Control)minimizebtn).BackColor = Color.Transparent;
		((Control)minimizebtn).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.minimize;
		((Control)minimizebtn).Location = new Point (708, 1);
		((Control)minimizebtn).Name = "minimizebtn";
		((Control)minimizebtn).Size = new Size (33, 26);
		minimizebtn.TabIndex = 0;
		minimizebtn.TabStop = false;
		((Control)minimizebtn).Click += minimizebtn_Click;
		((Control)email).Location = new Point (33, 183);
		((Control)email).Name = "email";
		((Control)email).Size = new Size (210, 20);
		((Control)email).TabIndex = 1;
		((Control)password).Location = new Point (276, 183);
		((Control)password).Name = "password";
		password.PasswordChar = '●';
		((Control)password).Size = new Size (210, 20);
		((Control)password).TabIndex = 2;
		((Control)serverStatus).AutoSize = true;
		((Control)serverStatus).BackColor = Color.Transparent;
		((Control)serverStatus).ForeColor = SystemColors.ButtonHighlight;
		((Control)serverStatus).Location = new Point (40, 332);
		((Control)serverStatus).Name = "serverStatus";
		((Control)serverStatus).Size = new Size (280, 15);
		((Control)serverStatus).TabIndex = 3;
		((Control)serverStatus).Text = "Retrieving server status...";
		UpdateBannerFromState ();
		serverStatus.TextAlign = (ContentAlignment)16;
		((Control)serverStatusImg).BackColor = Color.Transparent;
		((Control)serverStatusImg).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.server_online;
		((Control)serverStatusImg).Location = new Point (20, 323);
		((Control)serverStatusImg).Name = "serverStatusImg";
		((Control)serverStatusImg).Size = new Size (16, 16);
		serverStatusImg.TabIndex = 4;
		serverStatusImg.TabStop = false;
		serverPick.DropDownStyle = (ComboBoxStyle)2;
		((Control)serverPick).Font = new Font ("Microsoft Sans Serif", 8.25f, (FontStyle)1, (GraphicsUnit)3, (byte)238);
		((ListControl)serverPick).FormattingEnabled = true;
		((Control)serverPick).Location = new Point (548, 47);
		((Control)serverPick).Name = "serverPick";
		((Control)serverPick).Size = new Size (188, 21);
		((Control)serverPick).TabIndex = 5;
		((Control)currentWindowInfo).AutoSize = true;
		((Control)currentWindowInfo).BackColor = Color.Transparent;
		((Control)currentWindowInfo).Font = new Font ("Microsoft Sans Serif", 12.75f, (FontStyle)2, (GraphicsUnit)3, (byte)238);
		((Control)currentWindowInfo).ForeColor = Color.FromArgb (76, 178, 255);
		((Control)currentWindowInfo).Location = new Point (69, 116);
		((Control)currentWindowInfo).Name = "currentWindowInfo";
		((Control)currentWindowInfo).Size = new Size (437, 25);
		((Control)currentWindowInfo).TabIndex = 7;
		((Control)currentWindowInfo).Text = "ENTER YOUR ACCOUNT INFORMATION TO LOG IN:";
		currentWindowInfo.TextAlign = (ContentAlignment)256;
		currentWindowInfo.UseCompatibleTextRendering = true;
		((Control)consoleLog).BackColor = Color.Black;
		((TextBoxBase)consoleLog).BorderStyle = (BorderStyle)0;
		((Control)consoleLog).ForeColor = Color.White;
		((Control)consoleLog).Location = new Point (35, 389);
		((Control)consoleLog).Name = "consoleLog";
		((TextBoxBase)consoleLog).ReadOnly = true;
		consoleLog.ScrollBars = (RichTextBoxScrollBars)2;
		((Control)consoleLog).Size = new Size (719, 87);
		((Control)consoleLog).TabIndex = 9;
		((Control)consoleLog).Text = string.Empty;
		((Control)consoleLog).Visible = false;
		((Control)bottomBanner).BackColor = Color.Transparent;
		((Control)bottomBanner).Location = new Point (35, 389);
		((Control)bottomBanner).Name = "bottomBanner";
		((Control)bottomBanner).Size = new Size (719, 87);
		bottomBanner.SizeMode = (PictureBoxSizeMode)1;
		bottomBanner.TabIndex = 99;
		((Control)bottomBanner).Visible = false;
		((Control)advertisementLabel).AutoSize = false;
		((Control)advertisementLabel).BackColor = Color.Transparent;
		((Control)advertisementLabel).Font = GetAdvertisementFont ();
		((Control)advertisementLabel).ForeColor = Color.FromArgb (69, 133, 193);
		((Control)advertisementLabel).Location = new Point (308, 375);
		((Control)advertisementLabel).Name = "advertisementLabel";
		((Control)advertisementLabel).Size = new Size (200, 13);
		((Control)advertisementLabel).TabIndex = 100;
		((Control)advertisementLabel).Text = "ADVERTISEMENT";
		advertisementLabel.TextAlign = (ContentAlignment)32;
		((Control)advertisementLabel).Visible = true;
		((Control)rememberMe).AutoSize = true;
		((Control)rememberMe).BackColor = Color.Transparent;
		((Control)rememberMe).Font = new Font ("Microsoft Sans Serif", 10f, (FontStyle)1, (GraphicsUnit)3, (byte)238);
		((Control)rememberMe).ForeColor = Color.White;
		((Control)rememberMe).Location = new Point (33, 209);
		((Control)rememberMe).Margin = new Padding (3, 3, 3, 0);
		((Control)rememberMe).Name = "rememberMe";
		((Control)rememberMe).Size = new Size (268, 21);
		((Control)rememberMe).TabIndex = 3;
		((Control)rememberMe).Text = "REMEMBER MY EMAIL ADDRESS";
		((ButtonBase)rememberMe).UseVisualStyleBackColor = false;
		Timeout.Interval = 3000;
		((Control)onlineCount).AutoSize = true;
		((Control)onlineCount).BackColor = Color.Transparent;
		((Control)onlineCount).ForeColor = SystemColors.ButtonHighlight;
		((Control)onlineCount).Location = new Point (44, 337);
		((Control)onlineCount).Name = "onlineCount";
		((Control)onlineCount).Size = new Size (127, 13);
		((Control)onlineCount).TabIndex = 11;
		((Control)onlineCount).Text = "Retrieving server status...";
		onlineCount.TextAlign = (ContentAlignment)16;
		((Control)onlineCount).Visible = false;
		((Control)loginButton).BackColor = Color.Transparent;
		((ButtonBase)loginButton).FlatAppearance.BorderSize = 0;
		((ButtonBase)loginButton).FlatAppearance.MouseDownBackColor = Color.Transparent;
		((ButtonBase)loginButton).FlatAppearance.MouseOverBackColor = Color.Transparent;
		((ButtonBase)loginButton).FlatStyle = (FlatStyle)0;
		((Control)loginButton).ForeColor = Color.White;
		((ButtonBase)loginButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_disable;
		((Control)loginButton).Location = new Point (340, 250);
		((Control)loginButton).Name = "loginButton";
		((Control)loginButton).Size = new Size (152, 38);
		((Control)loginButton).TabIndex = 12;
		((Control)loginButton).Text = "LOG IN";
		((ButtonBase)loginButton).UseVisualStyleBackColor = false;
		registerText.ActiveLinkColor = Color.FromArgb (181, 255, 33);
		((Control)registerText).AutoSize = true;
		((Control)registerText).BackColor = Color.Transparent;
		((Control)registerText).Font = new Font ("Microsoft Sans Serif", 9f, (FontStyle)3, (GraphicsUnit)3, (byte)238);
		registerText.LinkColor = Color.FromArgb (181, 255, 33);
		((Control)registerText).Location = new Point (527, 260);
		((Control)registerText).Name = "registerText";
		((Control)registerText).Size = new Size (83, 15);
		((Control)registerText).TabIndex = 13;
		registerText.TabStop = true;
		((Control)registerText).Text = "regsiterText";
		registerText.VisitedLinkColor = Color.FromArgb (181, 255, 33);
		registerText.LinkClicked += new LinkLabelLinkClickedEventHandler (registerText_LinkClicked);
		((Control)emailLabel).AutoSize = true;
		((Control)emailLabel).BackColor = Color.Transparent;
		((Control)emailLabel).Font = new Font ("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)238);
		((Control)emailLabel).ForeColor = Color.FromArgb (76, 178, 255);
		((Control)emailLabel).Location = new Point (30, 161);
		((Control)emailLabel).Name = "emailLabel";
		((Control)emailLabel).Size = new Size (131, 18);
		((Control)emailLabel).TabIndex = 14;
		((Control)emailLabel).Text = "EMAIL ADDRESS:";
		emailLabel.TextAlign = (ContentAlignment)256;
		((Control)passwordLabel).AutoSize = true;
		((Control)passwordLabel).BackColor = Color.Transparent;
		((Control)passwordLabel).Font = new Font ("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)238);
		((Control)passwordLabel).ForeColor = Color.FromArgb (76, 178, 255);
		((Control)passwordLabel).Location = new Point (276, 161);
		((Control)passwordLabel).Name = "passwordLabel";
		((Control)passwordLabel).Size = new Size (100, 18);
		((Control)passwordLabel).TabIndex = 15;
		((Control)passwordLabel).Text = "PASSWORD:";
		passwordLabel.TextAlign = (ContentAlignment)256;
		((Control)troubleLabel).AutoSize = true;
		((Control)troubleLabel).BackColor = Color.Transparent;
		((Control)troubleLabel).Font = new Font ("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)238);
		((Control)troubleLabel).ForeColor = Color.White;
		((Control)troubleLabel).Location = new Point (30, 255);
		((Control)troubleLabel).Name = "troubleLabel";
		((Control)troubleLabel).Size = new Size (148, 16);
		((Control)troubleLabel).TabIndex = 16;
		((Control)troubleLabel).Text = "HAVING TROUBLE?";
		githubLink.ActiveLinkColor = Color.FromArgb (76, 178, 255);
		((Control)githubLink).AutoSize = true;
		((Control)githubLink).BackColor = Color.Transparent;
		githubLink.DisabledLinkColor = Color.FromArgb (76, 178, 255);
		((Control)githubLink).Font = new Font ("Microsoft Sans Serif", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)238);
		((Control)githubLink).ForeColor = Color.FromArgb (76, 178, 255);
		githubLink.LinkColor = Color.FromArgb (76, 178, 255);
		((Control)githubLink).Location = new Point (30, 271);
		((Control)githubLink).Name = "githubLink";
		((Control)githubLink).Size = new Size (186, 16);
		((Control)githubLink).TabIndex = 17;
		githubLink.TabStop = true;
		((Control)githubLink).Text = "Visit our \"GitHub Issues\" page";
		githubLink.VisitedLinkColor = Color.FromArgb (76, 178, 255);
		((Control)registerButton).BackColor = Color.Transparent;
		((ButtonBase)registerButton).FlatAppearance.BorderSize = 0;
		((ButtonBase)registerButton).FlatAppearance.MouseDownBackColor = Color.Transparent;
		((ButtonBase)registerButton).FlatAppearance.MouseOverBackColor = Color.Transparent;
		((ButtonBase)registerButton).FlatStyle = (FlatStyle)0;
		((Control)registerButton).ForeColor = Color.White;
		((ButtonBase)registerButton).Image = (Image)(object)GameLauncher.Properties.Resources.button_enable;
		((Control)registerButton).Location = new Point (623, 319);
		((Control)registerButton).Name = "registerButton";
		((Control)registerButton).Size = new Size (149, 37);
		((Control)registerButton).TabIndex = 19;
		((Control)registerButton).Text = "REGISTER";
		((ButtonBase)registerButton).UseVisualStyleBackColor = false;
		((Control)selectServerLabel).BackColor = Color.Transparent;
		((Control)selectServerLabel).Font = new Font ("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)238);
		((Control)selectServerLabel).ForeColor = Color.White;
		((Control)selectServerLabel).Location = new Point (370, 50);
		((Control)selectServerLabel).Name = "selectServerLabel";
		((Control)selectServerLabel).Size = new Size (171, 15);
		((Control)selectServerLabel).TabIndex = 20;
		((Control)selectServerLabel).Text = "SELECT SERVER:";
		selectServerLabel.TextAlign = (ContentAlignment)64;
		((Control)settingsButton).BackColor = Color.Transparent;
		((Control)settingsButton).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.settingsbtn;
		((Control)settingsButton).Location = new Point (741, 44);
		((Control)settingsButton).Name = "settingsButton";
		((Control)settingsButton).Size = new Size (31, 29);
		settingsButton.TabIndex = 21;
		settingsButton.TabStop = false;
		((Control)moveWindow).BackColor = Color.Transparent;
		((Control)moveWindow).Controls.Add ((Control)(object)playLoggedInAs);
		if (promoWebView != null && promoWebView != null) {
			((Control)this).Controls.Add ((Control)(object)promoWebView);
		}
		((Control)this).Controls.Add ((Control)(object)promoBrowser);
		((Control)moveWindow).Controls.Add ((Control)(object)logoutButton);
		((Control)moveWindow).Controls.Add ((Control)(object)selectServerLabel);
		((Control)moveWindow).Location = new Point (1, 1);
		((Control)moveWindow).Name = "moveWindow";
		((Control)moveWindow).Size = new Size (788, 87);
		((Control)moveWindow).TabIndex = 1;
		((Control)playLoggedInAs).AutoSize = true;
		((Control)playLoggedInAs).Font = new Font ("Microsoft Sans Serif", 8.25f, (FontStyle)1, (GraphicsUnit)3, (byte)238);
		((Control)playLoggedInAs).ForeColor = Color.FromArgb (76, 178, 255);
		((Control)playLoggedInAs).Location = new Point (354, 51);
		((Control)playLoggedInAs).Name = "playLoggedInAs";
		((Control)playLoggedInAs).Size = new Size (95, 13);
		((Control)playLoggedInAs).TabIndex = 0;
		((Control)playLoggedInAs).Text = "LOGGED IN AS";
		((Control)logoutButton).BackColor = Color.Transparent;
		((ButtonBase)logoutButton).FlatAppearance.BorderSize = 0;
		((ButtonBase)logoutButton).FlatAppearance.MouseDownBackColor = Color.Transparent;
		((ButtonBase)logoutButton).FlatAppearance.MouseOverBackColor = Color.Transparent;
		((ButtonBase)logoutButton).FlatStyle = (FlatStyle)0;
		((Control)logoutButton).Font = new Font ("Microsoft Sans Serif", 8.25f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)logoutButton).ForeColor = Color.White;
		((Control)logoutButton).Location = new Point (655, 42);
		((Control)logoutButton).Name = "logoutButton";
		((Control)logoutButton).Size = new Size (120, 28);
		((Control)logoutButton).TabIndex = 41;
		((Control)logoutButton).Text = "LOG OUT";
		((ButtonBase)logoutButton).TextAlign = (ContentAlignment)32;
		((ButtonBase)logoutButton).UseVisualStyleBackColor = false;
		((Control)logoutButton).Visible = false;
		try {
			Image val2 = LoadEmbedded ("GameLauncher.Logout.logout0.png");
			if (val2 != null) {
				((ButtonBase)logoutButton).Image = val2;
			}
		} catch {
		}
		((Control)verticalBanner).BackColor = Color.Transparent;
		((Control)verticalBanner).Location = new Point (526, 105);
		((Control)verticalBanner).Name = "verticalBanner";
		((Control)verticalBanner).Size = new Size (249, 118);
		verticalBanner.SizeMode = (PictureBoxSizeMode)1;
		verticalBanner.TabIndex = 22;
		verticalBanner.TabStop = false;
		((Control)settingsSave).BackColor = Color.Transparent;
		((ButtonBase)settingsSave).FlatAppearance.BorderSize = 0;
		((ButtonBase)settingsSave).FlatAppearance.MouseDownBackColor = Color.Transparent;
		((ButtonBase)settingsSave).FlatAppearance.MouseOverBackColor = Color.Transparent;
		((ButtonBase)settingsSave).FlatStyle = (FlatStyle)0;
		((Control)settingsSave).ForeColor = Color.White;
		((ButtonBase)settingsSave).Image = (Image)(object)GameLauncher.Properties.Resources.button_enable;
		((Control)settingsSave).Location = new Point (613, 420);
		((Control)settingsSave).Name = "settingsSave";
		((Control)settingsSave).Size = new Size (149, 37);
		((Control)settingsSave).TabIndex = 23;
		((Control)settingsSave).Text = "CONTINUE";
		((ButtonBase)settingsSave).UseVisualStyleBackColor = false;
		((Control)settingsLanguage).BackColor = Color.White;
		settingsLanguage.DropDownStyle = (ComboBoxStyle)2;
		((Control)settingsLanguage).Font = new Font ("Microsoft Sans Serif", 8.25f, (FontStyle)1, (GraphicsUnit)3, (byte)238);
		((Control)settingsLanguage).ForeColor = Color.FromArgb (51, 51, 51);
		((ListControl)settingsLanguage).FormattingEnabled = true;
		((Control)settingsLanguage).Location = new Point (77, 198);
		((Control)settingsLanguage).Name = "settingsLanguage";
		((Control)settingsLanguage).Size = new Size (185, 21);
		((Control)settingsLanguage).TabIndex = 24;
		((Control)settingsLanguageText).AutoSize = true;
		((Control)settingsLanguageText).BackColor = Color.Transparent;
		((Control)settingsLanguageText).ForeColor = Color.White;
		((Control)settingsLanguageText).Location = new Point (74, 180);
		((Control)settingsLanguageText).Name = "settingsLanguageText";
		((Control)settingsLanguageText).Size = new Size (113, 13);
		((Control)settingsLanguageText).TabIndex = 25;
		((Control)settingsLanguageText).Text = "SELECT LANGUAGE:";
		((Control)settingsQuality).BackColor = Color.White;
		settingsQuality.DropDownStyle = (ComboBoxStyle)2;
		((Control)settingsQuality).Font = new Font ("Microsoft Sans Serif", 8.25f, (FontStyle)1, (GraphicsUnit)3, (byte)238);
		((Control)settingsQuality).ForeColor = Color.FromArgb (51, 51, 51);
		((ListControl)settingsQuality).FormattingEnabled = true;
		((Control)settingsQuality).Location = new Point (77, 269);
		((Control)settingsQuality).Name = "settingsQuality";
		((Control)settingsQuality).Size = new Size (185, 21);
		((Control)settingsQuality).TabIndex = 26;
		((Control)settingsQualityText).AutoSize = true;
		((Control)settingsQualityText).BackColor = Color.Transparent;
		((Control)settingsQualityText).ForeColor = Color.White;
		((Control)settingsQualityText).Location = new Point (74, 251);
		((Control)settingsQualityText).Name = "settingsQualityText";
		((Control)settingsQualityText).Size = new Size (145, 13);
		((Control)settingsQualityText).TabIndex = 27;
		((Control)settingsQualityText).Text = "SELECT DOWNLOAD SIZE:";
		((Control)settingsLanguageDesc).BackColor = Color.Transparent;
		((Control)settingsLanguageDesc).ForeColor = Color.White;
		((Control)settingsLanguageDesc).Location = new Point (314, 179);
		((Control)settingsLanguageDesc).Name = "settingsLanguageDesc";
		((Control)settingsLanguageDesc).Size = new Size (427, 56);
		((Control)settingsLanguageDesc).TabIndex = 28;
		((Control)settingsLanguageDesc).Text = "Select the language that the game text and audio should be displayed in. This will not affect your chat or server options.";
		((Control)settingsQualityDesc).BackColor = Color.Transparent;
		((Control)settingsQualityDesc).ForeColor = Color.White;
		((Control)settingsQualityDesc).Location = new Point (313, 242);
		((Control)settingsQualityDesc).Name = "settingsQualityDesc";
		((Control)settingsQualityDesc).Size = new Size (405, 85);
		((Control)settingsQualityDesc).TabIndex = 29;
		((Control)settingsQualityDesc).Text = ((ResourceManager)(object)componentResourceManager).GetString ("settingsQualityDesc.Text");
		((Control)registerEmail).Location = new Point (33, 183);
		((Control)registerEmail).Name = "registerEmail";
		((Control)registerEmail).Size = new Size (210, 20);
		((Control)registerEmail).TabIndex = 30;
		((Control)registerTicket).Location = new Point (33, 245);
		((Control)registerTicket).Name = "registerTicket";
		((Control)registerTicket).Size = new Size (210, 20);
		((Control)registerTicket).TabIndex = 31;
		((Control)registerPassword).Location = new Point (276, 183);
		((Control)registerPassword).Name = "registerPassword";
		registerPassword.PasswordChar = '●';
		((Control)registerPassword).Size = new Size (210, 20);
		((Control)registerPassword).TabIndex = 32;
		((Control)registerConfirmPassword).Location = new Point (276, 245);
		((Control)registerConfirmPassword).Name = "registerConfirmPassword";
		registerConfirmPassword.PasswordChar = '●';
		((Control)registerConfirmPassword).Size = new Size (210, 20);
		((Control)registerConfirmPassword).TabIndex = 33;
		((Control)registerEmailText).AutoSize = true;
		((Control)registerEmailText).BackColor = Color.Transparent;
		((Control)registerEmailText).Font = new Font ("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)238);
		((Control)registerEmailText).ForeColor = Color.FromArgb (76, 178, 255);
		((Control)registerEmailText).Location = new Point (30, 161);
		((Control)registerEmailText).Name = "registerEmailText";
		((Control)registerEmailText).Size = new Size (131, 18);
		((Control)registerEmailText).TabIndex = 34;
		((Control)registerEmailText).Text = "EMAIL ADDRESS:";
		registerEmailText.TextAlign = (ContentAlignment)256;
		((Control)registerPasswordText).AutoSize = true;
		((Control)registerPasswordText).BackColor = Color.Transparent;
		((Control)registerPasswordText).Font = new Font ("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)238);
		((Control)registerPasswordText).ForeColor = Color.FromArgb (76, 178, 255);
		((Control)registerPasswordText).Location = new Point (276, 161);
		((Control)registerPasswordText).Name = "registerPasswordText";
		((Control)registerPasswordText).Size = new Size (100, 18);
		((Control)registerPasswordText).TabIndex = 35;
		((Control)registerPasswordText).Text = "PASSWORD:";
		registerPasswordText.TextAlign = (ContentAlignment)256;
		((Control)registerTicketText).AutoSize = true;
		((Control)registerTicketText).BackColor = Color.Transparent;
		((Control)registerTicketText).Font = new Font ("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)238);
		((Control)registerTicketText).ForeColor = Color.FromArgb (76, 178, 255);
		((Control)registerTicketText).Location = new Point (30, 224);
		((Control)registerTicketText).Name = "registerTicketText";
		((Control)registerTicketText).Size = new Size (64, 18);
		((Control)registerTicketText).TabIndex = 36;
		((Control)registerTicketText).Text = "TICKET:";
		registerTicketText.TextAlign = (ContentAlignment)256;
		((Control)registerConfirmPasswordText).AutoSize = true;
		((Control)registerConfirmPasswordText).BackColor = Color.Transparent;
		((Control)registerConfirmPasswordText).Font = new Font ("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)238);
		((Control)registerConfirmPasswordText).ForeColor = Color.FromArgb (76, 178, 255);
		((Control)registerConfirmPasswordText).Location = new Point (276, 224);
		((Control)registerConfirmPasswordText).Name = "registerConfirmPasswordText";
		((Control)registerConfirmPasswordText).Size = new Size (174, 18);
		((Control)registerConfirmPasswordText).TabIndex = 37;
		((Control)registerConfirmPasswordText).Text = "CONFIRM PASSWORD:";
		registerConfirmPasswordText.TextAlign = (ContentAlignment)256;
		((Control)registerAgree).AutoSize = true;
		((Control)registerAgree).BackColor = Color.Transparent;
		((Control)registerAgree).ForeColor = Color.FromArgb (76, 178, 255);
		((Control)registerAgree).Location = new Point (33, 330);
		((Control)registerAgree).Name = "registerAgree";
		((Control)registerAgree).Size = new Size (333, 17);
		((Control)registerAgree).TabIndex = 38;
		((Control)registerAgree).Text = "BY REGISTERING YOU AGREE TO THE TERMS OF SERVICE";
		((ButtonBase)registerAgree).UseVisualStyleBackColor = false;
		((Control)playProgress).Location = new Point (20, 436);
		((Control)playProgress).Name = "playProgress";
		((Control)playProgress).Size = new Size (580, 18);
		((Control)playProgress).TabIndex = 39;
		((Control)playProgressText).AutoSize = true;
		((Control)playProgressText).BackColor = Color.Transparent;
		((Control)playProgressText).Font = new Font ("Microsoft Sans Serif", 8.25f, (FontStyle)1, (GraphicsUnit)3, (byte)238);
		((Control)playProgressText).ForeColor = Color.FromArgb (76, 178, 255);
		((Control)playProgressText).Location = new Point (17, 457);
		((Control)playProgressText).Name = "playProgressText";
		((Control)playProgressText).Size = new Size (480, 13);
		((Control)playProgressText).TabIndex = 21;
		((Control)playProgressText).Text = "PLEASE WAIT";
		((Control)playButton).BackColor = Color.Transparent;
		((ButtonBase)playButton).FlatAppearance.BorderSize = 0;
		((ButtonBase)playButton).FlatAppearance.MouseDownBackColor = Color.Transparent;
		((ButtonBase)playButton).FlatAppearance.MouseOverBackColor = Color.Transparent;
		((ButtonBase)playButton).FlatStyle = (FlatStyle)0;
		((Control)playButton).BackColor = Color.Transparent;
		((Control)playButton).ForeColor = Color.White;
		((ButtonBase)playButton).Image = null;
		((Control)playButton).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.playButton_enable;
		((Control)playButton).BackgroundImageLayout = (ImageLayout)2;
		((Control)playButton).Location = new Point (618, 424);
		((Control)playButton).Margin = new Padding (0);
		((Control)playButton).Padding = new Padding (0);
		((Control)playButton).Name = "playButton";
		((Control)playButton).Size = new Size (165, 58);
		((Control)playButton).TabIndex = 40;
		((Control)playButton).Text = "PLAY";
		((ButtonBase)playButton).TextAlign = (ContentAlignment)32;
		((ButtonBase)playButton).UseCompatibleTextRendering = true;
		((ButtonBase)playButton).UseVisualStyleBackColor = false;
		Notification.Text = "notifyIcon1";
		Notification.Visible = false;
		forgotPassword.ActiveLinkColor = Color.FromArgb (76, 178, 255);
		((Control)forgotPassword).BackColor = Color.Transparent;
		forgotPassword.DisabledLinkColor = Color.FromArgb (76, 178, 255);
		((Control)forgotPassword).Font = new Font ("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)238);
		((Control)forgotPassword).ForeColor = Color.FromArgb (76, 178, 255);
		forgotPassword.LinkArea = new LinkArea (0, 100);
		forgotPassword.LinkColor = Color.FromArgb (76, 178, 255);
		((Control)forgotPassword).Location = new Point (276, 206);
		((Control)forgotPassword).Name = "forgotPassword";
		((Control)forgotPassword).Size = new Size (210, 21);
		((Control)forgotPassword).TabIndex = 0;
		forgotPassword.TabStop = true;
		((Control)forgotPassword).Text = "I forgot my password.";
		((Label)forgotPassword).TextAlign = (ContentAlignment)1024;
		forgotPassword.UseCompatibleTextRendering = true;
		forgotPassword.VisitedLinkColor = Color.FromArgb (76, 178, 255);
		((Control)playProgressTime).Anchor = (AnchorStyles)9;
		((Control)playProgressTime).BackColor = Color.Transparent;
		((Control)playProgressTime).Cursor = Cursors.Default;
		((Control)playProgressTime).Font = new Font ("Microsoft Sans Serif", 8.25f, (FontStyle)1, (GraphicsUnit)3, (byte)238);
		((Control)playProgressTime).ForeColor = Color.FromArgb (76, 178, 255);
		((Control)playProgressTime).Location = new Point (506, 457);
		((Control)playProgressTime).Name = "playProgressTime";
		((Control)playProgressTime).Size = new Size (160, 13);
		((Control)playProgressTime).TabIndex = 42;
		((Control)playProgressTime).Text = "CALCULATING";
		playProgressTime.TextAlign = (ContentAlignment)4;
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)0;
		((Control)this).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.loginbg;
		((Control)this).BackgroundImageLayout = (ImageLayout)3;
		((Form)this).ClientSize = new Size (790, 490);
		((Control)this).Controls.Add ((Control)(object)playProgressTime);
		((Control)this).Controls.Add ((Control)(object)playButton);
		((Control)this).Controls.Add ((Control)(object)playProgressText);
		((Control)this).Controls.Add ((Control)(object)playProgress);
		((Control)this).Controls.Add ((Control)(object)registerAgree);
		((Control)this).Controls.Add ((Control)(object)registerConfirmPasswordText);
		((Control)this).Controls.Add ((Control)(object)registerTicketText);
		((Control)this).Controls.Add ((Control)(object)registerPasswordText);
		((Control)this).Controls.Add ((Control)(object)registerEmailText);
		((Control)this).Controls.Add ((Control)(object)registerConfirmPassword);
		((Control)this).Controls.Add ((Control)(object)registerPassword);
		((Control)this).Controls.Add ((Control)(object)registerTicket);
		((Control)this).Controls.Add ((Control)(object)registerEmail);
		((Control)this).Controls.Add ((Control)(object)settingsQualityDesc);
		((Control)this).Controls.Add ((Control)(object)settingsQualityText);
		((Control)this).Controls.Add ((Control)(object)settingsQuality);
		((Control)this).Controls.Add ((Control)(object)settingsLanguageText);
		((Control)this).Controls.Add ((Control)(object)settingsLanguage);
		((Control)this).Controls.Add ((Control)(object)verticalBanner);
		((Control)this).Controls.Add ((Control)(object)settingsButton);
		((Control)this).Controls.Add ((Control)(object)registerButton);
		((Control)this).Controls.Add ((Control)(object)forgotPassword);
		((Control)this).Controls.Add ((Control)(object)githubLink);
		((Control)this).Controls.Add ((Control)(object)troubleLabel);
		((Control)this).Controls.Add ((Control)(object)passwordLabel);
		((Control)this).Controls.Add ((Control)(object)emailLabel);
		((Control)this).Controls.Add ((Control)(object)registerText);
		((Control)this).Controls.Add ((Control)(object)loginButton);
		((Control)this).Controls.Add ((Control)(object)onlineCount);
		((Control)this).Controls.Add ((Control)(object)rememberMe);
		((Control)this).Controls.Add ((Control)(object)bottomBanner);
		((Control)this).Controls.Add ((Control)(object)advertisementLabel);
		((Control)this).Controls.Add ((Control)(object)consoleLog);
		((Control)this).Controls.Add ((Control)(object)currentWindowInfo);
		((Control)this).Controls.Add ((Control)(object)serverPick);
		((Control)this).Controls.Add ((Control)(object)serverStatusImg);
		((Control)this).Controls.Add ((Control)(object)serverStatus);
		((Control)this).Controls.Add ((Control)(object)password);
		((Control)this).Controls.Add ((Control)(object)email);
		((Control)this).Controls.Add ((Control)(object)minimizebtn);
		((Control)this).Controls.Add ((Control)(object)closebtn);
		((Control)this).Controls.Add ((Control)(object)moveWindow);
		((Control)this).Controls.Add ((Control)(object)settingsSave);
		((Control)this).Controls.Add ((Control)(object)settingsLanguageDesc);
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).Icon = (Icon)((ResourceManager)(object)componentResourceManager).GetObject ("$this.Icon");
		((Form)this).MaximizeBox = false;
		((Control)this).Name = "mainScreen";
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "Need for Speed World";
		((Form)this).TransparencyKey = Color.FromArgb (204, 0, 0);
		((Form)this).Load += mainScreen_Load;
		((ISupportInitialize)closebtn).EndInit ();
		((ISupportInitialize)minimizebtn).EndInit ();
		((ISupportInitialize)serverStatusImg).EndInit ();
		((ISupportInitialize)settingsButton).EndInit ();
		((Control)moveWindow).ResumeLayout (false);
		((Control)moveWindow).PerformLayout ();
		((ISupportInitialize)verticalBanner).EndInit ();
		((ISupportInitialize)bottomBanner).EndInit ();
		((Control)this).ResumeLayout (false);
		((Control)this).PerformLayout ();
	}

	private void ClearAllContextMenus ()
	{
		((Control)this).ContextMenu = null;
		((Control)this).Text = "Need for Speed World";
		if (Notification != null) {
			Notification.Text = "Need for Speed World";
		}
		int count = ((ArrangedElementCollection)((Control)this).Controls).Count;
		for (int i = 0; i < count; i++) {
			Control val = ((Control)this).Controls [i];
			if (val != null) {
				val.ContextMenu = null;
			}
		}
	}

	private void OnPromoDelayTick (object sender, EventArgs e)
	{
		if (Timeout != null) {
			Timeout.Stop ();
		}
		ShowPromoPanel ();
		promoAlreadyShown = true;
	}

	private void PromoFirstShowDelayed ()
	{
		if (promoAlreadyShown) {
			ShowPromoPanel ();
			return;
		}
		if (promoBrowser != null) {
			((Control)promoBrowser).Visible = false;
		}
		if (promoWebView != null) {
			((Control)promoWebView).Visible = false;
		}
		if (Timeout != null) {
			Timeout.Stop ();
			Timeout.Interval = 1500;
			Timeout.Tick += OnPromoDelayTick;
			Timeout.Start ();
		} else {
			ShowPromoPanel ();
			promoAlreadyShown = true;
		}
	}

	private void HidePlayBanners ()
	{
		if (bottomBanner != null) {
			((Control)bottomBanner).Visible = false;
		}
		if (advertisementLabel != null) {
			((Control)advertisementLabel).Visible = false;
		}
		if (verticalBanner != null) {
			((Control)verticalBanner).Visible = false;
		}
	}

	private void UpdateBannerFromState ()
	{
		if (inPlayScreen) {
			HidePlayBanners ();
			return;
		}
		bool flag = true;
		bool flag2 = false;
		if (serverStatus != null) {
			string text = ((Control)serverStatus).Text;
			if (text != null) {
				flag2 = text.ToLower ().IndexOf ("up and running") >= 0;
			}
		}
		if (advertisementLabel != null) {
			((Control)advertisementLabel).Visible = flag;
		}
		if (bottomBanner != null) {
			((Control)bottomBanner).Visible = flag && flag2;
		}
	}

	private void HideTrayIcon ()
	{
		if (Notification != null) {
			Notification.Visible = false;
		}
	}

	private bool IsServerOnline ()
	{
		if (serverStatus != null) {
			string text = ((Control)serverStatus).Text;
			if (text != null) {
				string text2 = text.ToLower ();
				if (text2.IndexOf ("retriev") < 0) {
					return text2.IndexOf ("currently up and running") >= 0;
				}
			}
		}
		return false;
	}

	private void EnsureAdBannerVisibleSafe ()
	{
		if (inPlayScreen) {
			if (bottomBanner != null) {
				((Control)bottomBanner).Visible = false;
			}
			if (advertisementLabel != null) {
				((Control)advertisementLabel).Visible = false;
			}
		} else if (!IsServerOnline ()) {
			if (bottomBanner != null) {
				((Control)bottomBanner).Visible = false;
			}
			if (advertisementLabel != null) {
				((Control)advertisementLabel).Visible = true;
			}
		} else {
			EnsureAdBannerVisible ();
		}
	}
}
