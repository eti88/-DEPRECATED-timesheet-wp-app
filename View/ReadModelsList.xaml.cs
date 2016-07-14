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
using Windows.UI.Popups;
using timesheet.Model;
using System.Collections.ObjectModel;
using timesheet.ViewModel;
using Windows.Phone.UI.Input;
using System.Diagnostics;

// Il modello di elemento per la pagina base è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=390556

namespace timesheet.View
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class ReadModelsList : Page
    {
        ObservableCollection<ActivityModel> DB_ModelsList = new ObservableCollection<ActivityModel>();

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public ReadModelsList()
        {
            this.InitializeComponent();

            //HardwareButtons.BackPressed += HardwareBackButtons_BackPressed;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
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
        /// Ottiene il modello di visualizzazione per questa <see cref="Page"/>.
        /// È possibile sostituirlo con un modello di visualizzazione fortemente tipizzato.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
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
            ReadAllModelsList dbModels = new ReadAllModelsList();
            DB_ModelsList = dbModels.GetAllModels();
            if (DB_ModelsList.Count > 0)
            {
                Btn_Delete.IsEnabled = true;
            }

            listBoxobj.ItemsSource = DB_ModelsList.OrderByDescending(i => i.id).ToList();
            this.navigationHelper.OnNavigatedTo(e);
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e )
        {
            if (e.NavigationMode == NavigationMode.Back) { this.NavigationCacheMode = NavigationCacheMode.Disabled; }

               this.navigationHelper.OnNavigatedFrom(e);
               
        }

        #endregion

        protected void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }

        }
        
         
        private void AddModels_Click(object sender, RoutedEventArgs e) {
            Frame.Navigate(typeof(AddModel));
        }

        private async void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Sei sicuro di voler eliminare tutti i modelli?");
            dialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(Command)));
            dialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(Command)));
            await dialog.ShowAsync();
        }

        private void Command(IUICommand command)
        {
            if (command.Label.Equals("Yes"))
            {
                DatabaseHelper DB_Helper = new DatabaseHelper();
                DB_Helper.DeleteAllModels();
                DB_ModelsList.Clear();
                Btn_Delete.IsEnabled = false;
                listBoxobj.ItemsSource = DB_ModelsList;
            }
        }

        private void listBoxobj_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int selectedModelId = 0;
            if (listBoxobj.SelectedIndex != -1)
            {
                ActivityModel listItem = listBoxobj.SelectedItem as ActivityModel;
                Frame.Navigate(typeof(DeleteUpdateModel), selectedModelId = listItem.id);
            }
        }
    }
}
