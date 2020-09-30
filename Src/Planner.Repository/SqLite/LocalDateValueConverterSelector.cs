using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;

namespace Planner.Repository.SqLite
{
    public class LocalDateValueConverterSelector : ValueConverterSelector
    {
        // The dictionary in the base type is private, so we need our own one here.
        private readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo> converters
            = new ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo>();


        public LocalDateValueConverterSelector(ValueConverterSelectorDependencies dependencies) : base(dependencies)
        {
        }

        public override IEnumerable<ValueConverterInfo> Select(Type modelClrType, Type? providerClrType = null)
        {
            return base.Select(modelClrType, providerClrType)
                .Concat(AdditionalSelections(modelClrType,providerClrType));
        }

        private static Type? UnwrapNullableType(Type? type) => 
            type is null ? null:(Nullable.GetUnderlyingType(type) ?? type);

        private IEnumerable<ValueConverterInfo> AdditionalSelections(Type? modelClrType, Type? providerClrType)
        {
            if (modelClrType == null) yield break;
            var unwrppedModelClrType = UnwrapNullableType(modelClrType);
            var unwrappedProviderClrType = UnwrapNullableType(providerClrType);
            if ((unwrappedProviderClrType == null || unwrappedProviderClrType == typeof(DateTime))
                && unwrppedModelClrType == typeof(LocalDate))
            {
                yield return converters.GetOrAdd((unwrppedModelClrType, typeof(DateTime)),
                    k => new ValueConverterInfo(modelClrType, typeof(DateTime),
                        i => new LocalDateConverter()));
            }
        }
    }

    public class LocalDateConverter : ValueConverter<LocalDate, DateTime>
    {
        public LocalDateConverter(ConverterMappingHints? mappingHints = null) : 
            base(i=>i.ToDateTimeUnspecified(), i=>LocalDate.FromDateTime(i), mappingHints)
        {
        }
    }
}