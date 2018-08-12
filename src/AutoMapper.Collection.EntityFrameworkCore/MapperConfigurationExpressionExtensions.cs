using System;
using AutoMapper.EntityFrameworkCore;
using AutoMapper.EquivalencyExpression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper
{
    public static class MapperConfigurationExpressionExtensions
    {
        /// <summary>
        /// Generates and adds property maps based on the primary keys for the given <see cref="DbContext"/>. This is done by creating an
        /// instance of the <see cref="DbContext"/> using the parameterless constructor.
        /// </summary>
        public static void UseEntityFrameworkCoreModel<TContext>(this IMapperConfigurationExpression config)
            where TContext : DbContext, new()
        {
            using (var context = new TContext())
            {
                config.UseEntityFrameworkCoreModel<TContext>(context.Model);
            }
        }

        /// <summary>
        /// Generates and adds property maps based on the primary keys for the given <see cref="DbContext"/>. This is done by resolving an
        /// instance of the <see cref="DbContext"/> using a temporary <see cref="IServiceProvider"/> based on the <see cref="IServiceCollection"/>.
        /// This method is generally called from <see cref="IServiceCollection"/>.AddAutoMapper().
        /// </summary>
        public static void UseEntityFrameworkCoreModel<TContext>(this IMapperConfigurationExpression config, IServiceCollection services)
            where TContext : DbContext
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                config.UseEntityFrameworkCoreModel<TContext>(serviceProvider);
            }
        }

        /// <summary>
        /// Generates and adds property maps based on the primary keys for the given <see cref="DbContext"/>. This is done by resolving an
        /// instance of the <see cref="DbContext"/> using the <see cref="IServiceProvider"/>. This method is generally used when you are configuring
        /// AutoMapper using static initialization via <see cref="Mapper.Initialize(Action{IMapperConfigurationExpression})"/>.
        /// </summary>
        public static void UseEntityFrameworkCoreModel<TContext>(this IMapperConfigurationExpression config, IServiceProvider serviceProvider)
            where TContext : DbContext
        {
            using (var scope = serviceProvider.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<TContext>())
            {
                config.UseEntityFrameworkCoreModel<TContext>(context.Model);
            }
        }

        /// <summary>
        /// Generates and adds property maps based on the primary keys for the given <see cref="DbContext"/>. This method is generally
        /// only used if you are using <see cref="DbContextOptionsBuilder.UseModel(IModel)"/>.
        /// </summary>
        public static void UseEntityFrameworkCoreModel<TContext>(this IMapperConfigurationExpression config, IModel model)
            where TContext : DbContext
        {
            config.SetGeneratePropertyMaps(new GenerateEntityFrameworkCorePrimaryKeyPropertyMaps<TContext>(model));
        }
    }
}
