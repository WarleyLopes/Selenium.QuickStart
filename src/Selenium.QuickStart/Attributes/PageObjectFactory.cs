using System;

namespace Selenium.QuickStart.Attributes
{
    /// <summary>
    /// Custom attribute type for object instanciation on test classes to interact with methods you create on your page classes
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PageObject : Attribute { }
}
