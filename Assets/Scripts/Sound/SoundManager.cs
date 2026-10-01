using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance { get; private set; }

    [SerializeField] private GameObject audioSourcePrefab;
    [SerializeField] private GameObject soundLibrariesObj;

    private Dictionary<string, SoundLibrary> soundLibraries;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(this);
        }

        soundLibraries = new Dictionary<string, SoundLibrary>();
        foreach (Transform child in soundLibrariesObj.transform) {
            SoundLibrary soundLibrary = child.GetComponent<SoundLibrary>();
            if (soundLibrary) {
                var soundLibraryName = child.name;

                Debug.Log(soundLibraries.ContainsKey(soundLibraryName)
                    ? $"Overriden SoundLibrary {soundLibraryName}"
                    : $"Loaded SoundLibrary {soundLibraryName}");

                soundLibraries[soundLibraryName] = soundLibrary;
            }
        }
    }

    public GameObject GetAudioSourceObj() {
        return Instantiate(audioSourcePrefab, transform);
    }

    public struct SoundClipArgs {
        public readonly string soundLibrary;
        public readonly string soundName;
        public float? duration;
        public float volume;
        public float pitch;
        public Vector3? position;

        public SoundClipArgs(string soundLibrary, string soundName) {
            this.soundLibrary = soundLibrary;
            this.soundName = soundName;
            duration = null;
            volume = 1f;
            pitch = 1f;
            position = null;
        }
    }
    
    public void PlaySoundClip(SoundClipArgs args) {
        var audioSource = GetSoundClip(args);
        if (audioSource == null) return;
        
        audioSource.Play();
        Destroy(audioSource.gameObject, args.duration ?? audioSource.clip.length);
    }
    
    [CanBeNull]
    public AudioSource PlayLoopingClip(SoundClipArgs args) {
        var audioSource = GetSoundClip(args);
        if (audioSource == null) return null;

        audioSource.loop = true;
        audioSource.Play();
        if (args.duration.HasValue) {
            Destroy(audioSource.gameObject, args.duration.Value);
        }
        return audioSource;
    }

    [CanBeNull]
    public AudioSource GetSoundClip(SoundClipArgs args) {
        if (!soundLibraries.TryGetValue(args.soundLibrary, out var library)) {
            Debug.LogWarning($"SoundLibrary {args.soundLibrary} not loaded");
            return null;
        }

        var audioClip = library.RequestAudioClip(args.soundName);
        if (audioClip == null) {
            Debug.LogWarning($"SoundClip {args.soundName} not loaded in {args.soundLibrary}");
            return null;
        }

        var audioSourceObj = GetAudioSourceObj();

        var audioSource = audioSourceObj.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        
        audioSource.volume = args.volume;
        audioSource.pitch = args.pitch;
        
        var audioPos = args.position ?? audioSource.transform.position;
        audioSource.transform.position = audioPos;

        return audioSource;
    }

    public AudioSource GetAudioSource() {
        var audioSourceObj = GetAudioSourceObj();

        var audioSource = audioSourceObj.GetComponent<AudioSource>();

        return audioSource;
    }

    public void PlayMouseClick() {
        //Hardcoded for now
        //Assumes SoundLibrary named MouseClicks is loaded and that there are AudioClips named: [1, 2, 3, 4, 5, 6]

        var audioClip = "1";

        var mouseSoundClipArgs = new SoundClipArgs("MouseClicks", audioClip);
        PlaySoundClip(mouseSoundClipArgs);
    }
}
