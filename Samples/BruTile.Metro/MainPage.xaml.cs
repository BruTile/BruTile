using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace BruTile.Metro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void ButtonClickZoomin(object sender, RoutedEventArgs e)
        {
            canvas.ZoomInOneStep();            
        }

        void ButtonClickZoomout(object sender, RoutedEventArgs e)
        {
            canvas.ZoomOutOneStep();
        }        
        
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }
       
    }
}
