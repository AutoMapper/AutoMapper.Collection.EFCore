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
        }

        protected override DBContextBase GetDbContext()
        {
            return new DB();
        }

        protected override IMapper GetMapper()
        {
            return _mapper;
        }

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
