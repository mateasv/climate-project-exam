﻿using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace XamarinBase.ViewModels
{
    public class PlantViewModel : BaseViewModel
    {
        public Plant Plant { get; set; }

        private ImageSource _image;

        public ImageSource Image 
        {
            get
            {
                if(_image == null)
                {
                    _image = ImageSource.FromStream(() => new MemoryStream(Plant?.Image));
                }
                return _image;
            }
            set
            {
                _image = value;
            }
        
        }
    }
}
