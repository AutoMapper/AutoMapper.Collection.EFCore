using System;
using System.Reflection;
using AutoMapper.EquivalencyExpression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper.Collection.EntityFrameworkCore.Tests
{
    public class EntityFrameworkCoreUsingMicrosoftDITests : EntityFramworkCoreTestsBase //, IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;

        public EntityFrameworkCoreUsingMicrosoftDITests()
        {
            var services = new ServiceCollection();

            services
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<DB>(options => options.UseInMemoryDatabase("EfTestDatabase" + Guid.NewGuid()));

            services.AddAutoMapper(x =>
            {
                x.AddCollectionMappers();
                x.UseEntityFrameworkCoreModel<DB>(services);
                x.CreateMap<ThingDto, Thing>().ReverseMap();
            }, new Assembly[0]);

            this._serviceProvider = services.BuildServiceProvider();
            this._serviceScope = this._serviceProvider.CreateScope();

            mapper = GetMapper();               // needed for every test so pref to place into ctor instead of each Arrange
            db = GetDbContext();                // XUnit will create here in ctor and Dispose() after each test
        }

        public override void Dispose()
        {
            this._serviceScope?.Dispose();
            this._serviceProvider?.Dispose();
            base.Dispose();
        }

        private DBContextBase GetDbContext() => this._serviceScope.ServiceProvider.GetRequiredService<DB>();

        private IMapper GetMapper() => this._serviceScope.ServiceProvider.GetRequiredService<IMapper>();

        public class DB : DBContextBase
        {
            public DB(DbContextOptions dbContextOptions)
                : base(dbContextOptions)
            {
            }
        }
    }
}
