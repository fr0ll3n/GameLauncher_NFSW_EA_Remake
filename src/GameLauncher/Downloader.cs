using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using GameLauncher.App.Classes;

namespace GameLauncher;

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
