using System;
using System.Net;

namespace GameLauncherReborn;

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
