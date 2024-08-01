using Crosswords.Models;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Crosswords
{
    public partial class GamePage : Page
    {
        public GamePageViewModel viewModel;
        public GamePage()
        {
            InitializeComponent();
            viewModel = new GamePageViewModel(CrosswordGrid, DescriptionGrid);
            DataContext = viewModel;
        }

        public void CreateCrossword(List<CrosswordClue> crosswordclues)
        {
            viewModel.GenerateCrossword(crosswordclues);
            viewModel.ResultsText = "Twoje punkty: 0, punkty przeciwnika: 0";
        }
    }
}
