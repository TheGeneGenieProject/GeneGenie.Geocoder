﻿// <copyright file="Program.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Console
{
    /// <summary>
    /// Console app demonstrating varying uses of the <see cref="GeocodeManager"/> class which is the main method used to geocode addresses.
    /// </summary>
    public class Program
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
        protected static async Task Main(string[] args)
        {
            var choice = RenderAndSelectSampleChoice();

            switch (choice)
            {
                case SampleChoice.SimpleLookup:
                    var simpleLookup = new SimpleLookup();
                    await simpleLookup.ExecuteAsync();
                    break;
                case SampleChoice.DependencyInjection:
                    var diLookup = new DependencyInjectionLookup();
                    await diLookup.ExecuteAsync(args);
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

            switch (pressed.Key)
            {
                case ConsoleKey.D1:
                    return SampleChoice.SimpleLookup;
                case ConsoleKey.D2:
                    return SampleChoice.DependencyInjection;
                default:
                    return SampleChoice.Exit;
            }
        }
    }
}
