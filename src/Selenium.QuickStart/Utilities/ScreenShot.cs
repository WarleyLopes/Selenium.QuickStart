﻿using OpenQA.Selenium;
using Selenium.QuickStart.Core;
using System;

namespace Selenium.QuickStart.Utilities
{
    public static class ScreenShot
    {
        public static string CaptureAsBase64EncodedString()
        {
            ITakesScreenshot ts = (ITakesScreenshot)WebDriverHooks.Driver;
            try
            {
                Screenshot screenshot = ts.GetScreenshot();
                return screenshot.AsBase64EncodedString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
