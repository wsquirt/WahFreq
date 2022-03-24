# WahFreq
Trying to find Hertz from mp3 / mic / etc...

Find out some tips :

---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------

- https://softwareengineering.stackexchange.com/questions/136215/how-to-determine-frequency-in-hertz-real-time-with-java-sound
You will need to do some spectrum analysis. Take a short piece of audio data, maybe some 256 samples, then calculate a fast fourier transform (FFT) for each piece of data. However, a fourier transform itself is not enough since the process of cutting up sound in short samples introduces some distortion. This distortion will mainly be in the higher frequencies. For pitch detection, you can get away with simply filtering away this distortion using a low pass filter. Alternatively, you can window your sound sample, usually using hanning or blackman window functions (different sine-slopes, basically). In order to improve time resolution you should also overlap the individual samples you take. At last, you should average your individual spectra over as long a time as you want to analyze.
Doing all that will give you something called a Power Spectral Density function. This method of deriving it is called the Welch method. Hence, if you are lucky, Java Sound will include a method for calculating one of these. In many signal processing environments, this would be called "pwelch" or "psd".
Of course, the spectrum will use logarithmic frequencies (way more frequency points towards the high frequencies) and the amplitudes will most likely be denormalized and linear instead of simply dB values. Also, you will still have to find a good method to find your actual pitch frequency amongst all the harmonic noise etc.
What I want to say is this: Either your library has an easy function that does exactly what you want or this stuff is probably too complex to have an easy answer. Sorry.

---------- ---------- ----------

- https://softwareengineering.stackexchange.com/questions/136215/how-to-determine-frequency-in-hertz-real-time-with-java-sound
I suggest you look into some tutorials or textbooks on digital signal processing. What you are trying to do is actually quite complicated. Some of the things that make it complicated are:
Audio samples will be coming to you from the sound card in PCM (Pulse Code Modulated) samples. That is just a fancy term for time domain samples. That is, the analog sound wave going into the microphone is sampled at a regular interval and you are given those samples as numbers. This does not directly give you any frequency information, you will have to perform an FFT to transform the time domain data into the frequency domain.
Even after the FFT, you still do not have the array of frequencies you are looking for. What you have is an array of magnitudes at each sampled frequency. So you will have to do some kind of detection to determine which frequencies are actually present in your signal. This could be a simple threshold and peak pick, or something more involved.
In addition to this difficulty, there is a tradeoff when performing spectral analysis with the FFT between time and frequency resolution. If you want to get more frequency resolution, you must sacrifice time resolution. For example, if you want to be able to detect a 1Hz change in a signal that is sampled at 44100 Hz, you will need to perform an FFT of 44100 samples. Well, 44100 samples is an entire second of data, which means that even though you can detect a signal to 1Hz resolution, you don't know where in that second it occurred. This is why many pitch detection algorithms use time-domain methods like auto-correlation to find the pitch.
Another difficulty is that an instrument does not produce a pure tone (single frequency), but produces a number of harmonics as well. So you will not only have the true pitch frequency, but there will be other frequencies present in the signal that you will have to account for.
All of this is not to discourage you from doing this project, I am just trying to lay out some of the issues you might face when doing the project. I worked on a similar project and ran into these issues.

---------- ---------- ---------- ---------- ---------- ----------

https://www.unilim.fr/pages_perso/jean.debord/math/fourier/fft.htm

---------- ---------- ---------- ---------- ---------- ----------

REQUEST : I'm trying to create a program which gets the various "notes" in a sound file (WAV or MP3) and can get the frequency and amplitude of each. I've been searching around for this, and of course there is the problem of distinguishing individual "notes" in a music file which isn't a MIDI, but it seems that something along these lines can be done with NAudio or DirectSound. Any ideas?

ANSWERS :
	- What you are asking to do is extremely difficult.
	Step one would be to convert your audio from a time domain to a frequency domain. That is, you take a number of samples, and do a Fourier transform (implemented in your software as FFT).
	Next, you begin deciding what you call a note or not. This is as not as simple as picking out the loudest of the frequencies! Different instruments have different timbre, which is created by various harmonics. If you had a song of nothing but sine waves, this would be much simpler. However, you'll find that you'll start seeing notes where your ear tells you they don't exist.
	Now, psychoacoustics come into play. It is entirely possible for humans to "hear" notes that do not even have a fundamental. This is particularly true in a musical context. If I were to take a trombone and start playing a scale downward, at some point, the fundamental disappears or is mostly gone. However, you will still perceive that scale as going downward, when in fact the fundamental sound has all-but disappeared. Things get really tricky at this point.
	To answer your question, start with an FFT. Maybe this is sufficient for your needs. If not, begin reading the significant amount of technical literature on the subject.

---------- ---------- ---------- ---------- ---------- ----------

http://blog.bjornroche.com/2012/07/frequency-detection-using-fft-aka-pitch.html

---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------

