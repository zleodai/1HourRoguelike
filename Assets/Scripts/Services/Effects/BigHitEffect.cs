using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BigHitEffect : Effect
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float framesPerSecond = 60f;

    private SpriteRenderer _spriteRenderer;
    private bool _playing;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Play()
    {
        if (_playing) return;

        if (frames == null || frames.Length == 0)
        {
            Debug.LogWarning("BigHitEffect has no animation frames assigned.", this);
            Destroy(gameObject);
            return;
        }

        _playing = true;
        StartCoroutine(PlayFrames());
    }

    private IEnumerator PlayFrames()
    {
        float frameDuration = 1f / Mathf.Max(framesPerSecond, 1f);

        foreach (Sprite frame in frames)
        {
            if (frame != null)
            {
                _spriteRenderer.sprite = frame;
            }

            yield return new WaitForSeconds(frameDuration);
        }

        Destroy(gameObject);
    }
}