using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using HomeController.model;
using HomeController.utils;

namespace HomeController.view
{
    /// <summary>
    /// This is a presenter in the MVP-architecture.
    /// </summary>
    public class MainPresenter
    {
        private IMainView mainView;
        private IHouseModel houseModel;
        public MainPresenter(IMainView mainView)
        {
            Logger.Logg("Main", Logger.MainPresenter_Cat, "=============================================");
            Logger.Logg("Main", Logger.MainPresenter_Cat, "MainPresenter");
            this.mainView = mainView;
            houseModel = HouseModelFactory.GetHouseModel();
            houseModel.ModelHasChanged += new Definition.VoidEventHandler(ModelEventHandler_ModelHasChanged);
            houseModel.LCULedHasChanged += new Definition.LEDChangedEventHandler(ModelEventHandler_LCULedHasChanged);
        }

        // This is the handler method for the event ModelHasChanged that comes from the model.
        public void ModelEventHandler_ModelHasChanged()
        {
            var loggings = houseModel.GetLoggings();
            mainView.SetLoggItems(loggings);
        }

        public void ModelEventHandler_LCULedHasChanged(RGBValue rgbValue)
        {
            //No need to read model here since the value is supplied in the event.
            mainView.SetColorForBackdoorLED(rgbValue);
        }

        internal void StopApplication()
        {
            mainView.AddLoggItem("Stop clicked.");
            Application.Current.Exit();
        }

        internal void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            mainView.AddLoggItem("Info button clicked.");
        }

        public void ConnectBtn_Click()
        {
            houseModel.ConnectToRemoteLCU();
        }
    }
}
