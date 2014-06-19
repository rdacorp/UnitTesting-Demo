using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using SQLite;
using Todo.Controllers;
using Todo.Models;
using Todo.Services;
using Xunit;

namespace Todo.Lab.Tests
{
	/// <summary>
	/// Example of a database integration test. Typically integration tests should be avoided for continuous iteration environments,
	/// because they are hard to control the state of the environment over multiple tests. However they are useful before you 
	/// deploy to production.
	/// 
	/// It is recommended that you keep your unit tests and your integration tests in separate projects to make it easier to run in
	/// continuous iteration environments.
	/// </summary>
	public class US1_DB_Integration
	{
		private readonly List<TodoItem> Todos = new List<TodoItem> {
			new TodoItem() { Id = 1, Completed = false, Title = "Test 1" },
			new TodoItem() { Id = 2, Completed = true, Title = "Test 2" }
		};

		private async Task PrepareDatabase()
		{
			var path = Path.Combine(Path.GetDirectoryName(typeof(TodoService).Assembly.Location), "todos.db");

			if (File.Exists(path))
				File.Delete(path);

			var db = new SQLiteAsyncConnection(path);
			await db.CreateTableAsync<TodoItem>();

			var count = await db.InsertAllAsync(Todos);
			Assert.Equal(Todos.Count, count);
		}

		[Fact]
		[Trait("User Story", "US1")]
		[Trait("Type", "Integration")]
		public async Task Get_all_tasks()
		{
			// arrange
			throw new NotImplementedException();

			// act

			// assert
		}
	}
}