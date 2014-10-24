using Abp.Localization.Dictionaries.Xml;
using Shouldly;
using Xunit;

namespace Abp.Tests.Localization
{
    public class Test_XmlLocalizationDictionaryBuilder
    {
        [Fact]
        public void Can_Build_LocalizationDictionary_From_Xml_String()
        {
            var dictionary = XmlLocalizationDictionary.BuildFomXmlString(
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<localizationDictionary culture=""tr"">
  <texts>
    <text name=""hello"" value=""Merhaba"" />
    <text name=""world"">Dünya</text>
  </texts>
</localizationDictionary>"
                );

            dictionary.CultureInfo.Name.ShouldBe("tr");
            dictionary["hello"].ShouldBe("Merhaba");
            dictionary["world"].ShouldBe("Dünya");
        }
    }
}