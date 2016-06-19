using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PeopleManager : Singleton<PeopleManager>
{
    [SerializeField]
    public List<GameObject> _passerbyPrefabs;
    [SerializeField]
    private List<Transform> _spawnPoints;

    private List<GameObject> _spawnedPasserbies = new List<GameObject>();
    private float _timer;
    private float _cooldown;

    void OnEnable()
    {
        _cooldown = Random.Range(15.0f, 30.0f);
        _timer = 0.0f;
    }

    void OnDisable()
    {
        foreach(GameObject go in _spawnedPasserbies)
        {
            Destroy(go);
        }
        _spawnedPasserbies.Clear();
    }

    void Update()
    {
        for(int i = 0; i < _spawnedPasserbies.Count; ++i)
        {
            _spawnedPasserbies[i].transform.position -= _spawnedPasserbies[i].transform.up * 2.0f * Time.deltaTime;
            if(ShouldDestroy(_spawnedPasserbies[i]))
            {
                GameObject go = _spawnedPasserbies[i];
                _spawnedPasserbies.RemoveAt(i);
                i -= 1;
                Destroy(go);
            }
        }

        _timer += Time.deltaTime;
        if(_timer > _cooldown)
        {
            SpawnPasserby();
        }
    }

    void SpawnPasserby()
    {
        _timer = 0.0f;
        _cooldown = Random.Range(15.0f, 30.0f);

        int prefabI = Random.Range(0, _passerbyPrefabs.Count);
        int spawnPointI = Random.Range(0, _spawnPoints.Count);
        GameObject newPasserby = (GameObject)Instantiate(_passerbyPrefabs[prefabI], _spawnPoints[spawnPointI].position, _spawnPoints[spawnPointI].rotation);
        _spawnedPasserbies.Add(newPasserby);
    }

    bool ShouldDestroy(GameObject go)
    {
        Vector3 pos = go.transform.position;
        return Mathf.Abs(pos.x) > 60.0f || Mathf.Abs(pos.y) > 60.0f;
    }

    public void AddPasserby(GameObject go)
    {
        _spawnedPasserbies.Add(go);
    }

    public GameObject GetRandomPerson()
    {
        int prefabI = Random.Range(0, _passerbyPrefabs.Count);
        return _passerbyPrefabs[prefabI];
    }
}
