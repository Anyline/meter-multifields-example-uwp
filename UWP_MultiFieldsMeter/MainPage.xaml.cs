using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP_MultiFieldsMeter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // for AnylineExamplesApp.Windows
        public static readonly string LICENSE_KEY = "eyAiZGVidWdSZXBvcnRpbmciOiAib24iLCAibGljZW5zZUtleVZlcnNpb24iOiAyLCAibWFqb3JWZXJzaW9uIjogIjMiLCAicGluZ1JlcG9ydGluZyI6IHRydWUsICJwbGF0Zm9ybSI6IFsgIldpbmRvd3MiIF0sICJzY29wZSI6IFsgIkFMTCIgXSwgInNob3dXYXRlcm1hcmsiOiB0cnVlLCAidG9sZXJhbmNlRGF5cyI6IDkwLCAidmFsaWQiOiAiMjAyMC0wMS0zMSIsICJ3aW5kb3dzSWRlbnRpZmllciI6IFsgIkFueWxpbmVFeGFtcGxlc0FwcC5XaW5kb3dzIiBdIH0Ka21tb1ljQng5TTFPS29tU0JnY1R5ZmRHM1BtT3RFbDA3VkhjYU1TZndyb25aNVJBeWp5dG9CeG5OY2NMNVJwegpkdm1raDZOQi9PYzN5eWcrMnRya0VOVzNaM2tielFaV2g1d0VIUE1zT3l1R01aclNPTlVtUnpOT0VIQ2xJTUVSCjVNMVkxWFlFb0RBeEx2VitwRGJYV3JFcXpLUnlFRCtiK0w3czN0UzgxdSs2QXNGYXNod0VJZnBXZ3d0d0UvdnEKZER4YzBNVmU3dEFrWTFKT0E5alVIYmFSNUNWMUZuVC9zYmNNMVpma1JYcVRxV0lMbWRiMWJRTktyTkV0NUpZUQpTeGZSc3hxSHczL3Z5aGJlV2NETXYyc3ZaZzZQWkgyVFgzaHhnQVZpRG5TMlhtMXFpWnhETlRzZUdqR3hrVDB1Cld2bk5ZMmV4czZtc1BkL0dWclNwdGc9PQo=";

        public MainPage()
        {
            this.InitializeComponent();

            // We don't want to keep multiple instances of the scan views that we're navigating to.
            NavigationCacheMode = NavigationCacheMode.Required;
            ((Frame)Window.Current.Content).CacheSize = 0;

            // Back button functionality
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

            GoScan.Click -= GoScan_Click;
            GoScan.Click += GoScan_Click;
        }

        void App_BackRequested(object sender, BackRequestedEventArgs args)
        {
            if (!(Window.Current.Content is Frame rootFrame)) return;

            // Navigate back if possible, and if the event has not already been handled.
            if (args.Handled == false)
            {
                args.Handled = true;
                if (rootFrame.CanGoBack && rootFrame.CurrentSourcePageType != typeof(MainPage))
                    rootFrame.GoBack();
                else
                    Application.Current.Exit();
            }
        }

        private void GoScan_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(BarcodeScanPage));
        }

    }
}
