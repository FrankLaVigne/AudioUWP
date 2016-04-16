using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;

namespace AudioUWP
{
    public class AudioEffects
    {
        private AudioGraph _graph;
        private AudioFileInputNode _fileInputNode;
        private AudioDeviceOutputNode _deviceOutputNode;

        private EchoEffectDefinition _echoEffectDefinition;
        private ReverbEffectDefinition _reverbEffectDefinition;
        private EqualizerEffectDefinition _eqEffectDefinition;
        private LimiterEffectDefinition _limiterEffectDefinition;


        public StorageFile AudioFile { get; private set; }


        public AudioEffects (StorageFile _storageFile)
        {

            this.AudioFile = _storageFile;


        }

        private async void Init ()
        {
            CreateAudioFileInputNodeResult fileInputResult = await _graph.CreateFileInputNodeAsync(this.AudioFile);



        }


        private void CreateEchoEffect()
        {
            _echoEffectDefinition = new EchoEffectDefinition(_graph);

            _echoEffectDefinition.WetDryMix = 0.7f;
            _echoEffectDefinition.Feedback = 0.5f;
            _echoEffectDefinition.Delay = 200.0f;

            _fileInputNode.EffectDefinitions.Add(_echoEffectDefinition);
            _fileInputNode.DisableEffectsByDefinition(_echoEffectDefinition);
        }

    }
}
