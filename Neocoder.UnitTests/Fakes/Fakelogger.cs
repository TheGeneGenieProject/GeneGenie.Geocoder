// <copyright file="Fakelogger.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.UnitTests.Fakes
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Neocoder.Services;

    public class Fakelogger : ILogger<GoogleGeocoder>
    {
        public int Critical { get; set; }

        public int Debug { get; set; }

        public int Error { get; set; }

        public int Information { get; set; }

        public int None { get; set; }

        public int Trace { get; set; }

        public int Warning { get; set; }

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
                Critical++;
            }
            else if (logLevel == LogLevel.Debug)
            {
                Debug++;
            }
            else if (logLevel == LogLevel.Error)
            {
                Error++;
            }
            else if (logLevel == LogLevel.Information)
            {
                Information++;
            }
            else if (logLevel == LogLevel.Trace)
            {
                Trace++;
            }
            else if (logLevel == LogLevel.Warning)
            {
                Warning++;
            }
            else
            {
                None++;
            }

            LoggedEventIds.Add(eventId);
        }
    }
}
