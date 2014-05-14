using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Moq;
using Todo.Controllers;
using Todo.Models;
using Todo.Services;
using Xunit;

namespace Todo.Tests.Controllers
{
	public class TodoControllerTests
	{
		[Fact]
		public async Task Get_all_task_items()
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

		[Fact]
		public async Task Clear_all_completed_tasks()
		{
			// arrange
			var list = new List<TodoItem> {
				new TodoItem() { Id = 1, Completed = false, Title = "Test 1" },
				new TodoItem() { Id = 2, Completed = true, Title = "Test 2" }
			};
			var notCompleted = list.Where(x => x.Completed == false).ToList();
			var serviceMock = new Moq.Mock<ITodoService>();
			serviceMock.Setup(x => x.ClearCompleted()).ReturnsAsync(notCompleted);
			var controller = new TodoController(serviceMock.Object);

			// act
			var result = await controller.ClearCompleted();

			// assert
			serviceMock.Verify(x => x.ClearCompleted(), Times.Once);
			Assert.NotEmpty(result);
			Assert.Equal(1, result.Count);
			Assert.Equal(notCompleted, result);
		}

		[Fact]
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

		[Fact]
		public async Task Update_existing_task()
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
		public void Update_task_that_does_not_exist()
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

		[Fact]
		public async Task Delete_existing_task()
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
		public async Task Delete_task_that_does_not_exist()
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
