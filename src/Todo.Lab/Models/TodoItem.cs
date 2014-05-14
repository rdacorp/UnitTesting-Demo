using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;

namespace Todo.Models
{
	[Table("todos")]
	public class TodoItem
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Title { get; set; }
		public bool Completed { get; set; }
	}
}