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
    public sealed partial class LcuUserControl : UserControl
    {
        private readonly MainPresenter mainPresenter;
        private List<RcuRowUserControl> rcuUserControlList = new List<RcuRowUserControl>();

        public LcuUserControl(MainPresenter mainPresenter)
        {
            this.mainPresenter = mainPresenter;
            this.InitializeComponent();
        }

        public string LcuName { get; set; } 

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

        // Adds an RCU GUI-row to this LCU-GUI.
        public void AddRcu(string rcuName)
        {
            var rcuUserControl = new RcuRowUserControl(mainPresenter);
            rcuUserControl.LcuName = LcuName;
            rcuUserControl.Name = rcuName;
            rcuUserControl.NameText= rcuName;
            rcuUserControl.RcuName = rcuName;
            rcuUserControl.ClearOutput();
            rcuUserControl.AddTextToOutput("Rcu " + rcuName + " created.\r\n");
            rcuUserControlList.Add(rcuUserControl);
            RcuStackPanel.Children.Add(rcuUserControl);
        }

        // Returns all RcuUserControls that is part of this LcuUserControl.
        public List<RcuRowUserControl> GetRcuUserControls()
        {
            return rcuUserControlList;
        }

        public void AddTextToOutput(string text)
        {
            InfoTextBlock.Text += text;
        }

        private void ListenAllBtn_Click(object sender, RoutedEventArgs e)
        {
            InfoTextBlock.Text += "";
            mainPresenter.ListenAllBtn_Click(LcuName);
        }

        private void ConnectAllBtn_Click(object sender, RoutedEventArgs e)
        {
            InfoTextBlock.Text += "";
            mainPresenter.ConnectAllBtn_Click(LcuName);
        }

        private void ClearAllBtn_Click(object sender, RoutedEventArgs e)
        {
            InfoTextBlock.Text += "";
            mainPresenter.ClearAllBtn_Click(LcuName);
        }
    }
}
