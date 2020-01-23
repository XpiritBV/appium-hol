# Writing Maintainable Test Automation

In this lab we will rewrite the previous tests so the code becomes more maintaintable.

We will apply the following principles:
* Single Responsibility (S in _SOLID_)
* Dont Repeat Yourself (_DRY_)
* Descriptive and Meaningfull Phrases (_DAMP_)

## Applying Single Responsibility

If you look at the tests we wrote, then you can see the test class takes a lot of responsibilities. It interacts with Appium, it starts the driver and cleans it up, in the tests we find element and we itneract with elements. How can we clean this up?

First start with moving the start and stop of the application to a class that will take care of this.

Create a new folder in the project called `PageObjects` and in this folder you create a new static class called `CarvedRockApplication`. This class implements two methods:

* `StartApplication()`
* `CloseApplication()`

Implement the `StartApplication` method as follows:

``` C#
public WindowsDriver<WindowsElement> StartApplication()
{
    var capabilities = new AppiumOptions();
    capabilities.AddAdditionalCapability(MobileCapabilityType.App, @"C:\temp\appium-hol\AppsToTest\WinForms\CarvedRock.exe");
    capabilities.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Windows");
    capabilities.AddAdditionalCapability(MobileCapabilityType.DeviceName, "WindowsPC");

    //start the application
    _driver = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723/wd/hub"), capabilities);
    return _driver;
}
```

And implement `CloseApplication` as follows:

```c#
public static void CloseApplication()
{ 
    if (_driver != null)
    {
        try
        {
            _driver.CloseApp();
        }

        finally
        {
            _driver.Dispose();
        }
    }

    _driver = null;
}
```

Next, you replace all the code that starts the application in the previous created test methods with a call to `CarvedRockApplication.StartApplication()`.

## Apply the "Dont Repeat Yourself" Principle

In the test code there is some duplicate code for finding things in the user interface. Apply the refactoring tools to extract these repetitions and place them in functions with a meaningful name.

Some examples:

Before:

```C#
// click add
var addButton = driver.FindElementByName("AddNewItem");
addButton.Click();
```

Refactor this into a function `ClickAddOnMainForm()` and replace the repetitions in the other test functions

Before:

```C#
//close the dialog
var closeButton = AddDialogWindow.FindElementByName("Close");
closeButton.Click();
```

Refactor this into a method named `CloseAddDialog()` and replace the repetitions in the other tests

Do this for most of the repetitions you find, and see how the code in the test becomes more readable and expressive of the intents you have instead of technical steps to get it done.

## Apply "Descriptive and Meaningful Phrases" pattern

This is a bit more involved type of refactoring, where we are going to apply a well known pattern in the test industry called the _PageObject Pattern_.

We create a class, a so called page object, for every window we have in our application. So for our CarvedRock application this means we write the following classes that relfect all windows in the application:
* A class `MainForm`, representing the _Main Form_ of the application
* A class `AddNewItemDialog`, representing the _Add New Item_ Dialog
* A class `ItemDetail`, representing the _Details_ dialog shown after clicking one row in the listview.

The `MainForm` will abstract away all interactions with the _Main Form_. This means it needs to get public methods that abstract things we can do on the form. In this application this is limited to the following functions:
* `AddNewItem`
   * This triggers the _Add New Item_ dialog and returns an `AddNewItemDialog` object 
* `ShowDetailOfItem`
  * This triggers the selection of an item in the listview and returns an instance of the `ItemDetail` class

Create the `MainForm` class first like this:

```c#
public class MainForm 
{
    public AddNewItemDialog OpenNewItemDialog()
    {
        // click add button and return new dialog object

        return new AddNewItemDialog();
    }

    public DetailsDialog SelectItemInList(string itemText)
    {
        // find item in the listview and return yourself
        return this;
    }

    public bool IsItemInListview(string itemText)
    {
        // verify we can find the item in the list
        return (true);
    }
}
``` 

Next, we create the class that abstracts the `AddNewItemDialog`. Here is the skeleton code for that class to get started:

```c#
public class AddNewItemDialog 
{
    public MainForm AddNewItem(string newItemText, string newItemDetailText)
    {
        // verify we are showing the dialog
        
        // find the input field for ItemText
        // clear the field
        // send the keys


        // find the input field for ItemDetail
        // clear the field
        // send the keys

        // close the dialog

        // return MainForm object
        return new MainForm();
    }
}
```

And create the class that abstracts the `Details Dialog`

```c#
public class DetailsDialog : BaseWinFormPageObject
{
    public MainForm CloseDetailDialog()
    {
        // close dialog and return MainForm
        return new MainForm();
    }
}
```

We now have the main structure to write a test that becomes a _meaningful phrase_.

For example, we can now construct the following steps we want to test:

```c#
//arrange
var mainForm = CarvedRockApplication.StartApplication();

//act
var modifiedForm = mainForm.OpenNewItemDialog()
    .AddNewItem("NewItem", "NewItemDetail")
    .SelectItemInList("NewItem")
    .CloseDetailDialog();

// assert

// close app
CarvedRockApplication.CloseApplication();
```

The only thing we need to do is to fix the fact that we need to pass the driver along in these classes, because all the methods that interact with the forms, need to use the driver to call one of the `FindBy*` methods.

So we change the `CarvedRockApplication.StartApplication();` function to return a new instance of the `MainForm` class and when we create this instance we pass the created driver to the constructor. Se we create a new constructor in the `MainForm` class.

Before we make this change, we want all page objects to have the same base behavior which is that it passes along the driver when it returns a new page object. This way the test code itself does not have to deal with this anymore, making the test clean.

Create a base class that all PageObjects will inherit from. This class looks as follows:

```c#
public class BaseWinFormPageObject
{
    protected WindowsDriver<WindowsElement> _driver;

    public BaseWinFormPageObject(WindowsDriver<WindowsElement> driver)
    {
        _driver = driver;
    }
}
```

Next, make all PageObject classes inherit from this base class and give them all a constructor that accepts the driver and passes it along to the base class. Here is the example for the `MainForm` class:

```c#
public class MainForm : BaseWinFormPageObject
{
    public MainForm(WindowsDriver<WindowsElement> driver) : base(driver) { }
}
```

Do this for all page objects you created and make changes to the action methods that return new instances of a page object, and pass the `_driver` to the constructor.

Finally, we need to implement the actual methods to interact with the windows. For this, you can borrow all the code you already wrote in the intial tests. 

For example, to click on the _Add_ button on the main form you can use the following code and add it to the `MainForm` method `OpenNewItemDialog()`:

```c#
public AddNewItemDialog OpenNewItemDialog()
{
    // click add
    var addButton = _driver.FindElementByName("AddNewItem");
    addButton.Click();

    return new AddNewItemDialog(_driver);
}
```

And for the method to select an item and show the details dialog you can do the following. I added some extra functionality to scroll to an item that we want to select:

```c#
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
```

Continue implementing all methods until we can really test the application.

You have completed the lab if you can run the following test with success:

```C#
[TestMethod]
public void More_Maintainable_Version_Multiple_Add_Item_Dialog_AddsNewItem()
{
    // arrange
    var mainForm = CarvedRockApplication.StartApplication();
    var numIterations = 5;
    MainForm modifiedForm = null;

    // act   
    while (numIterations-- > 0)
    {
        modifiedForm = mainForm
            .OpenNewItemDialog()
            .AddNewItem("NewItem" + numIterations, "NewItemDetail" + numIterations);
    }

    //assert
    Assert.IsTrue(modifiedForm.IsItemInListview("NewItem4"));
    CarvedRockApplication.CloseApplication();
}
```

It is easy to read that:
* we start the application
* open the _New Item_ dialog multiple times
* validate we can find the 4th item we added to the list

This test is maintainable, because when a change is made to e.g. the _Add New Item_ dialog, you only need to make a change in the way you interact with the dialog in that specific class. All tests always keep compiling and -when implemented correctly in the PageObject representing the dialog-, the test will also keep running.

If you want to see the results of this refactoring you can also switch to the `MaintainableCode` branch in this repo. You will find the full implementation.