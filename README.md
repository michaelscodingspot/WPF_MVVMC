# Wpf.MVVMC


[![Build status](https://img.shields.io/nuget/v/Wpf.Mvvmc.svg)](https://www.nuget.org/packages/Wpf.MVVMC/)

Nuget: `Install-Package Wpf.MVVMC`

## Description
This project is a navigation framework for WPF, which implements the MVVMC pattern. MVVMC adds Controllers to MVVM, which are responsible for navigation and switching between views (screens or parts of screen).

In MVVMC, the View and ViewModel will request a navigation action from the controller. The controller will create the new View and ViewModel. This way, we achieve a separation of concerns, and the View & ViewModel are responsible only to themselves, and don't create other Views.

See [this blog post on mvvmc framework] about the original motivation to create MVVMC.

## Example
For example, your application is a Wizard and the current screen is the "SelectEmployee" step. When clicking "Next", the next step should appear - EditEmployee. With Controllers, the ViewModel will request an "Action" from the Controller, which will execute the navigation according to the controller's logic:

```csharp
public class WizardController : Controller
{
	public void Next()
	{
		var currentViewModel = GetCurrentViewModel();
		if (currentViewModel is SelectEmployeeViewModel)
		{
			return EditEmployee();
		}
		...
	}

	public void EditEmployee()
	{
		//Creates EditEmployeeView and EditEmployeeViewModel and changes screen content to them
		ExecuteNavigation();
	}
````

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

