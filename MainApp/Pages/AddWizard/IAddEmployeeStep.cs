namespace MainApp.Pages.AddWizard
{
    public interface IAddEmployeeStep
    {
        bool IsNextLegal(out string errorMsg);
    }
}
