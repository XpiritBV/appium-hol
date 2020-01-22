using OpenQA.Selenium.Appium.Windows;

namespace UITests.PageObjects
{
    public class BaseWinFormPageObject
    {
        protected WindowsDriver<WindowsElement> _driver;

        public BaseWinFormPageObject(WindowsDriver<WindowsElement> driver)
        {
            _driver = driver;
        }
    }
}