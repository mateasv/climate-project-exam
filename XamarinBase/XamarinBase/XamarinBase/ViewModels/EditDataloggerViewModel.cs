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
    /// <summary>
    /// View model used by the EditDataloggerView
    /// </summary>
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

        /// <summary>
        /// Pushes a scan view and awaits a scan
        /// </summary>
        /// <returns></returns>
        public async Task ScanDatalogger()
        {
            var scan = new ZXingScannerPage();

            // assign handler to handle the scan
            scan.OnScanResult += (async result => await ScanHandler(result));

            //await (Application.Current.MainPage as NavigationPage).PushAsync(scan);

            await ScanHandler(new ZXing.Result("1", null, null, ZXing.BarcodeFormat.QR_CODE));
        }


        /// <summary>
        /// Handler for the scan
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task ScanHandler(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // return if text can not be parsed as an int
                if (!int.TryParse(result.Text, out int id)) return;

                try
                {
                    // get the datalogger from a the scanned id
                    var res = await _databaseService.GetAsync<Datalogger>(id);

                    if (res.IsSuccessStatusCode)
                    {
                        // extract the datalogger data from the json
                        var datalogger = await res.ContentToObjectAsync<Datalogger>();

                        // assign the datalogger to the datalogger reference in the datalogger view model
                        DataloggerViewModel.Datalogger = datalogger;
                    }
                    else
                    {
                        // datalogger not found
                        await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {res.ReasonPhrase}, Datalogger not found", "Cancel", "ok");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {ex.Message}", "Cancel", "ok");
                }

                // pop the scan view
                //await (Application.Current.MainPage as NavigationPage).PopAsync();
            });
        }

        /// <summary>
        /// Resets the view model by creating a new datalogger view model and datalogger
        /// </summary>
        public void Reset()
        {
            DataloggerViewModel = new DataloggerViewModel() { Datalogger = new Datalogger() };
        }
    }
}
