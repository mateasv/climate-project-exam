using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinBase.Services;

namespace XamarinBase.ViewModels
{
    public class ConnectionViewModel : BaseViewModel
    {
        private ISignalRService _signalRService;
        private IDatabaseService _databaseService;

        private string _databaseUrl;
        private string _signalRUrl;

        public ICommand ConfirmCmd { get; set; }

        public string SignalRUrl
        {
            get { return _signalRUrl; }
            set { _signalRUrl = value; OnPropertyChanged(); }
        }



        public string DatabaseUrl
        {
            get { return _databaseUrl; }
            set { _databaseUrl = value; OnPropertyChanged(); }
        }



        public ConnectionViewModel(ISignalRService signalRService, IDatabaseService databaseService)
        {
            _signalRService = signalRService;
            _databaseService = databaseService;

            ConfirmCmd = new Command(async () => await Confirm());


            SignalRUrl = _signalRService.ConnectionUrl;
            DatabaseUrl = _databaseService.APIUrl;
        }

        public async Task Confirm()
        {
            _databaseService.APIUrl = DatabaseUrl;
            _databaseService.Build();
            await (Application.Current.MainPage as NavigationPage).PopAsync();

        }
    }
}
