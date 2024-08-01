using Crosswords.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Crosswords.Core
{
    public static class MessageHandler
    {
        #region events
        public static event EventHandler<FieldActionArgs> LockField;
        public static event EventHandler<GuessActionArgs> GuessField;
        #endregion

        #region events args
        public class FieldActionArgs : EventArgs
        {
            public int X { get; set; }
            public int Y { get; set; }
            public bool ActionValue { get; set; }
        }

        public class GuessActionArgs : FieldActionArgs
        {
            public char CorrectValue { get; set; }
        }
        #endregion

        #region handle messages
        public static void HandleMessage(string message)
        {
            string[] parts = message.Split('|');
            if (parts.Length >= 2)
            {
                string messageType = parts[0];
                string messageContent = parts[1];

                switch (messageType)
                {
                    case "CONNECT":
                        HandleConnect();
                        break;

                    case "CROSSWORD":
                        HandleCrossword(messageContent);
                        break;

                    case "EDIT":
                        HandleEdit(messageContent);
                        break;

                    case "GUESS":
                        HandleGuess(messageContent);
                        break;

                    case "POINTS":
                        HandlePoints(messageContent);
                        break;

                    case "COMPLETE":
                        HandleComplete(messageContent);
                        break;

                    case "DISCONNECT":
                        HandleDisconnect();
                        break;

                    default:
                        Console.WriteLine($"Nieznany typ wiadomości: {messageType}");
                        break;
                }
            }
        }
        #endregion

        public static void HandleConnect()
        {
            MainWindow.Instance.InvokeOnUiThread(() => MainWindow.Instance.ChangePage(MainWindow.Instance.readyPage));
        }

        public static void HandleCrossword(string messageContent)
        {
            List<CrosswordClue> crosswordclues = JsonConvert.DeserializeObject<List<CrosswordClue>>(messageContent);
            MainWindow.Instance.InvokeOnUiThread(() => MainWindow.Instance.ChangePage(MainWindow.Instance.gamePage));
            MainWindow.Instance.InvokeOnUiThread(() => MainWindow.Instance.gamePage.CreateCrossword(crosswordclues));
        }

        private static void HandleEdit(string messageContent)
        {
            string[] parts = messageContent.Split(',');

            if (parts.Length == 3 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y) && bool.TryParse(parts[2], out bool isEdited))
            {
                // set field lock
                var args = new FieldActionArgs
                {
                    X = x,
                    Y = y,
                    ActionValue = isEdited
                };
                LockField(typeof(MessageHandler), args);
            }
        }

        private static void HandleGuess(string messageContent)
        {
            // set field to message value or something
            string[] parts = messageContent.Split(',');

            if (parts.Length == 3 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y) && char.TryParse(parts[2], out char correctValue))
            {
                // set field lock
                var args = new GuessActionArgs
                {
                    X = x,
                    Y = y,
                    CorrectValue = correctValue
                };
                GuessField(typeof(MessageHandler), args);
            }
        }

        private static void HandlePoints(string messageContent)
        {
            string[] parts = messageContent.Split(',');

            if (parts.Length == 2 && int.TryParse(parts[0], out int playerPoints) && int.TryParse(parts[1], out int enemyPoints))
            {
                string resultDisplay = $"Twoje punkty: {playerPoints}, punkty przeciwnika: {enemyPoints}";
                MainWindow.Instance.InvokeOnUiThread(() => MainWindow.Instance.gamePage.viewModel.ResultsText = resultDisplay);
            }
        }

        private static void HandleComplete(string messageContent)
        {
            // show result and back to ready page
            string[] parts = messageContent.Split(',');

            if (parts.Length == 3 && int.TryParse(parts[1], out int playerPoints) && int.TryParse(parts[2], out int enemyPoints))
            {
                string resultDisplay = $"{parts[0]}! Twoje punkty: {playerPoints}, punkty przeciwnika: {enemyPoints}";
                MainWindow.Instance.InvokeOnUiThread(() =>
                {
                    MainWindow.Instance.ChangePage(MainWindow.Instance.endGamePage);
                    MainWindow.Instance.endGamePage.viewModel.EndGameText = resultDisplay;
                });
            }
        }

        private static void HandleDisconnect()
        {
            string resultDisplay = "Przeciwnik rozłączył się z serwerem.";
            MainWindow.Instance.InvokeOnUiThread(() =>
            {
                MainWindow.Instance.ChangePage(MainWindow.Instance.endGamePage);
                MainWindow.Instance.endGamePage.viewModel.EndGameText = resultDisplay;
            });
        }
    }
}
