using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Moq;
using Todo.Controllers;
using Todo.Models;
using Todo.Services;
using Xunit;

namespace Todo.Lab.Tests
{
	public class US3
	{
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
		public void Status_not_found_when_changing_task_that_does_not_exist()
		{
			// arrange
			throw new NotImplementedException();

			// act

			// assert
		}
	}
}