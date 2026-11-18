using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml;

namespace GameLauncher;

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
