using System;
using System.Windows.Forms;

namespace Tokenizer.Sample {
	static class Program {
		/// <summary>
		/// App entry point.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
