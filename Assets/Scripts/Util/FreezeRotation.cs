using UnityEngine;

[DisallowMultipleComponent]
public class FreezeRotation : MonoBehaviour {
    private static readonly Quaternion rot = Quaternion.Euler(Vector3.zero);
    private void Update() {
        transform.rotation = rot;
    }
    private void LateUpdate() {
        transform.rotation = rot;
    }
}
