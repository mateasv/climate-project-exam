using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XamarinBase.ViewModels
{
    public class BaseViewModel : BindableObject
    {
        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

    }
}
