using System;
using Throne.World.Network;

namespace Throne.World.Security
{
    public sealed class SecurityViolationEventArgs : EventArgs
    {
        public WorldClient Client { get; private set; }
        public string Report { get; private set; }
        public string StackReport { get; private set; }
        public IncidentSeverityLevel Level { get; private set; }

        public SecurityViolationEventArgs(WorldClient offender, IncidentSeverityLevel level, string report, string stacktrace)
        {
            this.Client = offender;
            this.Level = level;
            this.Report = report;
            this.StackReport = stacktrace;
        }
    }
}
