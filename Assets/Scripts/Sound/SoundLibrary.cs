using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour {
    [SerializeField] private Dictionary<string, AudioClip> sounds;

    private string _libraryName;
    public string GetName() => _libraryName;

    private void Awake() {
        _libraryName = gameObject.name;
    }

    public AudioClip RequestAudioClip(string audioName) {
        if (!sounds.ContainsKey(audioName)) {
            Debug.LogWarning($"Requested AudioClip {audioName} from SoundLibrary {_libraryName} but it was not found");
            return null;
        }

        return sounds[audioName];
    }
}
