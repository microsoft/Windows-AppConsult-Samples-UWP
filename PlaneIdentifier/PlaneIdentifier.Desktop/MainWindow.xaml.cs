using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;

namespace PlaneIdentifier.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PlanesModel planeModel;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///PlaneIdentifier.Desktop/PlanesModel.onnx"));
            planeModel = await PlanesModel.CreatePlanesModel(modelFile);
        }

        private async void OnRecognize(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                var selectedStorageFile = await StorageFile.GetFileFromPathAsync(dialog.FileName);

                SoftwareBitmap softwareBitmap;

                using (IRandomAccessStream stream = await selectedStorageFile.OpenAsync(FileAccessMode.Read))
                {
                    // Create the decoder from the stream 
                    Windows.Graphics.Imaging.BitmapDecoder decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);

                    // Get the SoftwareBitmap representation of the file in BGRA8 format
                    softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }

                // Display the image
                //SoftwareBitmapSource imageSource = new SoftwareBitmapSource();
                //await imageSource.SetBitmapAsync(softwareBitmap);

                //PreviewImage.Source = imageSource;

                PreviewImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(fileName));

                // Encapsulate the image in the WinML image type (VideoFrame) to be bound and evaluated
                VideoFrame inputImage = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap);
                await EvaluateVideoFrameAsync(inputImage);
            }
        }

        private async Task EvaluateVideoFrameAsync(VideoFrame frame)
        {
            if (frame != null)
            {
                try
                {
                    PlanesModelInput inputData = new PlanesModelInput();
                    inputData.data = frame;
                    var results = await planeModel.EvaluateAsync(inputData);
                    var loss = results.loss.ToList().OrderBy(x => -(x.Value));
                    var labels = results.classLabel;

                    var lossStr = string.Join(",  ", loss.Select(l => l.Key + " " + (l.Value * 100.0f).ToString("#0.00") + "%"));
                    float value = results.loss["plane"];
                    bool isPlane = false;
                    if (value > 0.75)
                    {
                        isPlane = true;
                    }

                    string message = $"Predictions: {lossStr} - Is it a plane? {isPlane}";
                    Status.Text = message;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"error: {ex.Message}");
                    Status.Text = $"error: {ex.Message}";
                }
            }
        }
    }
}
