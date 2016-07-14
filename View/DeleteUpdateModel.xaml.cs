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
using timesheet.Model;
using Windows.Phone.UI.Input;

// Il modello di elemento per la pagina base è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=390556

namespace timesheet.View
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class DeleteUpdateModel : Page
    {
        private int Selected_Model_id = 0;
        private DatabaseHelper Db_Helper = new DatabaseHelper();
        private ActivityModel currentModel = new ActivityModel();

        private NavigationHelper navigationHelper;
        //private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public DeleteUpdateModel()
        {
            this.InitializeComponent();
            //HardwareButtons.BackPressed += HardwareBackButtons_BackPressed;
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
        public ActivityModel DefaultViewModel
        {
            get { return this.currentModel; }
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
            Selected_Model_id = int.Parse(e.Parameter.ToString());
            currentModel = Db_Helper.ReadModel(Selected_Model_id);
            txtNameBox.Text = currentModel.name;
            txtDescBox.Text = currentModel.description;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        
        void HardwareBackButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame.Navigate(typeof(ReadModelsList));
        }

        private void NavigateToList() {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(ReadModelsList));
            }
        }

        private void DeleteModel_Click(object sender, RoutedEventArgs e)
        {
            Db_Helper.DeleteModel(Selected_Model_id);
            NavigateToList();
        }

        private void UpdateModel_Click(object sender, RoutedEventArgs e)
        {
            currentModel.name = txtNameBox.Text;
            currentModel.description = txtDescBox.Text;
            Db_Helper.UpdateModel(currentModel);
            NavigateToList();
        }
    }
}
