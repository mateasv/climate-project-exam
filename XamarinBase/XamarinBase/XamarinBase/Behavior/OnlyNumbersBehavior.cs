using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace XamarinBase.Behavior
{
    public class OnlyNumbersBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.Completed += Validate;
            bindable.Unfocused += Validate;

            base.OnAttachedTo(bindable);
        }


        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.Completed -= Validate;
            bindable.Unfocused -= Validate;

            base.OnDetachingFrom(bindable);
        }

        private void Validate(object sender, EventArgs e)
        {
            if (sender is null) return;

            var entry = sender as Entry;

            if (entry.Text is null) return;

            var isValid = Regex.IsMatch(entry.Text, "^([-+] ?)?[0-9]+(.[0-9]+)?$");

            if (!isValid)
            {
                entry.Text = "";
            }

        }
    }
}
