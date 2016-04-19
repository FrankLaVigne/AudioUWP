using System;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;

namespace AudioUWP
{
    public class AudioEffects
    {
        private AudioGraph _audioGraph;
        private AudioFileInputNode _fileInputNode;
        private AudioDeviceOutputNode _deviceOutputNode;

        public StorageFile AudioFile { get; private set; }

        private EchoEffectDefinition _echoEffectDefinition;

        public async Task InitializeAudioGraph()
        {
            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media);
            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            this._audioGraph = result.Graph;
            CreateAudioDeviceOutputNodeResult outputDeviceNodeResult = await this._audioGraph.CreateDeviceOutputNodeAsync();
            _deviceOutputNode = outputDeviceNodeResult.DeviceOutputNode;
        }

        public void Play()
        {
            this._audioGraph.Start();
        }

        public async Task LoadFileIntoGraph(StorageFile audioFile)
        {
            this.AudioFile = audioFile;
            CreateAudioFileInputNodeResult audioFileInputResult = await this._audioGraph.CreateFileInputNodeAsync(this.AudioFile);

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
            _echoEffectDefinition = new EchoEffectDefinition(this._audioGraph);

            _echoEffectDefinition.WetDryMix = 0.7f;
            _echoEffectDefinition.Feedback = 0.5f;
            _echoEffectDefinition.Delay = 100.0f;

            _fileInputNode.EffectDefinitions.Add(_echoEffectDefinition);
        }

    }
}