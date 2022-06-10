using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XamarinBase.ViewModels
{
    /// <summary>
    /// Base view model used as the superclass of all view models
    /// </summary>
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
