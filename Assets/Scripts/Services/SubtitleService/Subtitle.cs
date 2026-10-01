using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Subtitle : MonoBehaviour {
    private string _text;

    private bool _playSound;
    
    private float _punctuationPause;
    private float _typingDuration;
    private float _afterTypedDuration;
    private float _charSoundDuration;
    private float _typingDurationLeft;

    private int _charCount;
    private int _visibleChars;
    
    private int _nextPunctuationIndex;

    private bool _paused;

    private string _dialogueSoundClipLibrary;
    private string _dialogueSoundClipName;
    private float _dialogueSoundClipPitch;
    private float _dialogueSoundClipPitchModifier;
    
    [SerializeField] private TextMeshProUGUI textRenderer;
    
    public IEnumerator PlaySequence(SubtitleService.SubtitleArgs args) {
        _text = args.text;
        _charCount = _text.Length;
        
        _playSound = args.playSound;
        
        _punctuationPause = args.punctuationPause;
        _typingDuration = args.charDuration * _charCount;
        _afterTypedDuration = args.afterTypedDuration;
        _typingDurationLeft = _typingDuration;
        
        _charSoundDuration = _typingDuration / _charCount;
        
        _nextPunctuationIndex = 0;
        
        textRenderer.text = _text;
        textRenderer.maxVisibleCharacters = 0;

        _paused = false;
        
        _dialogueSoundClipLibrary = args.dialogueSoundClipLibrary;
        _dialogueSoundClipName = args.dialogueSoundClipName;
        _dialogueSoundClipPitch = args.dialogueSoundClipPitch;
        _dialogueSoundClipPitchModifier = args.dialogueSoundClipPitchVariationModifier;

        yield return TextTypingSequence();
    }

    private readonly HashSet<char> punctuation = new HashSet<char>{
        ' ',
        '.',
        '?',
        '!',
        ',',
        ';',
        ':'
    };
    
    bool isPunctuation(char c) => punctuation.Contains(c);

    int getNextPunctuationIndex(int index) {
        for (; index < _charCount; index++) {
            if (isPunctuation(_text[index])) {
                return index;
            }
        }

        return index;
    }

    IEnumerator TextTypingSequence() {
        _visibleChars = 0;
        float timeElapsed = 0;

        while (_visibleChars < _charCount) {
            if (!_paused) {
                timeElapsed += Time.deltaTime;
            } else {
                _paused = false;
            }
            
            float progress = Math.Min(timeElapsed / _typingDuration, 1);
            
            _typingDurationLeft = Math.Max(_typingDuration - timeElapsed, 0);

            var targetCharacters = Mathf.Min(Mathf.FloorToInt(progress * _charCount), _charCount);

            if (targetCharacters != _visibleChars) {
                var nextPunctuationIndex = getNextPunctuationIndex(targetCharacters);

                _visibleChars = targetCharacters;
                textRenderer.maxVisibleCharacters = _visibleChars;
                
                if (targetCharacters == nextPunctuationIndex) {
                    _paused = true;
                    yield return new WaitForSeconds(_punctuationPause);
                } else if (nextPunctuationIndex != _nextPunctuationIndex) {
                    var charDiff = nextPunctuationIndex - _nextPunctuationIndex;
                    var soundClipDuration = (charDiff) * _charSoundDuration;
                    playDialogueSoundClip(soundClipDuration);
                    _nextPunctuationIndex = nextPunctuationIndex;
                }
            }

            yield return null;
        }
        
        yield return new WaitForSeconds(_afterTypedDuration);
    }

    void playDialogueSoundClip(float duration) {
        if (!_playSound || !SoundManager.Instance) return;
        var soundDuration = math.min(_typingDurationLeft, duration);
        SoundManager.Instance.PlaySoundClip(new SoundManager.SoundClipArgs(_dialogueSoundClipLibrary, _dialogueSoundClipName) {
            duration = soundDuration,
            volume = 0.5f,
            pitch = Random.Range(_dialogueSoundClipPitch/_dialogueSoundClipPitchModifier, _dialogueSoundClipPitch*_dialogueSoundClipPitchModifier)
        });
    }
}
