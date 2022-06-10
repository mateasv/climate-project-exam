using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace XamarinBase.Behavior
{
    /// <summary>
    /// Behavior used in entries throughout the program.
    /// </summary>
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

        /// <summary>
        /// Handler called when the entry fires a Completed or Unfocused event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validate(object sender, EventArgs e)
        {
            // return if sender is null
            if (sender is null) return;

            // sender as entry
            var entry = sender as Entry;

            // if the text of the entry is null, then return
            if (entry.Text is null) return;

            // apply regex to the text
            var isValid = Regex.IsMatch(entry.Text, "^([-+] ?)?[0-9]+(.[0-9]+)?$");

            // if the text is not valid, reset the text
            if (!isValid)
            {
                entry.Text = "";
            }
        }
    }
}
