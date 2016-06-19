using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour
{
    [SerializeField]
    private GameObject _birdPrefab;

    private float _timer;
    private float _cooldown;

    void Start()
    {
        _cooldown = Random.Range(20.0f, 60.0f);
    }

    void Update ()
    {
        _timer += Time.deltaTime;
        if(_timer > _cooldown)
        {
            SpawnBird();
        }
	}

    [ContextMenu("Spawn bird")]
    void SpawnBird()
    {
        _timer = 0.0f;
        _cooldown = Random.Range(20.0f, 60.0f);
        GameObject bird = (GameObject)Instantiate(_birdPrefab, transform.position, Quaternion.identity);
        bird.transform.parent = transform;
    }
}
