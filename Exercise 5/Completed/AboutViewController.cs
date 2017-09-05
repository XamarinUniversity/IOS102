using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Fireworks
{
	partial class AboutViewController : UIViewController
	{
		public AboutViewController (IntPtr handle) : base (handle)
		{
		}

		partial void buttonClose_TouchUpInside (UIButton sender)
		{
			this.DismissViewController(true, null);
		}
	}
}
