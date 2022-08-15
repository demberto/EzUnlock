namespace EzUnlock.UI.Services
{
    interface IFilePickerService
    {
        string[] PickFiles();
        string PickFolder();
    }
}
