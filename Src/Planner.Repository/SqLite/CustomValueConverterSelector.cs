using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Planner.Repository.SqLite
{
    public abstract class CustomValueConverterSelector : ValueConverterSelector
    {
        // The dictionary in the base type is private, so we need our own one here.
        private readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo> converters
            = new ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo>();
        
        public CustomValueConverterSelector(ValueConverterSelectorDependencies dependencies) : base(dependencies)
        {
        }

        public override IEnumerable<ValueConverterInfo> Select(Type modelClrType, Type? providerClrType = null) =>
            base.Select(modelClrType, providerClrType)
                .Concat(AdditionalConverters(modelClrType,providerClrType));

        [return:NotNullIfNotNull("type")]
        private static Type? UnwrapNullableType(Type? type) => 
            type is null ? null:(Nullable.GetUnderlyingType(type) ?? type);

        private IEnumerable<ValueConverterInfo> AdditionalConverters(Type? modelClrType, Type? providerClrType) => 
            modelClrType == null ? 
                Array.Empty<ValueConverterInfo>() :
                CreateConvertersForType(modelClrType, UnwrapNullableType(providerClrType)).OfType<ValueConverterInfo>();

        protected abstract ValueConverterInfo?[] CreateConvertersForType(Type modelClrType,
            Type? unwrappedProviderClrType);
        
        protected ValueConverterInfo? TryCreateVci<TModel, TProvider>(
            Type? unwrappedProviderClrType, Type modelClrType,
            Expression<Func<TModel, TProvider>> toFunc, Expression<Func<TProvider, TModel>> fromFunc) =>
            IsSupportedTypeMapping<TModel, TProvider>(unwrappedProviderClrType, modelClrType)?
                converters.GetOrAdd((modelClrType, typeof(TProvider)),
                    k=>ConstructValueConverterInfo(modelClrType, toFunc, fromFunc)): 
                null;

        private static bool IsSupportedTypeMapping<TModel, TProvider>(Type? unwrappedProviderClrType, Type modelClrType) =>
            ProviderTypeDesired<TProvider>(unwrappedProviderClrType) &&
            ModelTypeDesired<TModel>(UnwrapNullableType(modelClrType));
        
        private static ValueConverterInfo ConstructValueConverterInfo<TModel, TProvider>(Type modelClrType, 
            Expression<Func<TModel, TProvider>> toFunc, Expression<Func<TProvider, TModel>> fromFunc) =>
            new ValueConverterInfo(modelClrType, typeof(DateTime),
                i =>new ValueConverter<TModel,TProvider>(toFunc, fromFunc, i.MappingHints));

        private static bool ModelTypeDesired<T>(Type? unwrppedModelClrType) => unwrppedModelClrType == typeof(T);

        private static bool ProviderTypeDesired<T>(Type? unwrappedProviderClrType) => 
            unwrappedProviderClrType == null || unwrappedProviderClrType == typeof(T);
    }
}