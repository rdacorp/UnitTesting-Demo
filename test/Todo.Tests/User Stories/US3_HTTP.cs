using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Newtonsoft.Json;
using Todo.Models;
using Xunit;

namespace Todo.Tests
{
	/// <summary>
	/// Example of how you can use a fake Owin Test Server for executing HTTP requests without actually setting up a server,
	/// this accomplishes the same as the test above, except that it sends it through the faked web server, so you actually
	/// get HTTP responses like the browser would expect.
	/// 
	/// Use this for instances where you need to check for specific HTTP protocol information that is expected to come back to the client
	/// or for instances where you need to test some process like authentication that goes through many layers of you web server.
	/// 
	/// Using a faked test server like this can be useful, but you should also test the actual class like we are doing above with the TodoController.
	/// </summary>
	public class US3_HTTP : IUseFixture<OwinServerFixture>
	{
		private TestServer _server;

		public void SetFixture(OwinServerFixture data)
		{
			_server = data.TestServer;
		}

		[Fact]
		[Trait("User Story", "US3")]
		public async Task Change_title_of_task()
		{
			// arrange
			var requestTodoItem = "{ id: 2, completed: false, title: 'Test 3' }";

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
		[Trait("User Story", "US3")]
		public async Task Change_completeness_of_task()
		{
			// arrange
			var requestTodoItem = "{ id: 2, completed: true, title: 'Test 2' }";

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
		[Trait("User Story", "US3")]
		public async Task Status_not_found_when_changing_task_that_does_not_exist()
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
	}
}