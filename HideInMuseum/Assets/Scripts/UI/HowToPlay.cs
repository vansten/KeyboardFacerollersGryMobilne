using UnityEngine;
using System.Collections;

public class HowToPlay : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _images;
    [SerializeField]
    private GameObject[] _texts;

    private Vector2 _prevMousePosition;
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
        if(Input.GetMouseButtonDown(0))
        {
            _prevMousePosition = Input.mousePosition;
        }

        if(Input.GetMouseButtonUp(0))
        {
            float deltaX = Input.mousePosition.x - _prevMousePosition.x;
            if(deltaX > 10)
            {
                // The move was right so it means that we want previous screnn (i.e. LeftClick)
                LeftClick();
            }
            else if(deltaX < -10)
            {
                // The move was left so it means that we want next screnn (i.e. RightClick)
                LeftClick();
            }
        }
	}

    public void RightClick()
    {
        SetCurrentInstruction(+_currentInstruction + 1);
    }

    public void LeftClick()
    {
        SetCurrentInstruction(+_currentInstruction - 1);
    }

    void SetCurrentInstruction(int instructionIndex)
    {
        _images[_currentInstruction].SetActive(false);
        _texts[_currentInstruction].SetActive(false);
        _currentInstruction = instructionIndex;

        if(_currentInstruction >= _images.Length)
        {
            _currentInstruction = 0;
        }
        else if(_currentInstruction < 0)
        {
            _currentInstruction = _images.Length - 1;
        }

        _images[_currentInstruction].SetActive(true);
        _texts[_currentInstruction].SetActive(true);
    }
}
