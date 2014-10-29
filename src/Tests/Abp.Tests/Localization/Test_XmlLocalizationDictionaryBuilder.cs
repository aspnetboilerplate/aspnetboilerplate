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
            var xmlString = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<localizationDictionary culture=""tr"">
  <texts>
    <text name=""hello"" value=""Merhaba"" />
    <text name=""world"">Dünya</text>
  </texts>
</localizationDictionary>";

            var dictionary = XmlLocalizationDictionary.BuildFomXmlString(xmlString);

            dictionary.CultureInfo.Name.ShouldBe("tr");
            dictionary["hello"].ShouldBe("Merhaba");
            dictionary["world"].ShouldBe("Dünya");
        }

        [Fact]
        public void Should_Throw_Exception_For_Duplicate_Name()
        {
            var xmlString = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<localizationDictionary culture=""tr"">
  <texts>
    <text name=""hello"" value=""Merhaba"" />
    <text name=""hello"" value=""Merhabalar""></text>
  </texts>
</localizationDictionary>";

            Assert.Throws<AbpException>(() => XmlLocalizationDictionary.BuildFomXmlString(xmlString));
        }
    }
}