using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Crosswords.Pages.EndGame
{
    public class EndGamePageViewModel : ObservableObject
    {
        private string endGameText;

        public string EndGameText
        {
            get => endGameText;
            set => SetProperty(ref endGameText, value);
        }

        private IRelayCommand endGameCommand;

        public IRelayCommand EndGameCommand
        {
            get => endGameCommand ??= new RelayCommand(EndGame);
        }

        private void EndGame()
        {
            MainWindow.Instance.InvokeOnUiThread(() =>
            {
                MainWindow.Instance.ChangePage(MainWindow.Instance.readyPage);
                MainWindow.Instance.readyPage.viewModel.ResetStatus();
            });
        }
    }
}
