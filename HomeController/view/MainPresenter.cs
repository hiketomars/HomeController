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
    public class MainPresenter
    {
        private IMainView mainView;
        private IHouseModel houseModel;
        public MainPresenter(IMainView mainView)
        {
            Logger.Logg("=============================================");
            Logger.Logg("MainPresenter");
            this.mainView = mainView;
            houseModel = HouseModelFactory.GetHouseModel();
            houseModel.ModelHasChanged += new Definition.VoidEventHandler(ModelEventHandler_ModelHasChanged);
        }

        // This is the handler method for the event ModelHasChanged that comes from the model.
        public void ModelEventHandler_ModelHasChanged()
        {
            var loggings = houseModel.GetLoggings();
            mainView.SetLoggingItems(loggings);
        }

        internal void StopApplication()
        {
            //LoggInGui("Stop clicked.");
            mainView.Logg("Stop clicked.");
            Application.Current.Exit();
        }

        internal void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            mainView.Logg("Info button clicked.");
            
        }
    }
}
