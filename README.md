# WPF_MVVMC
A WPF framework for navigation between pages with MVC-like pattern

This project is a lightweight infrastructure which implements a MVVMC navigation framework in WPF.

MVVMC stands for Model-View-ViewModel-Controller.
The idea is to combine Controllers with MVVM that are responsible for navigation.
All the concepts mimic the routing system in Asp.NET Core. Everything is done by naming convention.

##Regions:
A Region is a Control which simply contains a content presenter with dynamic content.
Each region area is controlled by a single controller.

##Controller:
A controller is connected to a region and changes the region's content. A method in a controlled called "MyAction()" needs to include a function "ExecuteNavigation()". This will look for a View and ViewModel with similar name in the same namespace. "MyActionView" and "MyActionViewModel". MyActionView can be a UserControl or any FrameworkElement. MyActionViewModel should inherit from MVVMCViewModel.

##How navigation works:
A View, ViewModels or services can Request to navigate between screens. 
From View a Command is available "NavigationCommand".
From ViewModel, we can get the controller with GetController() and call the Navigate(string actionName) method.

From anywehere, we can find controllers by:
NavigationServiceProvider.GetNavigationServiceInstance().GetController(string controllerName)
or 
NavigationServiceProvider.GetNavigationServiceInstance().GetController<TController>()

##Example of use
"MainApp" contains a simple application that demonstrates all possible usages of MVVMC
