using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameLauncher.App.Classes;

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
