using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace AudioUWP
{
    public class AudioEffects
    {
        private const string DEFAULT_AUDIO_FILENAME = "audio_clip.mp3";


        public AudioGraph Graph { get; set; }

        private AudioFileInputNode _fileInputNode;
        private AudioDeviceOutputNode _deviceOutputNode;

        public bool IsPlaying { get; set; }

        public StorageFile AudioFile { get; private set; }

        private EchoEffectDefinition _echoEffectDefinition;



        public async Task InitializeAudioGraph()
        {



            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media);
            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            this.Graph = result.Graph;
            CreateAudioDeviceOutputNodeResult outputDeviceNodeResult = await this.Graph.CreateDeviceOutputNodeAsync();

            _deviceOutputNode = outputDeviceNodeResult.DeviceOutputNode;

        }

        public void Play()
        {

            this.Graph.Start();
        }



        public async Task LoadFileIntoGraph(StorageFile audioFile)
        {
            this.AudioFile = audioFile;

            CreateAudioFileInputNodeResult audioFileInputResult = await this.Graph.CreateFileInputNodeAsync(this.AudioFile);

            if (audioFileInputResult.Status != AudioFileNodeCreationStatus.Success)
            {
                throw new Exception("File failed to load into graph.");
            }

            _fileInputNode = audioFileInputResult.FileInputNode;
            _fileInputNode.AddOutgoingConnection(_deviceOutputNode);

            CreateEchoEffect();

        }


        private void CreateEchoEffect()
        {
            _echoEffectDefinition = new EchoEffectDefinition(this.Graph);

            _echoEffectDefinition.WetDryMix = 0.7f;
            _echoEffectDefinition.Feedback = 0.5f;
            _echoEffectDefinition.Delay = 100.0f;

            _fileInputNode.EffectDefinitions.Add(_echoEffectDefinition);
        }

    }
}