using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageButton : CustomButton, IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler {
    [SerializeField] private TextMeshProUGUI textDisplay;

    private string _text;
    private bool _press;

    [SerializeField] private float scaleOnHover = 1.008f;
    private Vector3 _scale;

    [SerializeField] Button.ButtonClickedEvent onClick;
    private UnityEvent setOnClick;

    void Awake() {
        _text = textDisplay.text;
        _scale = transform.localScale;
    }

    void OnDestroy() {
        onClick.RemoveAllListeners();
        setOnClick.RemoveAllListeners();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (_press) return;
        
        transform.localScale = _scale * scaleOnHover;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (_press) return;

        transform.localScale = _scale;
    }

    public void OnPointerDown(PointerEventData eventData) {
        _press = true;

        transform.localScale = _scale;
    }

    public void OnPointerUp(PointerEventData eventData) {
        _press = false;

        setOnClick?.Invoke();
        onClick?.Invoke();
        
        if (SoundManager.Instance) SoundManager.Instance.PlayMouseClick();
    }

    public override UnityEvent GetOnClick() {
        setOnClick ??= new UnityEvent();

        return setOnClick;
    }
    
    public override void SetText(string text) {
        _text = text;
    }
}
