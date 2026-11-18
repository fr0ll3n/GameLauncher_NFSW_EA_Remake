using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Cache;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;

namespace GameLauncher;

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
