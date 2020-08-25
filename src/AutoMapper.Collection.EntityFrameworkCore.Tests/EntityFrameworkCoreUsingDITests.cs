using System;
using AutoMapper.EquivalencyExpression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper.Collection.EntityFrameworkCore.Tests
{
    public class EntityFramworkCoreUsingDITests : EntityFramworkCoreTestsBase   //, IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly Mapper _mapper;
        private readonly IServiceScope _serviceScope;

        public EntityFramworkCoreUsingDITests()
        {
            var services = new ServiceCollection();

            services
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<DB>(options => options.UseInMemoryDatabase("EfTestDatabase" + Guid.NewGuid()));

            _serviceProvider = services.BuildServiceProvider();

            _mapper = new Mapper(new MapperConfiguration(x =>
            {
                x.ConstructServicesUsing(type => ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, type));
                x.AddCollectionMappers();
                x.UseEntityFrameworkCoreModel<DB>(_serviceProvider);
                x.CreateMap<ThingDto, Thing>().ReverseMap();
            }));

            _serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            mapper = GetMapper();               // needed for every test so pref to place into ctor instead of each Arrange
            db = GetDbContext();                // XUnit will create here in ctor and Dispose() after each test
        }

        public override void Dispose()
        {
            _serviceScope?.Dispose();
            _serviceProvider?.Dispose();
            base.Dispose();
        }

        private DBContextBase GetDbContext() => _serviceScope.ServiceProvider.GetRequiredService<DB>();

        private IMapper GetMapper() => _mapper;

        public class DB : DBContextBase
        {
            public DB(DbContextOptions dbContextOptions)
                : base(dbContextOptions)
            {
            }
        }
    }
}
