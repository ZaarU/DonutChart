using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DonutChart
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PicassoImageView : Image, IPicassoImage
	{
        public string Url { get; set; }

        public PicassoImageView (object arg)
		{
            Url = arg as string;
			InitializeComponent ();
		}
    }
}