using Server.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinBase.Exstensions;
using XamarinBase.Services;
using ZXing.Net.Mobile.Forms;

namespace XamarinBase.ViewModels
{
    public class EditDataloggerViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;

        private DataloggerViewModel _dataloggerViewModel;
        


        public DataloggerViewModel DataloggerViewModel
        {
            get { return _dataloggerViewModel; }
            set { _dataloggerViewModel = value; OnPropertyChanged(); }
        }

        public ICommand ScanDataloggerCmd { get; set; }

        public EditDataloggerViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;

            ScanDataloggerCmd = new Command(async () => await ScanDatalogger());
        }

        public async Task ScanDatalogger()
        {
            var scan = new ZXingScannerPage();
            scan.OnScanResult += (async result => await ScanHandler(result));


            //await (Application.Current.MainPage as NavigationPage).PushAsync(scan);

            


            await ScanHandler(new ZXing.Result("1", null, null, ZXing.BarcodeFormat.QR_CODE));
        }


        private async Task ScanHandler(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!int.TryParse(result.Text, out int id)) return;

                try
                {
                    var res = await _databaseService.GetAsync<Datalogger>(id);

                    if (res.IsSuccessStatusCode)
                    {
                        var datalogger = await res.ContentToObjectAsync<Datalogger>();

                        DataloggerViewModel.Datalogger = datalogger;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {res.ReasonPhrase}, Datalogger not found", "Cancel", "ok");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {ex.Message}", "Cancel", "ok");
                }

                //await (Application.Current.MainPage as NavigationPage).PopAsync();
            });
        }

        public void Reset()
        {
            DataloggerViewModel = new DataloggerViewModel() { Datalogger = new Datalogger() };
        }
    }
}
