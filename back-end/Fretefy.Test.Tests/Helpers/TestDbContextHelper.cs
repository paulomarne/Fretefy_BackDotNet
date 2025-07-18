using Fretefy.Test.Infra.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;

namespace Fretefy.Test.Tests.Helpers
{
    public static class TestDbContextHelper
    {
        public static TestDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new TestDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}

