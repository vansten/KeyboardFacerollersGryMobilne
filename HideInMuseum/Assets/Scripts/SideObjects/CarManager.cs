using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _carPrefabs;
    [SerializeField]
    private List<Transform> _spawnPoints;

    private List<GameObject> _cars = new List<GameObject>();
    private float _timer = 0.0f;
    private float _cooldown = 30.0f;

    void OnEnable()
    {
        _timer = 0.0f;
        _cooldown = Random.Range(25.0f, 45.0f);
    }

    void OnDisable()
    {
        foreach(GameObject car in _cars)
        {
            Destroy(car.gameObject);
        }

        _cars.Clear();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _cooldown)
        {
            SpawnCar();
        }
    }
    
    [ContextMenu("Spawn car")]
    void SpawnCar()
    {
        _timer = 0.0f;
        _cooldown = Random.Range(25.0f, 45.0f);
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
        GameObject carPrefab = _carPrefabs[Random.Range(0, _carPrefabs.Count)];
        GameObject go = (GameObject)Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
        go.SetActive(true);
        _cars.Add(go);
    }
}
