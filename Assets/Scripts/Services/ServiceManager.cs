using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class ServiceManager : MonoBehaviour {
    private static ServiceManager _instance;
    
    public static ServiceManager Instance {
        get {
            if (_instance != null) return _instance;
            
            _instance = FindAnyObjectByType<ServiceManager>();
            if (_instance != null) return _instance;
            
            GameObject serviceManager = new GameObject("ServiceManager");
            _instance = serviceManager.AddComponent<ServiceManager>();
            
            return _instance;
        }
    }

    private Dictionary<Type, Service> _services;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        
        _services = new Dictionary<Type, Service>();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        foreach (var root in scene.GetRootGameObjects()) {
            foreach (var candidate in root.GetComponentsInChildren<Service>(true)) {
                var type = candidate.GetType();

                if (_services.TryGetValue(type, out var canonical)
                    && canonical != null
                    && candidate != canonical) {
                    Destroy(candidate.gameObject);
                }
            }
        }
    }

    public T GetService<T>() where T : Service {
        var type = typeof(T);
        if (!_services.ContainsKey(type) && !AttemptLoadService<T>()) {
            throw new Exception($"{type.Name} failed to load");
        }

        var service = (T)_services[type];
        if (service == null) {
            throw new Exception($"{type.Name} destroyed");
        }
        
        return (T)_services[type];
    }
    
    private bool AttemptLoadService<T>() where T : Service {
        var type = typeof(T);
        T service = null;

        var services = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (services.Length > 0) {
            service = services[0];
        }

        if (service == null) {
            var serviceObj = new GameObject($"{type.Name}");
            service = serviceObj.AddComponent<T>();
        }

        if (service == null) return false;

        if (!service.gameObject.activeInHierarchy) {
            service.gameObject.SetActive(true);
        }
        service.enabled = true;
        service.transform.parent = transform;
        _services.Add(type, service);
        Debug.Log($"{type.Name} started");
        service.StartService();
        
        return true;
    }
}
