namespace MVVMC
{
    public interface IController
    {
        string ID { get; }
        MVVMCViewModel GetCurrentViewModel();
        void Navigate(string action, object parameter);
        void NavigateToInitial();
    }
}