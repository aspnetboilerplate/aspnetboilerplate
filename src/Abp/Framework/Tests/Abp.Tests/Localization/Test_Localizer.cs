using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Localization.Engine;
using Abp.Localization.Sources.XmlFiles;
using NUnit.Framework;

namespace Abp.Tests.Localization
{
    [TestFixture]
    public class Test_Localizer
    {
        private LocalizationEngine _localizationEngine;

        [TestFixtureSetUp]
        public void Initialize()
        {
            _localizationEngine = new LocalizationEngine();

            _localizationEngine.AddDictionary(
                new CultureInfo("en"),
                new LocalizationDictionary
                    {
                        {"hello", "Hello"},
                        {"world", "World"},
                        {"FourtyTwo", "Fourty Two (42)"}
                    }, isDefault: true);

            _localizationEngine.AddDictionary(
                new CultureInfo("tr"),
                new LocalizationDictionary
                    {
                        {"hello", "Merhaba"},
                        {"world", "Dünya"},
                    });

            _localizationEngine.AddDictionary(
                new CultureInfo("tr-TR"),
                new LocalizationDictionary
                    {
                        {"world", "Yeryüzü"},
                    });
        }

        [Test]
        public void Should_Get_Correct_String_On_Exact_Culture()
        {
            Assert.AreEqual("Yeryüzü", _localizationEngine.GetString("world", new CultureInfo("tr-TR")));
        }

        [Test]
        public void Should_Get_Most_Close_String_On_Base_Culture()
        {
            Assert.AreEqual("Merhaba", _localizationEngine.GetString("hello", new CultureInfo("tr-TR")));
        }

        [Test]
        public void Should_Get_Default_If_Not_Exists_On_Given_Culture()
        {
            Assert.AreEqual("Fourty Two (42)", _localizationEngine.GetString("FourtyTwo", new CultureInfo("tr")));
            Assert.AreEqual("Fourty Two (42)", _localizationEngine.GetString("FourtyTwo", new CultureInfo("tr-TR")));
        }

        [Test]
        public void Should_Get_All_Strings()
        {
            var localizedStrings = _localizationEngine.GetAllStrings(new CultureInfo("tr-TR")).OrderBy(ls => ls.Name).ToList();
            Assert.AreEqual(3, localizedStrings.Count);
            Assert.AreEqual("Fourty Two (42)", localizedStrings[0].Value);
            Assert.AreEqual("Merhaba", localizedStrings[1].Value);
            Assert.AreEqual("Yeryüzü", localizedStrings[2].Value);
        }
    }
}
