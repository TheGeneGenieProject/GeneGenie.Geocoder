// <copyright file="Program.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Console
{
    /// <summary>
    /// Console app demonstrating varying uses of the <see cref="GeocodeManager"/> class which is the main method used to geocode addresses.
    /// </summary>
    public static class Program
    {
        private enum SampleChoice
        {
            Exit = 0,
            SimpleLookup = 1,
            DependencyInjection = 2,
        }

        /// <summary>
        /// Main app entry point.
        /// </summary>
        /// <param name="args">Command line arguments, can be used to override the configuration json file in the Dependency Injection example.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        internal static async Task Main(string[] args)
        {
            var choice = RenderAndSelectSampleChoice();

            switch (choice)
            {
                case SampleChoice.SimpleLookup:
                    await SimpleLookup.ExecuteAsync().ConfigureAwait(false);
                    break;
                case SampleChoice.DependencyInjection:
                    await DependencyInjectionLookup.ExecuteAsync(args).ConfigureAwait(false);
                    break;
                default:
                    System.Console.WriteLine("Could not figure out selection, exiting.");
                    break;
            }

            System.Console.WriteLine("Complete, press a key to quit");
            System.Console.ReadKey();
        }

        private static SampleChoice RenderAndSelectSampleChoice()
        {
            System.Console.WriteLine("GeneGenie.Geocoder samples");
            System.Console.WriteLine();
            System.Console.WriteLine("1. Use the simplest lookup (lets the library auto-wire itself up).");
            System.Console.WriteLine("2. Use with .Net Core Dependency Injection.");
            System.Console.WriteLine();

            var pressed = System.Console.ReadKey(true);
            System.Console.WriteLine();

            if (pressed.Key == ConsoleKey.D1)
            {
                return SampleChoice.SimpleLookup;
            }
            if (pressed.Key == ConsoleKey.D2)
            {
                return SampleChoice.DependencyInjection;
            }
            return SampleChoice.Exit;
        }
    }
}
