using System;

using UIKit;

namespace Fireworks
{
	public partial class ViewController : UIViewController
	{
		SimpleParticleGen fireworks;

		public ViewController (IntPtr handle) : base (handle)
		{
			
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			fireworks = new SimpleParticleGen (UIImage.FromFile ("xamlogo.png"), View);

			buttonStart.TouchUpInside += delegate(object sender, EventArgs e) {
				fireworks.Start();
			};
		}

		partial void SliderSize_ValueChanged (UISlider sender)
		{
			fireworks.ScaleMax = sliderSize.Value;
		}

		partial void switchNight_ValueChanged (UISwitch sender)
		{
			if(switchNight.On)
				this.View.BackgroundColor = UIColor.FromRGB(25,25,112);
			else
				this.View.BackgroundColor = UIColor.White;
		} 
	}
}

