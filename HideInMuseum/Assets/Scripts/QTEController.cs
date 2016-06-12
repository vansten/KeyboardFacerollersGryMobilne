using UnityEngine;
using System.Collections;

public class QTEController : Singleton<QTEController>
{
    public Vector2 CooldownRange;

    public void SpawnQTE(Room room)
    {
        //Prevent from having multiple qte events in the same room
        if(room.IsQTEActive)
        {
            return;
        }

        int qte = Random.Range(0, int.MaxValue) % 2;
        if(qte == 0)
        {
            room.Darkness.SetActive(true);
            room.DarknessIcon.SetActive(true);
        }
        else
        {
            room.Water.SetActive(true);
            room.WaterIcon.SetActive(true);
        }

        room.IsQTEActive = true;
    }

    public void RemoveQTE(Room room)
    {
        room.Darkness.SetActive(false);
        room.DarknessIcon.SetActive(false);
        room.Water.SetActive(false);
        room.WaterIcon.SetActive(false);
        room.IsQTEActive = false;
    }
}
