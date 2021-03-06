﻿using System.Windows.Data;
using Melville.MVVM.Wpf.Bindings;
using NodaTime;

namespace Planner.Wpf.TaskList
{
    public static class DateConverter{
        public static readonly IValueConverter Instance = LambdaConverter.Create(
            (LocalDate ld)=>ld.ToDateTimeUnspecified(), LocalDate.FromDateTime);
    }
}