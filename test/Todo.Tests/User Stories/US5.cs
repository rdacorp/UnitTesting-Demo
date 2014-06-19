using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Todo.Controllers;
using Todo.Models;
using Todo.Services;
using Xunit;

namespace Todo.Tests
{
	public class US5
	{
		[Fact]
		[Trait("User Story", "US5")]
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
	}
}
