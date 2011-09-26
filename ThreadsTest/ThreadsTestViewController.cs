using MonoTouch.UIKit;
using System.Drawing;
using System;
using System.Threading;
using MonoTouch.Foundation;

namespace ThreadsTest
{
	public partial class ThreadsTestViewController : UIViewController
	{
		public ThreadsTestViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}
		
		const int LOOPS = 100 * 1000 * 1000;

		void FirstThread (ref int a, ref int b) {
			for (int i = 0; i < LOOPS; ++i) {
				a = i;
				b = i;
			}
		}

		void SecondThread (ref int a, ref int b) {
			int err = 0;
			for (int i = 0; i < LOOPS; ++i) {
				int vb = b;
				int va = a;
				if (vb > va)
					++err;
			}
			string res = string.Format ("Errors {0}", err);
			BeginInvokeOnMainThread (() => resultLabel.Text = res);
		}

		void FirstThreadMM (ref int a, ref int b) {
			for (int i = 0; i < LOOPS; ++i) {
				a = i;
				Thread.MemoryBarrier ();
				b = i;
			}
		}

		void SecondThreadMM (ref int a, ref int b) {
			int err = 0;
			for (int i = 0; i < LOOPS; ++i) {
				int vb = b;
				Thread.MemoryBarrier ();
				int va = a;
				if (vb > va)
					++err;
			}
			string res = string.Format ("Errors {0}", err);
			BeginInvokeOnMainThread (() => resultLabel.Text = res);
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			int[] data = new int [31];
			twoThreads.TouchUpInside += delegate {
				resultLabel.Text = "Running with no barriers";
				new Thread (() => FirstThread (ref data[0], ref data[30])).Start ();
				new Thread (() => SecondThread (ref data[0], ref data[30])).Start ();
			};

			twoThreadsWithBarrier.TouchUpInside += delegate {
				resultLabel.Text = "Running with barriers";
				new Thread (() => FirstThreadMM (ref data[0], ref data[30])).Start ();
				new Thread (() => SecondThreadMM (ref data[0], ref data[30])).Start ();
			};
			
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}
