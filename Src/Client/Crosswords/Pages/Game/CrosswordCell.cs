using Crosswords;
using System.Windows.Controls;
using Crosswords.Utils;
using System.Windows.Media;
using Crosswords.Core;
using System.Windows.Input;
using System.Windows;

public class CrosswordCell : Button
{
    private static int FONT_SIZE = 30;
    public int X { get; set; }
    public int Y { get; set; }
    private bool isLocked;
    private TextBox textBox;

    private static Color unlockedColor = Colors.Gainsboro;
    private static Color lockedColor = Colors.Crimson;
    private static Color completedColor = Colors.LightGreen;
    public static Color verticalColor = Colors.Plum;
    public static Color horizontalColor = Colors.Wheat;

    public CrosswordCell(int x, int y, int gridSize)
    {
        X = x;
        Y = y;

        Width = gridSize - 2;
        Height = gridSize - 2;
        Background = new SolidColorBrush(unlockedColor);
        FontSize = FONT_SIZE;

        InitializeTextBox();
    }

    public void setAsLabel(int fontSize, int number, bool isHorizontal)
    {
        FontSize = fontSize;
        isLocked = true;
        Content = number;
        Background = new SolidColorBrush(isHorizontal ? horizontalColor : verticalColor);
    }

    public void StartEdit()
    {
        if (isLocked) return;
        CrosswordGame.StartEdit(X, Y);
        textBox.Visibility = Visibility.Visible;
        textBox.Focus();
    }

    public void Lock(bool val)
    {
        isLocked = val;
        Background = new SolidColorBrush(isLocked ? lockedColor : unlockedColor);
    }

    public void Guessed(char guess)
    {
        isLocked = true;
        Content = char.ToUpper(guess);
        Background = new SolidColorBrush(completedColor);
    }

    #region helper methods
    private void InitializeTextBox()
    {
        textBox = new TextBox
        {
            Width = Width,
            Height = Height,
            FontSize = FONT_SIZE,
            TextAlignment = TextAlignment.Center,

            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
            VerticalAlignment = System.Windows.VerticalAlignment.Center,

            Visibility = System.Windows.Visibility.Hidden,
            MaxLength = 1,
            Background = Brushes.Transparent
        };

        textBox.KeyDown += OnEnter;
        textBox.LostFocus += OnLostFocus;

        Content = textBox;
    }

    private void OnEnter(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && textBox.Text.Length != 0)
        {
            CrosswordGame.GuessField(X, Y, textBox.Text[0]);
            textBox.Visibility = Visibility.Hidden;
        }
    }

    private void OnLostFocus(object sender, RoutedEventArgs e)
    {
        // CrosswordGame.StopEdit(X, Y);
        textBox.Visibility = Visibility.Hidden;
        textBox.Text = "";
    }
    #endregion
}
