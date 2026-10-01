
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSelectButton : CustomButton, IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler {

    [SerializeField] public string weaponName;
    [SerializeField] private Image selectedFrame;
    [SerializeField] private Image tintImage;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    
    private readonly Color DefaultTintColor = new(0/255f, 255/255f, 0/255f, 0/255f);
    private readonly Color SelectedTintColor = new(0/255f, 255/255f, 0/255f, 1/255f);
    private readonly Color ClickedTintColor = new(0/255f, 255/255f, 0/255f, 2/255f);
    
    private readonly Color DefaultTextColor = Color.white;
    private readonly Color SelectedTextColor = new(0/255f, 200/255f, 0/255f);
    private readonly Color ClickedTextColor = new(0/255f, 255/255f, 0/255f);
        
    private readonly Color DefaultFrameColor = Color.black;
    private readonly Color SelectedFrameColor = new(0/255f, 255/255f, 0/255f);

    private string text;

    private bool _hover;
    private bool hover {
        get => _hover;
        set {
            if (_hover == value) return;
            _hover = value;
            OnStateChange();
        }
    }
    
    private bool _press;
    private bool press {
        get => _press;
        set {
            if (_press == value) return;
            _press = value;
            OnStateChange();
        }
    }

    private bool _selected;
    public bool selected {
        get => _selected;
        set {
            if (_selected == value) return;
            _selected = value;
            OnStateChange();
        }
    }

    [SerializeField] private float scaleOnHover = 1.008f;
    private Vector3 scale;

    [SerializeField] Button.ButtonClickedEvent onClick;
    private UnityEvent setOnClick;

    void Awake() {
        text = weaponNameText.text;
        scale = transform.localScale;
        
        OnStateChange();
    }

    void OnDestroy() {
        onClick.RemoveAllListeners();
        setOnClick.RemoveAllListeners();
    }

    public void OnPointerEnter(PointerEventData _) => hover = true;
    public void OnPointerExit(PointerEventData _) => hover = false;
    public void OnPointerDown(PointerEventData _) => press = true;
    public void OnPointerUp(PointerEventData _) {
        press = false;
        
        setOnClick?.Invoke();
        onClick?.Invoke();
        
        if (SoundManager.Instance) SoundManager.Instance.PlayMouseClick();
    }

    public void OnStateChange() {
        if (press) {
            transform.localScale = scale;
            tintImage.color = ClickedTintColor;
            weaponNameText.color = ClickedTextColor;
            selectedFrame.color = DefaultFrameColor;
            return;
        }

        if (selected) {
            transform.localScale = scale;
            tintImage.color = SelectedTintColor;
            weaponNameText.color = SelectedTextColor;
            selectedFrame.color = SelectedFrameColor;
            return;
        }

        if (hover) {
            transform.localScale = scale * scaleOnHover;
            tintImage.color = SelectedTintColor;
            weaponNameText.color = SelectedTextColor;
            selectedFrame.color = DefaultFrameColor;
            return;
        }
        
        transform.localScale = scale;
        tintImage.color = DefaultTintColor;
        weaponNameText.color = DefaultTextColor;
        selectedFrame.color = DefaultFrameColor;
    }

    public override UnityEvent GetOnClick() {
        setOnClick ??= new UnityEvent();

        return setOnClick;
    }
    
    public override void SetText(string text) {
        this.text = text;
    }
}
