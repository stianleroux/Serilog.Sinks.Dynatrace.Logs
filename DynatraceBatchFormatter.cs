namespace Serilog.Sinks.Dynatrace.Logs;

using System.Text;
using Serilog.Sinks.Http;
internal class DynatraceBatchFormatter(long? eventBodyLimitBytes = 256 * 1024) : IBatchFormatter
{
    private readonly long eventBodyLimitBytes = eventBodyLimitBytes ?? 256 * 1024;

    public void Format(IEnumerable<string> logEvents, TextWriter output)
    {
        if (logEvents is null)
            throw new ArgumentException(nameof(logEvents));
        if (output is null)
            throw new ArgumentNullException(nameof(output));

        var any = false;
        var delimiter = "[";

        foreach (var logEvent in logEvents)
        {
            if (string.IsNullOrWhiteSpace(logEvent)) continue;

            if (Encoding.UTF8.GetByteCount(logEvent) <= this.eventBodyLimitBytes)
            {
                output.Write(delimiter);
                output.Write(logEvent);
                delimiter = ",";
                any = true;
            }
        }

        if (any)
        {
            output.Write(']');
        }
    }
}
