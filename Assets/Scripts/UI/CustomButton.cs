using UnityEngine;
using UnityEngine.Events;
public abstract class CustomButton : MonoBehaviour {
    public abstract UnityEvent GetOnClick();
    public abstract void SetText(string text);
}
