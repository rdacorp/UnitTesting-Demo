using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Models;

namespace Todo.Services
{
	public interface ITodoService {
		Task<List<TodoItem>> GetAllAsync();
		Task<TodoItem> AddAsync(TodoItem item);
		Task<TodoItem> UpdateAsync(int id, TodoItem todo);
		Task<bool> DeleteAsync(int id);
		Task<List<TodoItem>> ClearCompleted();
	}
}