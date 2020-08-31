using System;
using AutoMapper.EquivalencyExpression;
using Microsoft.EntityFrameworkCore;

namespace AutoMapper.Collection.EntityFrameworkCore.Tests
{
    public class EntityFrameworkCoreUsingCtorTests : EntityFrameworkCoreTestsBase
    {
        private readonly Mapper _mapper;

        public EntityFrameworkCoreUsingCtorTests()
        {
            _mapper = new Mapper(new MapperConfiguration(x =>
            {
                x.AddCollectionMappers();
                x.CreateMap<ThingDto, Thing>().ReverseMap();
                x.UseEntityFrameworkCoreModel<DB>();
            }));
        }

        protected override DBContextBase GetDbContext() => new DB();

        protected override IMapper GetMapper() => _mapper;

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
