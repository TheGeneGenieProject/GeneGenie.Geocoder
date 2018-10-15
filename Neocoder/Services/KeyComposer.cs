// <copyright file="KeyComposer.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace Neocoder.Services
{
    using System;
    using System.Globalization;
    using System.Text;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Creates a key based on the address we want to look up so that we can check
    /// with reasonable confidence if we have searched for it (or something similar)
    /// before.
    ///
    /// Removes some duplicate punctuation and whitespace, then reverses the string
    /// whilst taking care not to destroy the intention of unicode modifiers (where
    /// a code is added after a character to modify it to display an accent or umlaut
    /// for example).
    ///
    /// The final result is lowercased which makes it easier to search storage based
    /// on a guess of the partition and row key. The reversing primarily enables us to
    /// split the key to become a partition and row key so that addresses in similar
    /// regions are located near each other in storage.
    ///
    /// For example;
    ///
    /// Bedfordshire,England         -> dnalgneerihsdrofdeb
    /// Northamptonshire, , England  -> dnalgneerihsnotpmahtron
    /// Bedfordshire,,England        -> dnalgneerihsdrofdeb
    ///
    /// If we split at character 12, a lot of English counties will be grouped on the
    /// same partition.
    /// </summary>
    public class KeyComposer
    {
        private readonly ILogger<KeyComposer> logger;

        public KeyComposer(ILogger<KeyComposer> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Inefficent way of generating a hopefully efficient address lookup key.
        /// It might be possible to do something with Span<T> but the payoff in performance
        /// may not be needed and Span is to be adopted in the framework itself.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GenerateSourceKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            try
            {
                var cleaned = RemoveDiacritics(value).Trim().ToLower();
                return Reverse(SimpleAsciiFilter(cleaned));
            }
            catch (Exception ex)
            {
                logger.LogError((int)LogEventIds.AddressKeyError, ex, "Unable to remove diacritics from '{value}'", value);

                var cleaned = SimpleAsciiFilter(value).Trim().ToLower();
                return Reverse(cleaned);
            }
        }

        private string SimpleAsciiFilter(string value)
        {
            var arr = value.ToCharArray();

            arr = Array.FindAll(arr, c => char.IsLetterOrDigit(c));
            return new string(arr);
        }

        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private string Reverse(string s)
        {
            var charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
