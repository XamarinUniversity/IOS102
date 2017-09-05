using System;
using System.Drawing;
using System.Diagnostics;
using UIKit;
using CoreGraphics;

namespace Fireworks
{
	public class SimpleParticleGen
	{
		UIImage imgParticle;
		UIView parent;

		private CGPoint Location { get { return this.parent.Center; } }

		public nfloat DurrationMin { get; set; }
		public nfloat DurrationMax { get; set; }
		public nfloat DelayMin { get; set; }
		public nfloat DelayMax { get; set; }
		public nfloat ImmortalDelay { get; set; }

		public nfloat ScaleMin { get; set; }
		public nfloat ScaleMax { get; set; }
		public nfloat ScaleStart { get; set; }

		public nfloat AlphaStart { get; set; }
		public nfloat AlphaFinal { get; set; }

		public int NumberOfParticles { get; set; }

		public int DistanceMin { get; set; }
		public int DistanceMax { get; set; }

		public bool DestroyParticles { get; set; }
		public bool ContinousParticles { get; set; }

		private bool isRunning;

		Random random = new Random( System.DateTime.Now.Second );

		public SimpleParticleGen (UIImage imageParticle, UIView parent)
		{
			this.imgParticle = imageParticle;
			this.parent = parent;

			SetDefaults ();
		}

		public void SetDefaults ()
		{
			NumberOfParticles = 100;
			DurrationMin = 1;
			DurrationMax = 2;
			DelayMin = 0;
			DelayMax = 0.25f;
			ImmortalDelay = 0.5f; //time between immortal particles

			ScaleMin = 0.25f;
			ScaleMax = 1.0f;
			ScaleStart = 0.01f;

			DistanceMin = 50;
			DistanceMax = 140;

			AlphaStart = 1;
			AlphaFinal = 0;

			DestroyParticles = true;
			ContinousParticles = false;
		}

		CGPoint GetRandomPosition(nfloat scale)
		{
			//get random angle (radians) and distance
			double radius = random.Next(DistanceMin, DistanceMax);
			double angle = random.NextDouble () * 2 * Math.PI;  

			//convert to cartisian
			var xPos = (nfloat)(radius * Math.Cos(angle));
			var yPos = (nfloat)(radius * Math.Sin(angle));

			//offset for upper left origin
			xPos -= (ScaleStart)*imgParticle.Size.Width / 2;
			yPos -= (ScaleStart)*imgParticle.Size.Height / 2;

			return new CGPoint (xPos + Location.X, yPos + Location.Y); 
		}

		nfloat GetRandomScale()
		{
			if(ScaleMin == ScaleMax)
				return ScaleMax;

			return (nfloat)random.NextDouble() * (ScaleMax - ScaleMin) + ScaleMin;
		}

		void NewParticle (CGPoint startPosition, Action complete )
		{
			nfloat scale = 1;

			var particle = new UIImageView (imgParticle) 
			{
				Alpha = AlphaStart,
			};

			if(ScaleStart != 1.0f)
			{
				particle.Transform = CGAffineTransform.MakeScale(ScaleStart, ScaleStart);
			}

			particle.SetLocation ( startPosition );

			parent.Add (particle);

			UIView.Animate (random.NextDouble () * (DurrationMax - DurrationMin) + DurrationMin, random.NextDouble () * (DelayMax - DelayMin) + DelayMin, 
				UIViewAnimationOptions.CurveEaseOut, 
				() => 
				{
					scale = GetRandomScale ();

					particle.SetLocation (GetRandomPosition (scale));
					if (ScaleStart != scale)
					{
						particle.Transform = CGAffineTransform.MakeScale (scale, scale);
					}
					if (AlphaFinal != AlphaStart) 
					{
						particle.Alpha = AlphaFinal;
					}
				}, 
				() => {
					if (DestroyParticles == true) 
					{
						particle.RemoveFromSuperview ();
						particle = null;
					}
					if (complete != null)
					{
						complete ();
					}
				});
		}

		void StartContinuousParticles ()
		{
			isRunning = true;

			nfloat xOffSet = imgParticle.Size.Width*ScaleStart/2;
			nfloat yOffSet = imgParticle.Size.Height*ScaleStart/2;

			var startLoc = new CGPoint(Location.X - xOffSet, Location.Y - yOffSet);

			for(int i = 0; i < NumberOfParticles; i++)
				GetImmortalParticle(startLoc);
		}

		void GetImmortalParticle (CGPoint startLoc)
		{
			NewParticle (startLoc, () => 
			{
				if (isRunning == true)
					GetImmortalParticle (startLoc);
			});
		}

		public void Start ()
		{
			if(ContinousParticles == true)
			{
				if (isRunning == true)
					Stop ();

				StartContinuousParticles();
				return;
			}

			nfloat xOffSet = imgParticle.Size.Width*ScaleStart/2;
			nfloat yOffSet = imgParticle.Size.Height*ScaleStart/2;

			var startLoc = new CGPoint(Location.X - xOffSet, Location.Y - yOffSet);

			for (int i = 0; i < NumberOfParticles; i++)
				NewParticle(startLoc, null);
		}

		public void Stop ()
		{
			isRunning = false;
		}
	}

	public static class Extensions
	{
		public static void SetLocation(this UIView view, nfloat left, nfloat top)
		{
			if (view != null)
				view.Frame = new CGRect (new CGPoint (left, top), view.Frame.Size);
		}

		public static void SetLocation(this UIView view, CGPoint point)
		{
			if(view != null)
				view.Frame = new CGRect(point, view.Frame.Size);
		}
	}
}