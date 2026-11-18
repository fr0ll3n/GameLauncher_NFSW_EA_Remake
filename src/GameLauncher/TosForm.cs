using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GameLauncher;

public class TosForm : Form
{
	private Label titleLabel;

	private TextBox tosBox;

	private Label btnAccept;

	private Label btnDecline;

	private ImageList btnImages;

	private bool drag;

	private Point dragStart;

	private Point dragCursor;

	private bool hoverAccept;

	private bool hoverDecline;

	public bool Accepted { get; private set; }

	public TosForm ()
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Expected O, but got Unknown
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Expected O, but got Unknown
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Expected O, but got Unknown
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Expected O, but got Unknown
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Expected O, but got Unknown
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Expected O, but got Unknown
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Expected O, but got Unknown
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Expected O, but got Unknown
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Expected O, but got Unknown
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Expected O, but got Unknown
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Expected O, but got Unknown
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).StartPosition = (FormStartPosition)1;
		((Form)this).ClientSize = new Size (517, 438);
		((Control)this).MinimumSize = new Size (517, 438);
		((Control)this).BackColor = Color.Black;
		((Control)this).ForeColor = Color.FromArgb (51, 153, 255);
		((Control)this).Text = "Terms of Service";
		((Form)this).ShowInTaskbar = false;
		((Form)this).TopMost = true;
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).DoubleBuffered = true;
		((Form)this).TransparencyKey = Color.Red;
		try {
			((Control)this).BackgroundImage = LoadRes ("GameLauncher.Tos.nfsw_lp_tos_96dpi_bg.png");
			((Control)this).BackgroundImageLayout = (ImageLayout)0;
		} catch {
		}
		try {
			using Stream stream = AsmStream ("GameLauncher.Tos.tos.ico");
			if (stream != null) {
				((Form)this).Icon = new Icon (stream);
			}
		} catch {
		}
		btnImages = new ImageList {
			ColorDepth = (ColorDepth)32,
			ImageSize = new Size (131, 31)
		};
		try {
			btnImages.Images.Add (LoadRes ("GameLauncher.Tos.btn0.png"));
			btnImages.Images.Add (LoadRes ("GameLauncher.Tos.btn1.png"));
			btnImages.Images.Add (LoadRes ("GameLauncher.Tos.btn2.png"));
		} catch {
		}
		Font font = new Font ("Segoe UI", 12f, (FontStyle)1);
		Font font2 = new Font ("Segoe UI", 9f, (FontStyle)0);
		Font font3 = new Font ("Segoe UI", 11f, (FontStyle)1);
		titleLabel = new Label {
			BackColor = Color.Transparent,
			ForeColor = Color.FromArgb (76, 178, 255),
			Location = new Point (18, 6),
			Size = new Size (481, 21),
			Text = "NFS WORLD TERMS OF SERVICE",
			TextAlign = (ContentAlignment)1,
			Font = font
		};
		StringBuilder stringBuilder = new StringBuilder ();
		stringBuilder.AppendLine ("NEED FOR SPEED WORLD — COMMUNITY LAUNCHER TERMS");
		stringBuilder.AppendLine ();
		stringBuilder.AppendLine ("By clicking \"I ACCEPT\" you agree that:");
		stringBuilder.AppendLine ();
		stringBuilder.AppendLine ("1. This is an unofficial community launcher for private Need for Speed World servers (e.g. NightRiderz Horizon, WorldUnited). It is not affiliated with, endorsed by, or connected to Electronic Arts Inc.");
		stringBuilder.AppendLine ();
		stringBuilder.AppendLine ("2. You will play only on servers you are authorized to access and will follow each server's rules and code of conduct.");
		stringBuilder.AppendLine ();
		stringBuilder.AppendLine ("3. Game files, mods, and downloads are provided by third-party communities. Use them at your own risk. Run the launcher as Administrator when required for file writes.");
		stringBuilder.AppendLine ();
		stringBuilder.AppendLine ("4. Your account credentials are sent only to the game server you select. Do not reuse passwords from other services.");
		stringBuilder.AppendLine ();
		stringBuilder.AppendLine ("5. The launcher authors and server operators are not responsible for bans, data loss, or damages arising from use of this software or community servers.");
		stringBuilder.AppendLine ();
		stringBuilder.AppendLine ("6. You may stop using this launcher at any time. Declining these terms will exit the application.");
		stringBuilder.AppendLine ();
		stringBuilder.AppendLine ("If you do not agree, click \"I DECLINE\" to exit.");
		tosBox = new TextBox {
			BackColor = Color.Black,
			ForeColor = Color.White,
			Location = new Point (19, 31),
			Size = new Size (481, 353),
			Multiline = true,
			ReadOnly = true,
			ScrollBars = (ScrollBars)2,
			BorderStyle = (BorderStyle)0,
			Font = font2,
			Text = stringBuilder.ToString ()
		};
		((TextBoxBase)tosBox).Select (0, 0);
		btnDecline = MakeChromeButton ("I DECLINE", new Point (18, 394), font3);
		btnAccept = MakeChromeButton ("I ACCEPT", new Point (374, 394), font3);
		((Control)btnDecline).Click += delegate {
			Accepted = false;
			((Form)this).DialogResult = (DialogResult)2;
			((Form)this).Close ();
		};
		((Control)btnAccept).Click += delegate {
			Accepted = true;
			((Form)this).DialogResult = (DialogResult)1;
			((Form)this).Close ();
		};
		WireHover (btnDecline, () => hoverDecline, delegate(bool v) {
			hoverDecline = v;
		});
		WireHover (btnAccept, () => hoverAccept, delegate(bool v) {
			hoverAccept = v;
		});
		((Control)this).Controls.Add ((Control)btnDecline);
		((Control)this).Controls.Add ((Control)btnAccept);
		((Control)this).Controls.Add ((Control)titleLabel);
		((Control)this).Controls.Add ((Control)tosBox);
		((Control)this).MouseDown += new MouseEventHandler (DragDown);
		((Control)this).MouseMove += new MouseEventHandler (DragMove);
		((Control)this).MouseUp += new MouseEventHandler (DragUp);
		((Control)titleLabel).MouseDown += new MouseEventHandler (DragDown);
		((Control)titleLabel).MouseMove += new MouseEventHandler (DragMove);
		((Control)titleLabel).MouseUp += new MouseEventHandler (DragUp);
	}

	private Label MakeChromeButton (string text, Point loc, Font font)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		Label val = new Label {
			BackColor = Color.Transparent,
			ForeColor = Color.White,
			Location = loc,
			Size = new Size (131, 31),
			Text = text,
			TextAlign = (ContentAlignment)32,
			Cursor = Cursors.Hand,
			Font = font
		};
		if (btnImages != null && btnImages.Images.Count > 0) {
			val.ImageList = btnImages;
			val.ImageIndex = 0;
		}
		return val;
	}

	private void WireHover (Label b, Func<bool> getHover, Action<bool> setHover)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		((Control)b).MouseEnter += delegate {
			if (!getHover () && b.ImageList != null && b.ImageList.Images.Count > 1) {
				b.ImageIndex = 1;
				setHover (obj: true);
			}
		};
		((Control)b).MouseLeave += delegate {
			if (getHover () && b.ImageList != null) {
				b.ImageIndex = 0;
				setHover (obj: false);
			}
		};
		((Control)b).MouseDown += (MouseEventHandler)delegate {
			if (b.ImageList != null && b.ImageList.Images.Count > 2) {
				b.ImageIndex = 2;
			}
		};
		((Control)b).MouseUp += (MouseEventHandler)delegate {
			if (b.ImageList != null && b.ImageList.Images.Count > 1) {
				b.ImageIndex = (getHover () ? 1 : 0);
			}
		};
	}

	private void DragDown (object s, MouseEventArgs e)
	{
		drag = true;
		dragStart = ((Form)this).Location;
		dragCursor = Cursor.Position;
	}

	private void DragMove (object s, MouseEventArgs e)
	{
		if (drag) {
			((Form)this).Location = new Point (dragStart.X + Cursor.Position.X - dragCursor.X, dragStart.Y + Cursor.Position.Y - dragCursor.Y);
		}
	}

	private void DragUp (object s, MouseEventArgs e)
	{
		drag = false;
	}

	private static Stream AsmStream (string name)
	{
		return Assembly.GetExecutingAssembly ().GetManifestResourceStream (name);
	}

	private static Image LoadRes (string name)
	{
		using Stream stream = AsmStream (name);
		if (stream == null) {
			throw new FileNotFoundException ("Missing embedded resource: " + name);
		}
		return Image.FromStream (stream);
	}
}
