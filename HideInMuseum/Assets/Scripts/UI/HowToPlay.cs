using UnityEngine;
using System.Collections;

public class HowToPlay : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _images;
    [SerializeField]
    private GameObject[] _texts;

    private int _currentInstruction = 0;

    void OnEnable()
    {
        for(int i = 0; i < _images.Length; ++i)
        {
            _images[i].SetActive(false);
            _texts[i].SetActive(false);
        }
        _currentInstruction = 0;
        _images[_currentInstruction].SetActive(true);
        _texts[_currentInstruction].SetActive(true);
    }
	
	void Update ()
    {
	    if(Input.GetMouseButtonUp(0))
        {
            _images[_currentInstruction].SetActive(false);
            _texts[_currentInstruction].SetActive(false);
            _currentInstruction += 1;
            _currentInstruction %= _images.Length;
            _images[_currentInstruction].SetActive(true);
            _texts[_currentInstruction].SetActive(true);
        }
	}
}
