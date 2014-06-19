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

namespace Todo.Tests
{
	public class US3
	{
		[Fact]
		[Trait("User Story", "US3")]
		public async Task Change_title_of_task()
		{
			// arrange
			var existingTask = new TodoItem() { Id = 2, Completed = false, Title = "Test 3" };
			var serviceMock = new Moq.Mock<ITodoService>();
			serviceMock.Setup(x => x.UpdateAsync(existingTask.Id, existingTask)).ReturnsAsync(existingTask);
			var controller = new TodoController(serviceMock.Object);

			// act
			var result = await controller.UpdateTodo(existingTask.Id, existingTask);

			// assert
			serviceMock.Verify(x => x.UpdateAsync(existingTask.Id, existingTask), Times.Once);
			Assert.NotNull(result);
			Assert.Equal(existingTask, result);
		}

		[Fact]
		[Trait("User Story", "US3")]
		public async Task Change_completeness_of_task()
		{
			// arrange
			var existingTask = new TodoItem() { Id = 2, Completed = true, Title = "Test 1" };
			var serviceMock = new Moq.Mock<ITodoService>();
			serviceMock.Setup(x => x.UpdateAsync(existingTask.Id, existingTask)).ReturnsAsync(existingTask);
			var controller = new TodoController(serviceMock.Object);

			// act
			var result = await controller.UpdateTodo(existingTask.Id, existingTask);

			// assert
			serviceMock.Verify(x => x.UpdateAsync(existingTask.Id, existingTask), Times.Once);
			Assert.NotNull(result);
			Assert.Equal(existingTask, result);
		}

		[Fact]
		[Trait("User Story", "US3")]
		public void Status_not_found_when_changing_task_that_does_not_exist()
		{
			// arrange
			var taskId = 4;
			var existingTask = new TodoItem() { Id = 2, Completed = false, Title = "Test 3" };
			var serviceMock = new Moq.Mock<ITodoService>();
			serviceMock.Setup(x => x.UpdateAsync(taskId, existingTask)).ReturnsAsync(null);
			var controller = new TodoController(serviceMock.Object);

			// act
			var exc = Assert.Throws<AggregateException>(() => controller.UpdateTodo(taskId, existingTask).Result);
			var result = exc.InnerExceptions.Cast<HttpResponseException>().FirstOrDefault();

			// assert
			serviceMock.Verify(x => x.UpdateAsync(taskId, existingTask), Times.Once);
			Assert.NotNull(result);
			Assert.Equal(HttpStatusCode.NotFound, result.Response.StatusCode);
		}
	}
}