using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

using NKH.MindSqualls;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Diagnostics;

namespace MindstormsNXTControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
            this.Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StaticVariables.Brick.Disconnect();
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }
            
        }

        #region Δηλώσεις Μεταβλητών

        int currentPage = 1;

        #endregion Δηλώσεις Μεταβλητών

        #region Γραφικό Μέρος

        #region Συνάρτηση για το animation των μενού.

        private void pageFadeOut(int currentPage)
        {
            switch (currentPage)
            {
                case 1:
                    Storyboard StartMenuUnload = (Storyboard)FindResource("StartMenuOut");
                    StartMenuUnload.Begin(this);
                    break;
                case 2:
                    Storyboard MainMenuUnload = (Storyboard)FindResource("MainMenuOut");
                    MainMenuUnload.Begin(this);
                    break;
                case 3:
                    Storyboard AboutUnload = (Storyboard)FindResource("SettingsOut");
                    AboutUnload.Begin(this);
                    break;
                default:
                    break;
            }
        }

        #endregion Συνάρτηση για το animation των μενού.

        #endregion Γραφικό Μέρος

        #region Κώδικας για την Start Menu.

        private void ButtonGoToMainMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Navigate to Main Menu.
            pageFadeOut(currentPage);
            Storyboard MainMenuLoad = (Storyboard)FindResource("MainMenuIn");
            MainMenuLoad.Begin(this);
            currentPage = 2;
        }

        private void ButtonGoToSettings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Navigate to Settings.
            pageFadeOut(currentPage);
            Storyboard SettingsLoad = (Storyboard)FindResource("SettingsIn");
            SettingsLoad.Begin(this);
            currentPage = 3;
        }

        private void ButtonShutDown_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion Κώδικας για την Start Menu.

        #region Κώδικας για την Main Menu.

        private void ButtonGoBack_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Navigate to Start Menu.
            pageFadeOut(currentPage);
            Storyboard StartMenuLoad = (Storyboard)FindResource("StartMenuIn");
            StartMenuLoad.Begin(this);
            currentPage = 1;
        }

        private void ButtonConnectNXT_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StaticVariables.ComPort = com.Text.Trim();
            StaticVariables.ConnectNXT();
        }


        private ServiceHost HostProxy;
        private void ButtonConnectWP7_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string address = "http://localhost:31337/LegoService";

                HostProxy = new ServiceHost(typeof(LegoService), new Uri(address));

                // Enable metadata publishing.

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();

                smb.HttpGetEnabled = true;

                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;

                

                HostProxy.Description.Behaviors.Add(smb);



                // Open the ServiceHost to start listening for messages. Since

                // no endpoints are explicitly configured, the runtime will create

                // one endpoint per base address for each service contract implemented

                // by the service.



                HostProxy.Open();

                MessageBox.Show("The service is ready at " + address);

            }

            catch (AddressAccessDeniedException)
            {

                MessageBox.Show("You need to reserve the address for this service");

                HostProxy = null;

            }

            catch (AddressAlreadyInUseException)
            {

                MessageBox.Show("Something else is already using this address");

                HostProxy = null;

            }

            catch (Exception ex)
            {

                MessageBox.Show("Something bad happened on startup: " + ex.Message);

                HostProxy = null;

            }
        }

        private void ButtonUp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StaticVariables.MoveForward();
        }

        private void ButtonDown_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StaticVariables.MoveBack();
        }       

        private void ButtonLeft_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StaticVariables.MoveLeft();
        }

        private void ButtonRight_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StaticVariables.MoveRight();
        }
		
		private void ButtonStop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StaticVariables.Stop();
        }

        #endregion Κώδικας για την Main Menu.

        #region Κώδικας για την Settings.

        private void ButtonSaveSettings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //List<string> list = new List<string>();

            //foreach (object item in listBox.Items)
            //{
            //list.Add(item.ToString());
            //}
        }

        #endregion Κώδικας για την Settings.



        
    }
}
