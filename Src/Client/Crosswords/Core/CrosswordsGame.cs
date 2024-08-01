using Crosswords.Models;
using Crosswords.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Crosswords.Core
{
    public static class CrosswordGame
    {
        public static List<CrosswordClue> clue;
        public static bool[][] fields;

        public static CrosswordCell[][] cells;
        public static Coordinates size;
        public static Coordinates editedField;

        private static bool sendEdit;

        public static void CreateCrossword(List<CrosswordClue> clues)
        {
            clue = clues;
            size = CalculateSize();
            Debug.Write("Rozmiar: X:" + size.X + ", Y:" + size.Y);
            fields = GenerateCrosswordFields();

            MessageHandler.LockField += OnLockField;
            MessageHandler.GuessField += OnGuessField;
        }

        #region generating crossword
        public static Coordinates CalculateSize()
        {
            var size = new Coordinates();
            if (clue != null)
            {
                int minX = clue.Min(entry => entry.StartPoint.X);
                int minY = clue.Min(entry => entry.StartPoint.Y);
                //int maxX = clue.Max(entry => entry.StartPoint.X + (entry.Horizontal ? entry.Word.Length - 1 : 0));
                //int maxY = clue.Max(entry => entry.StartPoint.Y + (entry.Horizontal ? 0 : entry.Word.Length - 1));
                int maxX = clue.Max(entry => entry.StartPoint.X + (entry.Horizontal ? entry.Word.Length : 0));
                int maxY = clue.Max(entry => entry.StartPoint.Y + (entry.Horizontal ? 0 : entry.Word.Length));

                size.X = maxX - minX + 1;
                size.Y = maxY - minY + 1;
            }
            return size;
        }

        public static bool[][] GenerateCrosswordFields()
        {
            bool[][] fields = new bool[size.X][];

            for (int i = 0; i < size.X; i++)
            {
                fields[i] = new bool[size.Y + 1];
            }

            foreach (var clue in clue)
            {
                int x = clue.StartPoint.X;
                int y = clue.StartPoint.Y;

                for (int i = 0; i < clue.Word.Length; i++)
                //for (int i = 1; i <= clue.Word.Length; i++)
                {
                    fields[x + (clue.Horizontal ? i : 0)][y + (clue.Horizontal ? 0 : i)] = true;
                }
            }

            return fields;
        }
        #endregion

        public static void StartEdit(int x, int y)
        {
            if (sendEdit && editedField != null)
            {
                MainWindow.Instance.SendToServer(Message.FieldEdit(editedField.X, editedField.Y, false));
            } else
            {
                sendEdit = true;
            }
            editedField = new Coordinates { X = x, Y = y };
            MainWindow.Instance.SendToServer(Message.FieldEdit(x, y, true));
        }

        public static void GuessField(int x, int y, char guess)
        {
            MainWindow.Instance.SendToServer(Message.FieldGuess(x, y, guess));
            editedField = null;
            sendEdit = false;
        }

        #region events
        private static void OnLockField(object? sender, MessageHandler.FieldActionArgs e)
        {
            MainWindow.Instance.InvokeOnUiThread(() => cells[e.X][e.Y].Lock(e.ActionValue));
        }

        private static void OnGuessField(object? sender, MessageHandler.GuessActionArgs e)
        {
            MainWindow.Instance.InvokeOnUiThread(() => cells[e.X][e.Y].Guessed(e.CorrectValue));
        }
        #endregion
    }
}
