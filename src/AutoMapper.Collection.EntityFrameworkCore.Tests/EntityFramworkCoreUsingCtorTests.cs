using System;
using System.Linq;
using AutoMapper.EntityFrameworkCore;
using AutoMapper.EquivalencyExpression;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoMapper.Collection.EntityFrameworkCore.Tests
{
    public class EntityFramworkCoreUsingCtorTests : EntityFramworkCoreTestsBase
    {
        public EntityFramworkCoreUsingCtorTests()
        {
            Mapper.Reset();
            Mapper.Initialize(x =>
            {
                x.AddCollectionMappers();
                x.CreateMap<ThingDto, Thing>().ReverseMap();
                x.AddEntityFrameworkCoreKeys<DB>();
            });
        }

        protected override DBContextBase GetDbContext()
        {
            return new DB();
        }

        protected override IMapper GetMapper()
        {
            return Mapper.Instance;
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
