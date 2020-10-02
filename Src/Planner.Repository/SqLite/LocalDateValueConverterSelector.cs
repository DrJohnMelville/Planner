using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;

namespace Planner.Repository.SqLite
{

    public class LocalDateValueConverterSelector : CustomValueConverterSelector
    {
        public LocalDateValueConverterSelector(ValueConverterSelectorDependencies dependencies) : base(dependencies)
        {
        }
        protected override ValueConverterInfo?[] CreateConvertersForType(Type modelClrType, Type? unwrappedProviderClrType) =>
            new[]
            {
                TryCreateVci<LocalDate, DateTime>(unwrappedProviderClrType, modelClrType,
                    i => i.ToDateTimeUnspecified(), i => LocalDate.FromDateTime(i)),
                TryCreateVci<Instant, long>(unwrappedProviderClrType, modelClrType, 
                    i=>i.ToUnixTimeMilliseconds(), i=>Instant.FromUnixTimeMilliseconds(i)),
            };
    }
}