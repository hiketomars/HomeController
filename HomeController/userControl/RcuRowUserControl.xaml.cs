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
using HomeController.view;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace HomeController.userControl
{
    public sealed partial class RcuRowUserControl : UserControl
    {
        private readonly MainPresenter mainPresenter;

        public RcuRowUserControl(MainPresenter mainPresenter)
        {
            this.mainPresenter = mainPresenter;
            this.InitializeComponent();
        }

        public string LcuName { get; set; } // The name of the LCU that this RCU-row belongs to.
        public string RcuName { get; set; }

        public string NameText
        {
            get
            {
                return RcuNameTextBlock.Text;
            }
            set
            {
                RcuNameTextBlock.Text = value;
            }
        }

        public void AddTextToOutput(string text)
        {
            InfoTextBlock.Text += text;
        }

        private void ListenBtn_Click(object sender, RoutedEventArgs e)
        {
            InfoTextBlock.Text += "";
            mainPresenter.ListenBtn_Click(LcuName, RcuName);
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            InfoTextBlock.Text += "";
            mainPresenter.ConnectBtn_Click(LcuName, RcuName);
        }
    }
}
