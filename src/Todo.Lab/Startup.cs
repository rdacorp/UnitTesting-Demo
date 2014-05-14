using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Todo.Services;

[assembly: OwinStartup(typeof(Todo.Startup))]

namespace Todo
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			JsonConvert.DefaultSettings = () => {
				var settings = new JsonSerializerSettings {
					DateFormatHandling = DateFormatHandling.IsoDateFormat,
					DateParseHandling = DateParseHandling.DateTimeOffset,
					DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
					ContractResolver = new CamelCasePropertyNamesContractResolver(),
					MissingMemberHandling = MissingMemberHandling.Ignore,
					TypeNameHandling = TypeNameHandling.None
				};

				return settings;
			};

			var config = new HttpConfiguration();
			config.MapHttpAttributeRoutes();
			config.Formatters.Remove(config.Formatters.XmlFormatter);
			config.Formatters.JsonFormatter.SerializerSettings = JsonConvert.DefaultSettings();
			config.EnsureInitialized();

			ConfigureAutofac(app, config);

			app.UseWebApi(config);
		}

		protected virtual void ConfigureAutofac(IAppBuilder app, HttpConfiguration config)
		{
			if (app == null) {
				throw new ArgumentNullException("app");
			}

			if (config == null) {
				throw new ArgumentNullException("config");
			}

			var thisAssembly = typeof(Startup).Assembly;

			var builder = new ContainerBuilder();
			// TODO: register your types here with Autofac

			var container = builder.Build();

			var resolver = new AutofacWebApiDependencyResolver(container);
			config.DependencyResolver = resolver;

			app.UseAutofacMiddleware(container)
			   .UseAutofacWebApi(config);
		}
	}
}
