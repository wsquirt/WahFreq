using System;
using System.IO;
using System.Media;

/// <summary>
/// Summary description for AudioFileHelper
/// </summary>
public static class AudioFileHelper
{
	// looking for free lib to open any audio file type
	// i don't want to have a only-supported-windows library
	public static bool Open(string path)
	{
		using (FileStream fs = File.OpenRead(path))
		{
			;
		}
		return true;
	}
}
