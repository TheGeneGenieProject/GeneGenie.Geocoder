// <copyright file="FakeLogger.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Generic test logger for checking what <see cref="EventId"/> codes get written.
    /// </summary>
    /// <typeparam name="T">The class that the logger is for.</typeparam>
    public class FakeLogger<T> : ILogger<T>
    {
        public int CriticalCount { get; set; }

        public int DebugCount { get; set; }

        public int ErrorCount { get; set; }

        public int InformationCount { get; set; }

        public int NoneCount { get; set; }

        public int TraceCount { get; set; }

        public int WarningCount { get; set; }

        public List<EventId> LoggedEventIds { get; set; } = new List<EventId>();

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel == LogLevel.Critical)
            {
                CriticalCount++;
            }
            else if (logLevel == LogLevel.Debug)
            {
                DebugCount++;
            }
            else if (logLevel == LogLevel.Error)
            {
                ErrorCount++;
            }
            else if (logLevel == LogLevel.Information)
            {
                InformationCount++;
            }
            else if (logLevel == LogLevel.Trace)
            {
                TraceCount++;
            }
            else if (logLevel == LogLevel.Warning)
            {
                WarningCount++;
            }
            else
            {
                NoneCount++;
            }

            LoggedEventIds.Add(eventId);
        }
    }
}
