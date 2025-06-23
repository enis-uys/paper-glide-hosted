using UnityEngine;
using UnityEngine.Audio;

namespace Echidna.Audio
{
    public class StageMusic : MonoBehaviour
    {
        public MusicTrack TestTrack;
        public MusicTrack StageTrack;
        public AudioMixerGroup MixerGroup;

        private void Start()
        {
            MusicPlayer.Instance.SetMixerGroup(MixerGroup);
            MusicPlayer.Instance.PlayTrack(StageTrack);
        }

        private void Update()
        {
            //TODO: remove this test code
            if (Input.GetKeyDown(KeyCode.F8))
            {
                MusicPlayer.Instance.PlayTrack(TestTrack, 2.0f);
            }
        }
    }
}
