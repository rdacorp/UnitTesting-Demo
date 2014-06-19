using System;
using System.Threading.Tasks;
using Moq;
using Todo.Controllers;
using Todo.Models;
using Todo.Services;
using Xunit;

namespace Todo.Tests
{
	public class US2
	{
		[Fact]
		[Trait("User Story", "US2")]
		public async Task Add_new_task()
		{
			// arrange
			var newTask = new TodoItem() { Completed = false, Title = "Test 3" };
			var serviceMock = new Moq.Mock<ITodoService>();
			serviceMock.Setup(x => x.AddAsync(newTask)).ReturnsAsync(newTask);
			var controller = new TodoController(serviceMock.Object);

			// act
			var result = await controller.AddTodo(newTask);

			// assert
			serviceMock.Verify(x => x.AddAsync(newTask), Times.Once);
			Assert.NotNull(result);
			Assert.Equal(newTask, result);
		}
	}
}