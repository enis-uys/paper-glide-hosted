using UnityEngine;
using UnityEngine.Audio;

namespace Echidna.Audio
{
    /// <summary>
    /// A container for 2 <see cref="AudioSource"/> responsible for properly performing an intro loop
    /// </summary>
    public class MusicContainer : MonoBehaviour
    {
        private MusicTrack CurrentTrack;

        public AudioSource IntroSource;
        public AudioSource LoopSource;

        public bool IsInitialized { get; private set; } = false;
        public bool IsActive { get; private set; }
        public bool IsInLoopingState
        {
            get { return LoopSource.isPlaying && !IntroSource.isPlaying; }
        }

        private float _targetVolume = 1.0f;
        private float _volumeBeforeLerp = 0.0f;
        private float _volumeLerpSpeed = 1.0f;
        private float _volumeLerpAmount = 1.0f;
        private bool _fadingOut = false;

        private int _nextSource = 0;
        private double _nextTransitionTime = 0.0f;

        public void Initialize(bool isActive = false)
        {
            if (IsInitialized)
            {
                return;
            }

            if (!IntroSource)
            {
                GameObject introSource = new GameObject("Intro Source");
                IntroSource = introSource.AddComponent<AudioSource>();
                introSource.transform.SetParent(transform);
            }

            if (!LoopSource)
            {
                GameObject loopSource = new GameObject("Loop Source");
                LoopSource = loopSource.AddComponent<AudioSource>();
                loopSource.transform.SetParent(transform);
            }

            IsActive = isActive;
            IsInitialized = true;
        }

        private void Update()
        {
            if (_volumeLerpAmount < 1.0f)
            {
                _volumeLerpAmount += Time.unscaledDeltaTime * _volumeLerpSpeed;
                float lerpedVolume = Mathf.Lerp(
                    _volumeBeforeLerp,
                    _fadingOut ? 0.0f : _targetVolume,
                    _volumeLerpAmount
                );
                IntroSource.volume = lerpedVolume;
                LoopSource.volume = lerpedVolume;
            }
            else if (_fadingOut && (LoopSource.isPlaying || IntroSource.isPlaying))
            {
                IntroSource.Stop();
                LoopSource.Stop();
            }

            if (CurrentTrack == null)
            {
                return;
            }
            if (
                CurrentTrack.LoopMode == MusicTrackLoopMode.Intro
                && CurrentTrack.IntroMode == MusicTrackIntroMode.OneFile
            )
            {
                if (
                    AudioSettings.dspTime + (CurrentTrack.LoopLengthWithBoundary * 0.5f)
                    > _nextTransitionTime
                )
                {
                    ScheduleNextSingleFileLoop();
                }
            }
        }

        public void SetVolume(float volume)
        {
            _targetVolume = volume;
            _volumeLerpAmount = 1.0f;
        }

        public void SetTargetVolume(float volume)
        {
            _targetVolume = volume;
        }

        public void SetMixerGroup(AudioMixerGroup mixerGroup)
        {
            IntroSource.outputAudioMixerGroup = mixerGroup;
            LoopSource.outputAudioMixerGroup = mixerGroup;
        }

        //important to note that this is the function that actually plays the music
        public void PlayTrack(MusicTrack track, float fadeDuration = 0.1f)
        {
            IsActive = true;

            _targetVolume = track.Volume;
            if (fadeDuration > 0.0f)
            {
                _fadingOut = false;
                _volumeBeforeLerp = IntroSource.volume;
                _volumeLerpAmount = 0.0f;
                _volumeLerpSpeed = 1.0f / fadeDuration;
            }
            else
            {
                IntroSource.volume = track.Volume;
                LoopSource.volume = track.Volume;
            }

            bool shouldLoop =
                track.LoopMode == MusicTrackLoopMode.Full
                || track.LoopMode == MusicTrackLoopMode.Intro;

            CurrentTrack = track;
            IntroSource.time = 0.0f;
            LoopSource.time = 0.0f;
            LoopSource.loop = shouldLoop;

            if (track.IntroMode == MusicTrackIntroMode.TwoFiles)
            {
                IntroSource.clip = track.IntroClip;
            }
            else
            {
                IntroSource.clip = track.LoopingClip;
            }
            LoopSource.clip = track.LoopingClip;

            if (track.LoopMode == MusicTrackLoopMode.Intro)
            {
                if (track.IntroMode == MusicTrackIntroMode.TwoFiles)
                {
                    IntroSource.Play();
                    LoopSource.PlayScheduled(AudioSettings.dspTime + track.IntroClip.length);
                }
                else
                {
                    SetupSingleFileLoop();
                }
                return;
            }
            if (track.LoopMode == MusicTrackLoopMode.Full)
            {
                LoopSource.Play();
                return;
            }
        }

        public void StopTrack(float fadeDuration = 0.1f)
        {
            IsActive = false;

            if (fadeDuration > 0.0f)
            {
                _fadingOut = true;
                _volumeBeforeLerp = IntroSource.volume;
                _volumeLerpAmount = 0.0f;
                _volumeLerpSpeed = 1.0f / fadeDuration;
            }
            else
            {
                IntroSource.Stop();
                LoopSource.Stop();
            }
        }

        private void SetupSingleFileLoop()
        {
            double currentDspTime = AudioSettings.dspTime;
            double loopStartTime = currentDspTime + CurrentTrack.IntroBoundaryStart;

            IntroSource.time = 0.0f;
            LoopSource.time = (float)CurrentTrack.IntroBoundaryStart;

            IntroSource.PlayScheduled(currentDspTime);
            IntroSource.SetScheduledEndTime(loopStartTime);

            LoopSource.PlayScheduled(loopStartTime);

            _nextTransitionTime = loopStartTime + CurrentTrack.LoopLengthWithBoundary;
            SwapSources();
        }

        private void ScheduleNextSingleFileLoop()
        {
            switch (_nextSource)
            {
                case 0:
                    IntroSource.SetScheduledEndTime(_nextTransitionTime);
                    LoopSource.PlayScheduled(_nextTransitionTime);
                    LoopSource.time = (float)CurrentTrack.IntroBoundaryStart;
                    break;

                case 1:
                    LoopSource.SetScheduledEndTime(_nextTransitionTime);
                    IntroSource.PlayScheduled(_nextTransitionTime);
                    IntroSource.time = (float)CurrentTrack.IntroBoundaryStart;
                    break;
            }

            _nextTransitionTime += CurrentTrack.IntroBoundaryEnd;
            SwapSources();
        }

        private void SwapSources()
        {
            _nextSource++;
            _nextSource %= 2;
        }
    }
}
