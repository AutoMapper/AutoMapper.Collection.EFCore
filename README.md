<img src="https://s3.amazonaws.com/automapper/logo.png" alt="AutoMapper"> 

AutoMapper.Collection.EFCore
================================
`Automapper.Collection.EntityFrameworkCore` will help you mapping of EntityFrameowrk Core DbContext-object.
	
    Mapper.Initialize(cfg =>
    {
        cfg.AddCollectionMappers();
        cfg.SetGeneratePropertyMaps<GenerateEntityFrameworkCorePrimaryKeyPropertyMaps<DB>>();
        // Configuration code
    });

User defined equality expressions will overwrite primary key expressions.

What about comparing to a single existing Entity for updating?
--------------------------------
Automapper.Collection.EntityFramework does that as well through extension method from of DbSet<TEntity>.

Translate equality between dto and EF object to an expression of just the EF using the dto's values as constants.

	dbContext.Orders.Persist().InsertOrUpdate<OrderDTO>(newOrderDto);
	dbContext.Orders.Persist().InsertOrUpdate<OrderDTO>(existingOrderDto);
	dbContext.Orders.Persist().Remove<OrderDTO>(deletedOrderDto);
	dbContext.SubmitChanges();

**Note:** This is done by converting the OrderDTO to Expression<Func<Order,bool>> and using that to find matching type in the database.  You can also map objects to expressions as well.

Persist doesn't call submit changes automatically

How to get it
--------------------------------
Use NuGet Package Manager to install the package or use any of the following command in NuGet Package Manager Console.
	
	PM> Install-Package AutoMapper.Collection.EntityFrameworkCore
