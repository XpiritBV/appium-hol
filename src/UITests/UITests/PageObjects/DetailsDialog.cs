using OpenQA.Selenium.Appium.Windows;

namespace UITests.PageObjects
{
    public class DetailsDialog : BaseWinFormPageObject
    {


        public DetailsDialog(WindowsDriver<WindowsElement> driver) : base(driver)
        {

        }

        public MainForm CloseDetailDialog()
        {
            _driver.FindElementByAccessibilityId("button1").Click();
            return new MainForm(_driver);
        }

        public bool IsItemText(string itemText)
        {
            // verify we are showing the dialog
            var DetailsDialog = _driver.FindElementByAccessibilityId("Details");
            var itemTextFound = DetailsDialog.FindElementByAccessibilityId(itemText).Text;

            return itemTextFound == itemText;
        }

        public bool IsItemDetailText(string itemDetailText)
        {
            // verify we are showing the dialog
            var DetailsDialog = _driver.FindElementByAccessibilityId("Details");
            var itemDetailTextFound = DetailsDialog.FindElementByName(itemDetailText).Text;

            return itemDetailTextFound == itemDetailText;
        }

        public bool IsItemTextOnDialog(string itemText, string itemDetailText)
        {
            return (IsItemDetailText(itemDetailText) && IsItemText(itemText));
        }


    }
}