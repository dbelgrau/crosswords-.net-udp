using System.Net;

namespace CrosswordServer
{
    public static class ClientHandler
    {
        public static Crossword crossword;
        private static object lockObject = new object();

        #region events
        public static event EventHandler<PlayerActionArgs> PlayerReady;
        public static event EventHandler<PlayerBooleanActionArgs> PlayerConnection;
        public static event EventHandler<FieldActionArgs> FieldEdit;
        public static event EventHandler<FieldActionArgs> FieldGuess;
        public static event EventHandler CrosswordComplete;
        public static event EventHandler<PlayerActionArgs> Disconnect;
        #endregion

        #region events args
        public class PlayerActionArgs : EventArgs
        {
            public IPEndPoint Sender { get; set; }
        }
        public class PlayerBooleanActionArgs : PlayerActionArgs
        {
            public bool ActionValue { get; set; }
        }
        public class FieldActionArgs : PlayerBooleanActionArgs
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
        #endregion

        #region handle messages
        public static void HandleMessage(string message, IPEndPoint sender)
        {
            Console.WriteLine(message);
            string[] parts = message.Split('|');
            if (parts.Length >= 2)
            {
                string messageType = parts[0];
                string messageContent = parts[1];

                switch (messageType)
                {
                    case "CONNECT":
                        HandleConnect(sender);
                        break;

                    case "READY":
                        HandleReady(sender);
                        break;

                    case "EDIT":
                        HandleEdit(sender, messageContent);
                        break;

                    case "GUESS":
                        HandleGuess(sender, messageContent);
                        break;

                    case "DISCONNECT":
                        HandleDisconnect(sender);
                        break;

                    default:
                        Console.WriteLine($"Nieznany typ wiadomości: {messageType}");
                        break;
                }
            }
        }
        #endregion

        // connection
        private static void HandleConnect(IPEndPoint sender)
        {
            var args = new PlayerBooleanActionArgs
            {
                Sender = sender,
                ActionValue = true
            };

            PlayerConnection(typeof(ClientHandler), args);
        }

        // ready status
        private static void HandleReady(IPEndPoint sender)
        {
            var args = new PlayerBooleanActionArgs
            {
                Sender = sender
            };

            PlayerReady(typeof(ClientHandler), args);
        }

        // edit fields
        private static void HandleEdit(IPEndPoint sender, string editMessage)
        {
            string[] parts = editMessage.Split(',');

            lock (lockObject)
            {
                if (parts.Length == 3 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y) && bool.TryParse(parts[2], out bool isEdited))
                {
                    if (crossword.IsFieldEdited(x, y) && isEdited)
                    {
                        return;
                    }
                    crossword.SetFieldIsEdited(x, y, isEdited);

                    var args = new FieldActionArgs
                    {
                        Sender = sender,
                        ActionValue = isEdited,
                        X = x,
                        Y = y
                    };
                    FieldEdit(typeof(ClientHandler), args);
                }
            }
        }

        // guess field
        private static void HandleGuess(IPEndPoint sender, string guessMessage)
        {
            string[] parts = guessMessage.Split(',');

            lock (lockObject)
            {
                if (parts.Length == 3 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y) && char.TryParse(parts[2], out char guess))
                {
                    // if field already guessed return
                    if (crossword.IsFieldGuessed(x, y))
                    {
                        return;
                    }

                    bool isCorrectGuess = crossword.TryGuessField(x, y, guess, out bool isCompleted);

                    // if guess is correct
                    if (isCorrectGuess)
                    {
                        // set field guessed
                        crossword.SetFieldGuessed(x, y);

                        // send guess info to players
                        var args = new FieldActionArgs
                        {
                            Sender = sender,
                            ActionValue = true,
                            X = x,
                            Y = y
                        };
                        FieldGuess(typeof(ClientHandler), args);

                        // if that was the last field
                        if (isCompleted)
                        {
                            // send end info to players
                            CrosswordComplete(typeof(ClientHandler), EventArgs.Empty);
                        }
                    }
                    // if guess is not correct
                    else
                    {
                        // send fail info to player
                        var args = new FieldActionArgs
                        {
                            Sender = sender,
                            ActionValue = false,
                            X = x,
                            Y = y
                        };
                        FieldGuess(typeof(ClientHandler), args);
                        // unlock field
                        FieldEdit(typeof(ClientHandler), args);
                    }
                }
            }
        }

        // player decided to end game
        private static void HandleDisconnect(IPEndPoint sender)
        {
            var args = new PlayerActionArgs { Sender = sender };
            Disconnect(typeof(ClientHandler), args);
        }
    }
}
