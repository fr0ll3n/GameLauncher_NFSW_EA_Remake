using System;
using System.IO;
using System.Reflection;

namespace GameLauncher.App.Classes;

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
