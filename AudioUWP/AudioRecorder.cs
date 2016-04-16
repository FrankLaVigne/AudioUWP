using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace AudioUWP
{
    public class AudioRecorder
    {
        private const string _audioFilename = "audio_clip.mp3";
        private string _fileName;
        private MediaCapture _mediaCapture;
        private InMemoryRandomAccessStream _memoryBuffer;

        public bool IsRecording { get; set; }

        public StorageFile AudioFile { get; private set; }


        private async Task<bool> Initialize()
        {
            if (_memoryBuffer != null)
            {
                _memoryBuffer.Dispose();
            }

            _memoryBuffer = new InMemoryRandomAccessStream();

            if (_mediaCapture != null)
            {
                _mediaCapture.Dispose();
            }

            try
            {
                MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Audio
                };

                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync(settings);

                //_mediaCapture.RecordLimitationExceeded += (MediaCapture sender) =>
                //{
                //    StopRecording();
                //    throw new Exception("Exceeded Record Limitation");
                //};
                //_mediaCapture.Failed += (MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs) =>
                //{
                //    IsRecording = false;
                //    throw new Exception(string.Format("Code: {0}. {1}", errorEventArgs.Code, errorEventArgs.Message));
                //};
            }
            catch (Exception ex)
            {
                //if (ex.InnerException != null && ex.InnerException.GetType() == typeof(UnauthorizedAccessException))
                //{
                //    throw ex.InnerException;
                //}
                throw ex;
            }
            return true;
        }


        public async void Record()
        {
            if (IsRecording)
            {
                throw new InvalidOperationException("Recording already in progress!");
            }

            await Initialize();

            await _mediaCapture.StartRecordToStreamAsync(MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Auto), _memoryBuffer);
            IsRecording = true;
        }

        public async void StopRecording()
        {
            await _mediaCapture.StopRecordAsync();
            IsRecording = false;
        }

        public async Task Play(CoreDispatcher dispatcher)
        {

            try
            {
                MediaElement playbackMediaElement = new MediaElement();

                if (this._memoryBuffer == null)
                {
                    throw new ArgumentNullException("Memory Buffer is null.");
                }

                IRandomAccessStream audioStream = _memoryBuffer.CloneStream();

                if (audioStream == null)
                {
                    throw new ArgumentNullException("Audio Stream is null.");
                }

                StorageFolder storageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;

                if (!string.IsNullOrEmpty(_fileName))
                {
                    StorageFile original = await storageFolder.GetFileAsync(_fileName);
                    await original.DeleteAsync();
                }


                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    StorageFile storageFile = await storageFolder.CreateFileAsync(_audioFilename, CreationCollisionOption.GenerateUniqueName);

                    this.AudioFile = storageFile;

                    _fileName = storageFile.Name;

                    using (IRandomAccessStream fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await RandomAccessStream.CopyAndCloseAsync(audioStream.GetInputStreamAt(0), fileStream.GetOutputStreamAt(0));
                        await audioStream.FlushAsync();
                        audioStream.Dispose();
                    }

                    // Open file
                    IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read);

                    playbackMediaElement.SetSource(stream, storageFile.FileType);
                    playbackMediaElement.Play();
                });

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
