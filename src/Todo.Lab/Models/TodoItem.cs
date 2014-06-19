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

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((TodoItem)obj);
		}

		protected bool Equals(TodoItem other)
		{
			return Id == other.Id && String.Equals(Title, other.Title) && Completed.Equals(other.Completed);
		}

		public override int GetHashCode()
		{
			unchecked {
				int hashCode = Id;
				hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Completed.GetHashCode();
				return hashCode;
			}
		}
	}
}