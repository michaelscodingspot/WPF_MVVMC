# Wpf.MVVMC


[![Build status](https://img.shields.io/nuget/v/Wpf.Mvvmc.svg)](https://www.nuget.org/packages/Wpf.MVVMC/)

Nuget: `Install-Package Wpf.MVVMC`

## Description
This project is a navigation framework for WPF, which implements the MVVMC pattern. MVVMC adds Controllers to MVVM, which are responsible for navigation and switching between views (screens or parts of screen).

In MVVMC, the View and ViewModel will request a navigation action from the controller. The controller will create the new View and ViewModel instances. This way, we achieve a separation of concerns, and the View & ViewModel are responsible only to themselves, and don't create or know about other Views.

To read more about MVVMC and the motivation for this framework, see the original blog posts: [Part 1](http://michaelscodingspot.com/2017/02/06/wpf-page-navigation-like-mvc-building-mvvm-framework-controllers/), [Part 2](https://michaelscodingspot.com/2017/02/15/wpf-page-navigation-like-mvc-part-2-mvvmc-framework/).

# Documentation 

* [Quickstart](#quickstart)
* [Regions](#regions)
* [File naming](#file-naming)
* [Controllers](#controllers)
* [Views](#views)
* [ViewModels](#viewmodels)
* [Navigation service](#navigation-service)
* [Go Back and Forward](#go-back-and-forward)

## Quickstart

Let's build a small Wizard application with 3 steps in it. First, create a WPF application and add the __Wpf.MVVMC__ Nuget package.

#### Step 1: Create a Region
Add a [Region](#regions) to the MainWindow, like this:
```xaml
<Window 
	xmlns:mvvmc="clr-namespace:MVVMC;assembly=MVVMC"
	...>
    <mvvmc:Region ControllerID="Wizard"></mvvmc:Region>
</Window>
```
A Region is an area on the screen with dynamic content, controlled by a Controller. 
The Region's controller is deterimned by the __ControllerID__ property which is set to "Wizard". Wpf.MVVMC is a convention based framework, so naming matters. In this case, we'll have to create a controller class called "WizardController". The Controller will be responsible for navigating between the wizard steps.

#### Step 2: Create a Controller

```csharp
public class WizardController : Controller
{
    public override void Initial()
    {
        FirstStep();
    }

    public void Next()
    {
        var currentVM = GetCurrentViewModel();
        if (currentVM is FirstStepViewModel)
        {
            SecondStep();
        }
        else if (currentVM is SecondStepViewModel)
        {
            ThirdStep();
        }
        else
        {
            MessageBox.Show("Finished!");
            App.Current.MainWindow.Close();
        }
    }

    private void FirstStep()
    {
        ExecuteNavigation();
    }

    private void SecondStep()
    {
        ExecuteNavigation();
    }

    private void ThirdStep()
    {
        ExecuteNavigation();
    }
}
```

__ExecuteNavigation()__ depends on the calling method name. When called from "FirstStep()" for example, it will navigate to "FirstStep" page. Which means it will create __FirstStepView__ and __FirstStepViewModel__ instances, and connect them for binding.

#### Step 3: Add Views
The View can be any WPF control, like a simple UserControl. It should be in __the same namespace__ as the Controller and the ViewModel. Let's add 3 User Controls to the project called __FirstStepView__, __SecondStepView__ and __ThirdStepView__. Each will have a caption and a __Next__ button. For example, FirstStepView.xaml will be:

```xaml
<UserControl x:Class="MvvmcQuickstart1.FirstStepView"
	     xmlns:mvvmc="clr-namespace:MVVMC;assembly=MVVMC"
             ...>
    <StackPanel>
    	<TextBlock>First step</TextBlock>
	<Button Command="{mvvmc:NavigateCommand Action=Next, ControllerID=Wizard}">Next</Button>
    </StackPanel>
</UserControl>
```
Using __mvvmc:NavigateCommand__ allows to navigate directly from the View. You can choose to leave it as is and the program is done! Creating a View-Model is optional. To initiate the navigation from the View-Model, you'll need to change the View to this:

```xaml
<UserControl x:Class="MvvmcQuickstart1.FirstStepView"
             ...>
    <StackPanel>
    	<TextBlock>First step</TextBlock>
	<Button Command="{Binding NextCommand}">Next</Button>
    </StackPanel>
</UserControl>
```

#### Step 4: Add ViewModels
The ViewModels need to be called same as the Views with the __ViewModel__ postfix, and in the same namespace. So we'll add __FirstStepViewModel__, __SecondStepViewModel__ and __ThirdStepViewModel__ classes. Each ViewModel needs to inherit from __MVVMCViewModel__ base class. For example, FirstStepViewModel class will be:

```csharp
using System.Windows.Input;
using MVVMC;
...

public class FirstStepViewModel : MVVMCViewModel
{
    public ICommand _nextCommand { get; set; }

    public ICommand NextCommand
    {
        get
        {
            if (_nextCommand == null)
            {
                _nextCommand = new DelegateCommand(() =>
                {
                    GetController().Navigate("Next", parameter: null);
                });
            }
            return _nextCommand;
        }
    }
}
```

(__DelegateCommand__ used here is part of the MVVMC package.)

That's it. We have a finished 3-step wizard.

#### The result:

With just a little bit of styling, the resulting program looks like this:

![Quickstart result](/Documentation/quickstart-result.gif)



## Regions:
A Region is a Control which simply contains a content presenter with dynamic content. On navigation, the content changes to the target View. Each region area is controlled by a single controller, which is specified by the __ControllerID__ property. 
```xaml
xmlns:mvvmc="clr-namespace:MVVMC;assembly=MVVMC"
...
<mvvmc:Region ControllerID="XXX" />
```
The Controller, in turn, controls a single Region, so there's 1 to 1 relation between Region and Controller.

Connecting Region and Controller happens with reflection. The framework looks for a class named [ControllerId]Controller that inherits from MVVMC.Controller.

In applications where you want the navigation to occur on the entire screen, the Window contents should be only the Region.
```xaml
<Window 
	xmlns:mvvmc="clr-namespace:MVVMC;assembly=MVVMC"
	...>
    <mvvmc:Region ControllerID="Wizard"></mvvmc:Region>
</Window>
```

The number of Regions is not limited. So in MainWindow.xaml, you might have a Region for the top bar, a Region for the Main-Content and a Region for the footer.

Regions can be nested. You can have a Region which navigates to some Page, which in turn can include additional Regions.

Sometimes, you'll want several different Controllers to control the same screen area. For example, the application has several full-screen flows which include multiple screens each. In that case, you'll have one region for the "MainController" with a Page for each Flow. Each of those pages will include another Region responsible for their respected flows.

> A __Page__ means a pair of a View and a ViewModel, where the ViewModel is optional. So the page "Employees" means there's a WPF UI element "EmployeesView" and optionally a class "EmployeesViewModel". A Page doesn't have to be full-screen sized. The size will be according to the Region's space on screen.

## Naming convention:
Wpf.MVVMC is convention based. The naming rules are:
1. Each Controller is in it's own namespace.
2. Views and ViewModels are controlled by a single Controller and should be in the same namespace as the controller.
3. A pair of a View and a ViewModel are called a Page, and should be named XXXView and XXXViewModel, with 'XXX' being the page's name.

It's recommended to create a separate folder for each Controller. This folder will contain the Controller class with the Views and ViewModels relevant to that Controller. This way, they will have a common and unique namespace.

## Controllers:
A controller contains the actual navigation logic. Each controller is connected to a single Region and the navigation executes by replacing the Region's content.

Each Controller should dervive from the base class __MVVMC.Controller__.

Each method in the controller can be considered an __Action__. When an Action method calls __ExecuteNavigation()__, the controller will create a View and ViewModel instance of the name of the same Action. For example:
```csharp
public class MyController : Controller
{
    public void Employees()
    {
        ExecuteNavigation()
    }
```
In this Controller we have the action "Employees". When called, an intance of "EmployeesView" and "EmployeesViewModel" will be created and the relevant Region's content will be replaced. If "EmployeesView" is not found in the same namespace as the Controller, exception will be thrown.

* Each Controller should implement the __Initial()__ Action method to determine which Page will be created when the Region is loaded.
* You can use the Navigate() method to go to a different action.
* Each Action method can be called with or without a parameter. The parameter is of type __object__.
* ExecuteNavigation can be called with an object parameter, and a ViewBag dictionary. Both of these will be populated in the ViewModel as properties. The View can bind to the ViewBag directly with mvvmc:ViewBagBinding - More on those features further on.
* The controller can call __GetCurrentPageName()__ to get the current page name in the Region.
* The controller can call __GetCurrentViewModel()__ to get the instance of the current ViewModel.

Here's another example:

```csharp
public class MטController : Controller
{
    public void Initial()
    {
    	ExecuteNavigation();//Will create InitialView and InitialViewModel
    }
    
    public void HireEmployee(object employee)
    {
        if (CanHire(employee))
		Navigate("HireStart", employee);
    	else
		HireError();
    }
    
    public void HireStart()
    {
    	ExecuteNavigation()
    }
    
    public void HireError()
    {
    	ExecuteNavigation()
    }
```

## Views:
A Views can be any WPF Control, like a User Control or a Custom Control. Each view can be placed in a single region and navigated by one Controller. The View must be named [Page]View and in the same namespace as the Controller it is connected to.

__When navigating to a Page__:
1. The View's instance is created
2. If a ViewModel class exists, the ViewModel instance is created.
3. The View's DataContext is set to the ViewModel instance, allowing Binding between them.

You can use NavigateCommand in a View's Xaml to initiate navigation, like this:
```xaml
xmlns:mvvmc="clr-namespace:MVVMC;assembly=MVVMC"
...
<Button Command="{mvvmc:NavigateCommand ControllerID='MainOperation', Action='AllEmployees'}">View Employees</Button>
```
Command Parameter can be included and passed to the navigation request.

The View can use a special binding called __ViewBagBinding__ to bind directly to the ViewModel's ViewBag:
```xaml
xmlns:mvvmc="clr-namespace:MVVMC;assembly=MVVMC"
...
<TextBlock Text="{mvvmc:ViewBagBinding Path=EmployeeName}"/>
```
See more info on the ViewBag in the ViewModel section.

## ViewModels:
A ViewModel is a regular class that must derive from __MVVMC.MVVMCViewModel__ or __MVVMC.MVVMCViewModel&lt;TController&gt;__. Creating a ViewModel for a Page is optional. The ViewModel's name must be [Page]ViewModel and in the same namespace as the Controller it is connected to.

__When deriving from MVVMC.MVVMCViewModel:__
You can use __GetController()__ to get an __IController__ instance. With IController you can:
* GetCurrentViewModel()
* Navigate(string action, object parameter)
* NavigateToInitial()
	
__When deriving from MVVMC.MVVMCViewModel&lt;TController&gt;__
This is the recommended way to create ViewModels. You'll have to specify the controller type as TController.
You will be able to use __TController GetExactController()__ to get an instance of the Controller the ViewModel is connected to.

For example, the following code will navigate to the "Info" Action in AllEmployeesController and pass the "SelectedEmployee" as parameter.
```csharp
public class SelectEmployeeViewModel : MVVMCViewModel<AllEmployeesController>
{
    
    public ICommand _selectEmployeeCommand;
    public ICommand SelectEmployeeCommand
    {
        get
        {
            if (_selectEmployeeCommand == null)
                _selectEmployeeCommand = new DelegateCommand(() =>
                {
                    GetExactController().Info(SelectedEmployee);
                },
                ()=>
                {
                    return SelectedEmployee != null;
                });
            return _selectEmployeeCommand;
        }
    }
```
If the ViewModel derived from MVVMC.MVVMCViewModel, we'd have to use `GetController().Navigate("Info", SelectedEmployee)`.

The MVVMCViewModel has the __NavigationParameter__ and __ViewBag__ properties, which are populated by the Controller during naviagtion.
* The NavigationParameter is of type object and can be anything.
* The ViewBag is of type Dictionary<string, object>, and can be binded to directly from the View.

After navigation, the Controller will call the virtual Initialize() method, which you can override. That is the place to make use of the NavigationParamater to populate the ViewModel properties for example.

## Navigation service:
The Navigation-Service is exposed by the __INavigationService__ interface and can be used everywhere with the static `NavigationServiceProvider.GetNavigationServiceInstance()`. It's a singleton for now, and might be changed to some kind of injection pattern in the future.

INavigationService allows:
* Controller GetController(string controllerID)
* TController GetController<TControllerType>()
* MVVMCViewModel GetCurrentViewModelByControllerID(string controllerID);
* string GetCurrentPageNameByControllerID(string controllerID);
* NavigateWithController<TViewModel>(object parameter) - The Page is according to the given TViewModel.

Which basically means we can navigate to everything from anywhere.

## Go Back and Forward:
Historical navigation is available for each controller. So you can tell a controller to "Go Back" to the previous page or "Go Forward" again, after going back.


Each controller now exposes __GoBack__ and __GoForward__ methods. These methods will execute navigation immediately, without invoking the Controller's Action. For example, in a wizard application we might have a __FirstStep__, __SecondStep__ and so on. If we are in the 3rd step, invoking __GoBack__ will  create and navigate to __SecondStepView__ and __SecondStepViewModel__ without actually invoking the `SecondStep()` method in the Controller.

Each controller has a protected property __HistoryMode__ which is an enum with 2 modes: __DiscardParameterInstance__ (default) and __SaveParameterInstance__. This can be set in your Controllers, and even changed per navigation. On a regular navigation, we pass a Parameter and a ViewBag each time (which are saved in the created ViewModel). When in __DiscardParameterInstance__ mode, the "GoBack" method will expect a parameter and a ViewBag as parameters, since these were discarded after the navigation.

```
public virtual void GoBack(object parameter, Dictionary<string, object> viewBag)
```

In __SaveParameterInstance__ mode, the instances of __parameter__ and __viewBag__ are saved. You have the choice to pass a new parameter or use the previous one with the `public virtual void GoBack()` method.
Note that in __SaveParameterInstance__ mode, instances are saved, which might lead to memory leaks.

Additional methods and properties available are:
* `ClearHistory()` method
* `CanGoBack` and `CanGoForward` which can be overridden to custom logic (affects GoBackCommand and GoForwardCommand available in XAML).
* `History` property, which is a List with navigation history.
* __NavigationService__ exposes events: `CanGoBackChangedEvent` and `CanGoForwardChangedEvent`

In XAML, you can use `mvvmc:GoBackCommand` and `mvvmc:GoForwardCommand` like this:

```xaml
xmlns:mvvmc="clr-namespace:MVVMC;assembly=MVVMC"
...
<Button Margin="5" Command="{mvvmc:GoBackCommand ControllerID='MainOperation',
	HistoricalNavigationMode=UseCommandParameter}">Back</Button>
<Button Margin="5" Command="{mvvmc:GoForwardCommand ControllerID='MainOperation',
	HistoricalNavigationMode=UseCommandParameter}" 
	CommandParameter="{Binding MyNavigationParameter}">Forward</Button>
```
__HistoricalNavigationMode__ can be either __UseCommandParameter__ or __UseSavedParameter__. When using __UseSavedParameter__, the Controller's HistoryMode should be set to __SaveParameterInstance__ or an exception will be thrown.
The command is enabled or disabled automatically according to `CanGoBack` and `CanGoForward`.


