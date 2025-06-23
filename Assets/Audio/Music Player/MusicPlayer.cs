using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Echidna.Audio
{
    public enum AfterPlayBackAction
    {
        Nothing, ResumePrevious
    }

    /// <summary>
    /// Class that handles playing music with or without intro globally
    /// </summary>
    public class MusicPlayer : Singleton<MusicPlayer>
    {
        private MusicContainer _primaryContainer, _secondaryContainer;

        protected override void Awake()
        {
            base.Awake();
            if (Instance)
            {
                Instance.gameObject.name = "[Music Player]";

                GameObject primaryContainer = new GameObject("Primary Container");
                primaryContainer.transform.SetParent(gameObject.transform);
                _primaryContainer = primaryContainer.AddComponent<MusicContainer>();
                _primaryContainer.Initialize();

                GameObject secondaryContainer = new GameObject("Secondary Container");
                secondaryContainer.transform.SetParent(gameObject.transform);
                _secondaryContainer = secondaryContainer.AddComponent<MusicContainer>();
                _secondaryContainer.Initialize(true);
            }
        }

        public void PlayTrack(MusicTrack track, float fadeDuration = 0.1f)
        {
            if (!_primaryContainer.IsActive)
            {
                _secondaryContainer.StopTrack(fadeDuration);
                StopAllCoroutines();
                StartCoroutine(QueueTrackOnPrimary(track, fadeDuration));
                return;
            }
            if (!_secondaryContainer.IsActive)
            {
                _primaryContainer.StopTrack(fadeDuration);
                StopAllCoroutines();
                StartCoroutine(QueueTrackOnSecondary(track, fadeDuration));
                return;
            }
        }

        private IEnumerator QueueTrackOnPrimary(MusicTrack track, float fadeDuration)
        {
            yield return new WaitForSeconds(fadeDuration);
            _primaryContainer.PlayTrack(track, fadeDuration);
        }
        private IEnumerator QueueTrackOnSecondary(MusicTrack track, float fadeDuration)
        {
            yield return new WaitForSeconds(fadeDuration);
            _secondaryContainer.PlayTrack(track, fadeDuration);
        }

        public void SetMixerGroup(AudioMixerGroup mixerGroup)
        {
            _primaryContainer.SetMixerGroup(mixerGroup);
            _secondaryContainer.SetMixerGroup(mixerGroup);
        }
    }
}