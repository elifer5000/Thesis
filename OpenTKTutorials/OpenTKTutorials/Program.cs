using System;
using System.Windows.Forms;

namespace OpenTKTutorials
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			using (TutorialTerrain tut = new TutorialTerrain())
			{
				tut.Run();
			}
		}
		
	}
}
