using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace cntrl
{
    public class MyMaskedTextBox : TextBox
    {
        //For Masking
        private MaskedTextProvider _mprovider = null;
        public string Mask
        {
            get
            {
                if (_mprovider != null) return _mprovider.Mask;
                else return "";
            }
            set
            {
                _mprovider = new MaskedTextProvider(value);
                _mprovider.Set(this.Text);
                this.Text = _mprovider.ToDisplayString();
            }
        }

        //Some credentials
        private bool _ignoreSpace = true;
        public bool IgnoreSpace
        {
            get { return _ignoreSpace; }
            set { _ignoreSpace = value; }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (this.SelectionLength > 1)
            {
                this.SelectionLength = 0;
                e.Handled = true;
            }
            if (e.Key == Key.Insert ||
                e.Key == Key.Delete ||
                e.Key == Key.Back ||
               (e.Key == Key.Space && _ignoreSpace))
            {
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

        //To come over insert key problem.
        private void PressKey(Key key)
        {
            KeyEventArgs eInsertBack = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key);
            eInsertBack.RoutedEvent = KeyDownEvent;
            InputManager.Current.ProcessInput(eInsertBack);
        }
        private bool _InsertIsON = false;
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (!_InsertIsON)
            {
                PressKey(Key.Insert);
                _InsertIsON = true;
            }
        }

        //inserted text will be valid/invalid
        private bool _NewTextIsOk = false;
        public bool NewTextIsOk
        {
            get { return _NewTextIsOk; }
            set { _NewTextIsOk = value; }
        }
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            System.ComponentModel.MaskedTextResultHint hint;
            int TestPosition;

            if (e.Text.Length == 1)
                this._NewTextIsOk = _mprovider.VerifyChar(e.Text[0], this.CaretIndex, out hint);
            else
                this._NewTextIsOk = _mprovider.VerifyString(e.Text, out TestPosition, out hint);

            base.OnPreviewTextInput(e);
        }

        //When the text is about to be inserted:
        protected override void OnTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            string PreviousText = this.Text;
            if (NewTextIsOk)
            {
                base.OnTextInput(e);
                if (_mprovider.VerifyString(this.Text) == false) this.Text = PreviousText;
                while (!_mprovider.IsEditPosition(this.CaretIndex) && _mprovider.Length > this.CaretIndex) this.CaretIndex++;

            }
            else
                e.Handled = true;
        }

        //prevent the control from losing the focus until the mask is full with valid data
        private bool _stayInFocusUntilValid = false;
        public bool StayInFocusUntilValid
        {
            get { return _stayInFocusUntilValid; }
            set { _stayInFocusUntilValid = value; }
        }
        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (StayInFocusUntilValid)
            {
                _mprovider.Clear();
                _mprovider.Add(this.Text);
                if (!_mprovider.MaskFull) e.Handled = true;
            }

            base.OnPreviewLostKeyboardFocus(e);
        }
    }
}
