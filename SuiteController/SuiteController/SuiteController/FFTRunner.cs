using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.MediaFoundation;
using CSCore.SoundIn;
using CSCore.Streams;
using CSCore.Codecs;
using CSCore.SoundOut;
using CSCore.Streams.Effects;
using CSCore.DSP;


namespace SuiteController
{
    

    class FFTRunner
    {
        // Sample Size
        const FftSize fftSize = FftSize.Fft16384;
        private ISoundOut _soundOut;
        private SoundInSource wasapiCaptureSource;
        WasapiCapture wasapiCapture;
        BasicSpectrumProvider provider;

        /** Initializes the realtime audio processing handlers */
        public void BeginRecording()
        {
            // recoreds output data from wasapi loopback sound card.
            using (wasapiCapture = new WasapiLoopbackCapture())
            {
                wasapiCapture.Initialize();
                wasapiCaptureSource = new SoundInSource(wasapiCapture);

                // TODO: Stereo or Mono?
                using (var stereoSource = wasapiCaptureSource.ToStereo())
                {
                    // creates the spectrum Provider (Our own FFTProvider)
                    provider = new BasicSpectrumProvider(stereoSource.WaveFormat.Channels, stereoSource.WaveFormat.SampleRate, fftSize);
                    // creates the handler that uses the SpectrumProvider.
                    var handler = new FFTHandler(FftSize.Fft4096)
                    {
                        SpectrumProvider = provider,
                        UseAverage = true,
                        height = 100,
                        BarCount = 50,
                        BarSpacing = 2,
                        IsXLogScale = true,
                        ScalingStrategy = ScalingStrategy.Sqrt
                    };
                    
                    // notifies the spectrum provider each block read
                    var notificationSource = new SingleBlockNotificationStream(wasapiCaptureSource.ToSampleSource());
                    notificationSource.SingleBlockRead += (s, a) => provider.Add(a.Left, a.Right);
                    var wsrc = notificationSource.ToWaveSource();
                    
                    // reads through the wave source as it is playing
                    // This is the key to getting the realtime music.
                    byte[] buffer = new byte[wsrc.WaveFormat.BytesPerSecond];
                    wasapiCaptureSource.DataAvailable += (s, e) =>
                    {
                        int read = wsrc.Read(buffer, 0, buffer.Length);

                    };

                    // starts the listening.
                    wasapiCapture.Start();

                    // gathers the data and sends it to the handler in a loop.
                    var fftBuffer = new float[(int)fftSize];
                    while (true)
                    {
                        if (provider.GetFftData(fftBuffer))
                        {

                            Console.Clear();
                            handler.CreateSpectrumLineInternal(fftBuffer, 100);
                        }
                    }

                    // Stops Listening.
                    wasapiCapture.Stop();
                }
            }


            //    bool Data_Available = false;
            //    Double[] Audio_Samples = new Double[0];


            //    var waveIn = new WasapiLoopbackCapture();
            //    waveIn.DataAvailable += ( sender, e) =>
            //{
            //    Int32 sample_count = e.ByteCount / (waveIn.WaveFormat.BitsPerSample / 8);
            //    Single[] data = new Single[sample_count];

            //    for (int i = 0; i < sample_count; ++i)
            //    {
            //        data[i] = BitConverter.ToSingle(e.Data, i * 4);
            //    }

            //    int j = 0;
            //    Audio_Samples = new Double[sample_count / 2];
            //    for (int sample = 0; sample < data.Length; sample += 2)
            //    {
            //        Audio_Samples[j] = (Double)data[sample];
            //        Audio_Samples[j] += (Double)data[sample + 1];
            //        ++j;

            //        Console.WriteLine(Audio_Samples[j].ToString());
            //    }

            //    Data_Available = true;
            //};
            //    waveIn.Initialize();
            //    //waveIn.Stopped += OnRecordingStopped;
            //    waveIn.Start();

            //    while (true)
            //    {
            //        if (Data_Available)
            //        {
            //            Data_Available = false;
            //            //Console.WriteLine(Audio_Samples.ToString());
            //        }
            //    }



            //using (WasapiCapture capture = new WasapiLoopbackCapture())
            //{
            //    //if nessesary, you can choose a device here
            //    //to do so, simply set the device property of the capture to any MMDevice
            //    //to choose a device, take a look at the sample here: http://cscore.codeplex.com/

            //    //initialize the selected device for recording
            //    capture.Initialize();


            //    var eq = new Equalizer(new SoundInSource(capture));

            //    var fft = new FftProvider(3, FftSize.Fft1024);


            //    var tenb = Equalizer.Create10BandEqualizer(new SoundInSource(capture));


            //create a wavewriter to write the data to
            //using (WaveWriter w = new WaveWriter("dump.wav", capture.WaveFormat))
            //{
               

            //    //setup an eventhandler to receive the recorded data
            //    capture.DataAvailable += (s, e) =>
            //    {
            //            //save the recorded audio
            //            w.Write(e.Data, e.Offset, e.ByteCount);

            //    };

            //    Console.WriteLine("starting...");

            //    //start recording
            //    capture.Start();

            //    Console.ReadKey();

            //    capture.Stop();

            //}
        
        }
    }
}
