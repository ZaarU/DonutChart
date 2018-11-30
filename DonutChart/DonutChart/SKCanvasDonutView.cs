using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace DonutChart
{
    internal class SKCanvasDonutView : SKCanvasView
    {
        /// <summary>
        /// Gets or sets the data entries.
        /// </summary>
        /// <value>The entries.</value>
        public IEnumerable<Entry> Entries { get; set; }

        /// <summary>
        /// Gets or sets the radius of the hole in the center of the chart.
        /// </summary>
        /// <value>The hole radius.</value>
        public float HoleRadius { get; set; } = 0.9f;

        /// <summary>
        /// Gets or sets the speed of animation.
        /// </summary>
        /// <value>Count of frames in animation</value>
        public int AnimationEndFrameCount { get; set; } = 20;

        /// <summary>
        /// Gets or sets text color of inner text in HEX with alpha.
        /// </summary>
        /// <value>String in HEX. Alpha optional. #AAFFFFFF or #ffffff</value>
        public string TextColorHEX
        {
            get
            {
                return TextColor.ToString();
            }
            set
            {
                TextColor = SKColor.Parse(value);
            }
        }

        /// <summary>
        /// Gets or sets text color of inner text.
        /// </summary>
        /// <value>SKColor of text.</value>
        public SKColor TextColor { get; set; } = SKColor.Parse("#ffffffff");

        /// <summary>
        /// Inner text.
        /// </summary>
        /// <value>Innder string. Maximum 10 symbols</value>
        public string InnerText
        {
            get => _innerText;
            set
            {
                if (value.Length <= _textLengthMax)
                {
                    _innerText = value;
                }
                else
                {
                    _innerText = value.Substring(0, _textLengthMax);
                }
            }
        }

        #region Constants

        public const float PI = (float)Math.PI;

        private const float _uprightAngle = PI / 2f;
        private const float _totalAngle = 2f * PI;
        private const float _textSizeCoef = 0.355f;
        private const float _textYOffesCoef = 0.7f;
        private const int _textLengthMax = 10;

        private bool _isRunAnimation;
        private int _animationFrame;
        private string _innerText = null;

        #endregion

        public SKCanvasDonutView()
        {
            _animationFrame = AnimationEndFrameCount;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);

            base.OnPaintSurface(e);
        }

        public void Draw(SKCanvas canvas, int width, int height)
        {
            canvas.Clear();

            DrawContent(canvas, width, height, _animationFrame / (float)AnimationEndFrameCount);
        }

        public async Task StartAnimarion()
        {
            if (_isRunAnimation == true)
            {
                return;
            }

            _isRunAnimation = true;
            for (_animationFrame = 1; _animationFrame <= AnimationEndFrameCount; _animationFrame++)
            {
                InvalidateSurface();
                await Task.Delay(10);
            }
            _animationFrame = AnimationEndFrameCount;
            _isRunAnimation = false;
        }

        private void DrawContent(SKCanvas canvas, int width, int height, float animationPercent)
        {
            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.Translate(width / 2, height / 2);

                var sumValue = this.Entries.Sum(x => Math.Abs(x.Value));
                // TOP Margin
                var radius = (float)(Math.Min(width, height) - (2 * Margin.Top)) / 2;

                var start = 0.0f;
                for (int i = 0; i < this.Entries.Count(); i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var end = start + (Math.Abs(entry.Value) / sumValue);

                    // Sector
                    var path = CreateSectorPath(start, start + Math.Abs(start - end) * animationPercent, radius, radius * HoleRadius);
                    using (var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color,
                        IsAntialias = true,
                    })
                    {
                        canvas.DrawPath(path, paint);
                    }

                    start = end;
                }

                if (!string.IsNullOrEmpty(InnerText))
                {
                    var paint = new SKPaint()
                    {
                        Style = SKPaintStyle.StrokeAndFill,
                        TextSize = _textSizeCoef * radius * HoleRadius,
                        Color = TextColor,
                        FakeBoldText = true
                    };

                    var textXPoint = -radius * HoleRadius * InnerText.Length / _textLengthMax + radius * 0.05f;

                    canvas.DrawText(InnerText,
                        new SKPoint(textXPoint, paint.TextSize / 2 * _textYOffesCoef),
                        paint
                   );
                }
            }
        }

        public static SKPath CreateSectorPath(float start, float end, float outerRadius, float innerRadius = 0.0f, float margin = 0.0f)
        {
            var path = new SKPath();

            // if the sector has no size, then it has no path
            if (start == end)
            {
                return path;
            }

            // the the sector is a full circle, then do that
            if (end - start == 1.0f)
            {
                path.AddCircle(0, 0, outerRadius, SKPathDirection.Clockwise);
                path.AddCircle(0, 0, innerRadius, SKPathDirection.Clockwise);
                path.FillType = SKPathFillType.EvenOdd;
                return path;
            }

            // calculate the angles
            var startAngle = (_totalAngle * start) - _uprightAngle;
            var endAngle = (_totalAngle * end) - _uprightAngle;
            var large = endAngle - startAngle > PI ? SKPathArcSize.Large : SKPathArcSize.Small;
            var sectorCenterAngle = ((endAngle - startAngle) / 2f) + startAngle;

            // get the radius bits
            var cectorCenterRadius = ((outerRadius - innerRadius) / 2f) + innerRadius;

            // calculate the angle for the margins
            var offsetR = outerRadius == 0 ? 0 : ((margin / (_totalAngle * outerRadius)) * _totalAngle);
            var offsetr = innerRadius == 0 ? 0 : ((margin / (_totalAngle * innerRadius)) * _totalAngle);

            // get the points
            var a = GetCirclePoint(outerRadius, startAngle + offsetR);
            var b = GetCirclePoint(outerRadius, endAngle - offsetR);
            var c = GetCirclePoint(innerRadius, endAngle - offsetr);
            var d = GetCirclePoint(innerRadius, startAngle + offsetr);

            // add the points to the path
            path.MoveTo(a);
            path.ArcTo(outerRadius, outerRadius, 0, large, SKPathDirection.Clockwise, b.X, b.Y);
            path.LineTo(c);

            if (innerRadius == 0.0f)
            {
                // take a short cut
                path.LineTo(d);
            }
            else
            {
                path.ArcTo(innerRadius, innerRadius, 0, large, SKPathDirection.CounterClockwise, d.X, d.Y);
            }

            path.Close();

            return path;
        }

        public static SKPoint GetCirclePoint(float r, float angle)
        {
            return new SKPoint(r * (float)Math.Cos(angle), r * (float)Math.Sin(angle));
        }
    }
}