namespace GameLauncher;

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
