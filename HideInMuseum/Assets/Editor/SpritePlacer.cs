using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class SpritePlacer : EditorWindow
{
    private static SpritePlacer _instance;
    private Sprite _sprite;
    private Transform _parent;

    [MenuItem("Window/Sprite placer")]
    private static void Init()
    {
        _instance = GetWindow<SpritePlacer>();
        _instance._sprite = null;
        _instance.Show();
    }

    void OnGUI()
    {
        _sprite = (Sprite)EditorGUILayout.ObjectField("Sprite to place:",_sprite, typeof(Sprite), false);
        _parent = (Transform)EditorGUILayout.ObjectField("Parent:", _parent, typeof(Transform), true);
        if(_sprite != null && _parent != null)
        {
            if(GUILayout.Button("Generate"))
            {
                Generate();
            }
        }
    }

    void Generate()
    {
        string title = "Sprite placer";
        string info = "Finding sprites";
        float progress = 0.0f;
        EditorUtility.DisplayProgressBar(title, info, progress);
        SpriteRenderer[] spritesOnLevel = FindObjectsOfType<SpriteRenderer>();
        progress = 0.2f;
        title = "Generating positions for new sprites";
        List<Vector3> newPositionsList = new List<Vector3>();
        int maxI = 70;
        int maxJ = 70;
        for(int i = 0; i < maxI; ++i)
        {
            for(int j = 0; j < maxJ; ++j)
            {
                Vector3 positionToAdd = new Vector3(-30 + i, -30 + j, 0.0f);
                newPositionsList.Add(positionToAdd);
                info = "Adding position " + positionToAdd.ToString();
                progress = 0.2f + (0.3f * (float)(i * maxJ + j) / (float)(maxJ * maxI));
                EditorUtility.DisplayProgressBar(title, info, progress);
            }
        }

        title = "Removing unnecessary positions";

        List<Vector3> toRemove = new List<Vector3>();
        for(int i = 0; i < spritesOnLevel.Length; ++i)
        {
            info = "Removing sprite's " + i + " position";
            progress = 0.5f + (0.1f * (float)i / (float)spritesOnLevel.Length);
            EditorUtility.DisplayProgressBar(title, info, progress);
            for (int j = 0; j < newPositionsList.Count; ++j)
            {
                float dist = Vector3.Distance(newPositionsList[j], spritesOnLevel[i].transform.position);
                if(dist < 1.0f)
                {
                    toRemove.Add(newPositionsList[j]);
                }
            }
        }
        
        for(int i = 0; i < toRemove.Count;++i)
        {
            newPositionsList.Remove(toRemove[i]);
        }
        toRemove.Clear();

        title = "Placing sprites";
        SpriteRenderer prefab = spritesOnLevel[0];
        for(int i = 0; i < newPositionsList.Count; ++i)
        {
            info = "Placing sprite on position: " + newPositionsList[i].ToString();
            progress = 0.7f + (0.3f * (float)i / (float)newPositionsList.Count);
            EditorUtility.DisplayProgressBar(title, info, progress);
            SpriteRenderer newSpriteRenderer = (SpriteRenderer)Instantiate(prefab, newPositionsList[i], Quaternion.identity);
            newSpriteRenderer.sprite = _sprite;
            newSpriteRenderer.sortingLayerName = "Default";
            newSpriteRenderer.sortingOrder = -1;
            newSpriteRenderer.name = _sprite.name + " (" + i + ")";
            newSpriteRenderer.transform.parent = _parent;
        }

        title = "Finished";
        info = "Uf";
        progress = 1.0f;
        EditorUtility.DisplayProgressBar(title, info, progress);
        EditorUtility.ClearProgressBar();
    }
}
