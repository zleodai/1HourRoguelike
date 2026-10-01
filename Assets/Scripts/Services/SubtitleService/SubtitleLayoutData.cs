using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SubtitleLayoutData", menuName = "Persistence/SubtitleLayoutData")]
public class SubtitleLayoutData: ScriptableObject {
    [SerializeField] public Dictionary<string, GameObject> layouts;
}
