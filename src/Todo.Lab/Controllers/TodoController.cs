using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Todo.Models;
using Todo.Services;

namespace Todo.Controllers
{
	[RoutePrefix("todo")]
	public class TodoController : ApiController
	{
		private readonly ITodoService _service;

		public TodoController(ITodoService service)
		{
			_service = service;
		}

		[HttpGet, Route("")]
		public Task<List<TodoItem>> GetAll()
		{
			return _service.GetAllAsync();
		}

		[HttpPut, Route("{id}")]
		public async Task<TodoItem> UpdateTodo(int id, [FromBody] TodoItem todo)
		{
			var item = await _service.UpdateAsync(id, todo);

			if (item == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return item;
		}

		[HttpPost, Route("")]
		public Task<TodoItem> AddTodo([FromBody] TodoItem item)
		{
			return _service.AddAsync(item);
		}

		[HttpDelete, Route("{id}")]
		public async Task<HttpResponseMessage> DeleteTodo(int id)
		{
			var deleted = await _service.DeleteAsync(id);

			return new HttpResponseMessage(deleted ? HttpStatusCode.OK : HttpStatusCode.NotFound);
		}

		[HttpPost, Route("clearCompleted")]
		public Task<List<TodoItem>> ClearCompleted()
		{
			return _service.ClearCompleted();
		}
	}
}