using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinBase.ViewModels;

namespace XamarinBase.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPlantView : ContentView
    {
        public EditPlantView()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<EditPlantViewModel>();
        }
    }
}