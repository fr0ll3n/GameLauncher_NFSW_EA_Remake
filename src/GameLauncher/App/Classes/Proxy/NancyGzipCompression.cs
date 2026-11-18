using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Nancy;
using Nancy.Bootstrapper;

namespace GameLauncher.App.Classes.Proxy;

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
