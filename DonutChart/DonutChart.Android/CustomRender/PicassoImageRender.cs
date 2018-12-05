
using Android.Content;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Com.Squareup.Picasso;

using DonutChart;
using DonutChart.Droid.CustomRender;
using Android.App;
using Android.Support.V7.View;

[assembly: ExportRenderer(typeof(PicassoImageView), typeof(PicassoImageRender))]
namespace DonutChart.Droid.CustomRender
{
    public class PicassoImageRender : ImageRenderer
    {
        public PicassoImageRender(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (Control != null && e.NewElement is IPicassoImage imageForms)
            {
                (Control.Context as Activity).RunOnUiThread(() =>
                {
                    Picasso.With(Control.Context)
                        .Load(imageForms.Url)
                        .Placeholder(Resource.Drawable.abc_ic_ab_back_material)
                        .Into(Control);
                });

            }
        }
    }
}