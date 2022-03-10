using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.EquivalencyExpression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AutoMapper.EntityFrameworkCore
{
    public class GenerateEntityFrameworkCorePrimaryKeyPropertyMaps : IGeneratePropertyMaps
    {
        private readonly IModel _model;

        public GenerateEntityFrameworkCorePrimaryKeyPropertyMaps() => throw new InvalidOperationException($"Use {nameof(MapperConfigurationExpressionExtensions.UseEntityFrameworkCoreModel)} instead of using SetGeneratePropertyMaps.");

        public GenerateEntityFrameworkCorePrimaryKeyPropertyMaps(IModel model) => _model = model;

        public IEnumerable<PropertyMap> GeneratePropertyMaps(TypeMap typeMap)
        {
            var propertyMaps = typeMap.PropertyMaps;
            var keyMembers = _model.FindEntityType(typeMap.DestinationType)?.FindPrimaryKey().Properties ?? new List<IProperty>();
            return propertyMaps.Where(p => keyMembers.Any(m => m.Name == p.DestinationMember.Name));
        }
    }
}