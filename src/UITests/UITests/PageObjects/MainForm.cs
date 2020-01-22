using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Collections.Generic;
using System.Text;

namespace UITests.PageObjects
{
    public class MainForm : BaseWinFormPageObject
    {
        public MainForm(WindowsDriver<WindowsElement> driver) : base(driver) { }

        public AddNewItemDialog OpenNewItemDialog()
        {
            // click add
            var addButton = _driver.FindElementByName("AddNewItem");
            addButton.Click();

            return new AddNewItemDialog(_driver);
        }

        public DetailsDialog SelectItemInList(string itemText)
        {
            while(!IsItemInListview(itemText))
            {
                ScrollDown();
            }

            FindElementByTextInList(itemText).Click();
            return new DetailsDialog(_driver);
        }

       

        public bool IsItemInListview(string itemText)
        {
            // verify we can find the item in the list
            AppiumWebElement newElement = FindElementByTextInList(itemText);

            return (newElement != null);
        }

        private AppiumWebElement FindElementByTextInList(string itemText)
        {
            var listview = _driver.FindElementByAccessibilityId("listView1");
            var newElement = listview.FindElementByName(itemText);
            return newElement;
        }

        private void ScrollDown()
        {
            var scrollDownButton = _driver.FindElementByAccessibilityId("DownButton");
            scrollDownButton.Click();
            scrollDownButton.Click();
        }
    }
}
