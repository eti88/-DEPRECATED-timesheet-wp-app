using timesheet.Common;
using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using timesheet.Model;
using timesheet.View;
using SQLite;
using System.Threading.Tasks;
using System.Globalization;
using Windows.Phone.UI.Input;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;


// Il modello di applicazione hub è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkId=391641

namespace timesheet
{
    
    /// <summary>
    /// Fornisci un comportamento specifico dell'applicazione in supplemento alla classe Application predefinita.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;
        //Locazione e nome del database 
        public static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite"));
        CultureInfo ci = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);
        ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        private readonly string CRITTERCISM_APP_ID = "557035ab67a3707e4fe35f34";

        /// <summary>
        /// Inizializza l'oggetto Application singleton. Si tratta della prima riga del codice creato
        /// eseguita e, come tale, corrisponde all'equivalente logico di main() o WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            InitializeDb();
            this.Suspending += this.OnSuspending;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            SettingsInit();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {

            if (args.Kind == ActivationKind.VoiceCommand)
            {

                var voiceArgs = (IVoiceCommandActivatedEventArgs)args;
                var result = voiceArgs.Result;

                var frame = Window.Current.Content as Frame;
                frame.Navigate(typeof(Home), result);
                Window.Current.Content = frame;
                Window.Current.Activate();
                
            }

            base.OnActivated(args);

        }

        /// <summary>
        /// Richiamato quando l'applicazione viene avviata normalmente dall'utente.  All'avvio dell'applicazione
        /// verranno utilizzati altri punti di ingresso per aprire un file specifico, per visualizzare
        /// risultati di ricerche e così via.
        /// </summary>
        /// <param name="e">Dettagli sulla richiesta e il processo di avvio.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            AppSettings.IsAdBlockerActive = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses["adBlock"].IsActive;
            AppSettings.DisplayAds = !AppSettings.IsAdBlockerActive;

            // Inizializzo il crash report
            CrittercismSDK.Crittercism.Init(CRITTERCISM_APP_ID);

            Frame rootFrame = Window.Current.Content as Frame;

                // Non ripetere l'inizializzazione dell'applicazione se la finestra già dispone di contenuto,
                // assicurarsi solo che la finestra sia attiva.
                if (rootFrame == null)
                {
                    // Creare un frame che agisca da contesto di navigazione e passare alla prima pagina.
                    rootFrame = new Frame();

                    // Associare il frame a una chiave SuspensionManager.
                    SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                    // TODO: modificare questo valore su una dimensione di cache appropriata per l'applicazione.
                    rootFrame.CacheSize = 1;

                    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        // Ripristinare lo stato della sessione salvata solo se appropriato.
                        try
                        {
                            await SuspensionManager.RestoreAsync();
                        }
                        catch (SuspensionManagerException error)
                        {
                            CrittercismSDK.Crittercism.LogHandledException(error);
                            // Errore durante il ripristino dello stato.
                            // Si presuppone che non esista alcuno stato e continua.
                        }
                    }

                    // Posizionare il frame nella finestra corrente.
                    Window.Current.Content = rootFrame;
                }

                if (rootFrame.Content == null)
                {
                    // Rimuove l'avvio della navigazione turnstile.
                    if (rootFrame.ContentTransitions != null)
                    {
                        this.transitions = new TransitionCollection();
                        foreach (var c in rootFrame.ContentTransitions)
                        {
                            this.transitions.Add(c);
                        }
                    }

                    rootFrame.ContentTransitions = null;
                    rootFrame.Navigated += this.RootFrame_FirstNavigated;

                    // Quando lo stack di navigazione non viene ripristinato, esegui la navigazione alla prima pagina,
                    // configurando la nuova pagina per passare le informazioni richieste come parametro di
                    // navigazione.
                        if (!rootFrame.Navigate(typeof(Home), e.Arguments))
                        {
                            throw new Exception("Failed to create initial page");
                        }
                    
                }

            // Assicurarsi che la finestra corrente sia attiva.
            Window.Current.Activate();
            
            // Invia il Crash report
            // await HockeyClient.Current.SendCrashesAsync();
        }

        /// <summary>
        /// Ripristina le transizioni del contenuto dopo l'avvio dell'applicazione.
        /// </summary>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            
                var rootFrame = sender as Frame;
                rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
                rootFrame.Navigated -= this.RootFrame_FirstNavigated;

        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null && rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }

            
        }

        /// <summary>
        /// Richiamato quando l'esecuzione dell'applicazione viene sospesa.  Lo stato dell'applicazione viene salvato
        /// senza che sia noto se l'applicazione verrà terminata o ripresa con il contenuto
        /// della memoria ancora integro.
        /// </summary>
        /// <param name="sender">Origine della richiesta di sospensione.</param>
        /// <param name="e">Dettagli relativi alla richiesta di sospensione.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {

            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch(Exception error)
            {
                CrittercismSDK.Crittercism.LogHandledException(error);
            }
            return false;
        }

        private void InitializeDb() {
            if (!CheckFileExists("db.sqlite").Result)
            {
                using (var db = new SQLiteConnection(DB_PATH))
                {
                    db.CreateTable<TimeActivities>();
                    db.CreateTable<ActivityModel>();
                    var dbHelper = new DatabaseHelper();

                    // Se la tab dei modelli è vuota allora inserisci dei modelli di default
                    if (ci.TwoLetterISOLanguageName == "en")
                    {
                        dbHelper.InsertModel(new ActivityModel() { name = "Programming", description = "code writing" });
                        dbHelper.InsertModel(new ActivityModel() { name = "Drawing", description = "draw somethings" });
                        dbHelper.InsertModel(new ActivityModel() { name = "Cooking", description = "became a chef" });
                        dbHelper.InsertModel(new ActivityModel() { name = "Debugging", description = "debug your awesome code!" });
                        dbHelper.InsertModel(new ActivityModel() { name = "Eating", description = "eat somethings" });
                        dbHelper.InsertModel(new ActivityModel() { name = "Running", description = "" });
                    }
                    else if (ci.TwoLetterISOLanguageName == "it")
                    {
                        dbHelper.InsertModel(new ActivityModel() { name = "Programmazione", description = "Scrivo del codice" });
                        dbHelper.InsertModel(new ActivityModel() { name = "Disegno", description = "Disegno qualcosa" });
                        dbHelper.InsertModel(new ActivityModel() { name = "Cucinare", description = "preparo qualcosa da mangiare" });
                        dbHelper.InsertModel(new ActivityModel() { name = "Debugging", description = "cerco gli errori nel codice" });
                        dbHelper.InsertModel(new ActivityModel() { name = "Mangiare", description = "Mangio qualcosa" });
                        dbHelper.InsertModel(new ActivityModel() { name = "Corsa", description = "Vado a correre" });
                    }
                }
            }
        }

        private void SettingsInit()
        {
            // Creo la relativa chiave se non presente nei settaggi
            if (!settings.Values.ContainsKey("liveTile"))
            {
                settings.Values["liveTile"] = false;
            }

            SettingsSetup();
        }

        private void SettingsSetup()
        {
            if ((bool)settings.Values["liveTile"] == true)
            {
                CreateLiveTile();
            }
            else
            {
                TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            }
        }

        public static void CreateLiveTile()
        {
            CultureInfo ci = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);
            //crea un Tile updater e abilita le notifiche
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();
            var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText01);
            // Parte frontale con logo
            var tileImage = tileXml.GetElementsByTagName("image")[0] as XmlElement;
            tileImage.SetAttribute("src", "ms-appx:///Assets/SmallLogo.scale-240.png");
            // parte posteriore con testo
            var tileText = tileXml.GetElementsByTagName("text");
            if (ci.TwoLetterISOLanguageName == "en")
            {
                (tileText[0] as XmlElement).InnerText = "No activity";
                (tileText[1] as XmlElement).InnerText = "running";
            }
            else
            {
                (tileText[0] as XmlElement).InnerText = "No attività";
                (tileText[1] as XmlElement).InnerText = "in esecuzione";
            }
            

            var tileNotification = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }
    }
}
