using System;
using System.Reflection;
using AutoMapper.EquivalencyExpression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper.Collection.EntityFrameworkCore.Tests
{
    public class EntityFrameworkCoreUsingMicrosoftDITests : EntityFramworkCoreTestsBase
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

            _serviceProvider = services.BuildServiceProvider();
            _serviceScope = _serviceProvider.CreateScope();

            mapper = _serviceScope.ServiceProvider.GetRequiredService<IMapper>();
            db = _serviceScope.ServiceProvider.GetRequiredService<DB>();
        }

        public override void Dispose()
        {
            _serviceScope?.Dispose();
            _serviceProvider?.Dispose();
            base.Dispose();
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
