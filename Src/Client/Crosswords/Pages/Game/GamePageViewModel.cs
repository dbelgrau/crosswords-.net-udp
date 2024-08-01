using CommunityToolkit.Mvvm.ComponentModel;
using Crosswords.Core;
using Crosswords.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public class GamePageViewModel : ObservableObject
{
    private static readonly int gridSize = 45;
    private static readonly int labelSize = 30;

    private Grid crosswordGrid;
    private Grid descriptionGrid;
    private string resultsText;

    public GamePageViewModel(Grid crosswordGrid, Grid descriptionGrid)
    {
        this.crosswordGrid = crosswordGrid;
        this.descriptionGrid = descriptionGrid;
    }

    public string ResultsText
    {
        get => resultsText;
        set => SetProperty(ref resultsText, value);
    }

    public void GenerateCrossword(List<CrosswordClue> clues)
    {
        CrosswordGame.CreateCrossword(clues);

        //GenerateGridDefinitions(crosswordGrid, CrosswordGame.size.X, CrosswordGame.size.Y, gridSize);
        GenerateGridDefinitions(crosswordGrid, CrosswordGame.size.X, CrosswordGame.size.Y, gridSize);

        CrosswordGame.cells = GenerateCells(crosswordGrid);
        GenerateGridDefinitions(descriptionGrid, 0, clues.Count, labelSize);

        GenerateNumberCells(crosswordGrid, clues);
        GenerateDescriptions(descriptionGrid, clues);
    }

    #region generate crossword

    private void GenerateDescriptions(Grid grid, List<CrosswordClue> clues)
    {
        for (int i = 0; i < clues.Count; i++)
        {
            CrosswordClue clue = clues[i];

            TextBlock textBlock = new TextBlock
            {
                FontSize = 20,
                Text = $"{i + 1}. {clue.Description}",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = new SolidColorBrush(clue.Horizontal ? CrosswordCell.horizontalColor : CrosswordCell.verticalColor)
            };

            Grid.SetRow(textBlock, i);

            grid.Children.Add(textBlock);
        }
    }

    private void GenerateNumberCells(Grid grid, List<CrosswordClue> clues)
    {
        for (int i = 0; i < clues.Count; i++)
        {
            CrosswordClue clue = clues[i];

            int x = clue.StartPoint.X - (clue.Horizontal ? 1 : 0);
            int y = clue.StartPoint.Y - (clue.Horizontal ? 0 : 1);

            var cell = new CrosswordCell(x, y, gridSize);
            cell.setAsLabel(15, i+1, clue.Horizontal);

            CrosswordGame.cells[x][y] = cell;

            Grid.SetColumn(cell, x);
            Grid.SetRow(cell, y);
            grid.Children.Add(cell);
        }
    }

    private CrosswordCell[][] GenerateCells(Grid grid)
    {
        CrosswordCell[][] cells = InitializeCellsArray(CrosswordGame.size.X, CrosswordGame.size.Y);

        for (int i = 0; i < CrosswordGame.size.X; i++)
        {
            for (int j = 0; j < CrosswordGame.size.Y; j++)
            {
                if (CrosswordGame.fields[i][j])
                {
                    CrosswordCell cell = new CrosswordCell(i, j, gridSize);
                    cell.Click += (sender, e) => cell.StartEdit();

                    Grid.SetColumn(cell, i);
                    Grid.SetRow(cell, j);
                    grid.Children.Add(cell);
                    cells[i][j] = cell;
                }
            }
        }
        return cells;
    }

    private void GenerateGridDefinitions(Grid grid, int width, int height, int cellSize)
    {
        grid.ColumnDefinitions.Clear();
        grid.RowDefinitions.Clear();

        for (int i = 0; i < width; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(cellSize) });
        }

        for (int i = 0; i < height; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
        }

        grid.HorizontalAlignment = HorizontalAlignment.Center;
        grid.VerticalAlignment = VerticalAlignment.Center;
    }

    private CrosswordCell[][] InitializeCellsArray(int width, int height)
    {
        CrosswordCell[][] cells = new CrosswordCell[width][];

        for (int i = 0; i < width; i++)
        {
            cells[i] = new CrosswordCell[height];
        }
        return cells;
    }

    #endregion
}
