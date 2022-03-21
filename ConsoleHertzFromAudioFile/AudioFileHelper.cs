using System;

/// <summary>
/// Summary description for AudioFileHelper
/// </summary>
public static class AudioFileHelper
{
	public static bool Open(string path)
	{
		SoundPlayer player = new SoundPlayer(@"c:\mywavfile.wav");
		player.Play();
	}
}
