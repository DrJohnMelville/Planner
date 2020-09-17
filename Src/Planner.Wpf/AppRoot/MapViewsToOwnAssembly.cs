using System;
using Melville.MVVM.Wpf.ViewFrames;
using Planner.Wpf.TaskList;

namespace Planner.Wpf.AppRoot
{
    public class MapViewsToOwnAssembly : ViewMappingConvention
    {
        protected override Type? ViewTypeFromName(object model, string viewTypeName) => 
            typeof(DailyTaskListView).Assembly.GetType(viewTypeName);

        protected override string? ViewTypeNameFromModel(object model) => 
            base.ViewTypeNameFromModel(model)?.Replace("Planner.WpfViews", "Planner.Wpf");
    }
}