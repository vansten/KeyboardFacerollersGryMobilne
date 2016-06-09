using UnityEngine;
using System.Collections;

public enum RoomType
{
    OldCameras = 0,
    CostumesAndSuits,
    Posters,
    Tapes,
    Animations, 
    Characters,
    Entrance
}

public class Room : MonoBehaviour
{
    public RoomType Type;
}
