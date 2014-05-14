using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using SQLite;
using Todo.Models;

namespace Todo.Services
{
	public class TodoService : ITodoService
	{
		private SQLiteAsyncConnection _db;

		public TodoService()
		{
			var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "todos.db");
			_db = new SQLiteAsyncConnection(path);
		}

		public async Task<List<TodoItem>> GetAllAsync()
		{
			await _db.CreateTableAsync<TodoItem>();
			return await _db.Table<TodoItem>().ToListAsync();
		}

		public async Task<TodoItem> AddAsync(TodoItem item)
		{
			await _db.CreateTableAsync<TodoItem>();

			var result = await _db.InsertAsync(item);

			return item;
		}

		public async Task<TodoItem> UpdateAsync(int id, TodoItem todo)
		{
			await _db.CreateTableAsync<TodoItem>();

			var item = await _db.Table<TodoItem>().Where(x => x.Id == id).FirstOrDefaultAsync();

			if (item == null)
				return null;

			item.Title = todo.Title;
			item.Completed = todo.Completed;

			var result = await _db.UpdateAsync(item);

			return todo;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			await _db.CreateTableAsync<TodoItem>();

			var result = await _db.ExecuteAsync("DELETE FROM todos WHERE Id = ?", id);

			return result > 0;
		}

		public async Task<List<TodoItem>> ClearCompleted()
		{
			await _db.CreateTableAsync<TodoItem>();

			var result = await _db.ExecuteAsync("DELETE FROM todos WHERE Completed = 1");

			return await GetAllAsync();
		}
	}
}