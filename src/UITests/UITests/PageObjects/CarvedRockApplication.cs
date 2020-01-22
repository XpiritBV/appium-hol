using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Collections.Generic;
using System.Text;

namespace UITests.PageObjects
{
    public static class CarvedRockApplication
    {
        static WindowsDriver<WindowsElement> driver;
        public static MainForm StartApplication()
        {
            var capabilities = new AppiumOptions();
            capabilities.AddAdditionalCapability(MobileCapabilityType.App, @"C:\temp\appium-hol\AppsToTest\WinForms\CarvedRock.exe");
            capabilities.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Windows");
            capabilities.AddAdditionalCapability(MobileCapabilityType.DeviceName, "WindowsPC");

            //start the application
            driver = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723/wd/hub"), capabilities);

            return new MainForm(driver);
        }

        public static void CloseApplication()
        { 
            if(driver!=null)
            {
                try
                {
                    driver.CloseApp();
                }

                finally
                {
                    driver.Dispose();
                }
            }

            driver = null;
        }
    }
}
