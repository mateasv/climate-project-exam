using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace XamarinBase.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarcodeView : ContentPage
    {
        public BarcodeView()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            var scan = new ZXingScannerPage();
            await (Application.Current.MainPage as NavigationPage).PushAsync(scan);
            scan.OnScanResult += (async result =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await (Application.Current.MainPage as NavigationPage).PopAsync();
                    barcodeLabel.Text = $"Scanned Barcode: {result.Text}";
                });
            });
        }
    }
}