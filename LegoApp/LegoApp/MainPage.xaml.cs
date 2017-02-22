using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using LegoApp.LegoServiceReference;
using System.ServiceModel;

namespace LegoApp
{
    public partial class MainPage : PhoneApplicationPage
    {

        LegoServiceClient client;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            client = new LegoServiceClient(new BasicHttpBinding(), new EndpointAddress("http://v-digkan2:31337/LegoService"));
        }

        private void upBtn_Click(object sender, RoutedEventArgs e)
        {
            client.ForwardAsync();
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            client.StopAsync();
        }

        private void downBtn_Click(object sender, RoutedEventArgs e)
        {
            client.BackAsync();
        }

        private void leftBtn_Click(object sender, RoutedEventArgs e)
        {
            client.LeftAsync();
        }

        private void rightBtn_Click(object sender, RoutedEventArgs e)
        {
            client.RightAsync();
        }
    }
}