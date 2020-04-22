using Anyline.SDK.Models;
using Anyline.SDK.Plugins.Meter;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_MultiFieldsMeter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResultsPage : Page
    {
        public ResultsPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var barcodeAndMeterResults = e.Parameter as Tuple<string, List<MeterMultiFieldsScanResult>>;

            if (barcodeAndMeterResults == null) return;

            var defaultMargin = new Thickness(20, 5, 20, 5);

            stackResults.Children.Add(new TextBlock { Text = "Barcode", FontSize = 15, Foreground = new SolidColorBrush(Colors.Gray), Margin = defaultMargin });
            stackResults.Children.Add(new TextBlock { Text = barcodeAndMeterResults.Item1, FontSize = 17, Foreground = new SolidColorBrush(Colors.Black), Margin = defaultMargin, FontWeight = FontWeights.Bold });

            stackResults.Children.Add(new Line { X1 = 0, X2 = 1, Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.LightGray), Margin = new Thickness(0, 10, 0, 10), StrokeThickness = 1 });

            foreach (var scanResult in barcodeAndMeterResults.Item2)
            {
                stackResults.Children.Add(new TextBlock { Text = "Counter", FontSize = 15, Foreground = new SolidColorBrush(Colors.Gray), Margin = defaultMargin });
                stackResults.Children.Add(new TextBlock { Text = scanResult.Counter, FontSize = 17, Foreground = new SolidColorBrush(Colors.Black), Margin = defaultMargin, FontWeight = FontWeights.Bold });

                stackResults.Children.Add(new TextBlock { Text = "Result", FontSize = 15, Foreground = new SolidColorBrush(Colors.Gray), Margin = defaultMargin });
                stackResults.Children.Add(new TextBlock { Text = scanResult.Result, FontSize = 17, Foreground = new SolidColorBrush(Colors.Black), Margin = defaultMargin, FontWeight = FontWeights.Bold });

                WriteableBitmap bitmap = await scanResult.CutoutImage.GetBitmapAsync();
                var img = new Image()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Source = bitmap,
                    Stretch = Stretch.Uniform,
                    Width = ApplicationView.GetForCurrentView().VisibleBounds.Width / 2,
                    Margin = defaultMargin
                };

                stackResults.Children.Add(img);

                stackResults.Children.Add(new Line { X1 = 0, X2 = 1, Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.LightGray), Margin = new Thickness(0, 10, 0, 10), StrokeThickness = 1 });
            }
        }
    }
}
