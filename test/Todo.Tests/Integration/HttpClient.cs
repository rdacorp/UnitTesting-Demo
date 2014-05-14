using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
using Xunit;

namespace Todo.Tests.Integration
{
	public class HttpClient : IUseFixture<HttpClient.OwinServerFixture>
	{
		#region OWIN Server

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

		#endregion

		private TestServer _server;

		public void SetFixture(OwinServerFixture data)
		{
			_server = data.TestServer;
		}

		[Fact]
		public async Task Get_all_task_items()
		{
			// arrange

			// act
			var response = await _server.CreateRequest("/todo").GetAsync();
			var resultString = await response.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeObject<List<TodoItem>>(resultString);

			// assert
			Assert.NotEmpty(result);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(2, result.Count);
		}

		[Fact]
		public async Task Clear_all_completed_tasks()
		{
			// arrange

			// act
			var response = await _server.CreateRequest("/todo/clearCompleted").PostAsync();
			var resultString = await response.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeObject<List<TodoItem>>(resultString);

			// assert
			Assert.NotEmpty(result);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(1, result.Count);
		}

		[Fact]
		public async Task Add_new_task()
		{
			// arrange
			var requestTodoItem = "{ completed: true, title: 'Test 3' }";

			// act
			var response = await _server
				.CreateRequest("/todo")
				.And(request => {
					request.Content = new StringContent(requestTodoItem, Encoding.UTF8, "application/json");
				}).PostAsync();
			var resultString = await response.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeObject<TodoItem>(resultString);

			// assert
			Assert.NotNull(result);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal("Test 3", result.Title);
		}

		[Fact]
		public async Task Update_existing_task()
		{
			// arrange
			var requestTodoItem = "{ id: 2, completed: true, title: 'Test 3' }";

			// act
			var response = await _server
				.CreateRequest("/todo/2")
				.And(request => {
					request.Content = new StringContent(requestTodoItem, Encoding.UTF8, "application/json");
				}).SendAsync("PUT");
			var resultString = await response.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeObject<TodoItem>(resultString);

			// assert
			Assert.NotNull(result);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(2, result.Id);
			Assert.Equal("Test 3", result.Title);
		}

		[Fact]
		public async Task Update_task_that_does_not_exist()
		{
			// arrange
			var requestTodoItem = "{ id: 4, completed: true, title: 'Test 3' }";

			// act
			var response = await _server
				.CreateRequest("/todo/4")
				.And(request => {
					request.Content = new StringContent(requestTodoItem, Encoding.UTF8, "application/json");
				}).SendAsync("PUT");

			// assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}

		[Fact]
		public async Task Delete_existing_task()
		{
			// arrange

			// act
			var response = await _server.CreateRequest("/todo/2").SendAsync("DELETE");

			// assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Fact]
		public async Task Delete_task_that_does_not_exist()
		{
			// arrange

			// act
			var response = await _server.CreateRequest("/todo/4").SendAsync("DELETE");

			// assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}
	}
}