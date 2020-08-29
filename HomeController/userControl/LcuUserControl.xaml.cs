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

        public void SetTextToOutput(string text)
        {
            InfoTextBlock.Text = text;
        }

        public void AddTextToOutput(string text)
        {
            InfoTextBlock.Text += text;
        }

        // Check and disable all UseVirtualxxx-controls. 
        // This will force use of virtual properties. Typically used when no read world values are available.
        public void CheckAndDisableUseVirtualIo(bool checkAndDisable)
        {
            if(checkAndDisable)
            {
                UseVirtualDoorOpenCb.IsChecked = true;
                UseVirtualDoorFloatingCb.IsChecked = true;
                UseVirtualDoorLockedCb.IsChecked = true;

                UseVirtualDoorOpenCb.IsEnabled = false;
                UseVirtualDoorFloatingCb.IsEnabled = false;
                UseVirtualDoorLockedCb.IsEnabled = false;

            }
            else
            {
                UseVirtualDoorOpenCb.IsChecked = false;
                UseVirtualDoorFloatingCb.IsChecked = false;
                UseVirtualDoorLockedCb.IsChecked = false;

                UseVirtualDoorOpenCb.IsEnabled = true;
                UseVirtualDoorFloatingCb.IsEnabled = true;
                UseVirtualDoorLockedCb.IsEnabled = true;
            }
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

        private void DoorIsOpenCb_OnClick(object sender, RoutedEventArgs e)
        {
            mainPresenter.DoorIsOpen_OnClick(LcuName, DoorIsOpenCb.IsChecked);
        }

        private void DoorIsFloatingCb_OnClick(object sender, RoutedEventArgs e)
        {
            mainPresenter.DoorIsUnsealed_OnClick(LcuName, DoorIsFloatingCb.IsChecked);
        }

        private void DoorIsLockedCb_OnClick(object sender, RoutedEventArgs e)
        {
            mainPresenter.DoorIsLocked_OnClick(LcuName, DoorIsLockedCb.IsChecked);
        }

        private void UseVirtualDoor_OnClick(object sender, RoutedEventArgs e)
        {
            //mainPresenter.UseVirtualDoor_OnClick(LcuName, UseVirtualDoorCb.IsChecked);
            UseVirtualDoorOpenCb.IsChecked = !UseVirtualDoorOpenCb.IsChecked;
            UseVirtualDoorFloatingCb.IsChecked = UseVirtualDoorOpenCb.IsChecked;
            UseVirtualDoorLockedCb.IsChecked = UseVirtualDoorOpenCb.IsChecked;
        }

        private void UseVirtualDoorOpen_OnClick(object sender, RoutedEventArgs e)
        {
            mainPresenter?.UseVirtualDoorOpen_OnClick(LcuName, UseVirtualDoorOpenCb.IsChecked);
        }

        private void UseVirtualDoorFloating_OnClick(object sender, RoutedEventArgs e)
        {
            mainPresenter?.UseVirtualDoorFloating_OnClick(LcuName, UseVirtualDoorFloatingCb.IsChecked);
        }

        // todo Nedanstådende tre eventhanterare anterar Checked/Unchecked event så namnet borde bytas till ..._Checked resp ..._Unchecked.
        private void UseVirtualDoorLocked_OnClick(object sender, RoutedEventArgs e)
        {
            mainPresenter?.UseVirtualDoorLocked_OnClick(LcuName, UseVirtualDoorLockedCb.IsChecked);
        }

        private void CheckUncheckAllUseVirtual_OnClick(object sender, RoutedEventArgs e)
        {
            CheckUncheckAllUseVirtualCb.IsChecked = false;
            mainPresenter?.CheckUncheckAllUseVirtual_OnClick(LcuName);
        }

        public void CheckAllUseVirtual()
        {
            UseVirtualDoorOpenCb.IsChecked = true;
            UseVirtualDoorFloatingCb.IsChecked = true;
            UseVirtualDoorLockedCb.IsChecked = true;
        }

        public void UncheckAllUseVirtual()
        {
            UseVirtualDoorOpenCb.IsChecked = false;
            UseVirtualDoorFloatingCb.IsChecked = false;
            UseVirtualDoorLockedCb.IsChecked = false;
        }
    }
}
