using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin.Testing;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Todo.Models;
using Todo.Services;

namespace Todo.Tests
{
	public class OwinServerFixture : IDisposable
	{
		private TestServer _server;

		public OwinServerFixture()
		{
			var list = new List<TodoItem> {
				new TodoItem() { Id = 1, Completed = false, Title = "Test 1" },
				new TodoItem() { Id = 2, Completed = true, Title = "Test 2" }
			};

			ServiceMock = new Moq.Mock<ITodoService>();
			ServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(list);

			var notCompleted = list.Where(x => x.Completed == false).ToList();
			ServiceMock.Setup(x => x.ClearCompleted()).ReturnsAsync(notCompleted);

			var newTask = new TodoItem() { Completed = false, Title = "Test 3" };
			ServiceMock.Setup(x => x.AddAsync(It.IsAny<TodoItem>())).ReturnsAsync(newTask);

			const int taskId = 2;
			var existingTask = new TodoItem() { Id = 2, Completed = false, Title = "Test 3" };
			ServiceMock.Setup(x => x.UpdateAsync(taskId, It.IsAny<TodoItem>())).ReturnsAsync(existingTask);
			ServiceMock.Setup(x => x.DeleteAsync(taskId)).ReturnsAsync(true);

			const int missingTaskId = 4;
			ServiceMock.Setup(x => x.UpdateAsync(missingTaskId, existingTask)).ReturnsAsync(null);
			ServiceMock.Setup(x => x.DeleteAsync(missingTaskId)).ReturnsAsync(false);

			TestServer = TestServer.Create(Configuration);
		}

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

			var thisAssembly = typeof(Startup).Assembly;

			var builder = new ContainerBuilder();
			builder.RegisterApiControllers(thisAssembly);
			builder.RegisterInstance(ServiceMock.Object).As<ITodoService>();

			var container = builder.Build();

			var resolver = new AutofacWebApiDependencyResolver(container);
			config.DependencyResolver = resolver;

			app.UseAutofacMiddleware(container)
			   .UseAutofacWebApi(config);

			app.UseWebApi(config);
		}

		public Mock<ITodoService> ServiceMock { get; private set; }

		public TestServer TestServer { get; private set; }

		public void Dispose()
		{
			if (_server != null)
				_server.Dispose();
		}
	}
}