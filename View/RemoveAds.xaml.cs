using timesheet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Store;
using System.Diagnostics;

// Il modello di elemento per la pagina base è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=390556

namespace timesheet.View
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class RemoveAds : Page
    {
        private NavigationHelper navigationHelper;

        public class ProductItem
        {
            public string imgLink { get; set; }
            public string Status { get; set; }
            public string Name { get; set; }
            public string key { get; set; }
            public Visibility BuyNowButtonVisible { get; set; }
        }

        public RemoveAds()
        {
            this.InitializeComponent();
            
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Ottiene l'elemento <see cref="NavigationHelper"/> associato a questa <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Popola la pagina con il contenuto passato durante la navigazione.  Vengono inoltre forniti eventuali stati
        /// salvati durante la ricreazione di una pagina in una sessione precedente.
        /// </summary>
        /// <param name="sender">
        /// Origine dell'evento. In genere <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Dati evento che forniscono il parametro di navigazione passato a
        /// <see cref="Frame.Navigate(Type, Object)"/> quando la pagina è stata inizialmente richiesta e
        /// un dizionario di stato mantenuto da questa pagina nel corso di una sessione
        /// precedente.  Lo stato è null la prima volta che viene visitata una pagina.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Mantiene lo stato associato a questa pagina in caso di sospensione dell'applicazione o se la
        /// viene scartata dalla cache di navigazione.  I valori devono essere conformi ai requisiti di
        /// serializzazione di <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">Origine dell'evento. In genere <see cref="NavigationHelper"/></param>
        /// <param name="e">Dati di evento che forniscono un dizionario vuoto da popolare con
        /// uno stato serializzabile.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region Registrazione di NavigationHelper

        /// <summary>
        /// I metodi forniti in questa sezione vengono utilizzati per consentire a
        /// NavigationHelper di rispondere ai metodi di navigazione della pagina.
        /// <para>
        /// La logica specifica della pagina deve essere inserita nel gestore eventi per  
        /// <see cref="NavigationHelper.LoadState"/>
        /// e <see cref="NavigationHelper.SaveState"/>.
        /// Il parametro di navigazione è disponibile nel metodo LoadState 
        /// oltre allo stato della pagina conservato durante una sessione precedente.
        /// </para>
        /// </summary>
        /// <param name="e">Fornisce dati per i metodi di navigazione e
        /// i gestori eventi che non sono in grado di annullare la richiesta di navigazione.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            RenderStoreItems();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region purchase

        private async void DisableAds_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            //string key = btn.Tag.ToString();
            string key = "adBlock";

            if (!Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses[key].IsActive)
            {
                ListingInformation li = await Windows.ApplicationModel.Store.CurrentApp.LoadListingInformationAsync();
                string pID = li.ProductListings[key].ProductId;

                var receipt = await Windows.ApplicationModel.Store.CurrentApp.RequestProductPurchaseAsync(pID);

                //renderStoreItems();
            }
        }

        public ObservableCollection<ProductItem> picItems = new ObservableCollection<ProductItem>();

        public async void RenderStoreItems()
        {
            picItems.Clear();

            try
            {
                ListingInformation li = await Windows.ApplicationModel.Store.CurrentApp.LoadListingInformationAsync();

                foreach (string key in li.ProductListings.Keys)
                {
                    ProductListing pListing = li.ProductListings[key];

                    string status = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses[key].IsActive ? "Purchased" : pListing.FormattedPrice;
                    string imageLink = string.Empty;

                    picItems.Add(
                        new ProductItem
                        {
                            imgLink = key.Equals("adBlock") ? @"Assets\Images\block-ads.png" : @"Assets\Images\block-ads.png",
                            Name = pListing.Name,
                            Status = status,
                            key = key,
                            BuyNowButtonVisible = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses[key].IsActive ? Visibility.Collapsed : Visibility.Visible
                        });
                }
                pics.ItemsSource = picItems;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        #endregion
    }
}
