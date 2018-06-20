# Wpf.MVVMC


[![Build status](https://img.shields.io/nuget/v/Wpf.Mvvmc.svg)](https://www.nuget.org/packages/Wpf.MVVMC/)

Nuget: `Install-Package Wpf.MVVMC`

## Description
This project is a navigation framework for WPF, which implements the MVVMC pattern. MVVMC adds Controllers to MVVM, which are responsible for navigation and switching between views (screens or parts of screen).

In MVVMC, the View and ViewModel will request a navigation action from the controller. The controller will create the new View and ViewModel. This way, we achieve a separation of concerns, and the View & ViewModel are responsible only to themselves, and don't create other Views.

To read more about MVVMC and the motivation for this framework, see the original blog posts: [Part 1](http://michaelscodingspot.com/2017/02/06/wpf-page-navigation-like-mvc-building-mvvm-framework-controllers/), [Part 2](http://michaelscodingspot.com/2017/02/06/wpf-page-navigation-like-mvc-building-mvvm-framework-controllers/).

# Documentation

* [Quickstart](#quickstart)
* [Regions](#regions)
* [File naming](#file-naming)
* [Controllers](#controllers)
* [Views](#views)
* [ViewModels](#viewmodels)
* [Navigation types](#navigation-types)
* [Parameter and ViewBag](#parameter-and-viewbag)

## Quickstart

Let's build a small Wizard application with 3 steps in it. First, create a WPF application and add the __Wpf.MVVMC__ Nuget package.

#### Step 1: Create a Region
Now, we'll need to add a [Region](#regions) to the MainWindow, like this:
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

__ExecuteNavigation()__ depends on the calling method name. When called from "FirstStep()" for example, it will navigate to "FirstStep" page. As mentioned, Wpf.MVVMC is name-convention based.
Navigating to "FirstStep" page means we need a UI element called "FirstStepView" and a view model called "FirstStepViewModel".

#### Step 3: Add Views
The View can be any WPF control, like a simple UserControl. It should be in __the same namespace__ as the Controller and the ViewModel. Let's add 3 User Controls to the project called __FirstStepView__, __SecondStepView__ and __ThirdStepView__. Each will have a caption and a __Next__ button:


#### Step 4: Add ViewModels



## Regions:
A Region is a Control which simply contains a content presenter with dynamic content.
Each region area is controlled by a single controller.

## File naming:

## Controllers:
A controller is connected to a region and changes the region's content. A method in a controlled called "MyAction()" needs to include a function "ExecuteNavigation()". This will look for a View and ViewModel with similar name in the same namespace. "MyActionView" and "MyActionViewModel". MyActionView can be a UserControl or any FrameworkElement. MyActionViewModel should inherit from MVVMCViewModel.

## Views:

## ViewModels:

## Navigation types:
A View, ViewModels or services can Request to navigate between screens. 
From View a Command is available "NavigationCommand".
From ViewModel, we can get the controller with GetController() and call the Navigate(string actionName) method.

From anywehere, we can find controllers by:
NavigationServiceProvider.GetNavigationServiceInstance().GetController(string controllerName)
or 
NavigationServiceProvider.GetNavigationServiceInstance().GetController<TController>()

## Parameter and ViewBag

