using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnService : Service
{
    [SerializeField] private EnemyPrefabDictionary enemies = new EnemyPrefabDictionary();

    public override void StartService()
    {
        // Required by Service. Enemy prefabs are selected when SpawnEnemy is called.
    }

    public void SpawnRandomEnemyAroundPlayer(float minDistance, float maxDistance, 
        PlayerController player) {
        if (minDistance < 0f || maxDistance < minDistance)
        {
            Debug.LogWarning("EnemySpawnService requires 0 <= minDistance <= maxDistance.", this);
            return;
        }

        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        float minDistanceSquared = minDistance * minDistance;
        float maxDistanceSquared = maxDistance * maxDistance;
        float distance = Mathf.Sqrt(UnityEngine.Random.Range(minDistanceSquared, maxDistanceSquared));

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * distance,
            Mathf.Sin(angle) * distance,
            0f);

        SpawnEnemy(player.transform.position + offset, player);
    }
    public void SpawnEnemy(Vector3 position, PlayerController player) {
        if (enemies == null || !enemies.TryGetRandomPrefab(out GameObject enemyPrefab))
        {
            Debug.LogWarning("EnemySpawnService has no valid enemy prefabs configured.", this);
            return;
        }

        GameObject enemyObject = Instantiate(enemyPrefab, position, enemyPrefab.transform.rotation);
        EnemyScript enemy = enemyObject.GetComponent<EnemyScript>();
        if (enemy == null)
        {
            Debug.LogError($"Enemy prefab '{enemyPrefab.name}' is missing EnemyScript.", enemyObject);
            Destroy(enemyObject);
            return;
        }

        enemy.OnSpawn(player);
    }
}

[Serializable]
public class EnemyPrefabDictionary : ISerializationCallbackReceiver
{
    [Serializable]
    public struct Entry
    {
        public string enemyName;
        public GameObject enemyPrefab;
    }

    [SerializeField] private List<Entry> entries = new List<Entry>();
    [NonSerialized] private Dictionary<string, GameObject> _lookup;

    public bool TryGetRandomPrefab(out GameObject prefab)
    {
        EnsureLookup();
        var candidates = new List<GameObject>(_lookup.Values);

        if (candidates.Count == 0)
        {
            prefab = null;
            return false;
        }

        prefab = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        return prefab != null;
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
            if (!string.IsNullOrWhiteSpace(entry.enemyName)
                && entry.enemyPrefab != null
                && !_lookup.ContainsKey(entry.enemyName))
            {
                _lookup.Add(entry.enemyName, entry.enemyPrefab);
            }
        }
    }
}

