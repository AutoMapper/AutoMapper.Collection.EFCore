<img src="https://s3.amazonaws.com/automapper/logo.png" alt="AutoMapper"> 

# AutoMapper.Collection.EntityFrameworkCore

`Automapper.Collection.EntityFrameworkCore` will help you mapping of EntityFrameowrk Core DbContext-object.

[![NuGet](http://img.shields.io/nuget/v/Automapper.Collection.EntityFrameworkCore.svg)](https://www.nuget.org/packages/Automapper.Collection.EntityFrameworkCore/)

## Configuration examples

- Usage together with Dependency injection and AutoMapper.Extensions.Microsoft.DependencyInjection pacakge
```	
var services = new ServiceCollection();
services
    .AddEntityFrameworkInMemoryDatabase()
    .AddDbContext<DB>();

services.AddAutoMapper((serviceProvider, automapper) =>
{
    automapper.AddCollectionMappers();
    automapper.UseEntityFrameworkCoreModel<DB>(serviceProvider);
}, typeof(DB).Assembly);

var serviceProvider = services.BuildServiceProvider();
```

**Note:** User defined equality expressions will overwrite primary key expressions.

What about comparing to a single existing Entity for updating?
--------------------------------
Automapper.Collection.EntityFrameworkCore does that as well through extension method from of DbSet<TEntity>.

Translate equality between dto and EF object to an expression of just the EF using the dto's values as constants.

```
	dbContext.Orders.Persist(mapper).InsertOrUpdate<OrderDTO>(newOrderDto);
	dbContext.Orders.Persist(mapper).InsertOrUpdate<OrderDTO>(existingOrderDto);
	dbContext.Orders.Persist(mapper).Remove<OrderDTO>(deletedOrderDto);
	dbContext.SubmitChanges();
```

**Note:** This is done by converting the OrderDTO to Expression<Func<Order,bool>> and using that to find matching type in the database.  You can also map objects to expressions as well.

Persist doesn't call submit changes automatically

How to get it
--------------------------------
Use NuGet Package Manager to install the package or use any of the following command in NuGet Package Manager Console.

```	
PM> Install-Package AutoMapper.Collection.EntityFrameworkCore
```
