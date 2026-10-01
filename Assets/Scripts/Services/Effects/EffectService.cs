using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectService : Service
{
    [SerializeField] private EffectPrefabDictionary effects = new EffectPrefabDictionary();

    public override void StartService()
    {
        // Effects are initialized when PlayEffect is called.
    }

    public void PlayEffect(string effectName, Vector2 position, Vector2 direction)
    {
        if (string.IsNullOrWhiteSpace(effectName)
            || effects == null
            || !effects.TryGetValue(effectName, out GameObject effectPrefab)
            || effectPrefab == null)
        {
            Debug.LogWarning($"EffectService could not find an effect named '{effectName}'.", this);
            return;
        }

        float angle = direction.sqrMagnitude > Mathf.Epsilon
            ? Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg
            : 0f;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        Vector3 spawnPosition = new Vector3(position.x, position.y, 0f);

        GameObject effectObject = Instantiate(effectPrefab, spawnPosition, rotation);
        Effect effect = effectObject.GetComponent<Effect>();
        if (effect == null)
        {
            Debug.LogError($"Effect prefab '{effectPrefab.name}' must have a component derived from Effect.", effectObject);
            Destroy(effectObject);
            return;
        }

        effect.Play();
    }
}

[Serializable]
public class EffectPrefabDictionary : ISerializationCallbackReceiver
{
    [Serializable]
    public struct Entry
    {
        public string effectName;
        public GameObject effectPrefab;
    }

    [SerializeField] private List<Entry> entries = new List<Entry>();
    [NonSerialized] private Dictionary<string, GameObject> _lookup;

    public bool TryGetValue(string effectName, out GameObject effectPrefab)
    {
        EnsureLookup();
        return _lookup.TryGetValue(effectName, out effectPrefab);
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        RebuildLookup();
    }

    private void EnsureLookup()
    {
        if (_lookup == null)
        {
            RebuildLookup();
        }
    }

    private void RebuildLookup()
    {
        _lookup = new Dictionary<string, GameObject>(StringComparer.Ordinal);

        if (entries == null)
        {
            return;
        }

        foreach (Entry entry in entries)
        {
            if (!string.IsNullOrWhiteSpace(entry.effectName)
                && entry.effectPrefab != null
                && !_lookup.ContainsKey(entry.effectName))
            {
                _lookup.Add(entry.effectName, entry.effectPrefab);
            }
        }
    }
}
