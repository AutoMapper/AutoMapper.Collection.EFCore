using System.Linq;
using AutoMapper.EntityFrameworkCore;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoMapper.Collection.EntityFrameworkCore.Tests
{
    public abstract class EntityFramworkCoreTestsBase
    {
        protected abstract DBContextBase GetDbContext();
        protected abstract IMapper GetMapper();

        [Fact]
        public void Persist_InsertOrUpdate_WhenEntityExist_ThenTheEntityShouldBeInTheModifiedState()
        {
            // Arrange
            var mapper = GetMapper();
            var db = GetDbContext();

            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            var item = db.Things.First();

            // Act
            db.Things.Persist(mapper).InsertOrUpdate(new ThingDto { ID = item.ID, Title = "Test" });

            // Assert
            db.ChangeTracker.Entries<Thing>().Count(x => x.State == EntityState.Modified).Should().Be(1);
        }

        [Fact]
        public void Persist_InsertOrUpdate_WhenEntityExist_ThenTheEntityShouldBeUpdated()
        {
            // Arrange
            var mapper = GetMapper();
            var db = GetDbContext();

            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            var item = db.Things.First();

            // Act
            db.Things.Persist(mapper).InsertOrUpdate(new ThingDto { ID = item.ID, Title = "Test" });
            db.SaveChanges();

            // Assert
            db.Things.Count().Should().Be(3);
            db.Things.FirstOrDefault(x => x.ID == item.ID).Title.Should().Be("Test");
        }

        [Fact]
        public void Persist_InsertOrUpdate_WhenEntityDoesNotExist_ThenTheEntityShouldBeInTheAddedState()
        {
            // Arrange
            var mapper = GetMapper();
            var db = GetDbContext();

            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            // Act
            db.Things.Persist(mapper).InsertOrUpdate(new ThingDto { Title = "Test" });

            // Assert
            db.ChangeTracker.Entries<Thing>().Count(x => x.State == EntityState.Added).Should().Be(1);
        }

        [Fact]
        public void Persist_InsertOrUpdate_WhenEntityDoesNotExist_ThenTheEntityShouldBeInserted()
        {
            // Arrange
            var mapper = GetMapper();
            var db = GetDbContext();

            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            // Act
            db.Things.Persist(mapper).InsertOrUpdate(new ThingDto { Title = "Test" });
            db.SaveChanges();

            // Assert
            db.Things.Count().Should().Be(4);
            db.Things.OrderByDescending(x => x.ID).FirstOrDefault().Title.Should().Be("Test");
        }

        [Fact]
        public void Persist_Remove_WhenEntityDoesNotExist_ThenTheEntityShouldBeInTheDeletedState()
        {
            // Arrange
            var mapper = GetMapper();
            var db = GetDbContext();

            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            var item = db.Things.First();

            // Act
            db.Things.Persist(mapper).Remove(new ThingDto { ID = item.ID, Title = "Test" });

            // Assert
            db.ChangeTracker.Entries<Thing>().Count(x => x.State == EntityState.Deleted).Should().Be(1);
        }

        [Fact]
        public void Persist_Remove_WhenEntityDoesNotExist_ThenTheEntityShouldBeDeleted()
        {
            // Arrange
            var mapper = GetMapper();
            var db = GetDbContext();

            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            var item = db.Things.First();

            // Act
            db.Things.Persist(mapper).Remove(new ThingDto { ID = item.ID, Title = "Test" });
            db.SaveChanges();

            // Assert
            db.Things.Count().Should().Be(2);
            db.Things.Find(item.ID).Should().BeNull();
        }

        public abstract class DBContextBase : DbContext
        {
            protected DBContextBase(DbContextOptions dbContextOptions)
                : base(dbContextOptions)
            {
            }

            protected DBContextBase() { }

            public DbSet<Thing> Things { get; set; }
        }

        public class Thing
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public override string ToString() { return Title; }
        }

        public class ThingDto
        {
            public int ID { get; set; }
            public string Title { get; set; }
        }
    }
}
