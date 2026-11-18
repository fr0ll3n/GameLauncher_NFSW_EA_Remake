using System.IO;
using System.Reflection;

namespace GameLauncher.App.Classes;

internal class ExtractResource
{
	public static byte[] AsByte (string filename)
	{
		Stream manifestResourceStream = Assembly.GetExecutingAssembly ().GetManifestResourceStream (filename);
		if (manifestResourceStream == null) {
			return null;
		}
		byte[] array = new byte[manifestResourceStream.Length];
		manifestResourceStream.Read (array, 0, array.Length);
		return array;
	}

	public static string AsString (string filename)
	{
		return new StreamReader (Assembly.GetExecutingAssembly ().GetManifestResourceStream (filename)).ReadToEnd ();
	}
}
