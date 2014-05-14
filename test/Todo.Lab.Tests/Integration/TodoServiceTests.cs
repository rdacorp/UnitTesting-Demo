using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin.Testing;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Todo.Models;
using Todo.Services;
using Xunit;

namespace Todo.Tests.Integration
{
	public class TodoServiceTests
	{
		[Fact]
		public async Task Get_all_task_items()
		{
			// arrange

			// act

			// assert
		}

		[Fact]
		public async Task Clear_all_completed_tasks()
		{
			// arrange

			// act

			// assert
		}

		[Fact]
		public async Task Add_new_task()
		{
			// arrange

			// act

			// assert
		}

		[Fact]
		public async Task Update_existing_task()
		{
			// arrange

			// act

			// assert
		}

		[Fact]
		public async Task Update_task_that_does_not_exist()
		{
			// arrange

			// act

			// assert
		}

		[Fact]
		public async Task Delete_existing_task()
		{
			// arrange

			// act

			// assert
		}

		[Fact]
		public async Task Delete_task_that_does_not_exist()
		{
			// arrange

			// act

			// assert
		}
	}
}