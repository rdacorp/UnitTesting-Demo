using System;
using System.Net;
using System.Threading.Tasks;
using Moq;
using Todo.Controllers;
using Todo.Services;
using Xunit;

namespace Todo.Tests
{
	public class US4
	{
		[Fact]
		[Trait("User Story", "US4")]
		public async Task Delete_task()
		{
			// arrange
			var taskId = 1;
			var serviceMock = new Moq.Mock<ITodoService>();
			serviceMock.Setup(x => x.DeleteAsync(taskId)).ReturnsAsync(true);
			var controller = new TodoController(serviceMock.Object);

			// act
			var result = await controller.DeleteTodo(taskId);

			// assert
			serviceMock.Verify(x => x.DeleteAsync(taskId), Times.Once);
			Assert.NotNull(result);
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		}

		[Fact]
		[Trait("User Story", "US4")]
		public async Task Status_not_found_when_deleting_task_that_does_not_exist()
		{
			// arrange
			var taskId = 1;
			var serviceMock = new Moq.Mock<ITodoService>();
			serviceMock.Setup(x => x.DeleteAsync(taskId)).ReturnsAsync(false);
			var controller = new TodoController(serviceMock.Object);

			// act
			var result = await controller.DeleteTodo(taskId);

			// assert
			serviceMock.Verify(x => x.DeleteAsync(taskId), Times.Once);
			Assert.NotNull(result);
			Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
		}
	}
}