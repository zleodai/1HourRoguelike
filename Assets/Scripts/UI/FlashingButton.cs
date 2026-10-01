using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlashingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [SerializeField] private Image blinkingImage;
    [SerializeField] private float blinkInterval;
    private float blinkTimeStamp;
    
    private bool _hover;
    
    [SerializeField] private Button.ButtonClickedEvent onClick;

    private void Update() {
        if (_hover) return;
        
        if (Time.time > blinkTimeStamp) {
            blinkTimeStamp = Time.time + blinkInterval;
            blinkingImage.enabled = !blinkingImage.enabled;
        }
    }
    
    private void OnHoverEnter() {
        blinkingImage.enabled = true;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _hover = true;
        OnHoverEnter();
    }
    public void OnPointerExit(PointerEventData eventData) {
        _hover = false;
    }
    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log($"{gameObject.name} pressed");
        onClick?.Invoke();
        if (SoundManager.Instance) SoundManager.Instance.PlayMouseClick();
    }
}
