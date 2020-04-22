using Anyline.SDK.Plugins;
using Anyline.SDK.Plugins.Barcode;
using Anyline.SDK.ViewPlugins;
using Anyline.SDK.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWP_MultiFieldsMeter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BarcodeScanPage : Page, IScanResultListener<BarcodeScanResult>
    {
        private AbstractBaseScanViewPlugin scanViewPlugin;
        private ScanView anylineScanView;

        public BarcodeScanPage()
        {
            this.InitializeComponent();
            anylineScanView = AnylineScanView;
        }

        private void CameraView_CameraOpened(object sender, Size args)
        {
            try
            {
                anylineScanView.StartScanning();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"(APP) {e.Message}");
            }
        }

        private void CameraView_CameraClosed(object sender, bool success)
        {
            try
            {
                anylineScanView?.StopScanning();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"(APP) {e.Message}");
            }
        }

        private void CameraView_CameraError(object sender, Exception exception)
        {
            Debug.WriteLine($"(APP) Camera Error: {exception.Message}");
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            base.OnNavigatingFrom(args);

            Window.Current.VisibilityChanged -= Current_VisibilityChanged;

            await anylineScanView?.StopCameraAsync();

            anylineScanView?.Dispose();
            anylineScanView = null;

            scanViewPlugin?.Dispose();
            scanViewPlugin = null;
        }

        // important because the UWP camera stream automatically shuts down when a window is minimized
        private async void Current_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs args)
        {
            if (args.Visible == false)
            {
                await anylineScanView.StopCameraAsync();
            }
            if (args.Visible == true)
            {
                anylineScanView.StartCamera();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (anylineScanView != null)
            {
                anylineScanView.CameraView.CameraOpened -= CameraView_CameraOpened;
                anylineScanView.CameraView.CameraClosed -= CameraView_CameraClosed;
                anylineScanView.CameraView.CameraError -= CameraView_CameraError;

                anylineScanView.CameraView.CameraOpened += CameraView_CameraOpened;
                anylineScanView.CameraView.CameraClosed += CameraView_CameraClosed;
                anylineScanView.CameraView.CameraError += CameraView_CameraError;

                Window.Current.VisibilityChanged -= Current_VisibilityChanged;
                Window.Current.VisibilityChanged += Current_VisibilityChanged;
            }

            try
            {
                anylineScanView?.Init("Assets/barcode_config.json", MainPage.LICENSE_KEY);

                scanViewPlugin = anylineScanView?.ScanViewPlugin;
                (scanViewPlugin as BarcodeScanViewPlugin)?.AddScanResultListener(this);

                if (!anylineScanView.CameraView.IsCameraOpen())
                    anylineScanView.StartCamera();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            ThreadPoolTimer timer = ThreadPoolTimer.CreateTimer(async (t) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ttScanBarcode.Visibility = Visibility.Collapsed);
            }, TimeSpan.FromSeconds(5));
        }

        public void OnResult(BarcodeScanResult result)
        {
            (Window.Current.Content as Frame).Navigate(typeof(MeterScanPage), result.Result);

            // Removes this screen from the stack
            Frame.BackStack.Remove(Frame.BackStack.Last());
        }
    }
}
