using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace XamarinBase.ViewModels
{
    public class EditDataloggerViewModel : BaseViewModel
    {
        private DataloggerViewModel _dataloggerViewModel;



        public DataloggerViewModel DataloggerViewModel
        {
            get { return _dataloggerViewModel; }
            set { _dataloggerViewModel = value; }
        }

        public ICommand ScanDataloggerCmd { get; set; }

        public EditDataloggerViewModel()
        {
            ScanDataloggerCmd = new Command(async () => await ScanDatalogger());
        }

        public async Task ScanDatalogger()
        {
            var scan = new ZXingScannerPage();
            await (Application.Current.MainPage as NavigationPage).PushAsync(scan);

            //scan.OnScanResult += async (result) => await InitDatalogger(result);
            scan.OnScanResult += (async result =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await (Application.Current.MainPage as NavigationPage).PopAsync();
                    DataloggerViewModel.DataloggerId = int.Parse(result.Text);
                });
            });
        }


        private async Task InitDatalogger(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (int.TryParse(result.Text, out int dataloggerId))
                {
                    DataloggerViewModel.DataloggerId = dataloggerId;
                }

                await (Application.Current.MainPage as NavigationPage).PopAsync();
            });

            
        }
    }
}
