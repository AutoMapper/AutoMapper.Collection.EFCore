using System;
using AutoMapper.EquivalencyExpression;
using Microsoft.EntityFrameworkCore;

namespace AutoMapper.Collection.EntityFrameworkCore.Tests
{
    public class EntityFramworkCoreUsingCtorTests : EntityFramworkCoreTestsBase
    {
        private readonly Mapper _mapper;

        public EntityFramworkCoreUsingCtorTests()
        {
            _mapper = new Mapper(new MapperConfiguration(x =>
            {
                x.AddCollectionMappers();
                x.CreateMap<ThingDto, Thing>().ReverseMap();
                x.UseEntityFrameworkCoreModel<DB>();
            }));

            mapper = GetMapper();               // needed for every test so pref to place into ctor instead of each Arrange
            db = GetDbContext();                // XUnit will create here in ctor and Dispose() after each test
        }

        private DBContextBase GetDbContext() => new DB();

        private IMapper GetMapper() => _mapper;

        public class DB : DBContextBase
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("EfTestDatabase" + Guid.NewGuid());
                base.OnConfiguring(optionsBuilder);
            }
        }
    }
}
