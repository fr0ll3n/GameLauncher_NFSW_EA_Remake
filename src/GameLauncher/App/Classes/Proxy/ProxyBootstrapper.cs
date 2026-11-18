using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.TinyIoc;

namespace GameLauncher.App.Classes.Proxy;

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
