using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using GameLauncher.Properties;

namespace GameLauncher.App.Classes.Events;

public class About : Form
{
	private IniFile SettingFile = new IniFile ("Settings.ini");

	private IContainer components;

	private Button closeAbout;

	private Label aboutInfo;

	public About ()
	{
		InitializeComponent ();
	}

	internal static void showAbout (object sender, EventArgs e)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		MessageBox.Show ("About page is not yet ready");
	}

	private void About_Load (object sender, EventArgs e)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		((Control)closeAbout).Click += closeAbout_Click;
		if (File.Exists (SettingFile.Read ("InstallationDirectory") + "X:\\__no_nfsw_cursor__\\default.cur")) {
			Cursor val = new Cursor (Cursor.Current.Handle);
			IntPtr intPtr = User32.LoadCursorFromFile (SettingFile.Read ("InstallationDirectory") + "X:\\__no_nfsw_cursor__\\default.cur");
			((object)val).GetType ().InvokeMember ("handle", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField, null, val, new object[1] { intPtr });
			((Control)this).Cursor = val;
		}
	}

	private void closeAbout_Click (object sender, EventArgs e)
	{
		((Form)this).Close ();
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
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager (typeof(About));
		closeAbout = new Button ();
		aboutInfo = new Label ();
		((Control)this).SuspendLayout ();
		((Control)closeAbout).Location = new Point (12, 394);
		((Control)closeAbout).Name = "closeAbout";
		((Control)closeAbout).Size = new Size (493, 32);
		((Control)closeAbout).TabIndex = 0;
		((Control)closeAbout).Text = "Close";
		((ButtonBase)closeAbout).UseVisualStyleBackColor = true;
		((Control)aboutInfo).AutoSize = true;
		((Control)aboutInfo).BackColor = Color.Transparent;
		((Control)aboutInfo).Font = new Font ("Microsoft Sans Serif", 12f, (FontStyle)1, (GraphicsUnit)3, (byte)238);
		((Control)aboutInfo).ForeColor = Color.White;
		((Control)aboutInfo).Location = new Point (123, 198);
		((Control)aboutInfo).Name = "aboutInfo";
		((Control)aboutInfo).Size = new Size (276, 20);
		((Control)aboutInfo).TabIndex = 1;
		((Control)aboutInfo).Text = "Close this form before its too late";
		((ContainerControl)this).AutoScaleDimensions = new SizeF (6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackgroundImage = (Image)(object)GameLauncher.Properties.Resources.aboutbg;
		((Form)this).ClientSize = new Size (517, 438);
		((Control)this).Controls.Add ((Control)(object)aboutInfo);
		((Control)this).Controls.Add ((Control)(object)closeAbout);
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).Icon = (Icon)((ResourceManager)(object)componentResourceManager).GetObject ("$this.Icon");
		((Control)this).Name = "About";
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "About GameLauncher";
		((Form)this).TopMost = true;
		((Form)this).TransparencyKey = Color.Red;
		((Form)this).Load += About_Load;
		((Control)this).ResumeLayout (false);
		((Control)this).PerformLayout ();
	}
}
