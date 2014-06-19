using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Todo.Controllers;
using Todo.Models;
using Todo.Services;
using Xunit;

namespace Todo.Tests
{
	public class US1
	{
		[Fact]
		[Trait("User Story", "US1")]
		public async Task Get_all_tasks()
		{
			// arrange
			var list = new List<TodoItem> {
				new TodoItem() { Id = 1, Completed = false, Title = "Test 1" },
				new TodoItem() { Id = 2, Completed = true, Title = "Test 2" }
			};
			var serviceMock = new Moq.Mock<ITodoService>();
			serviceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(list);
			var controller = new TodoController(serviceMock.Object);

			// act
			var result = await controller.GetAll();

			// assert
			serviceMock.Verify(x => x.GetAllAsync(), Times.Once);
			Assert.NotEmpty(result);
			Assert.Equal(2, result.Count);
			Assert.Equal(list, result);
		}
	}
}