using Crosswords.Connect;
using Crosswords.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Crosswords.Utils;
using Crosswords.Pages.EndGame;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Crosswords
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public ConnectPage connectPage;
        public ReadyPage readyPage;
        public GamePage gamePage;
        public EndGamePage endGamePage;
        private UdpClient udpClient;
        private IPEndPoint serverEndPoint;

        public bool udpRunning;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            connectPage = new ConnectPage();
            readyPage = new ReadyPage();
            gamePage = new GamePage();
            endGamePage = new EndGamePage();

            MainFrame.Navigate(connectPage);

            Closing += OnCloseApp;
        }

        private async void UdpListen()
        {
            while (udpRunning)
            {
                await Task.Run(ReceiveMessages);
            }
        }

        public async Task InitializeClient(int clientPort, int serverPort)
        {
            serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);
            udpClient = new UdpClient(clientPort);

            udpRunning = true;
            UdpListen();
            await Task.CompletedTask;
        }

        public void ReceiveMessages()
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpClient.Receive(ref clientEndPoint);

            string message = Encoding.UTF8.GetString(data);

            MessageHandler.HandleMessage(message);
        }

        public void InvokeOnUiThread(Action action)
        {
            if (Dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                Dispatcher.Invoke(action);
            }
        }

        public void ChangePage(Page page)
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            MainFrame.Navigate(page);
        }

        public void SendToServer(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpClient.Send(data, data.Length, serverEndPoint);
        }

        private void OnCloseApp(object? sender, CancelEventArgs e)
        {
            if (udpRunning) SendToServer(Message.End());
        }
    }
}
