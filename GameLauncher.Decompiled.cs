using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Xml;
using GameLauncher.App.Classes;
using GameLauncher.App.Classes.Events;
using GameLauncher.App.Classes.Proxy;
using GameLauncher.Properties;
using GameLauncher.Resources;
using GameLauncherReborn;
using Microsoft.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Extensions;
using Nancy.Hosting.Self;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoapBox.JsonScheme;

[assembly: CompilationRelaxations (8)]
[assembly: RuntimeCompatibility (WrapNonExceptionThrows = true)]
[assembly: Debuggable (DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyTitle ("NFSW Launcher")]
[assembly: AssemblyDescription ("Official-style Need for Speed World launcher for community servers (Soapbox Race World).")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("NFSW Community")]
[assembly: AssemblyProduct ("NFSW Launcher")]
[assembly: AssemblyCopyright ("Copyright © NFSW Community. Open source. Not affiliated with Electronic Arts.")]
[assembly: AssemblyTrademark ("NFSW Launcher")]
[assembly: ComVisible (false)]
[assembly: Guid ("0a114c5c-3566-4a62-b05d-cba311988d8a")]
[assembly: AssemblyFileVersion ("1.0.1.0")]
[assembly: AssemblyInformationalVersion ("1.0.1")]
[assembly: TargetFramework (".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: SecurityPermission (SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion ("1.0.1.0")]
[module: UnverifiableCode]
[module: RefSafetyRules (11)]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	[AttributeUsage (AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
	internal sealed class RefSafetyRulesAttribute : Attribute
	{
		public readonly int Version;

		public RefSafetyRulesAttribute (int P_0)
		{
			Version = P_0;
		}
	}
}
namespace SoapBox.JsonScheme
{
	public class GetServerInformation
	{
		public string messageSrv { get; set; }

		public string homePageUrl { get; set; }

		public string facebookUrl { get; set; }

		public string discordUrl { get; set; }

		public string serverName { get; set; }

		public string country { get; set; }

		public int timezone { get; set; }

		public string bannerUrl { get; set; }

		public string adminList { get; set; }

		public string ownerList { get; set; }

		public int numberOfRegistered { get; set; }

		public List<string> activatedHolidaySceneryGroups { get; set; }

		public List<string> disactivatedHolidaySceneryGroups { get; set; }

		public int onlineNumber { get; set; }

		public string requireTicket { get; set; }

		public string serverVersion { get; set; }
	}
}
namespace GameLauncherReborn
{
	internal class Self
	{
		public static void runAsAdmin ()
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			string[] commandLineArgs = Environment.GetCommandLineArgs ();
			ProcessStartInfo processStartInfo = new ProcessStartInfo {
				Verb = "runas",
				FileName = Application.ExecutablePath
			};
			if (commandLineArgs.Length != 0) {
				processStartInfo.Arguments = commandLineArgs [0];
			}
			try {
				Process.Start (processStartInfo);
			} catch (Exception ex) {
				MessageBox.Show ("Failed to self-run as admin: " + ex);
			}
		}

		public static void centerScreen (Form form)
		{
			form.StartPosition = (FormStartPosition)0;
			((Control)form).Top = (Screen.PrimaryScreen.Bounds.Height - ((Control)form).Height) / 2;
			((Control)form).Left = (Screen.PrimaryScreen.Bounds.Width - ((Control)form).Width) / 2;
		}
	}
	public class WebClientWithTimeout : WebClient
	{
		protected override WebRequest GetWebRequest (Uri address)
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create (address);
			obj.UserAgent = "GameLauncher (+https://github.com/metonator/GameLauncher_NFSW)";
			obj.Timeout = 3000;
			return obj;
		}
	}
}
namespace GameLauncher
{
	public class CheckVersion
	{
		public string current_version { get; set; }

		public string github_build { get; set; }

		public Update update { get; set; }
	}
	public class Downloader
	{
		private const int LZMAOutPropsSize = 5;

		private const int LZMALengthSize = 8;

		private const int LZMAHeaderSize = 13;

		private const int HashThreads = 3;

		private const int DownloadThreads = 3;

		private const int DownloadChunks = 16;

		private ISynchronizeInvoke mFE;

		private Thread mThread;

		private ProgressUpdated mProgressUpdated;

		private DownloadFinished mDownloadFinished;

		private DownloadFailed mDownloadFailed;

		private ShowMessage mShowMessage;

		private static string mCurrentLocalVersion = string.Empty;

		private static string mCurrentServerVersion = string.Empty;

		private bool mDownloading;

		private int mHashThreads;

		private DownloadManager mDownloadManager;

		private static XmlDocument mIndexCached = null;

		private static bool mStopFlag = false;

		public bool Downloading {
			[CompilerGenerated]
			get {
				return mDownloading;
			}
		}

		public ProgressUpdated ProgressUpdated {
			get {
				return mProgressUpdated;
			}
			set {
				mProgressUpdated = value;
			}
		}

		public DownloadFinished DownloadFinished {
			get {
				return mDownloadFinished;
			}
			set {
				mDownloadFinished = value;
			}
		}

		public DownloadFailed DownloadFailed {
			get {
				return mDownloadFailed;
			}
			set {
				mDownloadFailed = value;
			}
		}

		public ShowMessage ShowMessage {
			get {
				return mShowMessage;
			}
			set {
				mShowMessage = value;
			}
		}

		public static string ServerVersion {
			[CompilerGenerated]
			get {
				return mCurrentServerVersion;
			}
		}

		public Downloader (ISynchronizeInvoke fe)
			: this (fe, 3, 3, 16)
		{
		}

		public Downloader (ISynchronizeInvoke fe, int hashThreads, int downloadThreads, int downloadChunks)
		{
			mHashThreads = hashThreads;
			mFE = fe;
			mDownloadManager = new DownloadManager (downloadThreads, downloadChunks);
		}

		public void StartDownload (string indexUrl, string package, string patchPath, bool calculateHashes, bool useIndexCache, ulong downloadSize)
		{
			mStopFlag = false;
			mThread = new Thread (Download);
			string[] parameter = new string[6] {
				indexUrl,
				package,
				patchPath,
				calculateHashes.ToString (),
				useIndexCache.ToString (),
				downloadSize.ToString ()
			};
			mThread.Start (parameter);
		}

		public void StartVerification (string indexUrl, string package, string patchPath, bool stopOnFail, bool clearHashes, bool writeHashes)
		{
			mStopFlag = false;
			mThread = new Thread (Verify);
			string[] parameter = new string[6] {
				indexUrl,
				package,
				patchPath,
				stopOnFail.ToString (),
				clearHashes.ToString (),
				writeHashes.ToString ()
			};
			mThread.Start (parameter);
		}

		public void Stop ()
		{
			mStopFlag = true;
			if (mDownloadManager != null && mDownloadManager.ManagerRunning) {
				mDownloadManager.CancelAllDownloads ();
			}
		}

		private void Downloader_DownloadFileCompleted (object sender, DownloadDataCompletedEventArgs e)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			string text = e.UserState.ToString ();
			if (e.Cancelled || ((AsyncCompletedEventArgs)e).Error != null) {
				MessageBox.Show ("Error downloading file '" + text + "'");
				if (((AsyncCompletedEventArgs)e).Error != null) {
					MessageBox.Show ("Downloader_DownloadFileCompleted Exception: " + ((AsyncCompletedEventArgs)e).Error.ToString ());
				}
			}
		}

		private XmlDocument GetIndexFile (string url, bool useCache)
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_0072: Expected O, but got Unknown
			try {
				if (useCache && mIndexCached != null) {
					return mIndexCached;
				}
				WebClient webClient = new WebClient ();
				webClient.DownloadDataCompleted += Downloader_DownloadFileCompleted;
				string tempFileName = Path.GetTempFileName ();
				webClient.DownloadFileAsync (new Uri (url), tempFileName);
				while (webClient.IsBusy) {
					if (mStopFlag) {
						webClient.CancelAsync ();
						return null;
					}
					Thread.Sleep (100);
				}
				XmlDocument val = new XmlDocument ();
				val.Load (tempFileName);
				mIndexCached = val;
				return val;
			} catch (Exception ex) {
				MessageBox.Show ("GetIndexFile Exception: " + ex.ToString ());
				return null;
			}
		}

		private void Download (object parameters)
		{
			//IL_0bb5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Expected O, but got Unknown
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Expected O, but got Unknown
			//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a92: Unknown result type (might be due to invalid IL or missing references)
			mDownloading = true;
			string[] obj = (string[])parameters;
			byte[] array = null;
			XmlNodeList val = null;
			string text = obj [0];
			string text2 = obj [1];
			if (!string.IsNullOrEmpty (text2)) {
				text = text + "/" + text2;
			}
			string text3 = obj [2];
			bool flag = bool.Parse (obj [3]);
			bool useCache = bool.Parse (obj [4]);
			ulong num = ulong.Parse (obj [5]);
			try {
				XmlDocument indexFile = GetIndexFile (text + "/index.xml", useCache);
				if (indexFile == null) {
					ISynchronizeInvoke obj2 = mFE;
					Delegate @delegate = mDownloadFailed;
					object[] array2 = new object[1];
					obj2.BeginInvoke (@delegate, array2);
					return;
				}
				long num2 = long.Parse (((XmlNode)indexFile).SelectSingleNode ("/index/header/length").InnerText);
				long num3 = 0L;
				long num4 = ((num == 0L) ? long.Parse (((XmlNode)indexFile).SelectSingleNode ("/index/header/compressed").InnerText) : ((long)num));
				long num5 = 0L;
				WebClient webClient = new WebClient ();
				webClient.Headers.Add ("Accept", "text/html,text/xml,application/xhtml+xml,application/xml,application/*,*/*;q=0.9,*/*;q=0.8");
				webClient.Headers.Add ("Accept-Language", "en-us,en;q=0.5");
				webClient.Headers.Add ("Accept-Encoding", "gzip,deflate");
				webClient.Headers.Add ("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
				int num6 = 1;
				array = null;
				val = ((XmlNode)indexFile).SelectNodes ("/index/fileinfo");
				mDownloadManager.Initialize (indexFile, text);
				if (flag) {
					HashManager.Instance.Clear ();
					HashManager.Instance.Start (indexFile, text3, text2 + ".hsh", mHashThreads);
				}
				int num7 = 0;
				List<string> list = new List<string> ();
				int i = 0;
				bool flag2 = false;
				int num11;
				foreach (XmlNode item in val) {
					XmlNode val2 = item;
					XmlNodeList val3 = val2.SelectNodes ("compressed");
					int num8 = ((val3.Count == 0) ? int.Parse (val2.SelectNodes ("length") [0].InnerText) : int.Parse (val3 [0].InnerText));
					num7 = ((num8 > num7) ? num8 : num7);
					string text4 = val2.SelectSingleNode ("path").InnerText;
					if (!string.IsNullOrEmpty (text3)) {
						int num9 = text4.IndexOf ("/");
						text4 = ((num9 >= 0) ? text4.Replace (text4.Substring (0, num9), text3) : text3);
					}
					string innerText = val2.SelectSingleNode ("file").InnerText;
					string fileName = text4 + "/" + innerText;
					int num10 = int.Parse (val2.SelectSingleNode ("section").InnerText);
					num11 = int.Parse (val2.SelectSingleNode ("offset").InnerText);
					if (flag) {
						if (list.Count == 0) {
							i = num10;
						}
						for (; i <= num10; i++) {
							list.Insert (0, $"{text}/section{i}.dat");
						}
						continue;
					}
					if (!HashManager.Instance.HashesMatch (fileName)) {
						if (i <= num10) {
							if (list.Count == 0) {
								i = num10;
							}
							for (; i <= num10; i++) {
								list.Insert (0, $"{text}/section{i}.dat");
							}
						}
						flag2 = true;
						continue;
					}
					if (flag2) {
						int num12 = num10;
						if (num11 == 0) {
							num12--;
						}
						for (; i <= num12; i++) {
							list.Insert (0, $"{text}/section{i}.dat");
						}
					}
					if (i < num10) {
						i = num10;
					}
					flag2 = false;
				}
				foreach (string item2 in list) {
					mDownloadManager.ScheduleFile (item2);
				}
				list.Clear ();
				list = null;
				num11 = 0;
				mDownloadManager.Start ();
				byte[] array3 = new byte[num7];
				byte[] array4 = new byte[13];
				int num13 = 0;
				foreach (XmlNode item3 in val) {
					XmlNode val4 = item3;
					if (mStopFlag) {
						break;
					}
					string text5 = val4.SelectSingleNode ("path").InnerText;
					string innerText2 = val4.SelectSingleNode ("file").InnerText;
					if (!string.IsNullOrEmpty (text3)) {
						int num14 = text5.IndexOf ("/");
						text5 = ((num14 >= 0) ? text5.Replace (text5.Substring (0, num14), text3) : text3);
					}
					string text6 = text5 + "/" + innerText2;
					int num15 = int.Parse (val4.SelectSingleNode ("length").InnerText);
					int num16 = 0;
					XmlNode val5 = val4.SelectSingleNode ("compressed");
					if (val4.SelectSingleNode ("section") != null && num6 < int.Parse (val4.SelectSingleNode ("section").InnerText)) {
						num6 = int.Parse (val4.SelectSingleNode ("section").InnerText);
					}
					string text7 = null;
					if (val4.SelectSingleNode ("hash") != null && HashManager.Instance.HashesMatch (text6)) {
						num16 += num15;
						if (val5 != null) {
							if (num == 0L) {
								num3 += int.Parse (val5.InnerText);
							}
							num5 += int.Parse (val5.InnerText);
							num11 += int.Parse (val5.InnerText);
						} else {
							if (num == 0L) {
								num3 += num15;
							}
							num5 += num15;
							num11 += num15;
						}
						if (mProgressUpdated != null) {
							object[] array5 = new object[4] { num2, num3, num4, text6 };
							mFE.Invoke ((Delegate)mProgressUpdated, array5);
						}
						int num17 = int.Parse (val4.SelectSingleNode ("section").InnerText);
						if (num13 != num17) {
							for (int j = num13 + 1; j < num17; j++) {
								mDownloadManager.CancelDownload ($"{text}/section{j}.dat");
							}
							num13 = num17 - 1;
						}
						continue;
					}
					Directory.CreateDirectory (text5);
					FileStream fileStream = File.Create (text6);
					int num18 = num15;
					if (val5 != null) {
						num18 = int.Parse (val5.InnerText);
					}
					int num19 = 0;
					bool flag3 = false;
					int num20 = 13;
					while (num19 < num18) {
						if (array == null || num11 >= array.Length) {
							num11 = ((val4.SelectSingleNode ("offset") != null && !flag3) ? int.Parse (val4.SelectSingleNode ("offset").InnerText) : 0);
							text7 = $"{text}/section{num6}.dat";
							for (int k = num13 + 1; k < num6; k++) {
								mDownloadManager.CancelDownload ($"{text}/section{k}.dat");
							}
							array = null;
							GC.Collect ();
							array = mDownloadManager.GetFile (text7);
							if (array == null) {
								MessageBox.Show ("DownloadManager returned a null buffer for file '" + text7 + "', aborting");
								if (mDownloadFailed != null) {
									if (!mStopFlag) {
										mFE.BeginInvoke ((Delegate)mDownloadFailed, new object[1] {
											new Exception ("DownloadManager returned a null buffer")
										});
										return;
									}
									ISynchronizeInvoke obj3 = mFE;
									Delegate delegate2 = mDownloadFailed;
									object[] array6 = new object[1];
									obj3.BeginInvoke (delegate2, array6);
								}
								return;
							}
							num13 = num6;
							num5 += array.Length;
							num6++;
							if (!mDownloadManager.GetStatus ($"{text}/section{num6}.dat").HasValue && num5 < num4) {
								mDownloadManager.ScheduleFile ($"{text}/section{num6}.dat");
							}
						} else {
							if (num18 - num19 > array.Length - num11) {
								text7 = $"{text}/section{num6}.dat";
								mDownloadManager.ScheduleFile (text7);
								flag3 = true;
							}
							int num21 = Math.Min (array.Length - num11, num18 - num19);
							if (num20 != 0) {
								if (val5 != null) {
									int num22 = Math.Min (num20, num21);
									Buffer.BlockCopy (array, num11, array4, 13 - num20, num22);
									Buffer.BlockCopy (array, num11 + num22, array3, 0, num21 - num22);
									num20 -= num22;
								} else {
									Buffer.BlockCopy (array, num11, array3, 0, num21);
									num20 = 0;
								}
							} else {
								Buffer.BlockCopy (array, num11, array3, num19 - ((val5 != null) ? 13 : 0), num21);
							}
							num11 += num21;
							num19 += num21;
							num3 += num21;
						}
						if (mProgressUpdated != null) {
							object[] array7 = new object[4] { num2, num3, num4, text6 };
							mFE.BeginInvoke ((Delegate)mProgressUpdated, array7);
						}
					}
					if (val5 != null) {
						if (!IsLzma (array4)) {
							MessageBox.Show ("Compression algorithm not recognized" + text7);
							throw new DownloaderException ("Compression algorithm not recognized: " + text7);
						}
						fileStream.Close ();
						fileStream.Dispose ();
						IntPtr outPropsSize = new IntPtr (5);
						byte[] array8 = new byte[5];
						for (int l = 0; l < 5; l++) {
							array8 [l] = array4 [l];
						}
						long num23 = 0L;
						for (int m = 0; m < 8; m++) {
							num23 += (long)(int)array4 [m + 5] << 8 * m;
						}
						if (num23 != num15) {
							MessageBox.Show ("Compression data length in header '" + num23 + "' != than in metadata '" + num15 + "'");
							throw new DownloaderException ("Compression data length in header '" + num23 + "' != than in metadata '" + num15 + "'");
						}
						num18 -= 13;
						IntPtr srcLen = new IntPtr (num18);
						IntPtr destLen = new IntPtr (num23);
						int num24 = LZMA.LzmaUncompressBuf2File (text6, ref destLen, array3, ref srcLen, array8, outPropsSize);
						if (num24 != 0) {
							MessageBox.Show ("Decompression returned " + num24);
							throw new UncompressionException (num24, "Decompression returned " + num24);
						}
						if (destLen.ToInt32 () != num15) {
							MessageBox.Show ("Decompression returned different size '" + destLen.ToInt32 () + "' than metadata '" + num15 + "'");
							throw new DownloaderException ("Decompression returned different size '" + destLen.ToInt32 () + "' than metadata '" + num15 + "'");
						}
						num16 += (int)destLen;
					} else {
						fileStream.Write (array3, 0, num15);
						num16 += num15;
					}
					if (fileStream != null) {
						fileStream.Close ();
						fileStream.Dispose ();
					}
				}
				if (!mStopFlag) {
					HashManager.Instance.WriteHashCache (text2 + ".hsh", writeOldHashes: false);
				}
				if (mStopFlag) {
					if (mDownloadFailed != null) {
						ISynchronizeInvoke obj4 = mFE;
						Delegate delegate3 = mDownloadFailed;
						object[] array9 = new object[1];
						obj4.BeginInvoke (delegate3, array9);
					}
				} else if (mDownloadFinished != null) {
					mFE.BeginInvoke ((Delegate)mDownloadFinished, (object[])null);
				}
			} catch (DownloaderException ex) {
				MessageBox.Show ("Download DownloaderException: " + ex.ToString ());
				if (mDownloadFailed != null) {
					try {
						mFE.BeginInvoke ((Delegate)mDownloadFailed, new object[1] { ex });
						return;
					} catch {
						return;
					}
				}
			} catch (Exception ex2) {
				MessageBox.Show ("Download Exception: " + ex2.ToString ());
				if (mDownloadFailed != null) {
					try {
						mFE.BeginInvoke ((Delegate)mDownloadFailed, new object[1] { ex2 });
						return;
					} catch {
						return;
					}
				}
			} finally {
				if (flag) {
					HashManager.Instance.Clear ();
				}
				mDownloadManager.Clear ();
				array = null;
				val = null;
				GC.Collect ();
				mDownloading = false;
			}
		}

		private void Verify (object parameters)
		{
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Expected O, but got Unknown
			string[] obj = (string[])parameters;
			string text = obj [0].Trim ();
			string text2 = obj [1].Trim ();
			if (!string.IsNullOrEmpty (text2)) {
				text = text + "/" + text2;
			}
			string text3 = obj [2].Trim ();
			bool flag = bool.Parse (obj [3]);
			bool flag2 = bool.Parse (obj [4]);
			bool flag3 = bool.Parse (obj [5]);
			bool flag4 = false;
			try {
				XmlDocument indexFile = GetIndexFile (text + "/index.xml", useCache: false);
				if (indexFile == null) {
					ISynchronizeInvoke obj2 = mFE;
					Delegate @delegate = mDownloadFailed;
					object[] array = new object[1];
					obj2.BeginInvoke (@delegate, array);
					return;
				}
				long num = long.Parse (((XmlNode)indexFile).SelectSingleNode ("/index/header/length").InnerText);
				WebClient webClient = new WebClient ();
				webClient.Headers.Add ("Accept", "text/html,text/xml,application/xhtml+xml,application/xml,application/*,*/*;q=0.9,*/*;q=0.8");
				webClient.Headers.Add ("Accept-Language", "en-us,en;q=0.5");
				webClient.Headers.Add ("Accept-Encoding", "gzip,deflate");
				webClient.Headers.Add ("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
				XmlNodeList obj3 = ((XmlNode)indexFile).SelectNodes ("/index/fileinfo");
				HashManager.Instance.Clear ();
				HashManager.Instance.Start (indexFile, text3, text2 + ".hsh", mHashThreads);
				long num2 = 0L;
				ulong num3 = 0uL;
				ulong num4 = 0uL;
				foreach (XmlNode item in obj3) {
					XmlNode val = item;
					string text4 = val.SelectSingleNode ("path").InnerText;
					string innerText = val.SelectSingleNode ("file").InnerText;
					if (!string.IsNullOrEmpty (text3)) {
						int num5 = text4.IndexOf ("/");
						text4 = ((num5 >= 0) ? text4.Replace (text4.Substring (0, num5), text3) : text3);
					}
					string fileName = text4 + "/" + innerText;
					int num6 = int.Parse (val.SelectSingleNode ("length").InnerText);
					if (val.SelectSingleNode ("hash") != null) {
						if (!HashManager.Instance.HashesMatch (fileName)) {
							num3 += ulong.Parse (val.SelectSingleNode ("length").InnerText);
							ulong num7 = ((val.SelectSingleNode ("compressed") != null) ? ulong.Parse (val.SelectSingleNode ("compressed").InnerText) : ulong.Parse (val.SelectSingleNode ("length").InnerText));
							num4 += num7;
							if (flag) {
								mFE.BeginInvoke ((Delegate)mDownloadFailed, new object[0]);
								return;
							}
							flag4 = true;
						}
					} else {
						if (flag) {
							throw new DownloaderException ("Without hash in the metadata I cannot verify the download");
						}
						flag4 = true;
					}
					if (mStopFlag) {
						ISynchronizeInvoke obj4 = mFE;
						Delegate delegate2 = mDownloadFailed;
						object[] array2 = new object[1];
						obj4.BeginInvoke (delegate2, array2);
						return;
					}
					num2 += num6;
					object[] array3 = new object[4] { num, num2, 0, innerText };
					mFE.BeginInvoke ((Delegate)mProgressUpdated, array3);
				}
				if (flag3) {
					HashManager.Instance.WriteHashCache (text2 + ".hsh", writeOldHashes: true);
				}
				if (flag4) {
					mFE.BeginInvoke ((Delegate)mDownloadFailed, new object[0]);
				} else {
					mFE.BeginInvoke ((Delegate)mDownloadFinished, (object[])null);
				}
			} catch (DownloaderException ex) {
				mFE.BeginInvoke ((Delegate)mDownloadFailed, new object[1] { ex });
			} catch (Exception ex2) {
				mFE.BeginInvoke ((Delegate)mDownloadFailed, new object[1] { ex2 });
			} finally {
				if (flag2) {
					HashManager.Instance.Clear ();
				}
				GC.Collect ();
			}
		}

		public static string GetXml (string url)
		{
			byte[] data = GetData (url);
			if (IsLzma (data)) {
				return DecompressLZMA (data);
			}
			return Encoding.UTF8.GetString (data).Trim ();
		}

		public static byte[] GetData (string url)
		{
			WebClient webClient = new WebClient ();
			webClient.Headers.Add ("Accept", "text/html,text/xml,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
			webClient.Headers.Add ("Accept-Language", "en-us,en;q=0.5");
			webClient.Headers.Add ("Accept-Charset", "utf-8;q=0.7,*;q=0.7");
			webClient.CachePolicy = new RequestCachePolicy (RequestCacheLevel.BypassCache);
			byte[] result = webClient.DownloadData (url);
			((Component)webClient).Dispose ();
			return result;
		}

		public static bool IsLzma (byte[] arr)
		{
			if (arr.Length >= 2 && arr [0] == 93) {
				return arr [1] == 0;
			}
			return false;
		}

		public static string DecompressLZMA (byte[] compressedFile)
		{
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			IntPtr srcLen = new IntPtr (compressedFile.Length - 13);
			byte[] array = new byte[srcLen.ToInt64 ()];
			IntPtr outPropsSize = new IntPtr (5);
			byte[] array2 = new byte[5];
			compressedFile.CopyTo (array, 13);
			for (int i = 0; i < 5; i++) {
				array2 [i] = compressedFile [i];
			}
			int num = 0;
			for (int j = 0; j < 8; j++) {
				num += compressedFile [j + 5] << 8 * j;
			}
			IntPtr destLen = new IntPtr (num);
			byte[] array3 = new byte[num];
			int num2 = LZMA.LzmaUncompress (array3, ref destLen, array, ref srcLen, array2, outPropsSize);
			if (num2 != 0) {
				MessageBox.Show ("Decompression returned " + num2);
			}
			return new string (Encoding.UTF8.GetString (array3).ToCharArray ());
		}
	}
	public abstract class DownloaderCommand
	{
		protected Downloader _downloader;

		public Downloader Downloader {
			[CompilerGenerated]
			get {
				return _downloader;
			}
		}

		protected DownloaderCommand (Downloader downloader)
		{
			_downloader = downloader;
		}

		public abstract void Execute (object[] parameters);
	}
	[Serializable]
	public class DownloaderException : Exception
	{
		public DownloaderException ()
		{
		}

		public DownloaderException (string message)
			: base (message)
		{
		}

		public DownloaderException (string message, Exception innerException)
			: base (message, innerException)
		{
		}

		protected DownloaderException (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}
	}
	public delegate void DownloadFailed (Exception ex);
	public delegate void DownloadFinished ();
	internal class DownloadManager
	{
		private class DownloadItem
		{
			public DownloadStatus Status;

			private byte[] _data;

			public byte[] Data {
				get {
					return _data;
				}
				set {
					_data = value;
				}
			}

			public DownloadItem ()
			{
				Status = DownloadStatus.Queued;
			}
		}

		public enum DownloadStatus
		{
			Queued,
			Downloading,
			Downloaded,
			Canceled
		}

		private const int MaxWorkers = 3;

		private const int MaxActiveChunks = 16;

		private static int _workerCount;

		private int _maxWorkers;

		private Dictionary<string, DownloadItem> _downloadList;

		private LinkedList<string> _downloadQueue;

		private List<BackgroundWorker> _workers;

		private int _freeChunks;

		private object _freeChunksLock;

		private bool _managerRunning;

		public bool ManagerRunning {
			[CompilerGenerated]
			get {
				return _managerRunning;
			}
		}

		static DownloadManager ()
		{
			_workerCount = 0;
		}

		public DownloadManager ()
			: this (3, 16)
		{
		}

		public DownloadManager (int maxWorkers, int maxActiveChunks)
		{
			_maxWorkers = maxWorkers;
			_freeChunks = maxActiveChunks;
			_downloadList = new Dictionary<string, DownloadItem> ();
			_downloadQueue = new LinkedList<string> ();
			_workers = new List<BackgroundWorker> ();
		}

		private void BackgroundWorker_DoWork (object sender, DoWorkEventArgs args)
		{
			try {
				WebClient webClient = new WebClient ();
				webClient.DownloadDataCompleted += DownloadManager_DownloadDataCompleted;
				webClient.CachePolicy = new RequestCachePolicy (RequestCacheLevel.NoCacheNoStore);
				while (true) {
					if (_freeChunks <= 0) {
						Thread.Sleep (100);
						continue;
					}
					lock (_downloadQueue) {
						if (_downloadQueue.Count == 0) {
							lock (_workers) {
								_workers.Remove ((BackgroundWorker)sender);
							}
							_workerCount--;
							break;
						}
					}
					string text = null;
					lock (_downloadQueue) {
						text = _downloadQueue.Last.Value;
						_downloadQueue.RemoveLast ();
						lock (_freeChunksLock) {
							_freeChunks--;
						}
					}
					lock (_downloadList [text]) {
						if (_downloadList [text].Status != DownloadStatus.Canceled) {
							_downloadList [text].Status = DownloadStatus.Downloading;
						}
					}
					while (webClient.IsBusy) {
						Thread.Sleep (100);
					}
					webClient.DownloadDataAsync (new Uri (text), (object)text);
					DownloadStatus downloadStatus = DownloadStatus.Downloading;
					while (downloadStatus == DownloadStatus.Downloading) {
						downloadStatus = _downloadList [text].Status;
						if (downloadStatus == DownloadStatus.Canceled) {
							break;
						}
						Thread.Sleep (100);
					}
					if (downloadStatus == DownloadStatus.Canceled) {
						webClient.CancelAsync ();
					}
					lock (_workers) {
						if (_workerCount > _maxWorkers || !_managerRunning) {
							_workers.Remove ((BackgroundWorker)sender);
							_workerCount--;
							break;
						}
					}
				}
			} catch (Exception) {
				lock (_workers) {
					_workers.Remove ((BackgroundWorker)sender);
					_workerCount--;
				}
			}
		}

		private void BackgroundWorker_RunWorkerComplete (object sender, RunWorkerCompletedEventArgs e)
		{
			_ = ((AsyncCompletedEventArgs)e).Error;
		}

		public void CancelAllDownloads ()
		{
			Stop ();
			lock (_downloadQueue) {
				_downloadQueue.Clear ();
			}
			foreach (string key in _downloadList.Keys) {
				lock (_downloadList [key]) {
					if (_downloadList [key].Data != null) {
						lock (_freeChunksLock) {
							_freeChunks++;
						}
					}
					_downloadList [key].Status = DownloadStatus.Canceled;
					_downloadList [key].Data = null;
				}
			}
		}

		public void CancelDownload (string fileName)
		{
			lock (_downloadQueue) {
				if (_downloadQueue.Contains (fileName)) {
					_downloadQueue.Remove (fileName);
				}
			}
			if (!_downloadList.ContainsKey (fileName)) {
				return;
			}
			lock (_downloadList [fileName]) {
				if (_downloadList [fileName].Data != null) {
					lock (_freeChunksLock) {
						_freeChunks++;
					}
				}
				_downloadList [fileName].Status = DownloadStatus.Canceled;
				_downloadList [fileName].Data = null;
			}
		}

		public void Clear ()
		{
			CancelAllDownloads ();
			while (_workerCount > 0) {
				Thread.Sleep (100);
			}
			lock (_downloadList) {
				_downloadList.Clear ();
			}
		}

		private void DownloadManager_DownloadDataCompleted (object sender, DownloadDataCompletedEventArgs e)
		{
			string text = e.UserState.ToString ();
			if (e.Cancelled || ((AsyncCompletedEventArgs)e).Error != null) {
				if (((AsyncCompletedEventArgs)e).Error != null && _downloadList.ContainsKey (text)) {
					lock (_downloadList [text]) {
						if (_downloadList [text].Status == DownloadStatus.Canceled || _maxWorkers <= 1) {
							_downloadList [text].Data = null;
							_downloadList [text].Status = DownloadStatus.Canceled;
						} else {
							_downloadList [text].Data = null;
							_downloadList [text].Status = DownloadStatus.Queued;
							lock (_downloadQueue) {
								_downloadQueue.AddLast (text);
							}
							lock (_workers) {
								_maxWorkers--;
							}
						}
					}
				}
				lock (_freeChunksLock) {
					_freeChunks++;
					return;
				}
			}
			lock (_downloadList [text]) {
				if (_downloadList [text].Status != DownloadStatus.Downloaded) {
					_downloadList [text].Data = new byte[e.Result.Length];
					Buffer.BlockCopy (e.Result, 0, _downloadList [text].Data, 0, e.Result.Length);
					_downloadList [text].Status = DownloadStatus.Downloaded;
				}
			}
		}

		public byte[] GetFile (string fileName)
		{
			byte[] result = null;
			ScheduleFile (fileName);
			DownloadStatus status;
			lock (_downloadList [fileName]) {
				status = _downloadList [fileName].Status;
			}
			while (status != DownloadStatus.Downloaded && status != DownloadStatus.Canceled) {
				Thread.Sleep (100);
				lock (_downloadList [fileName]) {
					status = _downloadList [fileName].Status;
				}
			}
			if (_downloadList [fileName].Status == DownloadStatus.Downloaded) {
				lock (_downloadList [fileName]) {
					result = _downloadList [fileName].Data;
					_downloadList [fileName].Data = null;
					lock (_freeChunksLock) {
						_freeChunks++;
					}
				}
			}
			return result;
		}

		public DownloadStatus? GetStatus (string fileName)
		{
			if (!_downloadList.ContainsKey (fileName)) {
				return null;
			}
			return _downloadList [fileName].Status;
		}

		public void Initialize (XmlDocument doc, string serverPath)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			_freeChunksLock = new object ();
			int num = 0;
			foreach (XmlNode item in ((XmlNode)doc).SelectNodes ("/index/fileinfo")) {
				XmlNode val = item;
				_ = val.SelectSingleNode ("path").InnerText;
				_ = val.SelectSingleNode ("file").InnerText;
				if (val.SelectSingleNode ("section") != null) {
					num = int.Parse (val.SelectSingleNode ("section").InnerText);
				}
			}
			for (int i = 1; i <= num; i++) {
				string key = $"{serverPath}/section{i}.dat";
				if (!_downloadList.ContainsKey (key)) {
					_downloadList.Add (key, new DownloadItem ());
				}
			}
		}

		public void ScheduleFile (string fileName)
		{
			if (_downloadList.ContainsKey (fileName)) {
				DownloadStatus downloadStatus = DownloadStatus.Queued;
				lock (_downloadList [fileName]) {
					downloadStatus = _downloadList [fileName].Status;
				}
				if (downloadStatus != 0 && downloadStatus != DownloadStatus.Canceled) {
					return;
				}
				lock (_downloadQueue) {
					if (_downloadQueue.Contains (fileName) && _downloadQueue.Last.Value != fileName) {
						_downloadQueue.Remove (fileName);
						_downloadQueue.AddLast (fileName);
					} else if (!_downloadQueue.Contains (fileName)) {
						_downloadQueue.AddLast (fileName);
					}
				}
				lock (_downloadList [fileName]) {
					_downloadList [fileName].Status = DownloadStatus.Queued;
				}
			} else {
				_downloadList.Add (fileName, new DownloadItem ());
				lock (_downloadQueue) {
					_downloadQueue.AddLast (fileName);
				}
			}
			if (_managerRunning && _workerCount < _maxWorkers) {
				lock (_workers) {
					BackgroundWorker backgroundWorker = new BackgroundWorker ();
					backgroundWorker.DoWork += BackgroundWorker_DoWork;
					backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerComplete;
					backgroundWorker.RunWorkerAsync ();
					_workers.Add (backgroundWorker);
					_workerCount++;
				}
			}
		}

		public void Start ()
		{
			_managerRunning = true;
			lock (_workers) {
				while (_workerCount < _maxWorkers) {
					BackgroundWorker backgroundWorker = new BackgroundWorker ();
					backgroundWorker.DoWork += BackgroundWorker_DoWork;
					backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerComplete;
					backgroundWorker.RunWorkerAsync ();
					_workers.Add (backgroundWorker);
					_workerCount++;
				}
			}
		}

		public void Stop ()
		{
			_managerRunning = false;
		}
	}
	internal class HashManager
	{
		private class HashTuple
		{
			public string Old;

			public string New;

			public long Ticks;

			public HashTuple (string oldHash, string newHash, long ticks)
			{
				Old = oldHash;
				New = newHash;
				Ticks = ticks;
			}

			public HashTuple (string oldHash, string newHash)
				: this (oldHash, newHash, DateTime.Now.AddYears (1).Ticks)
			{
			}
		}

		private const int MaxWorkers = 3;

		private const string HashFileName = "HashFile";

		private Dictionary<string, HashTuple> _fileList;

		private Queue<string> _queueHash;

		private static readonly object _queueHashLock;

		private static int _workerCount;

		private bool _useCache = true;

		private static HashManager _instance;

		internal static HashManager Instance {
			[CompilerGenerated]
			get {
				return _instance;
			}
		}

		static HashManager ()
		{
			_queueHashLock = new object ();
			_workerCount = 0;
			_instance = new HashManager ();
		}

		private HashManager ()
		{
			_useCache = true;
			_fileList = new Dictionary<string, HashTuple> ();
			_queueHash = new Queue<string> ();
		}

		private void BackgroundWorker_DoWork (object sender, DoWorkEventArgs args)
		{
			while (true) {
				lock (_queueHashLock) {
					if (_queueHash.Count == 0) {
						_workerCount--;
						break;
					}
				}
				string text = null;
				lock (_queueHashLock) {
					text = _queueHash.Dequeue ();
				}
				string old = null;
				if (File.Exists (text)) {
					if (string.IsNullOrEmpty (_fileList [text].Old)) {
						try {
							FileStream fileStream = File.OpenRead (text);
							old = Convert.ToBase64String (((HashAlgorithm)MD5.Create ()).ComputeHash ((Stream)fileStream));
						} catch (Exception) {
						}
					} else {
						old = _fileList [text].Old;
					}
				}
				lock (_fileList [text]) {
					_fileList [text].Old = old;
				}
			}
		}

		private void BackgroundWorker_RunWorkerComplete (object sender, RunWorkerCompletedEventArgs e)
		{
			_ = ((AsyncCompletedEventArgs)e).Error;
		}

		public void Clear ()
		{
			lock (_queueHash) {
				_queueHash.Clear ();
			}
			while (_workerCount > 0) {
				Thread.Sleep (100);
			}
			_fileList.Clear ();
		}

		public string GetHashOld (string fileName)
		{
			string text = string.Empty;
			while (true) {
				lock (_fileList [fileName]) {
					text = _fileList [fileName].Old;
				}
				if (text != string.Empty) {
					break;
				}
				Thread.Sleep (100);
			}
			return text;
		}

		public bool HashesMatch (string fileName)
		{
			try {
				while (true) {
					lock (_fileList [fileName]) {
						if (_fileList [fileName].Old != string.Empty) {
							return _fileList [fileName].New == _fileList [fileName].Old;
						}
					}
					Thread.Sleep (100);
				}
			} catch (Exception) {
				return false;
			}
		}

		public void Start (XmlDocument doc, string patchPath, string hashFileNameSuffix, int maxWorkers)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			foreach (XmlNode item in ((XmlNode)doc).SelectNodes ("/index/fileinfo")) {
				XmlNode val = item;
				string text = val.SelectSingleNode ("path").InnerText;
				string innerText = val.SelectSingleNode ("file").InnerText;
				if (!string.IsNullOrEmpty (patchPath)) {
					int num = text.IndexOf ("/");
					text = ((num < 0) ? patchPath : text.Replace (text.Substring (0, num), patchPath));
				}
				string text2 = text + "/" + innerText;
				if (val.SelectSingleNode ("hash") != null) {
					_fileList.Add (text2, new HashTuple (string.Empty, val.SelectSingleNode ("hash").InnerText));
					_queueHash.Enqueue (text2);
				} else {
					_fileList.Add (text2, new HashTuple (null, null));
				}
			}
			if (_useCache && File.Exists ("HashFile" + hashFileNameSuffix)) {
				FileStream fileStream = null;
				CryptoStream cryptoStream = null;
				StreamReader streamReader = null;
				try {
					ICryptoTransform cryptoTransform = new DESCryptoServiceProvider {
						Key = Encoding.ASCII.GetBytes ("12345678"),
						IV = Encoding.ASCII.GetBytes ("12345678")
					}.CreateDecryptor ();
					fileStream = new FileStream ("HashFile" + hashFileNameSuffix, FileMode.Open);
					cryptoStream = new CryptoStream ((Stream)fileStream, cryptoTransform, CryptoStreamMode.Read);
					streamReader = new StreamReader ((Stream)(object)cryptoStream);
					string text3 = null;
					while ((text3 = streamReader.ReadLine ()) != null) {
						string[] array = text3.Split (new char[1] { '\t' });
						string text4 = array [0];
						if (_fileList.ContainsKey (text4) && File.Exists (text4) && long.Parse (array [2]) == new FileInfo (text4).LastWriteTime.Ticks) {
							_fileList [text4].Old = array [1];
							_fileList [text4].Ticks = long.Parse (array [2]);
						}
					}
				} catch (CryptographicException) {
					streamReader = null;
					cryptoStream = null;
					_fileList.Clear ();
				} catch (Exception) {
					_fileList.Clear ();
				} finally {
					streamReader?.Close ();
					((Stream)(object)cryptoStream)?.Close ();
					fileStream?.Close ();
					File.Delete ("HashFile" + hashFileNameSuffix);
				}
			}
			_workerCount = 0;
			while (_workerCount < maxWorkers && _queueHash.Count > 0) {
				BackgroundWorker backgroundWorker = new BackgroundWorker ();
				backgroundWorker.DoWork += BackgroundWorker_DoWork;
				backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerComplete;
				backgroundWorker.RunWorkerAsync ();
				_workerCount++;
			}
		}

		public void WriteHashCache (string hashFileNameSuffix, bool writeOldHashes)
		{
			lock (_fileList) {
				FileStream fileStream = null;
				CryptoStream cryptoStream = null;
				StreamWriter streamWriter = null;
				try {
					fileStream = new FileStream ("HashFile" + hashFileNameSuffix, FileMode.Create);
					DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider ();
					byte[] array = (dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes ("12345678"));
					byte[] key = array;
					dESCryptoServiceProvider.Key = key;
					cryptoStream = new CryptoStream ((Stream)fileStream, dESCryptoServiceProvider.CreateEncryptor (), CryptoStreamMode.Write);
					streamWriter = new StreamWriter ((Stream)(object)cryptoStream);
					foreach (string key2 in _fileList.Keys) {
						string empty = string.Empty;
						empty = ((!writeOldHashes) ? _fileList [key2].New : _fileList [key2].Old);
						if (File.Exists (key2) && !string.IsNullOrEmpty (empty)) {
							streamWriter.WriteLine ($"{key2}\t{empty}\t{new FileInfo (key2).LastWriteTime.Ticks}");
						}
					}
				} catch (Exception) {
				} finally {
					if (streamWriter != null) {
						streamWriter.Close ();
						streamWriter.Dispose ();
					}
					((Stream)(object)cryptoStream)?.Close ();
					fileStream?.Close ();
				}
			}
		}
	}
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
					empty2 = ((httpWebResponse.StatusCode != System.Net.HttpStatusCode.InternalServerError) ? ((Exception)(object)ex).Message : new StreamReader (((WebResponse)httpWebResponse).GetResponseStream ()).ReadToEnd ());
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
					empty2 = ((httpWebResponse.StatusCode != System.Net.HttpStatusCode.InternalServerError) ? ((Exception)(object)ex).Message : new StreamReader (((WebResponse)httpWebResponse).GetResponseStream ()).ReadToEnd ());
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
					text2 = ((httpWebResponse.StatusCode != System.Net.HttpStatusCode.InternalServerError) ? ((Exception)(object)ex).Message : new StreamReader (((WebResponse)httpWebResponse).GetResponseStream ()).ReadToEnd ());
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
	internal static class Program
	{
		[STAThread]
		private static void Main ()
		{
			//Discarded unreachable code: IL_01c7
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Expected O, but got Unknown
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			try {
				try {
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
					ServicePointManager.Expect100Continue = false;
					ServicePointManager.DefaultConnectionLimit = 16;
				} catch {
				}
				try {
					string path = Path.Combine (Directory.GetCurrentDirectory (), "tempname.zip");
					if (File.Exists (path)) {
						File.Delete (path);
					}
				} catch {
				}
				try {
					if (!File.Exists ("LZMA.dll")) {
						byte[] array = ExtractResource.AsByte ("GameLauncher.LZMA.LZMA.dll");
						if (array != null && array.Length != 0) {
							File.WriteAllBytes ("LZMA.dll", array);
						}
					}
				} catch {
				}
				try {
					if (!File.Exists ("GameLauncherUpdater.exe")) {
						byte[] array2 = ExtractResource.AsByte ("GameLauncher.Updater.GameLauncherUpdater.exe");
						if (array2 != null && array2.Length != 0) {
							File.WriteAllBytes ("GameLauncherUpdater.exe", array2);
						}
					}
				} catch {
				}
				if (!new Mutex (initiallyOwned: false, "GameLauncherNFSW-ClassicEA").WaitOne (0, exitContext: false)) {
					MessageBox.Show ("There is a NFS World Launcher running!", "NFS World", (MessageBoxButtons)0, (MessageBoxIcon)64);
					return;
				}
				string[] obj5 = new string[4] { "Microsoft.WindowsAPICodePack.dll", "Microsoft.WindowsAPICodePack.Shell.dll", "Newtonsoft.Json.dll", "ICSharpCode.SharpZipLib.dll" };
				List<string> list = new List<string> ();
				string[] array3 = obj5;
				foreach (string text in array3) {
					if (!File.Exists (text)) {
						list.Add (text);
					}
				}
				if (list.Count > 0) {
					MessageBox.Show ("Cannot start. Missing files:\n\n• " + string.Join ("\n• ", list.ToArray ()) + "\n\nFolder: " + Directory.GetCurrentDirectory (), "GameLauncher", (MessageBoxButtons)0, (MessageBoxIcon)16);
					return;
				}
				try {
					if (Environment.OSVersion.Version.Major >= 6) {
						User32.SetProcessDPIAware ();
					}
				} catch {
				}
				Application.EnableVisualStyles ();
				Application.SetCompatibleTextRenderingDefault (false);
				try {
					IniFile iniFile = new IniFile (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Settings.ini"));
					_ = iniFile.Read ("AcceptedTos") != "1";
				} catch {
				}
				Application.Run ((Form)new mainScreen ());
			} catch (Exception ex) {
				try {
					MessageBox.Show ("Failed to start GameLauncher.\n\n" + ex.Message + "\n\n" + ex.GetType ().FullName + "\n\n" + ((ex.InnerException != null) ? (ex.InnerException.Message + "\n\n") : string.Empty) + (ex.StackTrace ?? string.Empty), "GameLauncher – Startup Error", (MessageBoxButtons)0, (MessageBoxIcon)16);
				} catch {
				}
			}
		}
	}
	public delegate void ProgressUpdated (long dowloadLength, long downloadCurrent, long compressedLength, string fileName);
	public delegate void ShowMessage (string message, string header);
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
	[Serializable]
	public class UncompressionException : Exception
	{
		private int mErrorCode;

		public int ErrorCode {
			[CompilerGenerated]
			get {
				return mErrorCode;
			}
		}

		public UncompressionException (int errorCode)
		{
			mErrorCode = errorCode;
		}

		public UncompressionException (int errorCode, string message)
			: base (message)
		{
			mErrorCode = errorCode;
		}

		public UncompressionException (int errorCode, string message, Exception innerException)
			: base (message, innerException)
		{
			mErrorCode = errorCode;
		}

		protected UncompressionException (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}
	}
	public class Update
	{
		public bool info { get; set; }

		public string download { get; set; }
	}
	internal class Updater
	{
		internal static void checkForUpdate (object sender, EventArgs e)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			try {
				WebClientWithTimeout webClientWithTimeout = new WebClientWithTimeout ();
				Uri uri = new Uri ("http://nfsw.metonator.ct8.pl/checkUpdate.php?version=" + Application.ProductVersion);
				webClientWithTimeout.CancelAsync ();
				((WebClient)webClientWithTimeout).DownloadStringAsync (uri);
				webClientWithTimeout.DownloadStringCompleted += delegate(object obj5, DownloadStringCompletedEventArgs e2) {
					//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
					//IL_003b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0041: Invalid comparison between Unknown and I4
					//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
					//IL_0083: Unknown result type (might be due to invalid IL or missing references)
					try {
						CheckVersion checkVersion = JsonConvert.DeserializeObject<CheckVersion> (e2.Result);
						if (checkVersion.update.info) {
							if ((int)MessageBox.Show ("An update is available. Do you wanna download it?\nYour version: " + Application.ProductVersion + "\nUpdated version: " + checkVersion.github_build, "GameLauncher", (MessageBoxButtons)4, (MessageBoxIcon)64) == 6) {
								new WebClientWithTimeout ().DownloadFile (checkVersion.update.download, "tempname.zip");
								Process.Start ("GameLauncherUpdater.exe", Process.GetCurrentProcess ().Id.ToString ());
							}
							return;
						}
						try {
							_ = ((Control)(Form)sender).Name == "mainScreen";
						} catch {
							MessageBox.Show ("Your launcher is up-to-date");
						}
					} catch {
						try {
							_ = ((Control)(Form)sender).Name == "mainScreen";
						} catch {
							MessageBox.Show ("Failed to check for update");
						}
					}
				};
			} catch {
				MessageBox.Show ("Failed to check for update");
			}
		}
	}
	public class VerifyCommand : DownloaderCommand
	{
		public VerifyCommand (Downloader downloader)
			: base (downloader)
		{
		}

		public override void Execute (object[] parameters)
		{
			_downloader.StartVerification ((string)parameters [0], (string)parameters [1], (string)parameters [2], (bool)parameters [3], (bool)parameters [4], (bool)parameters [5]);
		}
	}
}
namespace GameLauncher.Resources
{
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
}
namespace GameLauncher.Properties
{
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
	[CompilerGenerated]
	[GeneratedCode ("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.1.0.0")]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)(object)SettingsBase.Synchronized ((SettingsBase)(object)new Settings ());

		public static Settings Default {
			[CompilerGenerated]
			get {
				return defaultInstance;
			}
		}
	}
}
namespace GameLauncher.App.Classes
{
	public static class AntiCheat
	{
		public static int process_id;

		public static bool ChecksEnabled;

		public static void EnableChecks ()
		{
			ChecksEnabled = true;
			try {
				File.AppendAllText ("launcher.log", "[" + DateTime.Now.ToString ("HH:mm:ss") + "] [anticheat] EnableChecks\n");
			} catch {
			}
		}

		public static void DisableChecks (bool external = false)
		{
			ChecksEnabled = false;
			try {
				File.AppendAllText ("launcher.log", "[" + DateTime.Now.ToString ("HH:mm:ss") + "] [anticheat] DisableChecks\n");
			} catch {
			}
		}

		public static int Get_Cheat_Status ()
		{
			return 0;
		}
	}
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
	internal class DetectLinux
	{
		public static bool WineDetected ()
		{
			bool result = false;
			try {
				if (!string.IsNullOrEmpty (((object)Registry.CurrentUser.OpenSubKey ("Software\\Wine")).ToString ())) {
					result = true;
				}
			} catch {
				result = false;
			}
			return result;
		}

		public static bool MonoDetected ()
		{
			int platform = (int)Environment.OSVersion.Platform;
			if (platform != 4 && platform != 6) {
				return platform == 128;
			}
			return true;
		}

		public static bool LinuxDetected ()
		{
			if (!WineDetected ()) {
				return MonoDetected ();
			}
			return true;
		}
	}
	public static class DiscordPresence
	{
		private static object _client;

		private static bool _ready;

		public static void Init ()
		{
			try {
				Type type = Assembly.Load ("DiscordRPC").GetType ("DiscordRPC.DiscordRpcClient");
				_client = Activator.CreateInstance (type, "1200000000000000000");
				type.GetMethod ("Initialize")?.Invoke (_client, null);
				_ready = true;
			} catch {
				_ready = false;
			}
		}

		public static void SetInGame (string serverName)
		{
			if (!_ready) {
				return;
			}
			try {
				File.AppendAllText ("launcher.log", "[" + DateTime.Now.ToString ("HH:mm:ss") + "] [discord] In-Game " + serverName + "\n");
			} catch {
			}
		}

		public static void Clear ()
		{
			try {
				_ = _client;
			} catch {
			}
			_ready = false;
		}
	}
	internal class ExtractResource
	{
		public static byte[] AsByte (string filename)
		{
			Stream manifestResourceStream = Assembly.GetExecutingAssembly ().GetManifestResourceStream (filename);
			if (manifestResourceStream == null) {
				return null;
			}
			byte[] array = new byte[manifestResourceStream.Length];
			manifestResourceStream.Read (array, 0, array.Length);
			return array;
		}

		public static string AsString (string filename)
		{
			return new StreamReader (Assembly.GetExecutingAssembly ().GetManifestResourceStream (filename)).ReadToEnd ();
		}
	}
	internal class IniFile
	{
		private string Path;

		private string EXE = Assembly.GetExecutingAssembly ().GetName ().Name;

		public IniFile (string IniPath = null)
		{
			Path = new FileInfo (IniPath ?? (EXE + ".ini")).FullName.ToString ();
		}

		[DllImport ("kernel32.dll", CharSet = CharSet.Unicode)]
		private static extern long WritePrivateProfileString (string Section, string Key, string Value, string FilePath);

		[DllImport ("kernel32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetPrivateProfileString (string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

		public string Read (string Key, string Section = null)
		{
			StringBuilder stringBuilder = new StringBuilder (255);
			GetPrivateProfileString (Section ?? EXE, Key, string.Empty, stringBuilder, 255, Path);
			return stringBuilder.ToString ();
		}

		public void Write (string Key, string Value, string Section = null)
		{
			WritePrivateProfileString (Section ?? EXE, Key, Value, Path);
		}

		public void DeleteKey (string Key, string Section = null)
		{
			Write (Key, null, Section ?? EXE);
		}

		public void DeleteSection (string Section = null)
		{
			Write (null, null, Section ?? EXE);
		}

		public bool KeyExists (string Key, string Section = null)
		{
			return Read (Key, Section).Length > 0;
		}
	}
	internal static class Kernel32
	{
		[DllImport ("kernel32", CharSet = CharSet.Auto)]
		public static extern int GetDiskFreeSpaceEx (string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);
	}
	public static class LauncherUpdaterCheck
	{
		public static void CheckAsync (Action<string> log = null)
		{
			ThreadPool.QueueUserWorkItem (delegate {
				try {
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
					HttpWebRequest obj = (HttpWebRequest)WebRequest.Create ("https://api.github.com/repos/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest");
					obj.UserAgent = "ClassicEA-Launcher";
					obj.Timeout = 10000;
					HttpWebResponse httpWebResponse = (HttpWebResponse)obj.GetResponse ();
					try {
						using StreamReader streamReader = new StreamReader (((WebResponse)httpWebResponse).GetResponseStream ());
						dynamic val = JsonConvert.DeserializeObject (streamReader.ReadToEnd ());
						string text = val.tag_name;
						log?.Invoke ("Official SBRW latest release: " + text);
					} finally {
						((IDisposable)httpWebResponse)?.Dispose ();
					}
				} catch (Exception ex) {
					log?.Invoke ("Update check: " + ex.Message);
				}
			});
		}

		public static void EnableLargeAddressAware (string exePath, Action<string> log = null)
		{
			try {
				if (string.IsNullOrEmpty (exePath) || !File.Exists (exePath)) {
					return;
				}
				byte[] array = File.ReadAllBytes (exePath);
				if (array.Length < 64 || array [0] != 77 || array [1] != 90) {
					return;
				}
				int num = BitConverter.ToInt32 (array, 60);
				if (num <= 0 || num + 24 >= array.Length || array [num] != 80 || array [num + 1] != 69) {
					return;
				}
				int num2 = num + 22;
				ushort num3 = BitConverter.ToUInt16 (array, num2);
				if ((num3 & 0x20u) != 0) {
					log?.Invoke ("nfsw.exe already Large Address Aware");
					return;
				}
				num3 = (ushort)(num3 | 0x20u);
				array [num2] = (byte)(num3 & 0xFFu);
				array [num2 + 1] = (byte)((uint)(num3 >> 8) & 0xFFu);
				try {
					File.Copy (exePath, exePath + ".bak_laa", overwrite: false);
				} catch {
				}
				File.WriteAllBytes (exePath, array);
				log?.Invoke ("Enabled Large Address Aware on nfsw.exe");
			} catch (Exception ex) {
				log?.Invoke ("LAA patch: " + ex.Message);
			}
		}
	}
	public static class LocalProxy
	{
		private static HttpListener _listener;

		private static Thread _thread;

		private static string _realBase;

		private static volatile bool _running;

		public static string LastError = "";

		public static int Port { get; private set; }

		public static string GameUrl => "http://127.0.0.1:" + Port + "/nfsw/Engine.svc";

		public static bool Start (string realEngineSvcUrl)
		{
			Stop ();
			LastError = "";
			_realBase = (realEngineSvcUrl ?? "").TrimEnd (new char[1] { '/' });
			if (string.IsNullOrEmpty (_realBase)) {
				LastError = "Empty URL";
				return false;
			}
			try {
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				ServicePointManager.ServerCertificateValidationCallback = (object _003Cp0_003E, X509Certificate _003Cp1_003E, X509Chain _003Cp2_003E, SslPolicyErrors _003Cp3_003E) => true;
				ServicePointManager.DefaultConnectionLimit = 100;
				ServicePointManager.Expect100Continue = false;
			} catch {
			}
			Random random = new Random ();
			for (int i = 0; i < 20; i++) {
				Port = 2017 + random.Next (0, 200);
				try {
					_listener = new HttpListener ();
					_listener.Prefixes.Add ("http://127.0.0.1:" + Port + "/");
					_listener.Start ();
					_running = true;
					_thread = new Thread (ListenLoop) {
						IsBackground = true,
						Name = "LocalProxy"
					};
					_thread.Start ();
					Log ("Proxy listening on " + GameUrl + " -> " + _realBase);
					return true;
				} catch (Exception ex) {
					LastError = ex.Message;
					try {
						if (_listener != null) {
							_listener.Close ();
						}
					} catch {
					}
					_listener = null;
				}
			}
			return false;
		}

		public static void Stop ()
		{
			_running = false;
			try {
				if (_listener != null) {
					_listener.Stop ();
					_listener.Close ();
				}
			} catch {
			}
			_listener = null;
		}

		private static void Log (string msg)
		{
			try {
				File.AppendAllText ("launcher.log", "[" + DateTime.Now.ToString ("HH:mm:ss") + "] [proxy] " + msg + Environment.NewLine);
			} catch {
			}
		}

		private static void ListenLoop ()
		{
			while (_running && _listener != null) {
				try {
					HttpListenerContext ctx = _listener.GetContext ();
					ThreadPool.QueueUserWorkItem (delegate {
						Handle (ctx);
					});
				} catch {
					if (!_running) {
						break;
					}
				}
			}
		}

		private static void Handle (HttpListenerContext ctx)
		{
			string text = "";
			try {
				string text2 = ctx.Request.Url.AbsolutePath ?? "";
				int num = text2.IndexOf ("/nfsw/Engine.svc", StringComparison.OrdinalIgnoreCase);
				string text3 = ((num >= 0) ? text2.Substring (num + "/nfsw/Engine.svc".Length) : ((!text2.StartsWith ("/Engine.svc", StringComparison.OrdinalIgnoreCase)) ? text2 : text2.Substring ("/Engine.svc".Length)));
				if (string.IsNullOrEmpty (text3)) {
					text3 = "/";
				}
				if (!text3.StartsWith ("/")) {
					text3 = "/" + text3;
				}
				text = _realBase + text3 + ctx.Request.Url.Query;
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create (text);
				httpWebRequest.Method = ctx.Request.HttpMethod;
				httpWebRequest.Timeout = 120000;
				httpWebRequest.ReadWriteTimeout = 120000;
				httpWebRequest.AutomaticDecompression = DecompressionMethods.None;
				httpWebRequest.KeepAlive = false;
				httpWebRequest.AllowAutoRedirect = false;
				httpWebRequest.ServicePoint.Expect100Continue = false;
				httpWebRequest.ProtocolVersion = HttpVersion.Version11;
				string host = new Uri (_realBase).Host;
				NameValueCollection headers = ctx.Request.Headers;
				string[] allKeys = headers.AllKeys;
				foreach (string text4 in allKeys) {
					if (string.IsNullOrEmpty (text4)) {
						continue;
					}
					switch (text4.ToLowerInvariant ()) {
					case "content-type":
						httpWebRequest.ContentType = headers [text4];
						break;
					case "user-agent":
						httpWebRequest.UserAgent = headers [text4];
						break;
					case "referer":
						try {
							httpWebRequest.Referer = headers [text4];
						} catch {
						}
						break;
					default:
						try {
							httpWebRequest.Headers [text4] = headers [text4];
						} catch {
						}
						break;
					case "connection":
					case "host":
					case "content-length":
					case "transfer-encoding":
					case "accept-encoding":
						break;
					}
				}
				try {
					httpWebRequest.Host = host;
				} catch {
				}
				if (string.IsNullOrEmpty (httpWebRequest.UserAgent)) {
					httpWebRequest.UserAgent = "Blackbox";
				}
				byte[] array = null;
				if (ctx.Request.HasEntityBody) {
					using MemoryStream memoryStream = new MemoryStream ();
					ctx.Request.InputStream.CopyTo (memoryStream);
					array = memoryStream.ToArray ();
				}
				if (array != null && array.Length != 0) {
					httpWebRequest.ContentLength = array.Length;
					using Stream stream = ((WebRequest)httpWebRequest).GetRequestStream ();
					stream.Write (array, 0, array.Length);
				} else if (ctx.Request.HttpMethod == "POST" || ctx.Request.HttpMethod == "PUT") {
					httpWebRequest.ContentLength = 0L;
				}
				Log (ctx.Request.HttpMethod + " " + text2 + " body=" + ((array != null) ? array.Length : 0) + " -> " + text);
				if (text2.IndexOf ("GetPermanentSession", StringComparison.OrdinalIgnoreCase) >= 0) {
					Log ("GetPermanentSession headers: " + (object)headers);
					if (array != null && array.Length != 0) {
						Log ("GetPermanentSession body: " + Encoding.UTF8.GetString (array, 0, Math.Min (array.Length, 400)));
					}
				}
				string contentType = "application/xml;charset=UTF-8";
				WebHeaderCollection webHeaderCollection = null;
				int statusCode;
				byte[] array2;
				try {
					HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
					try {
						statusCode = (int)httpWebResponse.StatusCode;
						if (!string.IsNullOrEmpty (httpWebResponse.ContentType)) {
							contentType = httpWebResponse.ContentType;
						}
						webHeaderCollection = httpWebResponse.Headers;
						using Stream stream2 = ((WebResponse)httpWebResponse).GetResponseStream ();
						using MemoryStream memoryStream2 = new MemoryStream ();
						stream2?.CopyTo (memoryStream2);
						array2 = memoryStream2.ToArray ();
					} finally {
						((IDisposable)httpWebResponse)?.Dispose ();
					}
				} catch (WebException ex) {
					if (ex.Response != null) {
						HttpWebResponse httpWebResponse2 = (HttpWebResponse)ex.Response;
						statusCode = (int)httpWebResponse2.StatusCode;
						if (!string.IsNullOrEmpty (httpWebResponse2.ContentType)) {
							contentType = httpWebResponse2.ContentType;
						}
						webHeaderCollection = httpWebResponse2.Headers;
						using (Stream stream3 = ((WebResponse)httpWebResponse2).GetResponseStream ()) {
							using MemoryStream memoryStream3 = new MemoryStream ();
							stream3?.CopyTo (memoryStream3);
							array2 = memoryStream3.ToArray ();
						}
						Log ("ERR " + statusCode + " " + text2 + " len=" + array2.Length);
						if (array2.Length != 0) {
							Log ("ERR body: " + Encoding.UTF8.GetString (array2, 0, Math.Min (array2.Length, 400)));
						}
					} else {
						statusCode = 502;
						array2 = Encoding.UTF8.GetBytes ("proxy error: " + ((Exception)(object)ex).Message);
						Log ("ERR " + text + " : " + ((Exception)(object)ex).Message);
					}
				}
				ctx.Response.StatusCode = statusCode;
				ctx.Response.ContentType = contentType;
				ctx.Response.SendChunked = false;
				ctx.Response.ContentLength64 = array2.Length;
				if (webHeaderCollection != null) {
					allKeys = webHeaderCollection.AllKeys;
					foreach (string text5 in allKeys) {
						switch (text5.ToLowerInvariant ()) {
						case "content-length":
						case "transfer-encoding":
						case "content-type":
						case "connection":
							continue;
						}
						try {
							ctx.Response.Headers [text5] = webHeaderCollection [text5];
						} catch {
						}
					}
				}
				if (array2.Length != 0) {
					ctx.Response.OutputStream.Write (array2, 0, array2.Length);
				}
				Log ("OK " + statusCode + " " + text2 + " len=" + array2.Length);
			} catch (Exception ex2) {
				Log ("ERR2: " + ex2.Message);
				try {
					byte[] bytes = Encoding.UTF8.GetBytes (ex2.Message);
					ctx.Response.StatusCode = 500;
					ctx.Response.SendChunked = false;
					ctx.Response.ContentLength64 = bytes.Length;
					ctx.Response.OutputStream.Write (bytes, 0, bytes.Length);
				} catch {
				}
			} finally {
				try {
					ctx.Response.OutputStream.Close ();
				} catch {
				}
			}
		}
	}
	internal static class LZMA
	{
		[DllImport ("LZMA.dll", CharSet = CharSet.Ansi)]
		public static extern int LzmaUncompress (byte[] dest, ref IntPtr destLen, byte[] src, ref IntPtr srcLen, byte[] outProps, IntPtr outPropsSize);

		[DllImport ("LZMA.dll", CharSet = CharSet.Ansi)]
		public static extern int LzmaUncompressBuf2File (string destFile, ref IntPtr destLen, byte[] src, ref IntPtr srcLen, byte[] outProps, IntPtr outPropsSize);
	}
	public static class ModNetExtras
	{
		public static void FileANDFolder (string gamePath, Action<string> log = null)
		{
			if (string.IsNullOrEmpty (gamePath) || !Directory.Exists (gamePath)) {
				return;
			}
			string[] array = new string[1] { "modules" };
			string[] array2 = new string[1] { "scripts" };
			string[] array3 = new string[9] { "modules/udpcrc.soapbox.module", "modules/udpcrypt1.soapbox.module", "modules/udpcrypt2.soapbox.module", "modules/xmppsubject.soapbox.module", "scripts/global.ini", "ModManager.dat", "PocoFoundation.dll", "PocoNet.dll", "lightfx.dll" };
			string[] array4 = array;
			foreach (string path in array4) {
				string path2 = Path.Combine (gamePath, path);
				try {
					if (Directory.Exists (path2)) {
						Directory.Delete (path2, recursive: true);
					}
				} catch {
				}
			}
			array4 = array2;
			foreach (string path3 in array4) {
				try {
					Directory.CreateDirectory (Path.Combine (gamePath, path3));
				} catch {
				}
			}
			array4 = array3;
			foreach (string path4 in array4) {
				string path5 = Path.Combine (gamePath, path4);
				try {
					if (File.Exists (path5)) {
						File.Delete (path5);
					}
				} catch {
				}
			}
			log?.Invoke ("ModNet FileANDFolder cleanup done");
		}

		public static void CleanLinks (string gamePath, Action<string> log = null)
		{
			string path = Path.Combine (gamePath, ".links");
			if (!File.Exists (path)) {
				return;
			}
			int num = 0;
			try {
				foreach (string item in File.ReadLines (path)) {
					string[] array = item.Split (new char[1] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length != 2) {
						continue;
					}
					string path2 = array [0];
					if (!int.TryParse (array [1], out var result)) {
						continue;
					}
					string text = Path.Combine (gamePath, path2);
					try {
						if (result == 0) {
							string text2 = text + ".orig";
							if (IsReparse (text) && File.Exists (text)) {
								File.Delete (text);
								continue;
							}
							if (!File.Exists (text)) {
								num++;
								if (File.Exists (text2)) {
									try {
										File.Delete (text2);
									} catch {
									}
								}
								continue;
							}
							if (!File.Exists (text2)) {
								try {
									File.Delete (text);
								} catch {
								}
								continue;
							}
							try {
								if (File.Exists (text)) {
									File.Delete (text);
								}
								File.Move (text2, text);
							} catch {
								num++;
							}
							continue;
						}
						if (IsReparse (text) && Directory.Exists (text)) {
							Directory.Delete (text, recursive: true);
							continue;
						}
						if (!Directory.Exists (text)) {
							num++;
							continue;
						}
						string text3 = text + ".orig";
						if (Directory.Exists (text3)) {
							try {
								Directory.Delete (text, recursive: true);
								Directory.Move (text3, text);
							} catch {
								num++;
							}
						}
					} catch {
						num++;
					}
				}
				try {
					File.Delete (path);
				} catch {
				}
				log?.Invoke ("CleanLinks done (errors=" + num + ")");
			} catch (Exception ex) {
				log?.Invoke ("CleanLinks: " + ex.Message);
			}
		}

		private static bool IsReparse (string path)
		{
			try {
				if (File.Exists (path) || Directory.Exists (path)) {
					return (File.GetAttributes (path) & FileAttributes.ReparsePoint) != 0;
				}
			} catch {
			}
			return false;
		}

		public static void EnsureFirewallRules (string launcherExe, string gameDir, int proxyPort, Action<string> log = null)
		{
			try {
				string text = Path.Combine (gameDir ?? "", "nfsw.exe");
				RunNetsh ("advfirewall firewall delete rule name=\"ClassicEA Launcher\"");
				RunNetsh ("advfirewall firewall delete rule name=\"ClassicEA NFSW\"");
				RunNetsh ("advfirewall firewall delete rule name=\"ClassicEA Proxy\"");
				if (!string.IsNullOrEmpty (launcherExe) && File.Exists (launcherExe)) {
					RunNetsh ("advfirewall firewall add rule name=\"ClassicEA Launcher\" dir=in action=allow program=\"" + launcherExe + "\" enable=yes");
				}
				if (File.Exists (text)) {
					RunNetsh ("advfirewall firewall add rule name=\"ClassicEA NFSW\" dir=in action=allow program=\"" + text + "\" enable=yes");
				}
				if (proxyPort > 0) {
					RunNetsh ("advfirewall firewall add rule name=\"ClassicEA Proxy\" dir=in action=allow protocol=TCP localport=" + proxyPort + " enable=yes");
				}
				log?.Invoke ("Firewall rules applied");
			} catch (Exception ex) {
				log?.Invoke ("Firewall: " + ex.Message);
			}
		}

		private static void RunNetsh (string args)
		{
			try {
				Process.Start (new ProcessStartInfo {
					FileName = "netsh",
					Arguments = args,
					CreateNoWindow = true,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden
				})?.WaitForExit (5000);
			} catch {
			}
		}
	}
	public static class NrzChallenges
	{
		public class ChallengeInfo
		{
			public int Id;

			public string Name = "Challenge";

			public string Class = "?";

			public string Difficulty = "MEDIUM";

			public string ImageUrl = "";

			public string Mode = "TIME-ATTACK MODE";

			public string TargetTime = "";

			public string Car = "";

			public string Powerups = "";

			public string Reward = "";

			public string Players = "";

			public string Status = "NOT COMPLETED";

			public bool IsCompleted;
		}

		private static readonly string[] DiffNames = new string[5] { "VERY EASY", "EASY", "MEDIUM", "HARD", "VERY HARD" };

		private static string _cachedEmail;

		private static DateTime _cacheTime = DateTime.MinValue;

		private static List<ChallengeInfo> _cachedList;

		private static int _cachedYear;

		private static int _cachedWeek;

		private static string _sessionU;

		private static string _sessionP;

		private static string _sessionEmail;

		private static CookieContainer _cookies = new CookieContainer ();

		public static string LastError = "";

		public static string LastRaw = "";

		public static int LastServerYear;

		public static int LastServerWeek;

		public static string LastLoginRaw = "";

		public static string SessionU => _sessionU;

		public static string SessionP => _sessionP;

		public static bool HasSession {
			get {
				if (!string.IsNullOrEmpty (_sessionU)) {
					return !string.IsNullOrEmpty (_sessionP);
				}
				return false;
			}
		}

		public static CookieContainer GetSessionCookies ()
		{
			return _cookies ?? new CookieContainer ();
		}

		public static void GetIsoYearWeek (out int year, out int week)
		{
			try {
				DateTime now = DateTime.Now;
				year = now.Year;
				DateTime dateTime = new DateTime (now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
				int num = (int)dateTime.DayOfWeek;
				if (num == 0) {
					num = 7;
				}
				dateTime = dateTime.AddDays (4 - num);
				DateTime dateTime2 = new DateTime (dateTime.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				week = (int)Math.Ceiling (((dateTime - dateTime2).TotalDays + 1.0) / 7.0);
				if (week < 1) {
					week = 1;
				}
				if (week > 53) {
					week = 53;
				}
			} catch {
				year = DateTime.UtcNow.Year;
				week = 1;
			}
		}

		public static void InvalidateCache ()
		{
			_cacheTime = DateTime.MinValue;
			_cachedList = null;
			_cachedYear = 0;
			_cachedWeek = 0;
		}

		public static void ClearSession ()
		{
			_sessionU = null;
			_sessionP = null;
			_sessionEmail = null;
			_cookies = new CookieContainer ();
			InvalidateCache ();
		}

		public static bool WarmSession (string email, string plainPassword)
		{
			LastError = "";
			if (string.IsNullOrWhiteSpace (email) || string.IsNullOrWhiteSpace (plainPassword)) {
				LastError = "empty credentials";
				return false;
			}
			if (!Login (email.Trim (), plainPassword, out var eashU, out var eashP)) {
				if (string.IsNullOrEmpty (LastError)) {
					LastError = "login failed";
				}
				return false;
			}
			_sessionU = eashU;
			_sessionP = eashP;
			_sessionEmail = email.Trim ();
			InvalidateCache ();
			return true;
		}

		public static List<ChallengeInfo> FetchThisWeek (string email, string plainPassword)
		{
			LastError = "";
			GetIsoYearWeek (out var year, out var week);
			string eashU = _sessionU;
			string eashP = _sessionP;
			string text = (email ?? "").Trim ();
			if (string.IsNullOrEmpty (text) && !string.IsNullOrEmpty (_sessionEmail)) {
				text = _sessionEmail;
			}
			if (string.IsNullOrEmpty (eashU) || string.IsNullOrEmpty (eashP)) {
				if (string.IsNullOrWhiteSpace (text) || string.IsNullOrWhiteSpace (plainPassword)) {
					LastError = "no session and no credentials";
					return new List<ChallengeInfo> ();
				}
				if (!Login (text, plainPassword, out eashU, out eashP)) {
					if (string.IsNullOrEmpty (LastError)) {
						LastError = "login failed";
					}
					return new List<ChallengeInfo> ();
				}
				_sessionU = eashU;
				_sessionP = eashP;
				_sessionEmail = text;
			}
			if (_cachedList != null && _cachedList.Count > 0 && (DateTime.UtcNow - _cacheTime).TotalMinutes < 0.5) {
				return _cachedList;
			}
			string text2 = Gateway ("challenges", "GetWeek", new object[2] { year, week }, eashU, eashP);
			LastRaw = text2 ?? "";
			int year2 = 0;
			int week2 = 0;
			ExtractYearWeekFromDateField (text2, out year2, out week2);
			if (week2 <= 0) {
				week2 = week;
			}
			if (year2 <= 0) {
				year2 = year;
			}
			LastServerYear = year2;
			LastServerWeek = week2;
			List<int> years = new List<int> ();
			AddY (year);
			AddY (year2);
			AddY (year - 1);
			AddY (year + 1);
			AddY (year2 - 1);
			AddY (year2 + 1);
			List<int> weeks = new List<int> ();
			AddW (week);
			AddW (week2);
			for (int i = Math.Max (1, week - 2); i <= Math.Min (53, week + 2); i++) {
				AddW (i);
			}
			for (int j = Math.Max (1, week2 - 3); j <= Math.Min (53, week2 + 3); j++) {
				AddW (j);
			}
			AddW (week);
			for (int k = Math.Max (1, week - 3); k <= Math.Min (53, week + 3); k++) {
				AddW (k);
			}
			List<ChallengeInfo> list = new List<ChallengeInfo> ();
			int cachedWeek = week2;
			int cachedYear = year;
			string text3 = text2 ?? "";
			foreach (int item in years) {
				foreach (int item2 in weeks) {
					string text4 = Gateway ("challenges", "GetWeek", new object[2] { item, item2 }, eashU, eashP);
					if (string.IsNullOrWhiteSpace (text4) || (text4.IndexOf ("\"loop\"", StringComparison.OrdinalIgnoreCase) < 0 && text4.IndexOf ("\"date\"", StringComparison.OrdinalIgnoreCase) >= 0)) {
						string text5 = Gateway ("challenges", "GetWeek", new object[2] {
							item.ToString (),
							item2.ToString ()
						}, eashU, eashP);
						if (!string.IsNullOrWhiteSpace (text5) && text5.IndexOf ("loop", StringComparison.OrdinalIgnoreCase) >= 0) {
							text4 = text5;
						}
					}
					if (!string.IsNullOrWhiteSpace (text4)) {
						text3 = text4;
						LastRaw = text4;
						int num = ExtractWeekFromDateField (text4);
						if (num > 0) {
							AddW (num);
						}
						Dictionary<int, int> doneMap = ParseDone (Gateway ("challenges", "GetDone", new object[2] { item, item2 }, eashU, eashP));
						List<ChallengeInfo> list2 = ParseWeek (text4, doneMap);
						if (list2.Count > list.Count) {
							list = list2;
							cachedWeek = item2;
							cachedYear = item;
						}
						if (list.Count >= 3) {
							goto end_IL_03f4;
						}
					}
				}
				continue;
				end_IL_03f4:
				break;
			}
			if (list.Count == 0) {
				LastError = "GetWeek empty loop for " + cachedYear + " W" + week2 + " (tried W" + week + "+neighbors). raw=" + (string.IsNullOrEmpty (text3) ? "(empty)" : ((text3.Length > 220) ? text3.Substring (0, 220) : text3));
			}
			_cachedList = list;
			_cachedYear = cachedYear;
			_cachedWeek = cachedWeek;
			_cachedEmail = text;
			_cacheTime = DateTime.UtcNow;
			return list;
			void AddW (int w)
			{
				if (w >= 1 && w <= 53 && !weeks.Contains (w)) {
					weeks.Add (w);
				}
			}
			void AddY (int y)
			{
				if (y >= 2020 && y <= 2035 && !years.Contains (y)) {
					years.Add (y);
				}
			}
		}

		private static int ExtractWeekFromDateField (string raw)
		{
			ExtractYearWeekFromDateField (raw, out var _, out var week);
			return week;
		}

		private static void ExtractYearWeekFromDateField (string raw, out int year, out int week)
		{
			year = 0;
			week = 0;
			if (string.IsNullOrEmpty (raw)) {
				return;
			}
			try {
				Match match = Regex.Match (raw, "Week\\s+(\\d+)\\s*-\\s*\\d+\\s+[A-Za-z]+\\s+(\\d{4})", RegexOptions.IgnoreCase);
				if (match.Success) {
					int.TryParse (match.Groups [1].Value, out week);
					int.TryParse (match.Groups [2].Value, out year);
					return;
				}
				match = Regex.Match (raw, "Week\\s+(\\d+)", RegexOptions.IgnoreCase);
				if (match.Success) {
					int.TryParse (match.Groups [1].Value, out week);
				}
				match = Regex.Match (raw, "(20\\d{2})");
				if (match.Success) {
					int.TryParse (match.Groups [1].Value, out year);
				}
			} catch {
			}
		}

		private static bool Login (string email, string password, out string eashU, out string eashP)
		{
			eashU = null;
			eashP = null;
			try {
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				string text = Gateway ("session", "login", new object[3] { email, password, "" }, null, null);
				LastRaw = text ?? "";
				if (string.IsNullOrEmpty (text)) {
					LastError = "login empty response";
					return false;
				}
				string text2 = text.Trim ();
				if (text2.StartsWith ("\"") || (!text2.StartsWith ("{") && !text2.StartsWith ("["))) {
					LastError = text2.Trim (new char[1] { '"' });
					return false;
				}
				JObject jo = JObject.Parse (text);
				eashU = FindToken (jo, "easharpptr_u", "easharpptr-u", "Easharpptr_u", "easharpptrU");
				eashP = FindToken (jo, "easharpptr_p", "easharpptr-p", "Easharpptr_p", "easharpptrP");
				if (string.IsNullOrEmpty (eashU)) {
					eashU = email;
				}
				if (string.IsNullOrEmpty (eashP)) {
					LastError = "login ok but no easharpptr_p in: " + ((text.Length > 220) ? text.Substring (0, 220) : text);
					return false;
				}
				LastLoginRaw = text;
				try {
					Gateway ("session", "GetUserInfo", new object[0], eashU, eashP);
				} catch {
				}
				return true;
			} catch (Exception ex) {
				LastError = "login exception: " + ex.Message + " raw=" + ((LastRaw.Length > 120) ? LastRaw.Substring (0, 120) : LastRaw);
				return false;
			}
		}

		private static string FindToken (JObject jo, params string[] keys)
		{
			foreach (string key in keys) {
				string text = Str (jo, key);
				if (!string.IsNullOrEmpty (text)) {
					return text;
				}
			}
			foreach (JProperty item in jo.Properties ()) {
				if (item.Value is JObject jo2) {
					string text2 = FindToken (jo2, keys);
					if (!string.IsNullOrEmpty (text2)) {
						return text2;
					}
				}
			}
			return null;
		}

		private static string Str (JObject jo, string key)
		{
			JToken jToken = jo [key];
			if (jToken != null && jToken.Type != JTokenType.Null) {
				return jToken.ToString ();
			}
			return null;
		}

		private static string Gateway (string service, string method, object[] parameters, string eashU, string eashP)
		{
			try {
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				try {
					ServicePointManager.ServerCertificateValidationCallback = (object a, X509Certificate b, X509Chain c, SslPolicyErrors d) => true;
				} catch {
				}
				if (_cookies == null) {
					_cookies = new CookieContainer ();
				}
				JObject jObject = new JObject {
					 ["serviceName"] = service,
					 ["methodName"] = method,
					 ["parameters"] = JArray.FromObject (parameters ?? new object[0])
				};
				byte[] bytes = Encoding.UTF8.GetBytes (jObject.ToString (Formatting.None));
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create ("https://api.nightriderz.world/gateway.php?contentType=application/json");
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/json; charset=utf-8";
				httpWebRequest.Accept = "application/json";
				httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
				httpWebRequest.Timeout = 25000;
				httpWebRequest.ReadWriteTimeout = 25000;
				httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				httpWebRequest.CookieContainer = _cookies;
				httpWebRequest.KeepAlive = true;
				try {
					httpWebRequest.Headers ["Origin"] = "https://nightriderz.world";
				} catch {
				}
				try {
					httpWebRequest.Referer = "https://nightriderz.world/challenges";
				} catch {
				}
				if (!string.IsNullOrEmpty (eashU)) {
					try {
						httpWebRequest.Headers.Set ("easharpptr-u", eashU);
					} catch {
						try {
							httpWebRequest.Headers ["easharpptr-u"] = eashU;
						} catch {
						}
					}
				}
				if (!string.IsNullOrEmpty (eashP)) {
					try {
						httpWebRequest.Headers.Set ("easharpptr-p", eashP);
					} catch {
						try {
							httpWebRequest.Headers ["easharpptr-p"] = eashP;
						} catch {
						}
					}
				}
				httpWebRequest.ContentLength = bytes.Length;
				using (Stream stream = ((WebRequest)httpWebRequest).GetRequestStream ()) {
					stream.Write (bytes, 0, bytes.Length);
				}
				try {
					HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
					try {
						using StreamReader streamReader = new StreamReader (((WebResponse)httpWebResponse).GetResponseStream (), Encoding.UTF8);
						return streamReader.ReadToEnd ();
					} finally {
						((IDisposable)httpWebResponse)?.Dispose ();
					}
				} catch (WebException ex) {
					if (ex.Response != null) {
						using (StreamReader streamReader2 = new StreamReader (ex.Response.GetResponseStream (), Encoding.UTF8)) {
							return streamReader2.ReadToEnd ();
						}
					}
					LastError = "network: " + ((Exception)(object)ex).Message;
					return null;
				}
			} catch (Exception ex2) {
				LastError = "gateway: " + ex2.Message;
				return null;
			}
		}

		private static Dictionary<int, int> ParseDone (string raw)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int> ();
			if (string.IsNullOrEmpty (raw)) {
				return dictionary;
			}
			try {
				JToken jToken = JToken.Parse (raw);
				JArray jArray = jToken as JArray;
				if (jArray == null && jToken is JObject jObject) {
					JToken jToken2 = jObject ["loop"] ?? jObject ["data"] ?? jObject ["Done"] ?? jObject ["done"];
					jArray = jToken2 as JArray;
					if (jArray == null && jToken2 is JObject jObject2) {
						jArray = new JArray ();
						foreach (JProperty item in jObject2.Properties ()) {
							jArray.Add (item.Value);
						}
					}
				}
				if (jArray == null) {
					return dictionary;
				}
				foreach (JToken item2 in jArray) {
					if (item2 == null || item2.Type == JTokenType.Null || item2.Type == JTokenType.Integer) {
						continue;
					}
					int num = item2.Value<int?> ("id") ?? item2.Value<int?> ("ID") ?? item2.Value<int?> ("Id") ?? item2.Value<int?> ("challenge_id").GetValueOrDefault ();
					if (num == 0) {
						continue;
					}
					JToken jToken3 = item2 ["done"] ?? item2 ["Done"];
					if (jToken3 == null || jToken3.Type == JTokenType.Null) {
						continue;
					}
					int value;
					if (jToken3.Type == JTokenType.Integer) {
						value = jToken3.Value<int> ();
					} else if (jToken3.Type == JTokenType.Boolean) {
						value = (jToken3.Value<bool> () ? 1 : 0);
					} else {
						string text = jToken3.ToString ();
						if (text == "0" || text.Equals ("false", StringComparison.OrdinalIgnoreCase)) {
							value = 0;
						} else {
							if (!(text == "1") && !text.Equals ("true", StringComparison.OrdinalIgnoreCase)) {
								continue;
							}
							value = 1;
						}
					}
					dictionary [num] = value;
				}
			} catch {
			}
			return dictionary;
		}

		private static List<ChallengeInfo> ParseWeek (string raw, Dictionary<int, int> doneMap)
		{
			List<ChallengeInfo> list = new List<ChallengeInfo> ();
			if (string.IsNullOrEmpty (raw)) {
				return list;
			}
			try {
				JToken jToken = FindChallengeArray (JToken.Parse (raw));
				if (jToken != null && jToken.Type == JTokenType.Object) {
					JArray jArray = new JArray ();
					foreach (JProperty item in ((JObject)jToken).Properties ()) {
						jArray.Add (item.Value);
					}
					jToken = jArray;
				}
				if (jToken == null || jToken.Type != JTokenType.Array) {
					return list;
				}
				foreach (JToken item2 in (JArray)jToken) {
					if (item2 == null || item2.Type != JTokenType.Object) {
						continue;
					}
					JObject jObject = (JObject)item2;
					ChallengeInfo challengeInfo = new ChallengeInfo ();
					challengeInfo.Id = jObject.Value<int?> ("ID") ?? jObject.Value<int?> ("Id") ?? jObject.Value<int?> ("id").GetValueOrDefault ();
					challengeInfo.IsCompleted = false;
					challengeInfo.Status = "NOT COMPLETED";
					int num = jObject.Value<int?> ("difficulty") ?? jObject.Value<int?> ("Difficulty").GetValueOrDefault (2);
					if (num < 0) {
						num = 0;
					}
					if (num > 4) {
						num = 4;
					}
					challengeInfo.Difficulty = DiffNames [num];
					string text = Str (jObject, "image") ?? Str (jObject, "Image") ?? "";
					if (!string.IsNullOrEmpty (text)) {
						if (text.StartsWith ("http", StringComparison.OrdinalIgnoreCase)) {
							challengeInfo.ImageUrl = text;
						} else {
							challengeInfo.ImageUrl = "https://cdn.nightriderz.world/images/website/challenges/" + text.TrimStart (new char[1] { '/' });
						}
					}
					challengeInfo.Players = (jObject.Value<int?> ("made_count") ?? jObject.Value<int?> ("madeCount").GetValueOrDefault ()) + " PLAYERS COMPLETED";
					string text2 = Str (jObject, "reward") ?? Str (jObject, "Reward") ?? "";
					if (!string.IsNullOrEmpty (text2)) {
						challengeInfo.Reward = text2.Trim ();
					}
					challengeInfo.Name = ((challengeInfo.Id > 0) ? ("#" + challengeInfo.Id) : "Challenge");
					challengeInfo.Mode = "TIME-ATTACK MODE";
					if ((jObject ["conditions"] ?? jObject ["Conditions"]) is JArray jArray2) {
						foreach (JToken item3 in jArray2) {
							if (item3 == null || item3.Type != JTokenType.Object) {
								continue;
							}
							JObject jObject2 = (JObject)item3;
							string text3 = Str (jObject2, "type") ?? "";
							if (text3.Equals ("raceID", StringComparison.OrdinalIgnoreCase)) {
								string text4 = Str (jObject2, "race_name") ?? Str (jObject2, "raceName") ?? Str (jObject2, "name") ?? "";
								text4 = (text4 ?? "").Trim ();
								int num2 = text4.IndexOf ('(');
								if (num2 > 0) {
									text4 = text4.Substring (0, num2).Trim ();
								}
								if (!string.IsNullOrEmpty (text4)) {
									challengeInfo.Name = text4.ToUpperInvariant ();
								}
								int valueOrDefault = jObject2.Value<int?> ("value").GetValueOrDefault ();
								if (valueOrDefault >= 5000 && valueOrDefault < 6000) {
									challengeInfo.Mode = "TIME-ATTACK MODE";
								}
								string text5 = Str (jObject2, "race_type") ?? "";
								if (!string.IsNullOrEmpty (text5)) {
									challengeInfo.Class = text5;
								}
							} else if (text3.Equals ("eventDurationInMilliseconds", StringComparison.OrdinalIgnoreCase) || text3.IndexOf ("Duration", StringComparison.OrdinalIgnoreCase) >= 0) {
								string text6 = Str (jObject2, "value") ?? Str (jObject2, "display") ?? "";
								if (long.TryParse (text6, out var result) && result > 1000) {
									double num3 = (double)result / 1000.0;
									int num4 = (int)(num3 / 60.0);
									double num5 = num3 - (double)(num4 * 60);
									challengeInfo.TargetTime = $"-{num4:00}:{num5:00.000} MINS";
								} else if (!string.IsNullOrEmpty (text6)) {
									challengeInfo.TargetTime = text6;
								}
							} else if (text3.Equals ("car", StringComparison.OrdinalIgnoreCase)) {
								string text7 = Str (jObject2, "value") ?? Str (jObject2, "name") ?? Str (jObject2, "display") ?? "";
								if (!string.IsNullOrEmpty (text7)) {
									challengeInfo.Car = text7.ToUpperInvariant ();
								}
							} else if (text3.Equals ("pu", StringComparison.OrdinalIgnoreCase) || text3.IndexOf ("power", StringComparison.OrdinalIgnoreCase) >= 0) {
								string text8 = Str (jObject2, "value") ?? Str (jObject2, "display") ?? "NOT ALLOWED";
								challengeInfo.Powerups = "POWERUPS " + ((text8.Equals ("0") || text8.Equals ("false", StringComparison.OrdinalIgnoreCase)) ? "NOT ALLOWED" : text8.ToUpperInvariant ());
							}
						}
					}
					if (string.IsNullOrEmpty (challengeInfo.Powerups)) {
						challengeInfo.Powerups = "POWERUPS NOT ALLOWED";
					}
					if (doneMap != null && challengeInfo.Id > 0 && doneMap.ContainsKey (challengeInfo.Id)) {
						switch (doneMap [challengeInfo.Id]) {
						case 1:
							challengeInfo.IsCompleted = true;
							challengeInfo.Status = "COMPLETED";
							break;
						case 0:
							challengeInfo.IsCompleted = false;
							challengeInfo.Status = "CLAIM REWARD";
							break;
						default:
							challengeInfo.IsCompleted = false;
							challengeInfo.Status = "NOT COMPLETED";
							break;
						}
					} else {
						challengeInfo.IsCompleted = false;
						challengeInfo.Status = "NOT COMPLETED";
					}
					list.Add (challengeInfo);
				}
			} catch {
			}
			return list;
		}

		private static JToken FindChallengeArray (JToken root)
		{
			if (root == null) {
				return null;
			}
			if (root is JArray { Count: >0 } jArray && jArray [0] is JObject) {
				JObject jObject = (JObject)jArray [0];
				if (jObject ["ID"] != null || jObject ["id"] != null || jObject ["image"] != null || jObject ["difficulty"] != null) {
					return jArray;
				}
			}
			if (root is JObject jObject2) {
				string[] array = new string[7] { "loop", "Loop", "data", "challenges", "items", "list", "week" };
				foreach (string propertyName in array) {
					JToken jToken = jObject2 [propertyName];
					if (jToken != null) {
						JToken jToken2 = FindChallengeArray (jToken);
						if (jToken2 != null) {
							return jToken2;
						}
					}
				}
				foreach (JProperty item in jObject2.Properties ()) {
					if (item.Value is JArray) {
						JToken jToken3 = FindChallengeArray (item.Value);
						if (jToken3 != null) {
							return jToken3;
						}
					}
				}
			}
			return null;
		}

		private static string FormatRewards (JToken rewards)
		{
			try {
				if (rewards.Type == JTokenType.Array) {
					List<string> list = new List<string> ();
					foreach (JToken item in (JArray)rewards) {
						string text = (item.Value<string> ("type") ?? "").ToLowerInvariant ();
						long num = item.Value<long?> ("amount") ?? item.Value<long?> ("value").GetValueOrDefault ();
						switch (text) {
						case "sb":
						case "speedboost":
							list.Add ($"{num:n0} SB");
							continue;
						case "cash":
						case "money":
							list.Add ($"{num:n0} $");
							continue;
						}
						if (num > 0) {
							list.Add ($"{num:n0}");
						}
					}
					return string.Join (" · ", list);
				}
				if (rewards.Type == JTokenType.Object) {
					long valueOrDefault = rewards.Value<long?> ("sb").GetValueOrDefault ();
					long valueOrDefault2 = rewards.Value<long?> ("cash").GetValueOrDefault ();
					if (valueOrDefault > 0) {
						return $"{valueOrDefault:n0} SB";
					}
					if (valueOrDefault2 > 0) {
						return $"{valueOrDefault2:n0} $";
					}
				}
			} catch {
			}
			return "";
		}

		public static string BuildChallengesHtml (List<ChallengeInfo> challenges, int year, int week)
		{
			if (challenges == null) {
				challenges = new List<ChallengeInfo> ();
			}
			StringBuilder stringBuilder = new StringBuilder ();
			if (challenges.Count == 0) {
				stringBuilder.Append ("<div class='empty'>Sign in with your NightRiderz email &amp; password<br/>to load this week's live challenges and completion status.</div>");
			} else {
				foreach (ChallengeInfo challenge in challenges) {
					string text = (challenge.Difficulty ?? "MEDIUM").ToUpperInvariant ();
					string value = "b-med";
					if (text.Contains ("EASY") && !text.Contains ("VERY")) {
						value = "b-easy";
					} else if (text.Contains ("HARD")) {
						value = "b-hard";
					}
					string value2 = ((!challenge.IsCompleted) ? "st-todo" : ((challenge.Status != null && challenge.Status.IndexOf ("CLAIM", StringComparison.OrdinalIgnoreCase) >= 0) ? "st-claim" : "st-done"));
					stringBuilder.Append ("<div class='card'>");
					stringBuilder.Append ("<div class='img'>");
					if (!string.IsNullOrEmpty (challenge.ImageUrl)) {
						stringBuilder.Append ("<img src='").Append (Html (challenge.ImageUrl)).Append ("' alt=''/>");
					}
					stringBuilder.Append ("<span class='badge ").Append (value).Append ("'>")
						.Append (Html (text))
						.Append ("</span>");
					stringBuilder.Append ("<span class='cid'>#").Append (challenge.Id).Append ("</span></div>");
					if (!string.IsNullOrEmpty (challenge.Players)) {
						stringBuilder.Append ("<div class='players'>").Append (Html (challenge.Players)).Append ("</div>");
					}
					stringBuilder.Append ("<div class='row'><span class='cls'>").Append (Html (challenge.Class)).Append ("</span> ");
					stringBuilder.Append (Html (challenge.Mode)).Append ("<br/><b>").Append (Html (challenge.Name))
						.Append ("</b></div>");
					if (!string.IsNullOrEmpty (challenge.TargetTime)) {
						stringBuilder.Append ("<div class='row'>").Append (Html (challenge.TargetTime)).Append ("</div>");
					}
					if (!string.IsNullOrEmpty (challenge.Car)) {
						stringBuilder.Append ("<div class='row'>").Append (Html (challenge.Car)).Append ("</div>");
					}
					if (!string.IsNullOrEmpty (challenge.Powerups)) {
						stringBuilder.Append ("<div class='row'>").Append (Html (challenge.Powerups)).Append ("</div>");
					}
					if (!string.IsNullOrEmpty (challenge.Reward)) {
						stringBuilder.Append ("<div class='reward'>").Append (Html (challenge.Reward)).Append ("</div>");
					}
					stringBuilder.Append ("<div class='status ").Append (value2).Append ("'>")
						.Append (Html (challenge.Status))
						.Append ("</div>");
					stringBuilder.Append ("</div>");
				}
			}
			return "<!DOCTYPE html><html><head><meta http-equiv='X-UA-Compatible' content='IE=edge'/><meta charset='utf-8'/><style>html,body{margin:0;padding:0;width:771px;height:310px;overflow:hidden;background:#1a1d24;font-family:Segoe UI,Arial,sans-serif;color:#e8eaed}.wrap{box-sizing:border-box;width:100%;height:100%;padding:6px 8px;overflow-x:auto;overflow-y:hidden;white-space:nowrap}.head{font-size:10px;color:#9aa0a6;margin:0 0 4px;letter-spacing:1px}.head b{color:#ffc107}.card{display:inline-block;vertical-align:top;width:142px;margin-right:7px;background:#2a2f3a;border-radius:8px;overflow:hidden;white-space:normal;font-size:9px}.img{position:relative;height:68px;background:#111}.img img{width:100%;height:68px;object-fit:cover;display:block;border:0}.badge{position:absolute;top:5px;left:5px;padding:1px 5px;border-radius:3px;font-size:8px;font-weight:700;color:#fff}.b-hard{background:#e53935}.b-med{background:#f9a825;color:#111}.b-easy{background:#43a047}.cid{position:absolute;left:5px;bottom:3px;font-size:8px;color:#fff;text-shadow:0 1px 2px #000}.players{padding:3px 6px;color:#9aa0a6;font-size:8px}.row{margin:2px 6px;padding:4px 5px;background:#343b49;border-radius:3px;line-height:1.2}.row b{font-size:9px}.cls{display:inline-block;min-width:11px;padding:0 2px;text-align:center;background:#1a73e8;border-radius:2px;font-size:8px;font-weight:700;margin-right:3px}.reward{margin:4px 6px 3px;padding:5px;text-align:center;border-radius:3px;background:linear-gradient(90deg,#f9a825,#ff6f00);color:#111;font-weight:700;font-size:10px}.status{margin:0 6px 6px;padding:5px;text-align:center;border-radius:3px;font-weight:700;font-size:9px}.st-todo{background:#c62828;color:#fff}.st-claim{background:#f9a825;color:#111}.st-done{background:#2e7d32;color:#fff}.empty{color:#aaa;font-size:12px;padding:40px 12px;white-space:normal;text-align:center}</style></head><body><div class='wrap'><div class='head'>WEEKLY CHALLENGES · WEEK <b>" + week + "</b> · LIVE · <a style='color:#8ab4f8' href='https://nightriderz.world/challenges'>nightriderz.world/challenges</a></div>" + stringBuilder?.ToString () + "</div></body></html>";
		}

		private static string Html (string s)
		{
			if (s == null) {
				return "";
			}
			return SecurityElement.Escape (s) ?? "";
		}
	}
	public static class ServerAuth
	{
		public static string LastError = string.Empty;

		public static string LastRaw = string.Empty;

		private const string SbrwVersion = "2.2.4.0";

		private static readonly string UA = "SBRW Launcher 2.2.4.0 (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";

		private static readonly string XUA = "GameLauncherReborn 2.2.4.0 WinForms (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";

		private static string _launcherHash;

		private static string LauncherHash {
			get {
				if (_launcherHash != null) {
					return _launcherHash;
				}
				try {
					string location = Assembly.GetExecutingAssembly ().Location;
					if (!string.IsNullOrEmpty (location) && File.Exists (location)) {
						SHA1 sHA = SHA1.Create ();
						try {
							using FileStream fileStream = File.OpenRead (location);
							byte[] array = ((HashAlgorithm)sHA).ComputeHash ((Stream)fileStream);
							StringBuilder stringBuilder = new StringBuilder (array.Length * 2);
							for (int i = 0; i < array.Length; i++) {
								stringBuilder.Append (array [i].ToString ("x2"));
							}
							_launcherHash = stringBuilder.ToString ().ToUpperInvariant ();
							return _launcherHash;
						} finally {
							((IDisposable)sHA)?.Dispose ();
						}
					}
				} catch {
				}
				_launcherHash = "0000000000000000000000000000000000000000";
				return _launcherHash;
			}
		}

		private static void ApplySbrwHeaders (HttpWebRequest req)
		{
			req.UserAgent = UA;
			req.Headers ["X-UserAgent"] = XUA;
			req.Headers ["X-HWID"] = GetHwid ();
			req.Headers ["X-HiddenHWID"] = GetHwidAlt ();
			req.Headers ["X-GameLauncherHash"] = LauncherHash;
			req.Headers ["X-GameLauncherCertificate"] = "ClassicEA";
			req.Headers ["X-DiscordID"] = "0";
			req.Headers ["X-Electron"] = "false";
		}

		private static string GetHwid ()
		{
			try {
				string s = Environment.MachineName + "|" + Environment.UserName + "|" + Environment.ProcessorCount;
				SHA1 sHA = SHA1.Create ();
				try {
					byte[] array = sHA.ComputeHash (Encoding.UTF8.GetBytes (s));
					StringBuilder stringBuilder = new StringBuilder ();
					for (int i = 0; i < array.Length; i++) {
						stringBuilder.Append (array [i].ToString ("x2"));
					}
					return stringBuilder.ToString ().ToUpperInvariant ();
				} finally {
					((IDisposable)sHA)?.Dispose ();
				}
			} catch {
				return "UNKNOWNHWID";
			}
		}

		private static string GetHwidAlt ()
		{
			try {
				string s = "ALT|" + Environment.OSVersion?.ToString () + "|" + Environment.SystemDirectory;
				SHA1 sHA = SHA1.Create ();
				try {
					byte[] array = sHA.ComputeHash (Encoding.UTF8.GetBytes (s));
					StringBuilder stringBuilder = new StringBuilder ();
					for (int i = 0; i < array.Length; i++) {
						stringBuilder.Append (array [i].ToString ("x2"));
					}
					return stringBuilder.ToString ().ToUpperInvariant ();
				} finally {
					((IDisposable)sHA)?.Dispose ();
				}
			} catch {
				return "UNKNOWNALTHWID";
			}
		}

		public static string Authenticate (string serverEngineUrl, string email, string plainPassword)
		{
			LastError = string.Empty;
			LastRaw = string.Empty;
			try {
				if (string.IsNullOrEmpty (serverEngineUrl)) {
					return Fail ("No server URL");
				}
				string text = serverEngineUrl.TrimEnd (new char[1] { '/' });
				string text2 = plainPassword ?? "";
				string text3 = (email ?? "").Trim ();
				if (string.IsNullOrEmpty (text3) || string.IsNullOrEmpty (text2)) {
					return Fail ("Invalid E-mail or Password");
				}
				bool flag = text.ToLowerInvariant ().IndexOf ("nightriderz") >= 0;
				bool flag2 = text.ToLowerInvariant ().IndexOf ("worldunited") >= 0;
				Log ("Auth start -> " + text + " email=" + text3 + " passLen=" + text2.Length + " nrz=" + flag + " wu=" + flag2);
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				ServicePointManager.ServerCertificateValidationCallback = (object a, X509Certificate b, X509Chain c, SslPolicyErrors d) => true;
				if (flag) {
					string text4 = TryModern (text, text3, text2, lowerKeys: true);
					if (IsOk (text4)) {
						Log ("Auth OK (NRZ lowercase)");
						return text4;
					}
					text4 = TryModern (text, text3, text2, lowerKeys: true, includeUpgrade: false);
					if (IsOk (text4)) {
						Log ("Auth OK (NRZ no upgrade)");
						return text4;
					}
					if (string.IsNullOrEmpty (LastError)) {
						LastError = "Invalid E-mail or Password";
					}
					Log ("Auth FAIL NRZ: " + LastError + " raw=" + Trunc (LastRaw));
					return Fail (LastError);
				}
				if (flag2) {
					try {
						string text5 = TryClassic (text, text3, Sha1 (text2));
						if (IsOk (text5)) {
							Log ("Auth OK (WU classic)");
							return text5;
						}
						if (IsWrongLauncher (LastRaw) || IsWrongLauncher (text5)) {
							LastError = "WorldUnited requires the official SBRW Launcher.\nDownload: https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases\n\nNightRiderz works with this Classic EA launcher.";
							Log ("Auth FAIL WU: wrong launcher blocked");
							return Fail (LastError);
						}
					} catch (Exception ex) {
						Log ("WU classic exception: " + ex.Message);
						if (IsWrongLauncher (LastRaw)) {
							LastError = "WorldUnited requires the official SBRW Launcher.\nDownload: https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases";
							return Fail (LastError);
						}
					}
					if (string.IsNullOrEmpty (LastError) || LastError.IndexOf ("401") >= 0) {
						LastError = "Invalid E-mail or Password";
					}
					Log ("Auth FAIL WU: " + LastError + " raw=" + Trunc (LastRaw));
					return Fail (LastError);
				}
				string text6 = TryModern (text, text3, text2, lowerKeys: true);
				if (IsOk (text6)) {
					Log ("Auth OK (lowercase)");
					return text6;
				}
				text6 = TryModern (text, text3, text2, lowerKeys: false);
				if (IsOk (text6)) {
					Log ("Auth OK (capital)");
					return text6;
				}
				try {
					text6 = TryClassic (text, text3, Sha1 (text2));
					if (IsOk (text6)) {
						Log ("Auth OK (classic)");
						return text6;
					}
				} catch (Exception ex2) {
					Log ("Classic exception: " + ex2.Message);
				}
				if (string.IsNullOrEmpty (LastError)) {
					LastError = "Invalid E-mail or Password";
				}
				Log ("Auth FAIL: " + LastError + " raw=" + Trunc (LastRaw));
				return Fail (LastError);
			} catch (Exception ex3) {
				Log ("Auth exception: " + ex3.Message);
				return Fail (ex3.Message);
			}
		}

		private static bool IsWrongLauncher (string body)
		{
			if (string.IsNullOrEmpty (body)) {
				return false;
			}
			if (body.IndexOf ("wrong launcher", StringComparison.OrdinalIgnoreCase) < 0) {
				return body.IndexOf ("git.io/Download_NFSW", StringComparison.OrdinalIgnoreCase) >= 0;
			}
			return true;
		}

		private static bool IsOk (string xmlOrJson)
		{
			if (string.IsNullOrEmpty (xmlOrJson)) {
				return false;
			}
			if (xmlOrJson.IndexOf ("<UserId>0</UserId>", StringComparison.OrdinalIgnoreCase) >= 0) {
				return false;
			}
			if (xmlOrJson.IndexOf ("\"userId\":0", StringComparison.OrdinalIgnoreCase) >= 0) {
				return false;
			}
			if (IsWrongLauncher (xmlOrJson)) {
				return false;
			}
			if (xmlOrJson.IndexOf ("<LoginToken>", StringComparison.OrdinalIgnoreCase) >= 0 && xmlOrJson.IndexOf ("<UserId>", StringComparison.OrdinalIgnoreCase) >= 0 && xmlOrJson.IndexOf ("<UserId>0</UserId>", StringComparison.OrdinalIgnoreCase) < 0) {
				return true;
			}
			try {
				if (xmlOrJson.TrimStart (Array.Empty<char> ()).StartsWith ("{")) {
					JObject jObject = JObject.Parse (xmlOrJson);
					string value = TokenToString (jObject ["token"] ?? jObject ["Token"]);
					string text = TokenToString (jObject ["userId"] ?? jObject ["UserId"]);
					return !string.IsNullOrEmpty (value) && !string.IsNullOrEmpty (text) && text != "0";
				}
			} catch {
			}
			return false;
		}

		private static string TryModern (string baseUrl, string email, string password, bool lowerKeys, bool includeUpgrade = true)
		{
			try {
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create (baseUrl + "/User/modernAuth");
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Timeout = 30000;
				ApplySbrwHeaders (httpWebRequest);
				string s = ((!lowerKeys) ? (includeUpgrade ? JsonConvert.SerializeObject (new {
					Email = email,
					Password = password,
					upgrade = true
				}) : JsonConvert.SerializeObject (new {
					Email = email,
					Password = password
				})) : (includeUpgrade ? JsonConvert.SerializeObject (new {
					email = email,
					password = password,
					upgrade = true
				}) : JsonConvert.SerializeObject (new { email, password })));
				byte[] bytes = Encoding.UTF8.GetBytes (s);
				httpWebRequest.ContentLength = bytes.Length;
				using (Stream stream = ((WebRequest)httpWebRequest).GetRequestStream ()) {
					stream.Write (bytes, 0, bytes.Length);
				}
				int statusCode;
				string text;
				try {
					HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
					try {
						using StreamReader streamReader = new StreamReader (((WebResponse)httpWebResponse).GetResponseStream ());
						statusCode = (int)httpWebResponse.StatusCode;
						text = streamReader.ReadToEnd ();
					} finally {
						((IDisposable)httpWebResponse)?.Dispose ();
					}
				} catch (WebException ex) {
					if (!(ex.Response is HttpWebResponse httpWebResponse2)) {
						Log ("TryModern exception: " + ((Exception)(object)ex).Message);
						return null;
					}
					statusCode = (int)httpWebResponse2.StatusCode;
					using StreamReader streamReader2 = new StreamReader (((WebResponse)httpWebResponse2).GetResponseStream ());
					text = streamReader2.ReadToEnd ();
				}
				LastRaw = text;
				Log ("modernAuth keys=" + (lowerKeys ? "lower" : "capital") + " upgrade=" + includeUpgrade + " status=" + statusCode + " body=" + Trunc (text));
				if (statusCode >= 200 && statusCode < 300) {
					string text2 = ParseModernBody (text);
					if (IsOk (text2)) {
						return text2;
					}
				}
				TryParseError (text);
				return text;
			} catch (Exception ex2) {
				Log ("TryModern exception: " + ex2.Message);
				return null;
			}
		}

		private static string ParseModernBody (string body)
		{
			try {
				JObject jObject = JObject.Parse (body);
				string text = TokenToString (jObject ["userId"] ?? jObject ["UserId"]);
				string text2 = TokenToString (jObject ["token"] ?? jObject ["Token"]);
				Log ("ParseModern userId=" + text + " tokenLen=" + (text2?.Length ?? 0));
				if (string.IsNullOrEmpty (text) || string.IsNullOrEmpty (text2)) {
					LastError = "Invalid E-mail or Password";
					return Fail (LastError);
				}
				return "<?xml version=\"1.0\"?><LoginStatusVO><UserId>" + XmlEsc (text) + "</UserId><LoginToken>" + XmlEsc (text2) + "</LoginToken><Description/></LoginStatusVO>";
			} catch {
				LastError = "Invalid E-mail or Password";
				return Fail (LastError);
			}
		}

		private static void TryParseError (string body)
		{
			try {
				if (string.IsNullOrEmpty (body)) {
					return;
				}
				if (IsWrongLauncher (body)) {
					LastError = "Wrong launcher blocked by server";
				} else if (body.TrimStart (Array.Empty<char> ()).StartsWith ("{")) {
					JObject jObject = JObject.Parse (body);
					string text = TokenToString (jObject ["error"] ?? jObject ["Error"] ?? jObject ["description"] ?? jObject ["Description"]);
					if (!string.IsNullOrEmpty (text)) {
						if (text.IndexOf ("email", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf ("password", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf ("invalid", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf ("unauthorized", StringComparison.OrdinalIgnoreCase) >= 0) {
							LastError = "Invalid E-mail or Password";
						} else {
							LastError = text;
						}
					}
				} else {
					if (body.IndexOf ("<Description>", StringComparison.OrdinalIgnoreCase) < 0) {
						return;
					}
					int num = body.IndexOf ("<Description>", StringComparison.OrdinalIgnoreCase) + 13;
					int num2 = body.IndexOf ("</Description>", StringComparison.OrdinalIgnoreCase);
					if (num2 > num) {
						string text2 = body.Substring (num, num2 - num).Trim ();
						if (IsWrongLauncher (text2)) {
							LastError = "Wrong launcher blocked by server";
						} else if (!string.IsNullOrEmpty (text2)) {
							LastError = text2;
						}
					}
				}
			} catch {
			}
		}

		private static string TryClassic (string baseUrl, string email, string sha1Password)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create (baseUrl + "/User/authenticateUser?email=" + Uri.EscapeDataString (email) + "&password=" + Uri.EscapeDataString (sha1Password));
			httpWebRequest.Method = "GET";
			httpWebRequest.Timeout = 30000;
			ApplySbrwHeaders (httpWebRequest);
			try {
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
				try {
					using StreamReader streamReader = new StreamReader (((WebResponse)httpWebResponse).GetResponseStream ());
					string text = (LastRaw = streamReader.ReadToEnd ());
					Log ("classic status=200 body=" + Trunc (text));
					return text;
				} finally {
					((IDisposable)httpWebResponse)?.Dispose ();
				}
			} catch (WebException ex) {
				if (ex.Response is HttpWebResponse httpWebResponse2) {
					using StreamReader streamReader2 = new StreamReader (((WebResponse)httpWebResponse2).GetResponseStream ());
					string text2 = (LastRaw = streamReader2.ReadToEnd ());
					Log ("classic status=" + (int)httpWebResponse2.StatusCode + " body=" + Trunc (text2));
					TryParseError (text2);
					return text2;
				}
				throw;
			}
		}

		private static string Sha1 (string s)
		{
			SHA1 sHA = SHA1.Create ();
			try {
				byte[] array = sHA.ComputeHash (Encoding.UTF8.GetBytes (s ?? ""));
				StringBuilder stringBuilder = new StringBuilder ();
				byte[] array2 = array;
				foreach (byte b in array2) {
					stringBuilder.Append (b.ToString ("x2"));
				}
				return stringBuilder.ToString ();
			} finally {
				((IDisposable)sHA)?.Dispose ();
			}
		}

		private static string TokenToString (JToken tok)
		{
			if (tok == null || tok.Type == JTokenType.Null) {
				return null;
			}
			string text = tok.ToString ();
			if (string.IsNullOrWhiteSpace (text) || text == "null") {
				return null;
			}
			return text.Trim ();
		}

		private static string XmlEsc (string s)
		{
			return (s ?? "").Replace ("&", "&amp;").Replace ("<", "&lt;").Replace (">", "&gt;");
		}

		private static string Trunc (string s)
		{
			if (string.IsNullOrEmpty (s)) {
				return "";
			}
			s = s.Replace ("\r", " ").Replace ("\n", " ");
			if (s.Length <= 300) {
				return s;
			}
			return s.Substring (0, 300) + "...";
		}

		private static void Log (string msg)
		{
			try {
				File.AppendAllText ("launcher.log", DateTime.Now.ToString ("HH:mm:ss") + " [auth] " + msg + Environment.NewLine);
			} catch {
			}
		}

		private static string Fail (string d)
		{
			LastError = d ?? "Invalid E-mail or Password";
			return "<?xml version=\"1.0\"?><LoginStatusVO><UserId>0</UserId><LoginToken></LoginToken><Description>" + XmlEsc (LastError) + "</Description></LoginStatusVO>";
		}
	}
	public static class ServerMods
	{
		public class GetModInfo
		{
			public string basePath { get; set; }

			public string serverID { get; set; }
		}

		public class ModEntry
		{
			public string Name { get; set; }

			public string Checksum { get; set; }

			public long Size { get; set; }
		}

		public class ModIndex
		{
			public List<ModEntry> entries { get; set; }
		}

		public static string LastStatus = string.Empty;

		public static int LastTotal;

		public static int LastDownloaded;

		public static int LastSkipped;

		public static int LastFailed;

		public static void CleanupBeforeLaunch (string gameDir, Action<string> log)
		{
			EnableTls ();
			RemoveStrayJunk (gameDir, log);
			try {
				Directory.CreateDirectory (Path.Combine (gameDir, "GLOBAL", "EventData"));
				Directory.CreateDirectory (Path.Combine (gameDir, "CARS"));
			} catch {
			}
		}

		public static bool PrepareMods (string serverEngineUrl, string gameDir, Action<string> log)
		{
			if (log == null) {
				log = delegate {
				};
			}
			EnableTls ();
			LastDownloaded = (LastSkipped = (LastFailed = (LastTotal = 0)));
			try {
				if (string.IsNullOrEmpty (gameDir) || !Directory.Exists (gameDir)) {
					LastStatus = "Game folder missing";
					log (LastStatus);
					return false;
				}
				RemoveStrayJunk (gameDir, log);
				try {
					InstallLauncherModules (gameDir, log);
				} catch (Exception ex) {
					log ("ModNet modules: " + ex.Message);
				}
				string text = NormalizeEngineUrl (serverEngineUrl);
				if (string.IsNullOrEmpty (text)) {
					LastStatus = "No server URL for server mods";
					log (LastStatus);
					return true;
				}
				log ("GetModInfo from " + text);
				string text2 = null;
				try {
					text2 = HttpGet (text + "/Modding/GetModInfo");
				} catch (Exception ex2) {
					LastStatus = "No ModNet on server: " + ex2.Message;
					log (LastStatus);
					WriteLog (gameDir, LastStatus);
					return true;
				}
				WriteLog (gameDir, "GetModInfo raw: " + (text2 ?? string.Empty));
				if (string.IsNullOrWhiteSpace (text2) || !text2.TrimStart (Array.Empty<char> ()).StartsWith ("{")) {
					LastStatus = "Empty mod info";
					log (LastStatus);
					return true;
				}
				GetModInfo getModInfo = JsonConvert.DeserializeObject<GetModInfo> (text2);
				if (getModInfo == null || string.IsNullOrEmpty (getModInfo.basePath)) {
					LastStatus = "No mod basePath";
					log (LastStatus);
					return true;
				}
				string text3 = ((!string.IsNullOrEmpty (getModInfo.serverID)) ? getModInfo.serverID : "default");
				string text4 = Md5Hex (text3);
				string text5 = getModInfo.basePath.TrimEnd (new char[1] { '/' });
				string text6 = Path.Combine (gameDir, "MODS");
				string text7 = Path.Combine (text6, text4);
				Directory.CreateDirectory (text6);
				try {
					string[] array = new string[4] {
						text3,
						text3.ToLowerInvariant (),
						text3.ToUpperInvariant (),
						Md5Hex (text3.ToLowerInvariant ())
					};
					foreach (string text8 in array) {
						if (string.IsNullOrEmpty (text8) || string.Equals (text8, text4, StringComparison.Ordinal)) {
							continue;
						}
						string text9 = Path.Combine (text6, text8);
						if (Directory.Exists (text9)) {
							if (!Directory.Exists (text7)) {
								Directory.Move (text9, text7);
								log ("Migrated MODS\\" + text8 + " -> MODS\\" + text4);
							}
							break;
						}
					}
				} catch (Exception ex3) {
					log ("Mod folder migrate: " + ex3.Message);
				}
				Directory.CreateDirectory (text7);
				log ("Mod CDN: " + text5 + " serverID=" + text3 + " folder=MODS\\" + text4 + " (MD5 serverID)");
				WriteLog (gameDir, "Downloading mods into " + text7);
				ModIndex modIndex = null;
				try {
					string value = HttpGet (text5 + "/index.json");
					log ("Got index.json (in-memory, not written to MODS folder)");
					modIndex = JsonConvert.DeserializeObject<ModIndex> (value);
				} catch (Exception ex4) {
					LastStatus = "index.json failed: " + ex4.Message;
					log (LastStatus);
					WriteLog (gameDir, LastStatus);
					return true;
				}
				if (modIndex == null || modIndex.entries == null || modIndex.entries.Count == 0) {
					LastStatus = "No mod entries";
					log (LastStatus);
					return true;
				}
				LastTotal = modIndex.entries.Count;
				int num = 0;
				foreach (ModEntry entry in modIndex.entries) {
					num++;
					if (entry == null || string.IsNullOrEmpty (entry.Name)) {
						continue;
					}
					string fileName = Path.GetFileName (entry.Name.Replace ('/', Path.DirectorySeparatorChar));
					if (string.Equals (fileName, "index.json", StringComparison.OrdinalIgnoreCase) || string.Equals (fileName, "cars.json", StringComparison.OrdinalIgnoreCase) || string.Equals (fileName, "events.json", StringComparison.OrdinalIgnoreCase) || string.Equals (fileName, ".links", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith (".tmp", StringComparison.OrdinalIgnoreCase)) {
						log ("Skip non-mod entry: " + entry.Name);
						continue;
					}
					string text10 = Path.Combine (text7, entry.Name.Replace ('/', Path.DirectorySeparatorChar));
					bool flag = true;
					if (File.Exists (text10)) {
						try {
							if (!string.IsNullOrEmpty (entry.Checksum) && string.Equals (Sha1File (text10), entry.Checksum, StringComparison.OrdinalIgnoreCase)) {
								flag = false;
							} else if (entry.Size > 0 && new FileInfo (text10).Length == entry.Size) {
								flag = false;
							}
						} catch {
							flag = true;
						}
					}
					if (!flag) {
						LastSkipped++;
						if (num <= 8 || num % 25 == 0) {
							log ("OK " + entry.Name);
						}
						continue;
					}
					log ($"Mods [{num}/{LastTotal}] GET {entry.Name}");
					try {
						HttpDownload (text5 + "/" + entry.Name.Replace ("\\", "/"), text10);
						LastDownloaded++;
					} catch (Exception ex5) {
						LastFailed++;
						log ("Fail " + entry.Name + ": " + ex5.Message);
						WriteLog (gameDir, "Fail " + entry.Name + ": " + ex5.Message);
					}
				}
				try {
					string[] array2 = new string[6] { "index.json", "cars.json", "events.json", "cars.json.tmp", "events.json.tmp", ".links" };
					string[] array = array2;
					foreach (string text11 in array) {
						string path = Path.Combine (text7, text11);
						if (File.Exists (path)) {
							try {
								File.Delete (path);
								log ("Removed unexpected " + text11 + " from MODS folder");
							} catch {
							}
						}
					}
					array = array2;
					foreach (string text12 in array) {
						TryDelete (Path.Combine (gameDir, text12), log);
						TryDelete (Path.Combine (gameDir, "Data", text12), log);
						TryDelete (Path.Combine (gameDir, "CARS", text12), log);
					}
				} catch {
				}
				LastStatus = $"Mods: {LastDownloaded} new, {LastSkipped} cached, {LastFailed} failed / {LastTotal} -> {text7}";
				log (LastStatus);
				WriteLog (gameDir, LastStatus);
				return true;
			} catch (Exception ex6) {
				LastStatus = "Mod error: " + ex6.Message;
				log (LastStatus);
				try {
					WriteLog (gameDir, LastStatus + "\n" + ex6);
				} catch {
				}
				return true;
			}
		}

		private static void RemoveStrayJunk (string gameDir, Action<string> log)
		{
			if (string.IsNullOrEmpty (gameDir) || !Directory.Exists (gameDir)) {
				return;
			}
			string[] files;
			try {
				files = Directory.GetFiles (gameDir, "index.json", SearchOption.AllDirectories);
				for (int i = 0; i < files.Length; i++) {
					TryDelete (files [i], log);
				}
				files = Directory.GetFiles (gameDir, "cars.json", SearchOption.AllDirectories);
				for (int j = 0; j < files.Length; j++) {
					TryDelete (files [j], log);
				}
				files = Directory.GetFiles (gameDir, "events.json", SearchOption.AllDirectories);
				for (int k = 0; k < files.Length; k++) {
					TryDelete (files [k], log);
				}
				files = Directory.GetFiles (gameDir, "cars.json.tmp", SearchOption.AllDirectories);
				for (int l = 0; l < files.Length; l++) {
					TryDelete (files [l], log);
				}
				files = Directory.GetFiles (gameDir, "events.json.tmp", SearchOption.AllDirectories);
				for (int m = 0; m < files.Length; m++) {
					TryDelete (files [m], log);
				}
				files = Directory.GetFiles (gameDir, ".links", SearchOption.AllDirectories);
				for (int n = 0; n < files.Length; n++) {
					TryDelete (files [n], log);
				}
			} catch {
			}
			files = new string[6] { "index.json", "cars.json", "cars.json.tmp", "events.json", "events.json.tmp", ".links" };
			string[] array = files;
			foreach (string text in array) {
				TryDelete (Path.Combine (gameDir, text), log);
				TryDelete (Path.Combine (gameDir, "Data", text), log);
				TryDelete (Path.Combine (gameDir, "CARS", text), log);
			}
			try {
				files = Directory.GetFiles (gameDir, "*.tmp", SearchOption.TopDirectoryOnly);
				for (int num2 = 0; num2 < files.Length; num2++) {
					TryDelete (files [num2], log);
				}
			} catch {
			}
		}

		private static string Md5Hex (string s)
		{
			if (s == null) {
				s = "";
			}
			MD5 mD = MD5.Create ();
			try {
				byte[] array = mD.ComputeHash (Encoding.UTF8.GetBytes (s));
				StringBuilder stringBuilder = new StringBuilder (array.Length * 2);
				for (int i = 0; i < array.Length; i++) {
					stringBuilder.Append (array [i].ToString ("x2"));
				}
				return stringBuilder.ToString ();
			} finally {
				((IDisposable)mD)?.Dispose ();
			}
		}

		private static void RestoreVanillaCars (string gameDir, Action<string> log)
		{
		}

		private static void InstallLauncherModules (string gameDir, Action<string> log)
		{
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>> (HttpGet ("https://cdn.soapboxrace.world/launcher-modules/modules.json"));
			if (dictionary == null) {
				return;
			}
			string text = Path.Combine (gameDir, "scripts");
			Directory.CreateDirectory (text);
			foreach (KeyValuePair<string, string> item in dictionary) {
				string text2 = ((!item.Key.Equals ("global.ini", StringComparison.OrdinalIgnoreCase)) ? Path.Combine (gameDir, item.Key) : Path.Combine (text, item.Key));
				bool flag = true;
				if (File.Exists (text2) && !string.IsNullOrEmpty (item.Value)) {
					try {
						if (string.Equals (Sha256File (text2), item.Value, StringComparison.OrdinalIgnoreCase)) {
							flag = false;
						}
					} catch {
						flag = true;
					}
				}
				if (flag) {
					log ("ModNet module: " + item.Key);
					try {
						HttpDownload ("https://cdn.soapboxrace.world/launcher-modules/" + item.Key, text2);
					} catch (Exception ex) {
						log ("Module fail " + item.Key + ": " + ex.Message);
					}
				}
			}
		}

		private static string NormalizeEngineUrl (string url)
		{
			if (string.IsNullOrWhiteSpace (url)) {
				return null;
			}
			string text = url.Trim ();
			if ((text.IndexOf ("127.0.0.1", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf ("localhost", StringComparison.OrdinalIgnoreCase) >= 0) && text.IndexOf ("nightriderz", StringComparison.OrdinalIgnoreCase) >= 0) {
				return "https://horizon.nightriderz.world/Engine.svc";
			}
			if (text.IndexOf ("nightriderz", StringComparison.OrdinalIgnoreCase) >= 0 && text.IndexOf ("Engine.svc", StringComparison.OrdinalIgnoreCase) < 0) {
				text = text.TrimEnd (new char[1] { '/' }) + "/Engine.svc";
			}
			return text.TrimEnd (new char[1] { '/' });
		}

		private static void EnableTls ()
		{
			try {
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				ServicePointManager.Expect100Continue = false;
			} catch {
			}
		}

		private static string HttpGet (string url)
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create (url);
			obj.Method = "GET";
			obj.Timeout = 60000;
			obj.UserAgent = "SBRW Launcher 2.2.4 (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
			obj.Headers ["X-UserAgent"] = "GameLauncherReborn 2.2.4 WinForms (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
			HttpWebResponse httpWebResponse = (HttpWebResponse)obj.GetResponse ();
			try {
				using Stream stream = ((WebResponse)httpWebResponse).GetResponseStream ();
				using StreamReader streamReader = new StreamReader (stream);
				return streamReader.ReadToEnd ();
			} finally {
				((IDisposable)httpWebResponse)?.Dispose ();
			}
		}

		private static void HttpDownload (string url, string dest)
		{
			string directoryName = Path.GetDirectoryName (dest);
			if (!string.IsNullOrEmpty (directoryName)) {
				Directory.CreateDirectory (directoryName);
			}
			string text = dest + ".part";
			try {
				Path.Combine (Path.GetTempPath (), "nfsw_" + Guid.NewGuid ().ToString ("N") + ".part");
				text = dest + "." + Guid.NewGuid ().ToString ("N") + ".part";
			} catch {
			}
			try {
				HttpWebRequest obj2 = (HttpWebRequest)WebRequest.Create (url);
				obj2.Method = "GET";
				obj2.Timeout = 600000;
				obj2.ReadWriteTimeout = 600000;
				obj2.UserAgent = "SBRW Launcher 2.2.4 (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
				obj2.AutomaticDecompression = DecompressionMethods.None;
				HttpWebResponse httpWebResponse = (HttpWebResponse)obj2.GetResponse ();
				try {
					using Stream stream = ((WebResponse)httpWebResponse).GetResponseStream ();
					using FileStream fileStream = new FileStream (text, FileMode.Create, FileAccess.Write, FileShare.None, 81920);
					byte[] array = new byte[81920];
					int count;
					while ((count = stream.Read (array, 0, array.Length)) > 0) {
						fileStream.Write (array, 0, count);
					}
					fileStream.Flush (flushToDisk: true);
				} finally {
					((IDisposable)httpWebResponse)?.Dispose ();
				}
				if (File.Exists (dest)) {
					try {
						File.Delete (dest);
					} catch {
					}
				}
				try {
					File.Move (text, dest);
				} catch {
					File.Copy (text, dest, overwrite: true);
					try {
						File.Delete (text);
					} catch {
					}
				}
			} catch {
				try {
					if (File.Exists (text)) {
						File.Delete (text);
					}
				} catch {
				}
				throw;
			}
		}

		private static void TryDelete (string path, Action<string> log)
		{
			try {
				if (!File.Exists (path)) {
					return;
				}
				try {
					FileAttributes attributes = File.GetAttributes (path);
					if ((attributes & FileAttributes.ReadOnly) != 0) {
						File.SetAttributes (path, attributes & ~FileAttributes.ReadOnly);
					}
				} catch {
				}
				File.Delete (path);
				log?.Invoke ("Removed " + path);
			} catch (Exception ex) {
				try {
					log?.Invoke ("Could not remove " + path + ": " + ex.Message);
				} catch {
				}
			}
		}

		private static void WriteLog (string gameDir, string msg)
		{
			try {
				File.AppendAllText (Path.Combine (gameDir, "modsync.log"), DateTime.Now.ToString ("HH:mm:ss") + " " + msg + Environment.NewLine);
			} catch {
			}
			try {
				File.AppendAllText ("launcher.log", "MODS " + msg + Environment.NewLine);
			} catch {
			}
		}

		private static string Md5 (string input)
		{
			byte[] array = MD5.Create ().ComputeHash (Encoding.UTF8.GetBytes (input ?? string.Empty));
			StringBuilder stringBuilder = new StringBuilder ();
			byte[] array2 = array;
			foreach (byte b in array2) {
				stringBuilder.Append (b.ToString ("x2"));
			}
			return stringBuilder.ToString ();
		}

		private static string Sha1File (string path)
		{
			FileStream fileStream = File.OpenRead (path);
			byte[] array = ((HashAlgorithm)SHA1.Create ()).ComputeHash ((Stream)fileStream);
			StringBuilder stringBuilder = new StringBuilder ();
			byte[] array2 = array;
			foreach (byte b in array2) {
				stringBuilder.Append (b.ToString ("x2"));
			}
			return stringBuilder.ToString ();
		}

		private static string Sha256File (string path)
		{
			FileStream fileStream = File.OpenRead (path);
			byte[] array = ((HashAlgorithm)SHA256.Create ()).ComputeHash ((Stream)fileStream);
			StringBuilder stringBuilder = new StringBuilder ();
			byte[] array2 = array;
			foreach (byte b in array2) {
				stringBuilder.Append (b.ToString ("x2"));
			}
			return stringBuilder.ToString ();
		}

		private static void LinkModFolders (string gameDir, Action<string> log)
		{
		}

		private static void MakeJunction (string sourceDir, string destDir, Action<string> log)
		{
			try {
				if (!Directory.Exists (sourceDir)) {
					return;
				}
				string directoryName = Path.GetDirectoryName (destDir);
				if (!string.IsNullOrEmpty (directoryName)) {
					Directory.CreateDirectory (directoryName);
				}
				if (Directory.Exists (destDir)) {
					if ((File.GetAttributes (destDir) & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint) {
						return;
					}
					string text = destDir + ".orig";
					if (Directory.Exists (text)) {
						return;
					}
					Directory.Move (destDir, text);
				}
				ProcessStartInfo processStartInfo = new ProcessStartInfo ();
				processStartInfo.FileName = "cmd.exe";
				processStartInfo.Arguments = "/c mklink /J \"" + destDir + "\" \"" + sourceDir + "\"";
				processStartInfo.CreateNoWindow = true;
				processStartInfo.UseShellExecute = false;
				processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				Process.Start (processStartInfo)?.WaitForExit (8000);
				log?.Invoke ("Junction " + destDir);
			} catch (Exception ex) {
				log?.Invoke ("Junction fail " + destDir + ": " + ex.Message);
			}
		}
	}
	internal static class User32
	{
		[DllImport ("user32.dll")]
		public static extern IntPtr LoadCursorFromFile (string filename);

		[DllImport ("user32.dll")]
		public static extern bool SetProcessDPIAware ();
	}
}
namespace GameLauncher.App.Classes.Proxy
{
	public class NancyGzipCompression : IApplicationStartup
	{
		private static readonly IList<string> MimeTypes = new List<string> { "text/plain", "text/html", "text/xml", "text/css", "application/json", "application/x-javascript", "application/atom+xml", "application/xml;charset=UTF-8", "application/xml" };

		public void Initialize (IPipelines pipelines)
		{
			pipelines.AfterRequest += new Action<NancyContext> (CheckForCompression);
		}

		private static void CheckForCompression (NancyContext Context)
		{
			try {
				if (Context != null && Context.Response != null && RequestIsGzipCompatible (Context) && !ResponseIsCompressed (Context) && ResponseIsCompatibleMimeType (Context) && !ContentLengthIsTooSmall (Context)) {
					CompressResponse (Context);
				}
			} catch (Exception ex) {
				try {
					SbrwProxy.Log ("Gzip: " + ex.Message);
				} catch {
				}
			}
		}

		private static void CompressResponse (NancyContext Context)
		{
			bool deflate = false;
			try {
				if (Context.Request.Headers.AcceptEncoding != null) {
					deflate = Enumerable.Any<string> (Context.Request.Headers.AcceptEncoding, (Func<string, bool>)((string x) => x?.Contains ("deflate") ?? false));
				}
			} catch {
			}
			Context.Response.Headers ["Content-Encoding"] = (deflate ? "deflate" : "gzip");
			try {
				Context.Response.Headers ["Connection"] = "close";
			} catch {
			}
			Action<Stream> finalResponse = Context.Response.Contents;
			Context.Response.Contents = delegate(Stream responseStream) {
				if (deflate) {
					DeflateStream deflateStream = new DeflateStream (responseStream, CompressionLevel.Optimal, true);
					try {
						finalResponse ((Stream)(object)deflateStream);
						((Stream)(object)deflateStream).Flush ();
						return;
					} finally {
						((IDisposable)deflateStream)?.Dispose ();
					}
				}
				GZipStream gZipStream = new GZipStream (responseStream, CompressionLevel.Optimal, true);
				try {
					finalResponse ((Stream)(object)gZipStream);
					((Stream)(object)gZipStream).Flush ();
				} finally {
					((IDisposable)gZipStream)?.Dispose ();
				}
			};
		}

		private static bool ContentLengthIsTooSmall (NancyContext Context)
		{
			try {
				if (Context.Response.Headers.TryGetValue ("Content-Length", out var value)) {
					if (long.TryParse (value, out var result)) {
						return result <= 0;
					}
					return false;
				}
				return false;
			} catch {
			}
			return false;
		}

		private static bool ResponseIsCompressed (NancyContext Context)
		{
			try {
				return Context.Response.Headers != null && Enumerable.Any<string> ((IEnumerable<string>)Context.Response.Headers.Keys, (Func<string, bool>)((string x) => x != null && x.IndexOf ("Content-Encoding", StringComparison.OrdinalIgnoreCase) >= 0));
			} catch {
				return false;
			}
		}

		private static bool ResponseIsCompatibleMimeType (NancyContext Context)
		{
			try {
				string ct = Context.Response.ContentType ?? "";
				return Enumerable.Any<string> ((IEnumerable<string>)MimeTypes, (Func<string, bool>)((string m) => ct.IndexOf (m, StringComparison.OrdinalIgnoreCase) >= 0));
			} catch {
				return false;
			}
		}

		private static bool RequestIsGzipCompatible (NancyContext Context)
		{
			try {
				if (Context.Request.Headers.AcceptEncoding == null) {
					return false;
				}
				return Enumerable.Any<string> (Context.Request.Headers.AcceptEncoding, (Func<string, bool>)((string x) => x != null && (x.Contains ("gzip") || x.Contains ("deflate"))));
			} catch {
				return false;
			}
		}
	}
	public class ProxyBootstrapper : DefaultNancyBootstrapper
	{
		public override void Configure (INancyEnvironment environment)
		{
			base.Configure (environment);
			environment.Tracing (enabled: false, displayErrorTraces: false);
		}

		protected override void ApplicationStartup (TinyIoCContainer container, IPipelines pipelines)
		{
			base.ApplicationStartup (container, pipelines);
			new NancyGzipCompression ().Initialize (pipelines);
			SbrwProxy.Log ("Gzip compression pipeline enabled (official SBRW)");
		}
	}
	public class ProxyModule : NancyModule
	{
		[CompilerGenerated]
		private static class _003C_003Eo__1
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__8
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, Func<CallSite, object, Response>, CallSite<Func<CallSite, object, Response>>, object, object>> _003C_003Ep__1;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__2;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__9
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__10
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__11
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__12
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__13
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__14
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__15
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__16
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__17
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__18
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__19
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__20
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__21
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__22
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__23
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__24
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__25
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__26
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__27
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__28
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__29
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__30
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__31
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__32
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__33
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__34
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__35
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__36
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__37
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__38
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__39
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[CompilerGenerated]
		private static class _003C_003Eo__40
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
		}

		[StructLayout (LayoutKind.Auto)]
		[CompilerGenerated]
		private struct <<-ctor>b__40_0>d : IAsyncStateMachine
		{
			private static class <>o
			{
				public static CallSite<Func<CallSite, object, object>> <>p__0;

				public static CallSite<Func<CallSite, object, object>> <>p__1;

				public static CallSite<Func<CallSite, object, bool>> <>p__2;

				public static CallSite<Func<CallSite, object, object>> <>p__3;
			}

			public int <>1__state;

			public AsyncTaskMethodBuilder<object> <>t__builder;

			public ProxyModule <>4__this;

			public object a;

			public CancellationToken ct;

			private object <>u__1;

			private void MoveNext ()
			{
				int num = <>1__state;
				ProxyModule proxyModule = <>4__this;
				object result;
				try {
					dynamic awaiter;
					if (num != 0) {
						awaiter = proxyModule.Forward ("GET", (dynamic)a, ct).GetAwaiter ();
						if (!(bool)awaiter.IsCompleted) {
							num = (<>1__state = 0);
							<>u__1 = awaiter;
							ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
							if (awaiter2 == null) {
								INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
								<>t__builder.AwaitOnCompleted (ref awaiter3, ref this);
								awaiter3 = null;
							} else {
								<>t__builder.AwaitUnsafeOnCompleted (ref awaiter2, ref this);
							}
							awaiter2 = null;
							return;
						}
					} else {
						awaiter = <>u__1;
						<>u__1 = null;
						num = (<>1__state = -1);
					}
					result = awaiter.GetResult ();
				} catch (Exception exception) {
					<>1__state = -2;
					<>t__builder.SetException (exception);
					return;
				}
				<>1__state = -2;
				<>t__builder.SetResult (result);
			}

			void IAsyncStateMachine.MoveNext ()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext ();
			}

			[DebuggerHidden]
			private void SetStateMachine (IAsyncStateMachine stateMachine)
			{
				<>t__builder.SetStateMachine (stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine (IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine (stateMachine);
			}
		}

		[StructLayout (LayoutKind.Auto)]
		[CompilerGenerated]
		private struct <<-ctor>b__40_1>d : IAsyncStateMachine
		{
			private static class <>o
			{
				public static CallSite<Func<CallSite, object, object>> <>p__0;

				public static CallSite<Func<CallSite, object, object>> <>p__1;

				public static CallSite<Func<CallSite, object, bool>> <>p__2;

				public static CallSite<Func<CallSite, object, object>> <>p__3;
			}

			public int <>1__state;

			public AsyncTaskMethodBuilder<object> <>t__builder;

			public ProxyModule <>4__this;

			public object a;

			public CancellationToken ct;

			private object <>u__1;

			private void MoveNext ()
			{
				int num = <>1__state;
				ProxyModule proxyModule = <>4__this;
				object result;
				try {
					dynamic awaiter;
					if (num != 0) {
						awaiter = proxyModule.Forward ("POST", (dynamic)a, ct).GetAwaiter ();
						if (!(bool)awaiter.IsCompleted) {
							num = (<>1__state = 0);
							<>u__1 = awaiter;
							ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
							if (awaiter2 == null) {
								INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
								<>t__builder.AwaitOnCompleted (ref awaiter3, ref this);
								awaiter3 = null;
							} else {
								<>t__builder.AwaitUnsafeOnCompleted (ref awaiter2, ref this);
							}
							awaiter2 = null;
							return;
						}
					} else {
						awaiter = <>u__1;
						<>u__1 = null;
						num = (<>1__state = -1);
					}
					result = awaiter.GetResult ();
				} catch (Exception exception) {
					<>1__state = -2;
					<>t__builder.SetException (exception);
					return;
				}
				<>1__state = -2;
				<>t__builder.SetResult (result);
			}

			void IAsyncStateMachine.MoveNext ()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext ();
			}

			[DebuggerHidden]
			private void SetStateMachine (IAsyncStateMachine stateMachine)
			{
				<>t__builder.SetStateMachine (stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine (IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine (stateMachine);
			}
		}

		[StructLayout (LayoutKind.Auto)]
		[CompilerGenerated]
		private struct <<-ctor>b__40_2>d : IAsyncStateMachine
		{
			private static class <>o
			{
				public static CallSite<Func<CallSite, object, object>> <>p__0;

				public static CallSite<Func<CallSite, object, object>> <>p__1;

				public static CallSite<Func<CallSite, object, bool>> <>p__2;

				public static CallSite<Func<CallSite, object, object>> <>p__3;
			}

			public int <>1__state;

			public AsyncTaskMethodBuilder<object> <>t__builder;

			public ProxyModule <>4__this;

			public object a;

			public CancellationToken ct;

			private object <>u__1;

			private void MoveNext ()
			{
				int num = <>1__state;
				ProxyModule proxyModule = <>4__this;
				object result;
				try {
					dynamic awaiter;
					if (num != 0) {
						awaiter = proxyModule.Forward ("PUT", (dynamic)a, ct).GetAwaiter ();
						if (!(bool)awaiter.IsCompleted) {
							num = (<>1__state = 0);
							<>u__1 = awaiter;
							ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
							if (awaiter2 == null) {
								INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
								<>t__builder.AwaitOnCompleted (ref awaiter3, ref this);
								awaiter3 = null;
							} else {
								<>t__builder.AwaitUnsafeOnCompleted (ref awaiter2, ref this);
							}
							awaiter2 = null;
							return;
						}
					} else {
						awaiter = <>u__1;
						<>u__1 = null;
						num = (<>1__state = -1);
					}
					result = awaiter.GetResult ();
				} catch (Exception exception) {
					<>1__state = -2;
					<>t__builder.SetException (exception);
					return;
				}
				<>1__state = -2;
				<>t__builder.SetResult (result);
			}

			void IAsyncStateMachine.MoveNext ()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext ();
			}

			[DebuggerHidden]
			private void SetStateMachine (IAsyncStateMachine stateMachine)
			{
				<>t__builder.SetStateMachine (stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine (IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine (stateMachine);
			}
		}

		[StructLayout (LayoutKind.Auto)]
		[CompilerGenerated]
		private struct <<-ctor>b__40_3>d : IAsyncStateMachine
		{
			private static class <>o
			{
				public static CallSite<Func<CallSite, object, object>> <>p__0;

				public static CallSite<Func<CallSite, object, object>> <>p__1;

				public static CallSite<Func<CallSite, object, bool>> <>p__2;

				public static CallSite<Func<CallSite, object, object>> <>p__3;
			}

			public int <>1__state;

			public AsyncTaskMethodBuilder<object> <>t__builder;

			public ProxyModule <>4__this;

			public object a;

			public CancellationToken ct;

			private object <>u__1;

			private void MoveNext ()
			{
				int num = <>1__state;
				ProxyModule proxyModule = <>4__this;
				object result;
				try {
					dynamic awaiter;
					if (num != 0) {
						awaiter = proxyModule.Forward ("DELETE", (dynamic)a, ct).GetAwaiter ();
						if (!(bool)awaiter.IsCompleted) {
							num = (<>1__state = 0);
							<>u__1 = awaiter;
							ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
							if (awaiter2 == null) {
								INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
								<>t__builder.AwaitOnCompleted (ref awaiter3, ref this);
								awaiter3 = null;
							} else {
								<>t__builder.AwaitUnsafeOnCompleted (ref awaiter2, ref this);
							}
							awaiter2 = null;
							return;
						}
					} else {
						awaiter = <>u__1;
						<>u__1 = null;
						num = (<>1__state = -1);
					}
					result = awaiter.GetResult ();
				} catch (Exception exception) {
					<>1__state = -2;
					<>t__builder.SetException (exception);
					return;
				}
				<>1__state = -2;
				<>t__builder.SetResult (result);
			}

			void IAsyncStateMachine.MoveNext ()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext ();
			}

			[DebuggerHidden]
			private void SetStateMachine (IAsyncStateMachine stateMachine)
			{
				<>t__builder.SetStateMachine (stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine (IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine (stateMachine);
			}
		}

		[StructLayout (LayoutKind.Auto)]
		[CompilerGenerated]
		private struct <<-ctor>b__40_4>d : IAsyncStateMachine
		{
			private static class <>o
			{
				public static CallSite<Func<CallSite, object, object>> <>p__0;

				public static CallSite<Func<CallSite, object, object>> <>p__1;

				public static CallSite<Func<CallSite, object, bool>> <>p__2;

				public static CallSite<Func<CallSite, object, object>> <>p__3;
			}

			public int <>1__state;

			public AsyncTaskMethodBuilder<object> <>t__builder;

			public ProxyModule <>4__this;

			public object a;

			public CancellationToken ct;

			private object <>u__1;

			private void MoveNext ()
			{
				int num = <>1__state;
				ProxyModule proxyModule = <>4__this;
				object result;
				try {
					dynamic awaiter;
					if (num != 0) {
						awaiter = proxyModule.Forward ("GET", (dynamic)a, ct).GetAwaiter ();
						if (!(bool)awaiter.IsCompleted) {
							num = (<>1__state = 0);
							<>u__1 = awaiter;
							ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
							if (awaiter2 == null) {
								INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
								<>t__builder.AwaitOnCompleted (ref awaiter3, ref this);
								awaiter3 = null;
							} else {
								<>t__builder.AwaitUnsafeOnCompleted (ref awaiter2, ref this);
							}
							awaiter2 = null;
							return;
						}
					} else {
						awaiter = <>u__1;
						<>u__1 = null;
						num = (<>1__state = -1);
					}
					result = awaiter.GetResult ();
				} catch (Exception exception) {
					<>1__state = -2;
					<>t__builder.SetException (exception);
					return;
				}
				<>1__state = -2;
				<>t__builder.SetResult (result);
			}

			void IAsyncStateMachine.MoveNext ()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext ();
			}

			[DebuggerHidden]
			private void SetStateMachine (IAsyncStateMachine stateMachine)
			{
				<>t__builder.SetStateMachine (stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine (IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine (stateMachine);
			}
		}

		[StructLayout (LayoutKind.Auto)]
		[CompilerGenerated]
		private struct <<-ctor>b__40_5>d : IAsyncStateMachine
		{
			private static class <>o
			{
				public static CallSite<Func<CallSite, object, object>> <>p__0;

				public static CallSite<Func<CallSite, object, object>> <>p__1;

				public static CallSite<Func<CallSite, object, bool>> <>p__2;

				public static CallSite<Func<CallSite, object, object>> <>p__3;
			}

			public int <>1__state;

			public AsyncTaskMethodBuilder<object> <>t__builder;

			public ProxyModule <>4__this;

			public object a;

			public CancellationToken ct;

			private object <>u__1;

			private void MoveNext ()
			{
				int num = <>1__state;
				ProxyModule proxyModule = <>4__this;
				object result;
				try {
					dynamic awaiter;
					if (num != 0) {
						awaiter = proxyModule.Forward ("POST", (dynamic)a, ct).GetAwaiter ();
						if (!(bool)awaiter.IsCompleted) {
							num = (<>1__state = 0);
							<>u__1 = awaiter;
							ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
							if (awaiter2 == null) {
								INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
								<>t__builder.AwaitOnCompleted (ref awaiter3, ref this);
								awaiter3 = null;
							} else {
								<>t__builder.AwaitUnsafeOnCompleted (ref awaiter2, ref this);
							}
							awaiter2 = null;
							return;
						}
					} else {
						awaiter = <>u__1;
						<>u__1 = null;
						num = (<>1__state = -1);
					}
					result = awaiter.GetResult ();
				} catch (Exception exception) {
					<>1__state = -2;
					<>t__builder.SetException (exception);
					return;
				}
				<>1__state = -2;
				<>t__builder.SetResult (result);
			}

			void IAsyncStateMachine.MoveNext ()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext ();
			}

			[DebuggerHidden]
			private void SetStateMachine (IAsyncStateMachine stateMachine)
			{
				<>t__builder.SetStateMachine (stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine (IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine (stateMachine);
			}
		}

		[CompilerGenerated]
		private static class <>o__41
		{
			public static CallSite<Func<CallSite, ProxyModule, string, object, object>> <>p__0;

			public static CallSite<Func<CallSite, object, Response>> <>p__1;
		}

		public unsafe ProxyModule ()
		{
			Get ("/nfsw/Engine.svc/{path*}", async delegate(dynamic a, CancellationToken ct) {
				dynamic awaiter16 = Forward ("GET", a, ct).GetAwaiter ();
				if (!(bool)awaiter16.IsCompleted) {
					ICriticalNotifyCompletion awaiter17 = awaiter16 as ICriticalNotifyCompletion;
					AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder6 = default(AsyncTaskMethodBuilder<object>);
					if (awaiter17 == null) {
						INotifyCompletion awaiter18 = (INotifyCompletion)(object)awaiter16;
						asyncTaskMethodBuilder6.AwaitOnCompleted (ref awaiter18, ref *(<<-ctor>b__40_0>d*)/*Error near IL_0194: stateMachine*/);
					} else {
						asyncTaskMethodBuilder6.AwaitUnsafeOnCompleted (ref awaiter17, ref *(<<-ctor>b__40_0>d*)/*Error near IL_01a7: stateMachine*/);
					}
					/*Error near IL_01b0: leave MoveNext - await not detected correctly*/;
				}
				return awaiter16.GetResult ();
			});
			Post ("/nfsw/Engine.svc/{path*}", async delegate(dynamic a, CancellationToken ct) {
				dynamic awaiter13 = Forward ("POST", a, ct).GetAwaiter ();
				if (!(bool)awaiter13.IsCompleted) {
					ICriticalNotifyCompletion awaiter14 = awaiter13 as ICriticalNotifyCompletion;
					AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder5 = default(AsyncTaskMethodBuilder<object>);
					if (awaiter14 == null) {
						INotifyCompletion awaiter15 = (INotifyCompletion)(object)awaiter13;
						asyncTaskMethodBuilder5.AwaitOnCompleted (ref awaiter15, ref *(<<-ctor>b__40_1>d*)/*Error near IL_0194: stateMachine*/);
					} else {
						asyncTaskMethodBuilder5.AwaitUnsafeOnCompleted (ref awaiter14, ref *(<<-ctor>b__40_1>d*)/*Error near IL_01a7: stateMachine*/);
					}
					/*Error near IL_01b0: leave MoveNext - await not detected correctly*/;
				}
				return awaiter13.GetResult ();
			});
			Put ("/nfsw/Engine.svc/{path*}", async delegate(dynamic a, CancellationToken ct) {
				dynamic awaiter10 = Forward ("PUT", a, ct).GetAwaiter ();
				if (!(bool)awaiter10.IsCompleted) {
					ICriticalNotifyCompletion awaiter11 = awaiter10 as ICriticalNotifyCompletion;
					AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder4 = default(AsyncTaskMethodBuilder<object>);
					if (awaiter11 == null) {
						INotifyCompletion awaiter12 = (INotifyCompletion)(object)awaiter10;
						asyncTaskMethodBuilder4.AwaitOnCompleted (ref awaiter12, ref *(<<-ctor>b__40_2>d*)/*Error near IL_0194: stateMachine*/);
					} else {
						asyncTaskMethodBuilder4.AwaitUnsafeOnCompleted (ref awaiter11, ref *(<<-ctor>b__40_2>d*)/*Error near IL_01a7: stateMachine*/);
					}
					/*Error near IL_01b0: leave MoveNext - await not detected correctly*/;
				}
				return awaiter10.GetResult ();
			});
			Delete ("/nfsw/Engine.svc/{path*}", async delegate(dynamic a, CancellationToken ct) {
				dynamic awaiter7 = Forward ("DELETE", a, ct).GetAwaiter ();
				if (!(bool)awaiter7.IsCompleted) {
					ICriticalNotifyCompletion awaiter8 = awaiter7 as ICriticalNotifyCompletion;
					AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder3 = default(AsyncTaskMethodBuilder<object>);
					if (awaiter8 == null) {
						INotifyCompletion awaiter9 = (INotifyCompletion)(object)awaiter7;
						asyncTaskMethodBuilder3.AwaitOnCompleted (ref awaiter9, ref *(<<-ctor>b__40_3>d*)/*Error near IL_0194: stateMachine*/);
					} else {
						asyncTaskMethodBuilder3.AwaitUnsafeOnCompleted (ref awaiter8, ref *(<<-ctor>b__40_3>d*)/*Error near IL_01a7: stateMachine*/);
					}
					/*Error near IL_01b0: leave MoveNext - await not detected correctly*/;
				}
				return awaiter7.GetResult ();
			});
			Get ("/nfsw/Engine.svc", async delegate(dynamic a, CancellationToken ct) {
				dynamic awaiter4 = Forward ("GET", a, ct).GetAwaiter ();
				if (!(bool)awaiter4.IsCompleted) {
					ICriticalNotifyCompletion awaiter5 = awaiter4 as ICriticalNotifyCompletion;
					AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder2 = default(AsyncTaskMethodBuilder<object>);
					if (awaiter5 == null) {
						INotifyCompletion awaiter6 = (INotifyCompletion)(object)awaiter4;
						asyncTaskMethodBuilder2.AwaitOnCompleted (ref awaiter6, ref *(<<-ctor>b__40_4>d*)/*Error near IL_0194: stateMachine*/);
					} else {
						asyncTaskMethodBuilder2.AwaitUnsafeOnCompleted (ref awaiter5, ref *(<<-ctor>b__40_4>d*)/*Error near IL_01a7: stateMachine*/);
					}
					/*Error near IL_01b0: leave MoveNext - await not detected correctly*/;
				}
				return awaiter4.GetResult ();
			});
			Post ("/nfsw/Engine.svc", async delegate(dynamic a, CancellationToken ct) {
				dynamic awaiter = Forward ("POST", a, ct).GetAwaiter ();
				if (!(bool)awaiter.IsCompleted) {
					ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
					AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder = default(AsyncTaskMethodBuilder<object>);
					if (awaiter2 == null) {
						INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
						asyncTaskMethodBuilder.AwaitOnCompleted (ref awaiter3, ref *(<<-ctor>b__40_5>d*)/*Error near IL_0194: stateMachine*/);
					} else {
						asyncTaskMethodBuilder.AwaitUnsafeOnCompleted (ref awaiter2, ref *(<<-ctor>b__40_5>d*)/*Error near IL_01a7: stateMachine*/);
					}
					/*Error near IL_01b0: leave MoveNext - await not detected correctly*/;
				}
				return awaiter.GetResult ();
			});
		}

		private async Task<Response> Forward (string method, dynamic args, CancellationToken ct)
		{
			if (<>o__41.<>p__1 == null) {
				<>o__41.<>p__1 = CallSite<Func<CallSite, object, Nancy.Response>>.Create (Microsoft.CSharp.RuntimeBinder.Binder.Convert (CSharpBinderFlags.None, typeof(Response), typeof(ProxyModule)));
			}
			Func<CallSite, object, Response> target = <>o__41.<>p__1.Target;
			CallSite<Func<CallSite, object, Response>> <>p__ = <>o__41.<>p__1;
			return target (<>p__, await Task.Run (() => ForwardSync (method, args), ct));
		}

		private Response ForwardSync (string method, dynamic args)
		{
			try {
				string text = "";
				try {
					text = (string)args.path;
				} catch {
				}
				if (text == null) {
					text = "";
				}
				string text2 = SbrwProxy.ServerUrl.TrimEnd (new char[1] { '/' });
				if (!string.IsNullOrEmpty (text)) {
					if (!text.StartsWith ("/")) {
						text = "/" + text;
					}
					text2 += text;
				}
				string query = Request.Url.Query;
				if (!string.IsNullOrEmpty (query)) {
					text2 += (query.StartsWith ("?") ? query : ("?" + query));
				}
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create (text2);
				httpWebRequest.Method = method;
				httpWebRequest.Timeout = 180000;
				httpWebRequest.ReadWriteTimeout = 180000;
				httpWebRequest.AutomaticDecompression = DecompressionMethods.None;
				httpWebRequest.KeepAlive = false;
				httpWebRequest.AllowAutoRedirect = false;
				httpWebRequest.ServicePoint.Expect100Continue = false;
				httpWebRequest.ProtocolVersion = HttpVersion.Version11;
				string host = new Uri (SbrwProxy.ServerUrl).Host;
				try {
					httpWebRequest.Host = host;
				} catch {
				}
				foreach (KeyValuePair<string, IEnumerable<string>> header in Request.Headers) {
					string key = header.Key;
					if (string.IsNullOrEmpty (key)) {
						continue;
					}
					string text3 = key.ToLowerInvariant ();
					switch (text3) {
					case "host":
					case "connection":
					case "content-length":
					case "transfer-encoding":
						continue;
					}
					string text4 = ((header.Value != null && Enumerable.Any<string> (header.Value)) ? Enumerable.First<string> (header.Value) : "");
					switch (text3) {
					case "content-type":
						httpWebRequest.ContentType = text4;
						continue;
					case "user-agent":
						httpWebRequest.UserAgent = text4;
						continue;
					case "accept-encoding":
						httpWebRequest.Headers ["Accept-Encoding"] = "gzip, deflate";
						continue;
					}
					try {
						httpWebRequest.Headers [key] = text4;
					} catch {
					}
				}
				if (string.IsNullOrEmpty (httpWebRequest.UserAgent)) {
					httpWebRequest.UserAgent = "Blackbox";
				}
				if (httpWebRequest.Headers ["Accept-Encoding"] == null) {
					httpWebRequest.Headers ["Accept-Encoding"] = "gzip, deflate";
				}
				if (method != "GET" && method != "HEAD") {
					byte[] array = null;
					try {
						using MemoryStream memoryStream = new MemoryStream ();
						Request.Body.CopyTo (memoryStream);
						array = memoryStream.ToArray ();
					} catch {
						array = Encoding.UTF8.GetBytes (Request.Body.AsString (Encoding.UTF8) ?? "");
					}
					if (array != null && array.Length != 0) {
						httpWebRequest.ContentLength = array.Length;
						using Stream stream4 = ((WebRequest)httpWebRequest).GetRequestStream ();
						stream4.Write (array, 0, array.Length);
					} else {
						httpWebRequest.ContentLength = 0L;
					}
				}
				SbrwProxy.Log (method + " /" + text + " -> " + text2);
				HttpWebResponse httpWebResponse = null;
				try {
					httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
				} catch (WebException ex6) {
					WebException ex7 = ex6;
					WebException wex = ex7;
					if (wex.Response == null) {
						SbrwProxy.Log ("ERR no response: " + ((Exception)(object)wex).Message);
						return new Response {
							StatusCode = Nancy.HttpStatusCode.BadGateway,
							ContentType = "text/plain",
							Contents = delegate(Stream s) {
								byte[] bytes3 = Encoding.UTF8.GetBytes ("proxy error: " + ((Exception)(object)wex).Message);
								s.Write (bytes3, 0, bytes3.Length);
							}
						};
					}
					httpWebResponse = (HttpWebResponse)wex.Response;
				}
				HttpWebResponse httpWebResponse2 = httpWebResponse;
				try {
					int statusCode = (int)httpWebResponse.StatusCode;
					string contentType = httpWebResponse.ContentType ?? "application/xml;charset=UTF-8";
					string text5 = httpWebResponse.Headers ["Content-Encoding"] ?? "";
					byte[] bytes;
					using (Stream stream5 = ((WebResponse)httpWebResponse).GetResponseStream ()) {
						using MemoryStream memoryStream2 = new MemoryStream ();
						stream5?.CopyTo (memoryStream2);
						bytes = memoryStream2.ToArray ();
					}
					SbrwProxy.Log ("OK " + statusCode + " /" + text + " len=" + bytes.Length + " enc=" + text5);
					Response response = new Response {
						StatusCode = (Nancy.HttpStatusCode)statusCode,
						ContentType = contentType,
						Contents = delegate(Stream stream3) {
							if (bytes != null && bytes.Length != 0) {
								stream3.Write (bytes, 0, bytes.Length);
							}
						}
					};
					response.Headers ["Content-Length"] = bytes.Length.ToString ();
					if (!string.IsNullOrEmpty (text5)) {
						response.Headers ["Content-Encoding"] = text5;
					}
					try {
						response.Headers ["Connection"] = "close";
					} catch {
					}
					return response;
				} finally {
					((IDisposable)httpWebResponse2)?.Dispose ();
				}
			} catch (Exception ex8) {
				Exception ex9 = ex8;
				Exception ex5 = ex9;
				SbrwProxy.Log ("ERR: " + ex5.Message);
				return new Response {
					StatusCode = Nancy.HttpStatusCode.BadGateway,
					ContentType = "text/plain",
					Contents = delegate(Stream s) {
						byte[] bytes2 = Encoding.UTF8.GetBytes ("proxy error: " + ex5.Message);
						s.Write (bytes2, 0, bytes2.Length);
					}
				};
			}
		}
	}
	public static class SbrwProxy
	{
		private static NancyHost _host;

		public static string LastError = "";

		public static int ProxyPort { get; private set; }

		public static string ServerUrl { get; private set; }

		public static string GameUrl => "http://127.0.0.1:" + ProxyPort + "/nfsw/Engine.svc";

		public static bool Running => _host != null;

		public static bool Start (string realEngineSvcUrl, string serverName = "SBRW")
		{
			Stop ();
			LastError = "";
			ServerUrl = (realEngineSvcUrl ?? "").TrimEnd (new char[1] { '/' });
			if (string.IsNullOrEmpty (ServerUrl)) {
				LastError = "Empty server URL";
				return false;
			}
			Random random = new Random ();
			for (int i = 0; i < 15; i++) {
				ProxyPort = random.Next (2017, 2217);
				try {
					HostConfiguration configuration = new HostConfiguration {
						AllowChunkedEncoding = false,
						UrlReservations = new UrlReservations {
							CreateAutomatically = true
						}
					};
					_host = new NancyHost (new Uri ("http://127.0.0.1:" + ProxyPort), new ProxyBootstrapper (), configuration);
					_host.Start ();
					Log ("Nancy proxy on " + GameUrl + " -> " + ServerUrl);
					return true;
				} catch (Exception ex) {
					LastError = ex.Message;
					try {
						if (_host != null) {
							_host.Stop ();
							_host.Dispose ();
						}
					} catch {
					}
					_host = null;
				}
			}
			return false;
		}

		public static void Stop ()
		{
			try {
				if (_host != null) {
					_host.Stop ();
					_host.Dispose ();
				}
			} catch {
			}
			_host = null;
		}

		public static void Log (string msg)
		{
			try {
				File.AppendAllText ("launcher.log", "[" + DateTime.Now.ToString ("HH:mm:ss") + "] [proxy] " + msg + Environment.NewLine);
			} catch {
			}
		}
	}
}
namespace GameLauncher.App.Classes.Events
{
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
}
