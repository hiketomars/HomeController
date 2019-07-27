using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace HomeController.userControl
{
    public sealed partial class LcuUserControl : UserControl
    {
        public LcuUserControl()
        {
            this.InitializeComponent();
        }

        public string NameText
        {
            get
            {
                return LcuNameTextBlock.Text;
            }
            set
            {
                LcuNameTextBlock.Text = value;
            }
        }

        private TextBlock output;
        public TextBlock Output
        {
            get
            {
                return output;
            }
            set
            {
                output = value;
            }
        }

        public void AddTextToOutput(string text)
        {
            InfoTextBlock.Text += text;
        }

        private void ListenBtn_Click(object sender, RoutedEventArgs e)
        {
            InfoTextBlock.Text += "Listen button is not implemented yet.";
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            InfoTextBlock.Text += "Connect button is not implemented yet.";
        }
    }
}
