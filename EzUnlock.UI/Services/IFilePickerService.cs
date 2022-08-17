namespace EzUnlock.UI.Services
{
    internal interface IFilePickerService
    {
        string[] PickFiles();
        string? PickFolder();
    }
}
