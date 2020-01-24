using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace MVVMC
{
    public class ViewBagBinding : MarkupExtension
    {

        public string Path { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget service = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            DependencyObject targetObject = service.TargetObject as DependencyObject;
            DependencyProperty targetProperty = service.TargetProperty as DependencyProperty;

            var elem = targetObject as FrameworkElement; 
            if (elem == null)
                return null;

            RoutedEventHandler handlerLoaded = null;
            handlerLoaded = (s, e) =>
            {
                OnLoaded(elem, targetProperty);
                elem.Loaded -= handlerLoaded;
            };
            elem.Loaded += handlerLoaded;

            return null;
        }

        private void OnLoaded(FrameworkElement elem, DependencyProperty property)
        {

            var elemWithDataContext = FindSelfOrParentWithDataContext(elem);

            var vm = elemWithDataContext.DataContext == null ? null : elemWithDataContext.DataContext as MVVMCViewModel;
            if (vm == null)
                return;
            var viewBag = vm.ViewBag;
            if (viewBag == null)
                return;

            if (!viewBag.ContainsKey(Path))
                return;

            var propertyInfo = elem.GetType().GetProperty(property.Name);
            propertyInfo.SetValue(elem, viewBag[Path], null);
        }

        private  static FrameworkElement FindSelfOrParentWithDataContext(DependencyObject child)
        {
            if (child == null)
                return null;

            var fe = child as FrameworkElement;
            if (fe != null && fe.DataContext != null)
                return fe;

            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            return FindSelfOrParentWithDataContext(parentObject);
        }
    }
}
