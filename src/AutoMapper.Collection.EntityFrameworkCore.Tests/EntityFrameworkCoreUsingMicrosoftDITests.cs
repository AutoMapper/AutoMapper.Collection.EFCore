using System;
using System.Reflection;
using AutoMapper.EquivalencyExpression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper.Collection.EntityFrameworkCore.Tests
{
    public class EntityFrameworkCoreUsingMicrosoftDITests : EntityFramworkCoreTestsBase, IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;

        public EntityFrameworkCoreUsingMicrosoftDITests()
        {
            var services = new ServiceCollection();

            services
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<DB>(options => options.UseInMemoryDatabase("EfTestDatabase" + Guid.NewGuid()));

            services.AddAutoMapper(automapper =>
            {
                automapper.AddCollectionMappers();
                automapper.AddEntityFrameworkCoreKeys<DB>(services);
            }, new Assembly[0]);

            this._serviceProvider = services.BuildServiceProvider();
            this._serviceScope = this._serviceProvider.CreateScope();
        }

        public void Dispose()
        {
            this._serviceScope?.Dispose();
            this._serviceProvider?.Dispose();
        }

        protected override DBContextBase GetDbContext()
        {
            return this._serviceScope.ServiceProvider.GetRequiredService<DB>();
        }

        protected override IMapper GetMapper()
        {
            return this._serviceScope.ServiceProvider.GetRequiredService<IMapper>();
        }

        public class DB : DBContextBase
        {
            public DB(DbContextOptions dbContextOptions)
                : base(dbContextOptions)
            {
            }
        }
    }
}
