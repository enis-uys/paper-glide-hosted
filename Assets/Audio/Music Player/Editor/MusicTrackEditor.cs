using UnityEditor;
using Echidna.Audio;
using UnityEngine;

[CustomEditor(typeof(MusicTrack))]
public class MusicTrackEditor : Editor
{
    private MusicTrack _track;

    private SerializedProperty _introMode;
    private SerializedProperty _loopMode;

    private SerializedProperty _introAudio;
    private SerializedProperty _loopingAudio;

    private SerializedProperty _introBoundaryStart;
    private SerializedProperty _introBoundaryEnd;

    private SerializedProperty _volume;

    private readonly GUIContent[] LoopModeContent = new GUIContent[]
    {
        new GUIContent("None"),
        new GUIContent("Full loop"),
        new GUIContent("Loop with intro")
    };
    private readonly GUIContent[] IntroModeContent = new GUIContent[]
    {
        new GUIContent("One File"),
        new GUIContent("Two Files")
    };

    private const string LoopingAudioName = "Looping Audio";
    private const string NormalAudioName = "Audio";

    private void OnEnable()
    {
        _introMode = serializedObject.FindProperty("IntroMode");
        _loopMode = serializedObject.FindProperty("LoopMode");

        _introAudio = serializedObject.FindProperty("IntroClip");
        _loopingAudio = serializedObject.FindProperty("LoopingClip");

        _introBoundaryStart = serializedObject.FindProperty("IntroBoundaryStart");
        _introBoundaryEnd = serializedObject.FindProperty("IntroBoundaryEnd");

        _volume = serializedObject.FindProperty("Volume");
    }

    public override void OnInspectorGUI()
    {
        _track = (MusicTrack)target;

        EditorGUILayout.LabelField(
            new GUIContent("Looping Mode", "Choose how you want the audio to loop.")
        );
        _loopMode.enumValueIndex = GUILayout.SelectionGrid(
            _loopMode.enumValueIndex,
            LoopModeContent,
            3
        );

        if (_loopMode.enumValueIndex == 2)
        {
            EditorGUILayout.LabelField(
                new GUIContent(
                    "Intro Mode",
                    "Choose how you want to define the intro and loop segments of the track."
                )
            );
            _introMode.enumValueIndex = GUILayout.SelectionGrid(
                _introMode.enumValueIndex,
                IntroModeContent,
                2
            );
        }

        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

        string loopAudioDisplayName = NormalAudioName;
        if (_loopMode.enumValueIndex == 2 && _introMode.enumValueIndex == 1)
        {
            EditorGUILayout.PropertyField(_introAudio);
            loopAudioDisplayName = LoopingAudioName;
        }
        EditorGUILayout.PropertyField(_loopingAudio, new GUIContent(loopAudioDisplayName));
        EditorGUILayout.PropertyField(_volume);

        EditorGUILayout.Space();

        if (_loopMode.enumValueIndex == 2 && _introMode.enumValueIndex == 0)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Looping boundary start");
            EditorGUILayout.LabelField("Looping boundary end");

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            _introBoundaryStart.doubleValue = EditorGUILayout.DoubleField(
                _introBoundaryStart.doubleValue
            );
            _introBoundaryEnd.doubleValue = EditorGUILayout.DoubleField(
                _introBoundaryEnd.doubleValue
            );

            if (_introBoundaryStart.doubleValue > _track.LoopingClip.length)
            {
                _introBoundaryStart.doubleValue = _track.LoopingClip.length;
            }

            if (_introBoundaryStart.doubleValue < 0.0)
            {
                _introBoundaryStart.doubleValue = 0.0;
            }
            if (_introBoundaryEnd.doubleValue <= _introBoundaryStart.doubleValue)
            {
                _introBoundaryEnd.doubleValue = _introBoundaryStart.doubleValue + 0.01;
            }

            if (_introBoundaryEnd.doubleValue > _track.LoopingClip.length)
            {
                _introBoundaryEnd.doubleValue = _track.LoopingClip.length;
            }

            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
