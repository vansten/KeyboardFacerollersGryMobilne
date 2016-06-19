using UnityEngine;
using System.Collections;

public class QTEController : Singleton<QTEController>
{
    public Vector2 CooldownRange;
    public AudioClip WaterSpashClip;
    public AudioClip LightsOffClip;

    public void SpawnQTE(Room room)
    {
        //Prevent from having multiple qte in the same room
        //Also prevent from having qte in locked room
        if(room.IsQTEActive || !room.Unlocked)
        {
            return;
        }

        int qte = Random.Range(0, int.MaxValue) % 2;
        if(qte == 0)
        {
            room.Darkness.SetActive(true);
            room.DarknessIcon.SetActive(true);
            AudioSource.PlayClipAtPoint(LightsOffClip, Camera.main.transform.position);
        }
        else
        {
            room.Water.SetActive(true);
            room.WaterIcon.SetActive(true);
            AudioSource.PlayClipAtPoint(WaterSpashClip, Camera.main.transform.position);
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
