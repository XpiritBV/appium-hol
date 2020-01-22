using OpenQA.Selenium.Appium.Windows;

namespace UITests.PageObjects
{
    public class AddNewItemDialog : BaseWinFormPageObject
    {
        public AddNewItemDialog(WindowsDriver<WindowsElement> driver):base(driver)
        {
            
        }

        public MainForm AddNewItem(string newItemText, string newItemDetailText)
        {
            // verify we are showing the dialog
            var AddDialogWindow = _driver.FindElementByAccessibilityId("NewItemForm");

            var inputFieldItem = AddDialogWindow.FindElementByName("ItemText");
            inputFieldItem.Clear();
            inputFieldItem.SendKeys(newItemText);

            var InputFieldItemDetail = AddDialogWindow.FindElementByName("ItemDetail");
            InputFieldItemDetail.Clear();
            InputFieldItemDetail.SendKeys(newItemDetailText);

            //close the dialog
            var AddButton = AddDialogWindow.FindElementByAccessibilityId("button1");
            AddButton.Click();

            return new MainForm(_driver);
        }
    }
}