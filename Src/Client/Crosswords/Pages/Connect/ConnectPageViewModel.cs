using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Crosswords;
using Crosswords.Utils;
using System;

public class ConnectPageViewModel : ObservableObject
{
    private int serverPort;
    private int clientPort;
    private string errorMessage;

    public int ServerPort
    {
        get => serverPort;
        set { SetProperty(ref serverPort, value); }
    }

    public int ClientPort
    {
        get => clientPort;
        set => SetProperty(ref clientPort, value);
    }

    public string ErrorMessage
    {
        get => errorMessage;
        set { SetProperty(ref errorMessage, value); }
    }

    public IRelayCommand ConnectCommand { get; }

    public ConnectPageViewModel()
    {
        ConnectCommand = new RelayCommand(Connect);
        ServerPort = 8888;
        Random rnd = new();
        ClientPort = rnd.Next(9000, 9999);
    }

    private async void Connect()
    {
        try
        {
            await MainWindow.Instance.InitializeClient(ClientPort, ServerPort);
            MainWindow.Instance.SendToServer(Message.Connect());
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd połączenia: {ex.Message}";
        }
    }

}
