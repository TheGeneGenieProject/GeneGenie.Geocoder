// <copyright file="FakeLogger.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.Fakes
{
    /// <summary>
    /// Generic test logger for checking what <see cref="EventId"/> codes get written.
    /// </summary>
    /// <typeparam name="T">The class that the logger is for.</typeparam>
    internal sealed class FakeLogger<T> : ILogger<T>
    {
        /// <summary>
        /// The number of critical events written.
        /// </summary>
        public int CriticalCount { get; set; }

        /// <summary>
        /// The number of debug events written.
        /// </summary>
        public int DebugCount { get; set; }

        /// <summary>
        /// The number of error events written.
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// The number of informational events written.
        /// </summary>
        public int InformationCount { get; set; }

        /// <summary>
        /// The number of 'none' events written. This should really always be zero.
        /// </summary>
        public int NoneCount { get; set; }

        /// <summary>
        /// The number of trace level events written.
        /// </summary>
        public int TraceCount { get; set; }

        /// <summary>
        /// The number of warning events written.
        /// </summary>
        public int WarningCount { get; set; }

        /// <summary>
        /// The id's of the events written.
        /// </summary>
        public List<EventId> LoggedEventIds { get; } = new List<EventId>();

        /// <summary>
        /// We don't use this in our tests, only here to implement the interface.
        /// </summary>
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tests whether a log level is enabled. We log everything in test so always true.
        /// Only called by internal framework code, not our code.
        /// </summary>
        /// <param name="logLevel">The log level to test.</param>
        /// <returns>True</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <summary>
        /// Log an event to our test logger.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
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
