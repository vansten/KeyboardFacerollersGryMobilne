using UnityEngine;
using System.Collections;

public enum RoomType
{
    OldCameras = 0,
    NewCameras,
    Films,
    Posters,
    Tapes,
    Animations, 
    Characters,
}

public class Room : MonoBehaviour
{
    public RoomType Type;
}
