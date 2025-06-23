using UnityEngine;

namespace Echidna.Audio
{
    public enum MusicTrackLoopMode
    {
        None,
        Full,
        Intro
    }

    public enum MusicTrackIntroMode
    {
        OneFile,
        TwoFiles
    }

    [CreateAssetMenu(menuName = "Music Player/Track")]
    public class MusicTrack : ScriptableObject
    {
        [Header("Playback Settings")]
        public MusicTrackIntroMode IntroMode = MusicTrackIntroMode.TwoFiles;
        public MusicTrackLoopMode LoopMode = MusicTrackLoopMode.Intro;

        [Header("Audio Clips")]
        [Tooltip("The audio clip to be used for the intro.")]
        public AudioClip IntroClip;

        [Tooltip("The audio clip to be used for looping.")]
        public AudioClip LoopingClip;

        [Header("Loop Boundaries")]
        [Tooltip("Start time for the looping section (seconds).")]
        [Range(0.0f, float.MaxValue)]
        public double IntroBoundaryStart;

        [Tooltip("End time for the looping section (seconds).")]
        [Range(0.0f, float.MaxValue)]
        public double IntroBoundaryEnd = 1.0;

        [Header("Volume")]
        [Tooltip("Volume level of the music track.")]
        [Range(0.0f, 1.0f)]
        public float Volume = 1.0f;

        public double LoopLengthWithBoundary
        {
            get { return IntroBoundaryEnd - IntroBoundaryStart; }
        }
    }
}
