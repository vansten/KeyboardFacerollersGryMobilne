using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _carPrefabs;
    [SerializeField]
    private List<Transform> _spawnPoints;

    private List<GameObject> _carsToUpdate = new List<GameObject>();
    private float _timer = 0.0f;
    private float _cooldown = 30.0f;

    void OnEnable()
    {
        _timer = 0.0f;
        _cooldown = Random.Range(25.0f, 45.0f);
    }

    void OnDisable()
    {
        foreach(GameObject car in _carsToUpdate)
        {
            Destroy(car.gameObject);
        }

        _carsToUpdate.Clear();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _cooldown)
        {
            SpawnCar();
        }

        List<GameObject> toRemove = new List<GameObject>();
        foreach (GameObject car in _carsToUpdate)
        {
            car.transform.position -= car.transform.right * 5.0f * Time.deltaTime;
            if ((Mathf.Abs(car.transform.position.x) > 60.0f) || (Mathf.Abs(car.transform.position.y) > 60.0f))
            {
                toRemove.Add(car);
            }
        }

        foreach(GameObject car in toRemove)
        {
            _carsToUpdate.Remove(car);
            Destroy(car.gameObject);
        }

        toRemove.Clear();
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
        _carsToUpdate.Add(go);
    }
}
