using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace DonutChart
{
    public partial class MainPage : ContentPage
    {
        private SKCanvasDonutView _sKCanvasview;
        private const double _canvasViewHeight = 500;

        public MainPage()
        {
            InitializeComponent();
            var entries = new[]
                    {
                         new Entry(212)
                        {
                            Label = "UWP",
                            ValueLabel = "212",
                            Color = SKColor.Parse("#2c3e50")
                        },
                        new Entry(248)
                        {
                            Label = "Android",
                            ValueLabel = "248",
                            Color = SKColor.Parse("#77d065")
                        },
                        new Entry(128)
                        {
                            Label = "iOS",
                            ValueLabel = "128",
                            Color = SKColor.Parse("#b455b6")
                        },
                        new Entry(514)
                        {
                            Label = "Shared",
                            ValueLabel = "514",
                            Color = SKColor.Parse("#3498db")
                        }
                    };

            //var entries = new[]
            //        {
            //             new Entry(200)
            //            {
            //                Label = "UWP",
            //                ValueLabel = "212",
            //                Color = SKColor.Parse("#2c3e50")
            //            },
            //            new Entry(200)
            //            {
            //                Label = "Android",
            //                ValueLabel = "248",
            //                Color = SKColor.Parse("#77d065")
            //            }
            //        };

            _sKCanvasview = new SKCanvasDonutView
            {
                HeightRequest = _canvasViewHeight,
                Entries = entries,
                InnerText = "$-123.456M",
                TextColor = new SKColor(0, 0, 0, 255)
            };

            _stackLayout.Children.Insert(0, _sKCanvasview);
            _slider.Value = _slider.Maximum;
        }


        private void _sKCanvasview_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (_sKCanvasview != null)
            {
                _sKCanvasview.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await _sKCanvasview.StartAnimarion();
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            _sKCanvasview.HeightRequest = _canvasViewHeight * (e.NewValue / _slider.Maximum);
        }
    }
}
