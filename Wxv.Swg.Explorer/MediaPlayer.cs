using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NAudio;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace Wxv.Swg.Explorer
{
    public class MediaPlayer
    {
        public enum Format
        {
            MP3,
            Wave
        }

        private MediaPlayer() { }

        private IWavePlayer waveOut = new WaveOut();
        private MemoryStream memoryStream = new MemoryStream();
        private WaveStream waveStream;

        public void Play(byte[] data, Format format = Format.MP3)
        {
            waveOut.Stop();

            if (waveStream != null)
                waveStream.Dispose();
            memoryStream.Dispose();

            memoryStream = new MemoryStream(data);
            if (format == Format.MP3)
                waveStream = new Mp3FileReader(memoryStream);
            else
                waveStream = new WaveFileReader(memoryStream);
            waveOut.Init(waveStream);
            waveOut.Play();
        }

        public void Pause()
        {
            waveOut.Pause();
        }

        public void Stop()
        {
            waveOut.Stop();
        }

        private static Lazy<MediaPlayer> lazyInstance = new Lazy<MediaPlayer>(() => new MediaPlayer());
        public static MediaPlayer Instance { get { return lazyInstance.Value; } }
    }
}
