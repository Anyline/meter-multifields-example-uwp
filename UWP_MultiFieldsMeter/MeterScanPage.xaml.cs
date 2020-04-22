using Anyline.SDK.Plugins;
using Anyline.SDK.Plugins.Meter;
using Anyline.SDK.ViewPlugins;
using Anyline.SDK.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public sealed partial class MeterScanPage : Page, IScanResultListener<MeterScanResult>
    {
        private AbstractBaseScanViewPlugin scanViewPlugin;
        private ScanView anylineScanView;
        private string BarcodeResultString;
       
        ThreadPoolTimer TimerCountdown = null;
        int RemainingSeconds = 20;

        List<string> ValidCounters = new List<string> { "161", "162", "180", "181" };
        List<MeterMultiFieldsScanResult> ScanResults = new List<MeterMultiFieldsScanResult>();
        int LastCounterIndex = -1;

        public MeterScanPage()
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

        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            BarcodeResultString = args.Parameter as string;

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
                anylineScanView?.Init("Assets/energy_config_digital_multifields.json", MainPage.LICENSE_KEY);

                scanViewPlugin = anylineScanView?.ScanViewPlugin;
                (scanViewPlugin as MeterScanViewPlugin)?.AddScanResultListener(this);

                if (!anylineScanView.CameraView.IsCameraOpen())
                    anylineScanView.StartCamera();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            ThreadPoolTimer timerToolTip = ThreadPoolTimer.CreateTimer(async (t) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ttScanMeter.Visibility = Visibility.Collapsed);
            }, TimeSpan.FromSeconds(5));

            TimerCountdown = ThreadPoolTimer.CreatePeriodicTimer(async (t) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    tbTimer.Text = RemainingSeconds.ToString();
                    if (RemainingSeconds <= 0)
                    {
                        StopScanning();
                    }
                    RemainingSeconds--;
                });
            }, TimeSpan.FromSeconds(1));

        }

        public void OnResult(MeterScanResult result)
        {
            RemainingSeconds = 20;

            var multifieldsResult = (result as MeterMultiFieldsScanResult);
            if (!ValidCounters.Contains(multifieldsResult.Counter)) return;

            if (LastCounterIndex > 0 && ScanResults[0].Counter == multifieldsResult.Counter)
            {
                StopScanning();
            }

            if (LastCounterIndex == -1 || ScanResults[LastCounterIndex].Counter != multifieldsResult.Counter)
            {
                ScanResults.Add(multifieldsResult);
                LastCounterIndex++;
                return;
            }

            if (ScanResults[LastCounterIndex].Confidence < multifieldsResult.Confidence)
                ScanResults[LastCounterIndex] = multifieldsResult;
        }

        private void StopScanning_Click(object sender, RoutedEventArgs e)
        {
            StopScanning();
        }

        private void StopScanning()
        {
            if (anylineScanView != null)
                anylineScanView.StopScanning();

            if (TimerCountdown != null)
                TimerCountdown.Cancel();

            var barcodeAndMeterResults = new Tuple<string, List<MeterMultiFieldsScanResult>>(BarcodeResultString, ScanResults);
            (Window.Current.Content as Frame).Navigate(typeof(ResultsPage), barcodeAndMeterResults);

            // Removes this screen from the stack
            Frame.BackStack.Remove(Frame.BackStack.Last());
        }

    }
}
