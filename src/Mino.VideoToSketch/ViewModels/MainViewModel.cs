using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Mino.VideoToSketch.ViewModels;

public class MainViewModel : ReactiveObject
{
    private VideoCapture Capture { get; }
    private CancellationTokenSource? CaptureCancellationTokenSource { get; set; }
    [Reactive] public BitmapSource? ImageSource { get; private set; }

    public IReactiveCommand OpenCameraCommand { get; }
    public IReactiveCommand CloseCameraCommand { get; }

    public MainViewModel()
    {
        Capture = new VideoCapture();

        OpenCameraCommand = ReactiveCommand.Create(OpenCamera);
        CloseCameraCommand = ReactiveCommand.Create(CloseCamera);
    }

    private void OpenCamera()
    {
        if (Capture!.Open(0))
        {
            CaptureCancellationTokenSource = new();
            var thread = new Thread(() => PlayCamera(CaptureCancellationTokenSource.Token));
            thread.Start();
        }
    }

    private void CloseCamera()
    {
        if (CaptureCancellationTokenSource != null)
        {
            CaptureCancellationTokenSource.Cancel();
            CaptureCancellationTokenSource = null;
        }

        if (Capture!.IsOpened())
        {
            Capture.Release();
        }

        ImageSource = null;
    }

    private void PlayCamera(CancellationToken ctoken)
    {
        using var frame = new Mat();
        using var sketch = new Mat();

        while (Capture != null && !Capture.IsDisposed)
        {
            if (ctoken.IsCancellationRequested) break;

            if (Capture.Read(frame))
            {
                if (frame.Empty())
                    break;

                Convert(frame, sketch);

                Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {


                    ImageSource = sketch.ToWriteableBitmap();
                }));
            }
            else
            {
                break;
            }
        }
    }

    public void OnClosed()
    {
        CloseCamera();
        Capture?.Dispose();
    }

    private static void Convert(Mat frame, Mat img_blend)
    {
        using var img_gray = new Mat();
        Cv2.CvtColor(frame, img_gray, ColorConversionCodes.BGR2GRAY);

        using var img_blur = new Mat();
        Cv2.GaussianBlur(img_gray, img_blur, new(21, 21), 0, 0);

        //var img_blend = new Mat();
        Cv2.Divide(img_gray, img_blur, img_blend, scale: 256);

        Cv2.CvtColor(img_blend, img_blend, ColorConversionCodes.GRAY2BGR);
    }
}
