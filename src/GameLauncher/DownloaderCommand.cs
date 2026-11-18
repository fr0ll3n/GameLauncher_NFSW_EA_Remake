using System.Runtime.CompilerServices;

namespace GameLauncher;

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
