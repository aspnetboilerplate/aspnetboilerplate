using System;

namespace Abp.HtmlSanitizer;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class DisableHtmlSanitizerAttribute : Attribute
{

}