// <copyright file="KeyComposerTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Geocoder.Tests
{
    using System.Collections.Generic;
    using GeneGenie.Geocoder.Services;
    using GeneGenie.Geocoder.Tests.Setup;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class KeyComposerTests
    {
        private readonly KeyComposer keyComposer;

        public KeyComposerTests()
        {
            keyComposer = ConfigureDi.Services.GetRequiredService<KeyComposer>();
        }

        public static IEnumerable<object[]> WhitespaceKeyValueData =>
            new List<object[]>
            {
                new object[] { null, string.Empty },
                new object[] { string.Empty, string.Empty },
                new object[] { " ", string.Empty },
                new object[] { "  ", string.Empty },
                new object[] { " \t ", string.Empty },
                new object[] { " \n\r \r\n \t ", string.Empty },
            };

        public static IEnumerable<object[]> PunctuationKeyValueData =>
            new List<object[]>
            {
                new object[] { "?", string.Empty },
                new object[] { "-", string.Empty },
                new object[] { "--", string.Empty },
                new object[] { "---", string.Empty },
                new object[] { "----", string.Empty },
                new object[] { "--a--", "a" },
                new object[] { " ---- ", string.Empty },
                new object[] { ".", string.Empty },
                new object[] { "..", string.Empty },
                new object[] { "...", string.Empty },
                new object[] { ",,,", string.Empty },
            };

        public static IEnumerable<object[]> NonPunctuationKeyValueData =>
            new List<object[]>
            {
                new object[] { "aa", "aa" },
                new object[] { "aaa", "aaa" },
                new object[] { " aa ", "aa" },
                new object[] { "  aaa  ", "aaa" },
            };

        public static IEnumerable<object[]> DiacriticKeyValueData =>
            new List<object[]>
            {
                new object[] { "Le\u0301s", "sel" },
            };

        public static IEnumerable<object[]> RealworldKeyValueData =>
            new List<object[]>
            {
                new object[] { "Tiaro, Queensland, Australia", "ailartsuadnalsneeuqorait" },
                new object[] { "Lostwithiel, Cornwall, England", "dnalgnellawnrocleihtiwtsol" },
                new object[] { "Fawley, Hampshire, England", "dnalgneerihspmahyelwaf" },
                new object[] { "Moulshem, Chelmsford, England", "dnalgnedrofsmlehcmehsluom" },
                new object[] { "Boreham", "maherob" },
                new object[] { "Debenham, Suffolk, England", "dnalgnekloffusmahnebed" },
                new object[] { "East Brisbane, Queensland, Australia", "ailartsuadnalsneeuqenabsirbtsae" },
                new object[] { "Horton, Isis, Queensland", "dnalsneeuqsisinotroh" },
                new object[] { "Mt Auburn Station, Via Gayndah, Qld Australia", "ailartsuadlqhadnyagaivnoitatsnrubuatm" },
                new object[] { "Ennis, Clare, Ireland", "dnalerieralcsinne" },
                new object[] { "Auburn Station, Queensland, Australia", "ailartsuadnalsneeuqnoitatsnrubua" },
                new object[] { "Country Queensland, Australia", "ailartsuadnalsneeuqyrtnuoc" },
                new object[] { "Clifton, Queensland, Australia", "ailartsuadnalsneeuqnotfilc" },
                new object[] { "Roma, Queensland, Australia", "ailartsuadnalsneeuqamor" },
                new object[] { "Harrows Weald, Great Stanmore, London, MDX", "xdmnodnoleromnatstaergdlaewsworrah" },
                new object[] { "Chelsea, Middlesex, England", "dnalgnexeselddimaeslehc" },
                new object[] { "Putney, London, England", "dnalgnenodnolyentup" },
                new object[] { "Bushey, Hertfordshire, England", "dnalgneerihsdroftrehyehsub" },
                new object[] { "Wareside, Hertfordshire, England", "dnalgneerihsdroftrehediseraw" },
                new object[] { "Pelham, Hertford, England", "dnalgnedroftrehmahlep" },
                new object[] { "Marston Herefordshire England", "dnalgneerihsdroferehnotsram" },
                new object[] { "Harrows Weald, Great Stanmore, London, Middlesex", "xeselddimnodnoleromnatstaergdlaewsworrah" },
                new object[] { "Great Stanmore, London, England", "dnalgnenodnoleromnatstaerg" },
                new object[] { "King Street, Longacre, London", "nodnolercagnolteertsgnik" },
                new object[] { "Beddegelert, North West Wales, UK", "kuselawtsewhtrontrelegeddeb" },
                new object[] { "Marylebone, Middlesex, England", "dnalgnexeselddimenobelyram" },
                new object[] { "Upwell, Norfolk, England", "dnalgneklofronllewpu" },
                new object[] { "Dartford, Kent, England", "dnalgnetnekdroftrad" },
                new object[] { "Adelaide, South Australia, Australia", "ailartsuaailartsuahtuosedialeda" },
                new object[] { "Greenslopes, Queensland, Australia", "ailartsuadnalsneeuqsepolsneerg" },
                new object[] { "Malvern, Victoria, Australia", "ailartsuaairotcivnrevlam" },
                new object[] { "Deniliquin, New South Wales, Australia", "ailartsuaselawhtuoswenniuqilined" },
                new object[] { "Foster, Providence Cty, RI", "irytcecnedivorpretsof" },
                new object[] { "Clayville, Providence, RI", "irecnedivorpellivyalc" },
                new object[] { "Foster, RI/Killingly, CT area", "aeratcylgnillikirretsof" },
                new object[] { "Killingly, Windham, CT area", "aeratcmahdniwylgnillik" },
                new object[] { "Little Compton, Newport, RI", "irtropwennotpmocelttil" },
                new object[] { "Newport, Newport, RI", "irtropwentropwen" },
                new object[] { "Jasper County, South Carolina, USA", "asuanilorachtuosytnuocrepsaj" },
                new object[] { "Gillisonville, Jasper, South Carolina", "anilorachtuosrepsajellivnosillig" },
                new object[] { "McBride's Ford, Beaufort Dist., SC", "cstsidtrofuaebdrofsedirbcm" },
                new object[] { "St Helena, Beaufort, South Carolina, USA", "asuanilorachtuostrofuaebanelehts" },
                new object[] { "Orangeburg, South Carolina, USA", "asuanilorachtuosgrubegnaro" },
                new object[] { "Carrick-on-Suir", "riusnokcirrac" },
                new object[] { "Springfield, Tipperary, Ireland", "dnaleriyrareppitdleifgnirps" },
                new object[] { "Solohead, Tipperary, Ireland", "dnaleriyrareppitdaeholos" },
                new object[] { "Oxfordshire, England", "dnalgneerihsdrofxo" },
                new object[] { "Garret, Merton, Surrey", "yerrusnotremterrag" },
                new object[] { "Cookham, Berkshire", "erihskrebmahkooc" },
                new object[] { "Watford, Hertfordshire", "erihsdroftrehdroftaw" },
                new object[] { "Chalfont St Peter, Buckinghamshire", "erihsmahgnikcubreteptstnoflahc" },
                new object[] { "Bainville, Montana, USA", "asuanatnomellivniab" },
                new object[] { "Doniphan, Missouri", "iruossimnahpinod" },
                new object[] { "Intake, Montana, USA", "asuanatnomekatni" },
                new object[] { "Concord, Fayette, Ohio, United States", "setatsdetinuoihoetteyafdrocnoc" },
                new object[] { "Green Twp, Fayette, Ohio", "oihoetteyafpwtneerg" },
                new object[] { "Fayette County, Ohio, USA", "asuoihoytnuocetteyaf" },
                new object[] { "Concord Twp., Fayette Co., Ohio", "oihoocetteyafpwtdrocnoc" },
                new object[] { "Ross, Ohio, United States", "setatsdetinuoihossor" },
                new object[] { "Berkley, James, Virginia, United States", "setatsdetinuainigrivsemajyelkreb" },
                new object[] { "Fairfax, Virginia or Culpepper, Virginia, USA", "asuainigrivreppeplucroainigrivxafriaf" },
                new object[] { "Lynchburg, Highland, Ohio, USA", "asuoihodnalhgihgrubhcnyl" },
                new object[] { "Green Twp., Fayette, Ohio, USA", "asuoihoetteyafpwtneerg" },
                new object[] { "Concord, Fayette, Ohio, USA", "asuoihoetteyafdrocnoc" },
                new object[] { "Washington Court House, Fayette County, Ohio, USA", "asuoihoytnuocetteyafesuohtruocnotgnihsaw" },
                new object[] { "Green Township, Fayette, Ohio, USA", "asuoihoetteyafpihsnwotneerg" },
                new object[] { "Rockbridge, Virginia, USA", "asuainigrivegdirbkcor" },
                new object[] { "Rockbridge Co, Virginia, USA", "asuainigrivocegdirbkcor" },
                new object[] { "Clinton County, Ohio, USA", "asuoihoytnuocnotnilc" },
                new object[] { "Hillsboro, Highland, Ohio, USA", "asuoihodnalhgihorobsllih" },
                new object[] { "Warren, Clinton, Ohio, USA", "asuoihonotnilcnerraw" },
                new object[] { "Wayne Twp., Warren, Ohio, USA", "asuoihonerrawpwtenyaw" },
                new object[] { "King George, Virginia", "ainigrivegroeggnik" },
                new object[] { "Stafford, Virginia, USA", "asuainigrivdroffats" },
                new object[] { "Fredericksburg Stafford Co VA", "avocdroffatsgrubskcirederf" },
                new object[] { "Stafford Co VA", "avocdroffats" },
                new object[] { "St Paul Parish, Stafford Cty, Virginia", "ainigrivytcdroffatshsirapluapts" },
                new object[] { "Cohoes, Albany, N.Y.", "ynynablaseohoc" },
            };

        [Theory]
        [MemberData(nameof(WhitespaceKeyValueData))]
        public void Location_keys_are_filtered_of_whitespace(string source, string expected)
        {
            var key = keyComposer.GenerateSourceKey(source);

            Assert.Equal(expected, key);
        }

        [Theory]
        [MemberData(nameof(PunctuationKeyValueData))]
        public void Location_keys_are_filtered_of_duplicate_punctuation(string source, string expected)
        {
            var key = keyComposer.GenerateSourceKey(source);

            Assert.Equal(expected, key);
        }

        [Theory]
        [MemberData(nameof(NonPunctuationKeyValueData))]
        public void Location_keys_are_not_filtered_of_duplicate_non_punctuation_chars(string source, string expected)
        {
            var key = keyComposer.GenerateSourceKey(source);

            Assert.Equal(expected, key);
        }

        [Theory]
        [MemberData(nameof(DiacriticKeyValueData))]
        public void When_reversing_the_key_diacritics_are_removed(string source, string expected)
        {
            var key = keyComposer.GenerateSourceKey(source);

            Assert.Equal(expected, key);
        }

        [Theory]
        [MemberData(nameof(RealworldKeyValueData))]
        public void Location_keys_are_generated_for_realworld_data(string source, string expected)
        {
            var key = keyComposer.GenerateSourceKey(source);

            Assert.Equal(expected, key);
        }
    }
}
