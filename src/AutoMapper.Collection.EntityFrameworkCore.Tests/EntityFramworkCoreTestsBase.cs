﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.EntityFrameworkCore;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoMapper.Collection.EntityFrameworkCore.Tests
{
    public abstract class EntityFramworkCoreTestsBase : IDisposable
    {
        protected IMapper mapper;
        protected DBContextBase db;               // IDisposable

        [Fact]
        public void Persist_InsertOrUpdate_WhenEntityExist_ThenTheEntityShouldBeInTheModifiedState()
        {
            // Arrange
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
            // Arrange [mapper and db fields set in ctor]
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
        /// <summary>
        ///     EF recognizes setting property to the same value so does not raise a pointless UPDATE statement
        /// </summary>
        /// <remarks>this behaviour is like INPC in GUI land</remarks>
        public void Persist_InsertOrUpdate_WhenSameEntity_ThenTheEntityShouldBeInTheUnchangedState()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            //var item = db.Things.Where(x => x.Title == "Test2").First();  // first(special)
            var item = db.Things.Where(x => x.Title == "Test3").First();    // middleish
            //var item = db.Things.Where(x => x.Title == "Test4").First();  // last (special)

            // Act
            db.Things.Persist(mapper).InsertOrUpdate(new ThingDto { ID = item.ID, Title = item.Title });

            // Assert
            db.ChangeTracker.Entries<Thing>().Count(x => x.State != EntityState.Unchanged).Should().Be(0);
        }

        [Fact]
        /// <summary>
        ///     EF recognizes setting property to the same value so does not raise a pointless UPDATE statement
        /// </summary>
        /// <remarks>this behaviour is like INPC in GUI land</remarks>
        public void Persist_InsertOrUpdate_WhenSameEntity_ThenSavedEntityShouldBeSame()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            //var item = db.Things.Where(x => x.Title == "Test2").First();  // first(special)
            var item = db.Things.Where(x => x.Title == "Test3").First();    // middleish
            //var item = db.Things.Where(x => x.Title == "Test4").First();  // last (special)

            // Act
            db.Things.Persist(mapper).InsertOrUpdate(new ThingDto { ID = item.ID, Title = item.Title });
            db.SaveChanges();

            // Assert
            db.Things.Count().Should().Be(3);
            db.Things.FirstOrDefault(x => x.ID == item.ID).Title.Should().Be(item.Title);
        }

        [Fact]
        public void Persist_InsertOrUpdate_WhenEntityDoesNotExist_ThenTheEntityShouldBeInTheAddedState()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            // Act
            var createdThing = db.Things.Persist(mapper).InsertOrUpdate(new ThingDto { Title = "Test" });

            // Assert
            createdThing.Should().NotBeNull();
            db.ChangeTracker.Entries<Thing>().Count(x => x.State == EntityState.Added).Should().Be(1);
        }

        [Fact]
        public void Persist_InsertOrUpdate_WhenEntityDoesNotExist_ThenTheEntityShouldBeInserted()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            // Act
            var createdThing = db.Things.Persist(mapper).InsertOrUpdate(new ThingDto { Title = "Test" });
            db.SaveChanges();

            // Assert
            createdThing.Should().NotBeNull();
            db.Things.Count().Should().Be(4);
            var createdThingFromEF = db.Things.OrderByDescending(x => x.ID).FirstOrDefault();
            createdThingFromEF.Title.Should().Be("Test");
            createdThing.Should().BeEquivalentTo(createdThingFromEF);
        }

        [Fact]
        public async Task Persist_InsertOrUpdateAsync_WhenEntityExist_ThenTheEntityShouldBeInTheModifiedState()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            var item = db.Things.First();

            // Act
            await db.Things.Persist(mapper).InsertOrUpdateAsync(new ThingDto { ID = item.ID, Title = "Test" });

            // Assert
            db.ChangeTracker.Entries<Thing>().Count(x => x.State == EntityState.Modified).Should().Be(1);
        }

        [Fact]
        public async Task Persist_InsertOrUpdateAsync_WhenEntityExist_ThenTheEntityShouldBeUpdated()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            var item = db.Things.First();

            // Act
            await db.Things.Persist(mapper).InsertOrUpdateAsync(new ThingDto { ID = item.ID, Title = "Test" });
            db.SaveChanges();

            // Assert
            db.Things.Count().Should().Be(3);
            db.Things.FirstOrDefault(x => x.ID == item.ID).Title.Should().Be("Test");
        }

        [Fact]
        /// <summary>
        ///     EF recognizes setting property to the same value so does not raise a pointless UPDATE statement
        /// </summary>
        /// <remarks>this behaviour is like INPC in GUI land</remarks>
        public async Task Persist_InsertOrUpdateAsync_WhenSameEntity_ThenTheEntityShouldBeInTheUnchangedState()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            //var item = db.Things.Where(x => x.Title == "Test2").First();  // first(special)
            var item = db.Things.Where(x => x.Title == "Test3").First();    // middleish
            //var item = db.Things.Where(x => x.Title == "Test4").First();  // last (special)

            // Act
            await db.Things.Persist(mapper).InsertOrUpdateAsync(new ThingDto { ID = item.ID, Title = item.Title });

            // Assert
            db.ChangeTracker.Entries<Thing>().Count(x => x.State != EntityState.Unchanged).Should().Be(0);
        }

        [Fact]
        /// <summary>
        ///     EF recognizes setting property to the same value so does not raise a pointless UPDATE statement
        /// </summary>
        /// <remarks>this behaviour is like INPC in GUI land</remarks>
        public async Task Persist_InsertOrUpdateAsync_WhenSameEntity_ThenSavedEntityShouldBeSame()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            //var item = db.Things.Where(x => x.Title == "Test2").First();  // first(special)
            var item = db.Things.Where(x => x.Title == "Test3").First();    // middleish
            //var item = db.Things.Where(x => x.Title == "Test4").First();  // last (special)

            // Act
            await db.Things.Persist(mapper).InsertOrUpdateAsync(new ThingDto { ID = item.ID, Title = item.Title });
            db.SaveChanges();

            // Assert
            db.Things.Count().Should().Be(3);
            db.Things.FirstOrDefault(x => x.ID == item.ID).Title.Should().Be(item.Title);
        }

        [Fact]
        public async Task Persist_InsertOrUpdateAsync_WhenEntityDoesNotExist_ThenTheEntityShouldBeInTheAddedState()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            // Act
            var createdThing = await db.Things.Persist(mapper).InsertOrUpdateAsync(new ThingDto { Title = "Test" });

            // Assert
            createdThing.Should().NotBeNull();
            db.ChangeTracker.Entries<Thing>().Count(x => x.State == EntityState.Added).Should().Be(1);
        }

        [Fact]
        public async Task Persist_InsertOrUpdateAsync_WhenEntityDoesNotExist_ThenTheEntityShouldBeInserted()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            // Act
            var createdThing = await db.Things.Persist(mapper).InsertOrUpdateAsync(new ThingDto { Title = "Test" });
            db.SaveChanges();

            // Assert
            createdThing.Should().NotBeNull();
            db.Things.Count().Should().Be(4);
            var createdThingFromEF = db.Things.OrderByDescending(x => x.ID).FirstOrDefault();
            createdThingFromEF.Title.Should().Be("Test");
            createdThing.Should().BeEquivalentTo(createdThingFromEF);
        }

        [Fact]
        public void Persist_Remove_WhenEntityExist_ThenTheEntityShouldBeInTheDeletedState()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            var item = db.Things.First();

            // Act
            db.Things.Persist(mapper).Remove(new ThingDto { ID = item.ID, Title = "ignored" });

            // Assert
            db.ChangeTracker.Entries<Thing>().Count(x => x.State == EntityState.Deleted).Should().Be(1);
        }

        [Fact]
        public void Persist_Remove_WhenEntityExist_ThenTheEntityShouldBeDeleted()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            var item = db.Things.First();

            // Act
            db.Things.Persist(mapper).Remove(new ThingDto { ID = item.ID, Title = "ignored" });
            db.SaveChanges();

            // Assert
            db.Things.Count().Should().Be(2);
            db.Things.Find(item.ID).Should().BeNull();
        }

        [Fact]
        public void Persist_Remove_WhenEntityDoesNotExist_ThenShouldBeUnchangedState()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            // Act
            db.Things.Persist(mapper).Remove(new ThingDto { Title = "ignored" });   // i.e. ID=0
                                                                                    // hint: AutoMapper.Collection.EntityFrameworkCore.Remove silently ignores non-existent DELETEs

            // Assert
            db.ChangeTracker.Entries<Thing>().Count(x => x.State != EntityState.Unchanged).Should().Be(0);  // no Detached Deleted Modified Added
        }

        [Fact]
        public void Persist_Remove_WhenEntityDoesNotExist_ThenShouldBeNoChange()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            var item = db.Things.OrderByDescending(x => x.ID).First();

            // Act
            db.Things.Persist(mapper).Remove(new ThingDto { Title = "ignored" });   // i.e. ID=0
                                                                                    // hint: AutoMapper.Collection.EntityFrameworkCore.Remove silently ignores non-existent DELETEs
            db.SaveChanges();

            // Assert
            db.Things.Count().Should().Be(3);
            db.Things.Find(item.ID + 1).Should().BeNull();
        }

        [Fact]
        public async Task Persist_RemoveAsync_WhenEntityExist_ThenTheEntityShouldBeInTheDeletedState()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            var item = db.Things.First();

            // Act
            await db.Things.Persist(mapper).RemoveAsync(new ThingDto { ID = item.ID, Title = "ignored" });

            // Assert
            db.ChangeTracker.Entries<Thing>().Count(x => x.State == EntityState.Deleted).Should().Be(1);
        }

        [Fact]
        public async Task Persist_RemoveAsync_WhenEntityExist_ThenTheEntityShouldBeDeleted()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            var item = db.Things.First();

            // Act
            await db.Things.Persist(mapper).RemoveAsync(new ThingDto { ID = item.ID, Title = "ignored" });
            db.SaveChanges();

            // Assert
            db.Things.Count().Should().Be(2);
            db.Things.Find(item.ID).Should().BeNull();
        }

        [Fact]
        public async Task Persist_RemoveAsync_WhenEntityDoesNotExist_ThenShouldBeUnchangedState()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            // Act
            await db.Things.Persist(mapper).RemoveAsync(new ThingDto { Title = "ignored" });    // i.e. ID=0
                                                                                                // hint: AutoMapper.Collection.EntityFrameworkCore.Remove silently ignores non-existent DELETEs

            // Assert
            db.ChangeTracker.Entries<Thing>().Count(x => x.State != EntityState.Unchanged).Should().Be(0);  // no Detached Deleted Modified Added
        }

        [Fact]
        public async Task Persist_RemoveAsync_WhenEntityDoesNotExist_ThenShouldBeNoChange()
        {
            // Arrange [mapper and db fields set in ctor]
            db.Things.Add(new Thing { Title = "Test2" });
            db.Things.Add(new Thing { Title = "Test3" });
            db.Things.Add(new Thing { Title = "Test4" });
            db.SaveChanges();

            var item = db.Things.OrderByDescending(x => x.ID).First();

            // Act
            await db.Things.Persist(mapper).RemoveAsync(new ThingDto { Title = "ignored" });    // i.e. ID=0
                                                                                                // hint: AutoMapper.Collection.EntityFrameworkCore.Remove silently ignores non-existent DELETEs
            db.SaveChanges();

            // Assert
            db.Things.Count().Should().Be(3);
            db.Things.Find(item.ID + 1).Should().BeNull();
        }

        public virtual void Dispose() => db?.Dispose();

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