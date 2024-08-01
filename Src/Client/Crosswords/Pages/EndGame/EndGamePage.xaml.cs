using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Crosswords.Pages.EndGame
{
    /// <summary>
    /// Logika interakcji dla klasy EndGamePage.xaml
    /// </summary>
    public partial class EndGamePage : Page
    {
        public EndGamePageViewModel viewModel;

        public EndGamePage()
        {
            InitializeComponent();

            viewModel = new EndGamePageViewModel();
            DataContext = viewModel;
        }
    }
}
