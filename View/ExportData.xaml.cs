using timesheet.Common;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using timesheet.ViewModel;
using Windows.UI.Popups;
using Windows.Storage;
using Microsoft.Live;
using System.Diagnostics;
using Windows.ApplicationModel.Email;
using System.Globalization;


// Il modello di elemento per la pagina base è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=390556

namespace timesheet.View
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class ExportData : Page
    {
        private NavigationHelper navigationHelper;
        //private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private TimeActivitiesVMExports vm = new TimeActivitiesVMExports();
        CultureInfo ci = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);


        public ExportData()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
  
        }

        ////////////////////// ads handler error //////////////////////////

        private void error_ads(object sender, Microsoft.Advertising.Mobile.Common.AdErrorEventArgs e)
        {

#if DEBUG

            Debug.WriteLine("############### ADS ###############");
            Debug.WriteLine(e.Error.Message);
            Debug.WriteLine("############### ADS ###############");
#endif

        }

        ////////////////////////////////// end //////////////////////////

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region Registrazione di NavigationHelper

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
         
        private async void SaveOneDrive_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder dir = Windows.Storage.ApplicationData.Current.TemporaryFolder;
            string fileName = DateTime.Now.ToString("dd-MM-yy") + "-export.csv";
            if (vm.Activities != null && vm.Activities.Count > 0) { vm.Activities.Clear(); }
            vm.GetAllElements();
            var tmp = vm.Activities;
            await vm.WriteToFile(tmp, dir, fileName, false);
            MessageDialog msgbox = new MessageDialog("File Uploaded");
            StorageFile strFile = await dir.GetFileAsync(fileName);

            try
            {
                List<String> oneDriveScopes = new List<String>() { "wl.signin", "wl.basic", "wl.skydrive", "wl.skydrive_update" };
                LiveAuthClient authClient = new LiveAuthClient();
                LiveLoginResult authResult;
                authResult = await authClient.LoginAsync(oneDriveScopes);
                if (authResult.Session != null)
                {
                    var liveConnectClient = new LiveConnectClient(authResult.Session);
                    string skyDriveFolder = "me/skydrive";

                    LiveOperationResult result = await liveConnectClient.BackgroundUploadAsync(skyDriveFolder, fileName, strFile, OverwriteOption.Rename);
                }
            }
            catch (LiveAuthException ex)
            {
                msgbox.Content = "Error: " + ex.ToString();
            }
            catch (LiveConnectException ex)
            {
                msgbox.Content = "Error: " + ex.ToString();
            }

            await msgbox.ShowAsync();
        }

        private async void SendByMail_Click(object sender, RoutedEventArgs e)
        {
            string msgBody = null;
            string msgSubject = null;

            // Imposto correttamente i messaggi nella lingua appropiata
            switch (ci.TwoLetterISOLanguageName.ToLower()) 
            { 
                case "it":
                    msgBody = "In allegato il file esportato.";
                    msgSubject = "CSV Timesheet esportato";
                    break;

                case "en":
                    msgBody = "Attached the exported file.";
                    msgSubject = "CSV exported Timesheet";
                    break;

                default:
                    msgBody = "Attached the exported file.";
                    msgSubject = "CSV exported Timesheet";
                    break;
            
            }


            StorageFolder dir = Windows.Storage.ApplicationData.Current.TemporaryFolder;
            string fileName = DateTime.Now.ToString("dd-MM-yy") + "-export.csv";
            if (vm.Activities != null && vm.Activities.Count > 0) { vm.Activities.Clear(); }
            vm.GetAllElements();
            var tmp = vm.Activities;
            await vm.WriteToFile(tmp, dir, fileName, false);
       
            StorageFile strFile = await dir.GetFileAsync(fileName);   

            //genera oggetto mail
            EmailMessage mail = new EmailMessage();
            mail.Subject = msgSubject;
            mail.Body = msgBody;
            mail.Attachments.Add(new EmailAttachment(strFile.Name, strFile));
            
            await EmailManager.ShowComposeNewEmailAsync(mail);

        }

    }
}
