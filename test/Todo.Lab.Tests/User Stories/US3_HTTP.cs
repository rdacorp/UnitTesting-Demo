using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Newtonsoft.Json;
using Todo.Models;
using Xunit;

namespace Todo.Lab.Tests
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
			throw new NotImplementedException();

			// act

			// assert
		}

		[Fact]
		[Trait("User Story", "US3")]
		public async Task Change_completeness_of_task()
		{
			// arrange
			throw new NotImplementedException();

			// act

			// assert
		}

		[Fact]
		[Trait("User Story", "US3")]
		public async Task Status_not_found_when_changing_task_that_does_not_exist()
		{
			// arrange
			throw new NotImplementedException();

			// act

			// assert
		}
	}
}