using System.Diagnostics;
using CleanArchitectureUtility.Utilities.SerilogRegistration.Extensions;
using Serilog.Core;
using Serilog.Events;

namespace CleanArchitectureUtility.Utilities.SerilogRegistration.Enrichers;

public class ActivityEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var activity = Activity.Current;

        logEvent.AddPropertyIfAbsent(new LogEventProperty("SpanId", new ScalarValue(activity.GetSpanId())));
        logEvent.AddPropertyIfAbsent(new LogEventProperty("TraceId", new ScalarValue(activity.GetTraceId())));
        logEvent.AddPropertyIfAbsent(new LogEventProperty("ParentId", new ScalarValue(activity.GetParentId())));
    }
}