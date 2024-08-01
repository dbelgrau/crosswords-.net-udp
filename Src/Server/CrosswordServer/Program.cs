using CrosswordServer;
using CrosswordServer.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    private static UdpClient udpServer;
    private static IPEndPoint player1, player2;
    private static int player1Points, player2Points;
    private static bool player1Ready, player2Ready; 
    //private static string path = "../../../Data/data.json";
    private static string path = "./Data/data.json";
    private static Crossword crossword;

    #region server
    static void Main()
    {
        InitializeServer();
        ResetState();

        while (true)
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpServer.Receive(ref clientEndPoint);

            Thread clientThread = new Thread(() => HandleMessage(data, clientEndPoint));
            clientThread.Start();
        }
    }

    private static void InitializeServer()
    {
        int port = 8888;
        udpServer = new UdpClient(port);
        Console.WriteLine($"Serwer działa na porcie: {port}");

        ClientHandler.PlayerReady += OnPlayerReady;
        ClientHandler.PlayerConnection += OnPlayerConnection;
        ClientHandler.FieldEdit += OnFieldEdit;
        ClientHandler.FieldGuess += OnFieldGuess;
        ClientHandler.CrosswordComplete += OnCrosswordComplete;
        ClientHandler.Disconnect += OnDisconnect;
    }

    public static void ResetState()
    {
        player1Points = player2Points = 0;
        player1Ready = player2Ready = false;
        LoadCrossword();
    }

    public static void LoadCrossword()
    {
        var crosswordJson = CrosswordsLoader.LoadAndDeserializeCrossword(path);
        crossword = new Crossword(crosswordJson);
        ClientHandler.crossword = crossword;
    }

    #endregion

    #region Event Handling
    // menage player connection
    private static void OnPlayerConnection(object? sender, ClientHandler.PlayerBooleanActionArgs e)
    {
        // connecting
        if (e.ActionValue)
        {
            if (player1 == null)
            {
                player1 = e.Sender;
                SendToClient(player1, Message.Connected());
            }
            else if (player2 == null)
            {
                player2 = e.Sender;
                SendToClient(player2, Message.Connected());
            }
            else
            {
                SendToClient(e.Sender, Message.Error(Message.ErrorState.SERVER_FULL));
            }
        }
        // disconnecting
        else
        {
            if (IpEquals(player1, e.Sender))
            {
                player1 = null;
                player1Ready = false;
            }
            else if (IpEquals(player2, e.Sender))
            {
                player2 = null;
                player2Ready = false;
            }
        }
    }

    // send crossword if both players are ready
    private static void OnPlayerReady(object? sender, ClientHandler.PlayerActionArgs e)
    {
        if (IpEquals(player1, e.Sender))
        {
            player1Ready = true;
        }
        else if (IpEquals(player2, e.Sender))
        {
            player2Ready = true;
        }

        if (player1Ready && player2Ready)
        {
            InitGame();
        }
    }

    // send field edit state
    private static void OnFieldEdit(object? sender, ClientHandler.FieldActionArgs e)
    {
        if (IpEquals(player1, e.Sender))
        {
            SendToClient(player2, Message.FieldEdited(e.X, e.Y, e.ActionValue));
        }
        else if (IpEquals(player2, e.Sender))
        {
            SendToClient(player1, Message.FieldEdited(e.X, e.Y, e.ActionValue));
        }
    }

    // send field guess state
    private static void OnFieldGuess(object? sender, ClientHandler.FieldActionArgs e)
    {
        if (e.ActionValue)
        {
            if (IpEquals(player1, e.Sender))
            {
                player1Points++;
            }
            else if (IpEquals(player2, e.Sender))
            {
                player2Points++;
            }

            char correctValue = crossword.GetCorrectValue(e.X, e.Y);
            SendToAll(Message.FieldGuessed(e.X, e.Y, correctValue));
        }
        else
        {
            if (IpEquals(player1, e.Sender))
            {
                player1Points--;
            }
            else if (IpEquals(player2, e.Sender))
            {
                player2Points--;
            }
        }

        SendPoints();
    }

    // send actual points
    private static void SendPoints()
    {
        SendToClient(player1, Message.Points(player1Points, player2Points));
        SendToClient(player2, Message.Points(player2Points, player1Points));
    }

    // sent endgame state
    private static void OnCrosswordComplete(object? sender, EventArgs e)
    {
        if (player1Points > player2Points)
        {
            SendToClient(player1, Message.Complete(Message.CompleteState.VICTORY, player1Points, player2Points));
            SendToClient(player2, Message.Complete(Message.CompleteState.DEFEAT, player2Points, player1Points));
        }
        else if (player2Points > player1Points)
        {
            SendToClient(player2, Message.Complete(Message.CompleteState.VICTORY, player2Points, player1Points));
            SendToClient(player1, Message.Complete(Message.CompleteState.DEFEAT, player1Points, player2Points));
        }
        else
        {
            SendToAll(Message.Complete(Message.CompleteState.DRAW, player1Points, player2Points));
        }

        player1Ready = player2Ready = false;
        player1Points = player2Points = 0;
    }

    // handle disconnect
    private static void OnDisconnect(object? sender, ClientHandler.PlayerActionArgs e)
    {
        if (IpEquals(player1, e.Sender))
        {
            if (player2 != null ) SendToClient(player2, Message.Disconnect());
            player1 = null;
        }
        else if (IpEquals(player2, e.Sender))
        {
            if (player1 != null) SendToClient(player1, Message.Disconnect());
            player2 = null;
        }
        ResetState();
    }
    #endregion

    #region Helper Methods
    private static void HandleMessage(byte[] data, IPEndPoint sender)
    {
        string message = Encoding.UTF8.GetString(data);
        ClientHandler.HandleMessage(message, sender);
    }

    private static void InitGame()
    {
        var crosswordString = CrosswordsLoader.LoadCrosswordFromFile(path);
        SendToAll(Message.Crossword(crosswordString));
    }

    public static void SendToClient(IPEndPoint clientEndPoint, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpServer.Send(data, data.Length, clientEndPoint);
        int index = message.IndexOf('|');
        Console.WriteLine($"Sending to: {clientEndPoint.Address}, {clientEndPoint.Port}, message:{message.Substring(0, index)}");
    }

    public static void SendToAll(string message)
    {
        SendToClient(player1, message);
        SendToClient(player2, message);
    }

    public static bool IpEquals(IPEndPoint e1, IPEndPoint e2)
    {
        if (e1 == null || e2 == null) return false;
        return e1.Address.Equals(e2.Address) && e1.Port == e2.Port;
    }
    #endregion
}