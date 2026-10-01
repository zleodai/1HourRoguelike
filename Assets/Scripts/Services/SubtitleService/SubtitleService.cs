using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleService: Service {
    private SubtitleLayoutData subtitleData;
    private Canvas subtitleCanvas;
    private bool isInitialized;
    
    public override void StartService() {
        subtitleData = Resources.Load<SubtitleLayoutData>("MainSubtitleLayoutData");
        
        if (subtitleData == null) {
            Debug.LogWarning("Subtitle service could not load MainSubtitleLayoutData from resources");
            return;
        }
        var subtitleCanvasObj = new GameObject("SubtitleCanvas");
        subtitleCanvas = subtitleCanvasObj.AddComponent<Canvas>();
        subtitleCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var canvasScaler = subtitleCanvasObj.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0;
        canvasScaler.referencePixelsPerUnit = 100;
        
        isInitialized = true;        
    }

    public struct SubtitleArgs {
        public readonly string text;

        [CanBeNull] public Transform canvasTransform;
        public string subtitleLayoutName;
        
        public bool isWorldSpaceCanvas;
        
        [Obsolete("Use canvasTransform instead.")]
        [CanBeNull] public Canvas worldSpaceCanvas;
        [Obsolete("Use canvasTransform instead.")]
        [CanBeNull] public RectTransform worldSpaceCanvasRectTransform;

        public bool playSound;
        public float punctuationPause;
        public float charDuration;
        public float afterTypedDuration;
        public string dialogueSoundClipLibrary;
        public string dialogueSoundClipName;
        public float dialogueSoundClipPitch;
        public float dialogueSoundClipPitchVariationModifier;
        
        [CanBeNull] public Action onSubtitleEnd;

        public SubtitleArgs(string text) {
            this.text = text;

            canvasTransform = null;
            subtitleLayoutName = "ScreenSpaceDefault";
            
            isWorldSpaceCanvas = false;
            worldSpaceCanvas = null;
            worldSpaceCanvasRectTransform = null;

            playSound = true;
            punctuationPause = 0.025f;
            charDuration = 0.07f;
            afterTypedDuration = 1f;
            dialogueSoundClipLibrary = "Dialogue";
            dialogueSoundClipName = "3";
            dialogueSoundClipPitch = 0.5f;
            dialogueSoundClipPitchVariationModifier = 1.075f;

            onSubtitleEnd = null;
        }

        public static SubtitleArgs Voice1(string text) {
            return new SubtitleArgs(text) {
                dialogueSoundClipName = "1",
                dialogueSoundClipPitch = 0.27f,
            };
        }
        
        public static SubtitleArgs Voice2(string text) {
            return new SubtitleArgs(text) {
                dialogueSoundClipName = "3",
                dialogueSoundClipPitch = 0.5f,
            };
        }
    }

    public IEnumerator CreateAndPlaySubtitleChain(SubtitleArgs[] argEntries) {
        foreach (var args in argEntries) {
            yield return CreateAndPlaySubtitle(args);
        }
    }

    private Subtitle GetSubtitles(SubtitleArgs args, Transform canvasParent) {
        var subtitleLayout = subtitleData.layouts[args.subtitleLayoutName];
        if (!subtitleLayout) {
            throw new Exception($"Subtitle layout {args.subtitleLayoutName} does not exist");
        }
        
        var subtitles = Instantiate(subtitleLayout, canvasParent, false);
        subtitles.name = $"{args.subtitleLayoutName} subtitles";
        var subtitleScript = subtitles.GetComponent<Subtitle>();

        if (subtitleScript == null) {
            throw new Exception($"Subtitle layout {args.subtitleLayoutName} does not contain a Subtitle Script");
        }
        return subtitleScript;
    }

    private Transform GetWorldSpaceCanvasTransform(SubtitleArgs args) {
        if (args.worldSpaceCanvas != null) {
            return args.worldSpaceCanvas.transform;
        }
        
        var canvasObj = new GameObject("SubtitleCanvas", typeof(RectTransform), typeof(Canvas));
        var canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        var refRTransform = args.worldSpaceCanvasRectTransform;
        if (refRTransform is null) {
            throw new Exception("GetWorldSpaceSubtitles failed. args.worldSpaceCanvasRectTransform were null");
        }
        
        var rTransform = canvas.GetComponent<RectTransform>();
        rTransform.SetParent(refRTransform.parent);
        rTransform.anchorMin = refRTransform.anchorMin;
        rTransform.anchorMax = refRTransform.anchorMax;
        rTransform.pivot = refRTransform.pivot;
        rTransform.sizeDelta = refRTransform.sizeDelta;
        rTransform.offsetMin = refRTransform.offsetMin;
        rTransform.offsetMax = refRTransform.offsetMax;
        rTransform.localPosition = refRTransform.localPosition;
        rTransform.localScale = refRTransform.localScale;
        rTransform.localRotation = refRTransform.localRotation;
        
        return canvasObj.transform;
    }

    private Subtitle GetWorldSpaceSubtitles(SubtitleArgs args) {
        if (args.canvasTransform != null) {
            return GetSubtitles(args, args.canvasTransform);
        }
        
        var subtitle = GetSubtitles(args, GetWorldSpaceCanvasTransform(args));
        var subRTransform = subtitle.GetComponent<RectTransform>();
        subRTransform.offsetMax = Vector2.zero;
        subRTransform.offsetMin = Vector2.zero;
        return subtitle;
    }

    private Subtitle GetScreenSpaceSubtitles(SubtitleArgs args) {
        if (args.canvasTransform != null) {
            return GetSubtitles(args, args.canvasTransform);
        }
        
        return GetSubtitles(args, subtitleCanvas.transform);
    }
    
    public IEnumerator CreateAndPlaySubtitle(SubtitleArgs args) {
        if (!isInitialized) throw new Exception("Subtitles not initialized");

        var subtitleScript = args.isWorldSpaceCanvas ? GetWorldSpaceSubtitles(args) : GetScreenSpaceSubtitles(args);
        
        yield return subtitleScript.PlaySequence(args);
        
        if (args.isWorldSpaceCanvas && args.worldSpaceCanvas == null) {
            Destroy(subtitleScript.transform.parent.gameObject);
        }
        Destroy(subtitleScript.gameObject);
        
        args.onSubtitleEnd?.Invoke();
    }

    public Subtitle CreateSubtitles(SubtitleArgs args) {
        if (!isInitialized) throw new Exception("Subtitles not initialized");
        
        return args.isWorldSpaceCanvas ? GetWorldSpaceSubtitles(args) : GetScreenSpaceSubtitles(args);
    }
}
