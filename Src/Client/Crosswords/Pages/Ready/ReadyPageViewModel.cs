using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Crosswords;
using Crosswords.Utils;

public class ReadyPageViewModel : ObservableObject
{
    public string statusMessage;
    private bool ready;

    public string StatusMessage
    {
        get => statusMessage;
        set { SetProperty(ref statusMessage, value); }
    }

    public IRelayCommand ReadyCommand { get; }

    public ReadyPageViewModel()
    {
        ReadyCommand = new RelayCommand(Ready);
        ResetStatus();
    }

    public void ResetStatus()
    {
        ready = false;
        StatusMessage = "Połączono z serwerem, oczekiwanie na gotowość graczy";
    }

    private void Ready()
    {
        if(!ready)
        {
            MainWindow.Instance.SendToServer(Message.Ready());
            StatusMessage = "Gotów, oczekiwanie na gotowość drugiego gracza";
        }
    }
}
