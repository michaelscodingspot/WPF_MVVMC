using MVVMC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Pages.MainOperation
{
    public class AboutViewModel : MVVMCViewModel
    {
        public string Text { get {
                return 
@"This project uses MVVMC, a lighweight infrastructure for navigation between views in WPF.

The idea is to combine Controllers with MVVM that are responsible for navigation.

Navigation areas & controllers:
A navigation area is a Control which simply contains a content presenter with dynamic content.
Each navigation area is controlled by a single controller.  

How navigation works:
View, ViewModels or services can Request to navigate between screens. 
This request calls a method in the specified controller, with specified page XXX as parameter.
By default the controller will find a View called XXXView and a view model XXXViewModel, 
connect them and show them as content.

The controller however can (and should) contain logic to process navigation requests and can
do entirely different navigation according to context."; } }


    }
}
