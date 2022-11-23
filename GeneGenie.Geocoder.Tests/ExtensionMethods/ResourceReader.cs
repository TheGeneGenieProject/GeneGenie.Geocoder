// <copyright file="ResourceReader.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests.ExtensionMethods
{
    using System.IO;
    using System.Reflection;

    internal static class ResourceReader
    {
        public static string ReadEmbeddedFile(string resourceName)
        {
            var assembly = typeof(ResourceReader).GetTypeInfo().Assembly;
            var fileName = resourceName.Replace("/", ".");

            using (var stream = assembly.GetManifestResourceStream(fileName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Could not find {fileName} embedded as a resource.");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}