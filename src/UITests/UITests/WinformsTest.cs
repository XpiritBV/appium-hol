using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using UITests.PageObjects;

namespace UITests
{
    [TestClass]
    public class WinformsTest
    {

        static TestContext _ctx;

        [ClassInitialize]
        public static void Initialize(TestContext ctx)
        {
            _ctx = ctx;
        }

        [TestMethod]
        public void Test_Application_Start_and_Stop()
        {
            var capabilities = new AppiumOptions();
            capabilities.AddAdditionalCapability(MobileCapabilityType.App, @"C:\temp\appium-hol\AppsToTest\WinForms\CarvedRock.exe");
            capabilities.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Windows");
            capabilities.AddAdditionalCapability(MobileCapabilityType.DeviceName, "WindowsPC");

            //start the application
            var driver = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723/wd/hub"), capabilities);

            var screenshotFilename = Guid.NewGuid().ToString() + ".png";
            driver.GetScreenshot().SaveAsFile(screenshotFilename);
            _ctx.AddResultFile(screenshotFilename);

            // stop the application
            driver.CloseApp();
            driver.Dispose();
        }

        [TestMethod]
        public void Test_Application_Add_Item_Dialog_Is_Shown()
        {

            var capabilities = new AppiumOptions();
            capabilities.AddAdditionalCapability(MobileCapabilityType.App, @"C:\temp\appium-hol\AppsToTest\WinForms\CarvedRock.exe");
            capabilities.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Windows");
            capabilities.AddAdditionalCapability(MobileCapabilityType.DeviceName, "WindowsPC");

            //start the application
            var driver = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723/wd/hub"), capabilities);

            // click add
            var addButton = driver.FindElementByName("AddNewItem");
            addButton.Click();

            // verify we are showing the dialog
            var AddDialogWindow = driver.FindElementByAccessibilityId("NewItemForm");

            var textLabel1 = AddDialogWindow.FindElementByAccessibilityId("label1");
            Assert.IsTrue(textLabel1 != null);

            //close the dialog
            var closeButton = AddDialogWindow.FindElementByName("Close");
            closeButton.Click();

            //stop the application
            driver.CloseApp();
            driver.Dispose();

        }

        [TestMethod]
        public void Test_Application_Add_Item_Dialog_AddsNewItem()
        {
            var capabilities = new AppiumOptions();
            capabilities.AddAdditionalCapability(MobileCapabilityType.App, @"C:\temp\appium-hol\AppsToTest\WinForms\CarvedRock.exe");
            capabilities.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Windows");
            capabilities.AddAdditionalCapability(MobileCapabilityType.DeviceName, "WindowsPC");

            //start the application
            var driver = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723/wd/hub"), capabilities);

            // click add
            var addButton = driver.FindElementByName("AddNewItem");
            addButton.Click();

            // verify we are showing the dialog
            var AddDialogWindow = driver.FindElementByAccessibilityId("NewItemForm");

            var textLabel1 = AddDialogWindow.FindElementByAccessibilityId("label1");
            Assert.IsTrue(textLabel1 != null);

            var inputFieldItem = AddDialogWindow.FindElementByName("ItemText");
            inputFieldItem.Clear();
            inputFieldItem.SendKeys("New Item Text");

             var InputFieldItemDetail = AddDialogWindow.FindElementByName("ItemDetail");
            InputFieldItemDetail.Clear();
            InputFieldItemDetail.SendKeys("New item details text");

            //close the dialog
            var AddButton = AddDialogWindow.FindElementByAccessibilityId("button1");
            AddButton.Click();

            // scroll the list to the end
            var scrollDownButton = driver.FindElementByAccessibilityId("DownButton");
            scrollDownButton.Click();
            scrollDownButton.Click();

            // verify we can find the item in the list
            var listview = driver.FindElementByAccessibilityId("listView1");
            var newElement = listview.FindElementByName("New Item Text");

            Assert.IsTrue(newElement != null);

            //stop the application
            driver.CloseApp();
            driver.Dispose();
        }

        [TestMethod]
        public void More_Maintainable_Version_Add_Item_Dialog_AddsNewItem()
        {
            //arrange
            var mainForm = CarvedRockApplication.StartApplication();

            //act
            var modifiedForm = mainForm.OpenNewItemDialog()
                .AddNewItem("NewItem", "NewItemDetail")
                .SelectItemInList("NewItem")
                .CloseDetailDialog()
                ;

            //assert
            Assert.IsTrue(modifiedForm.IsItemInListview("NewItem")) ;
            CarvedRockApplication.CloseApplication();
        }

        [TestMethod]
        public void More_Maintainable_Version_Multiple_Add_Item_Dialog_AddsNewItem()
        {
            //arrange
            var mainForm = CarvedRockApplication.StartApplication();
            var numIterations = 5;
            //act

            MainForm modifiedForm=null;
            while (numIterations-- > 0)
            {
                 modifiedForm = mainForm.OpenNewItemDialog()
                    .AddNewItem("NewItem" + numIterations, "NewItemDetail" + numIterations);
            }

            //assert
            Assert.IsTrue(modifiedForm.IsItemInListview("NewItem4"));
            CarvedRockApplication.CloseApplication();
        }
        
    }
}
