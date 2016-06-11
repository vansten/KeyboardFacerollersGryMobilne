using UnityEngine;
using System.Collections;

public class Entrance : MonoBehaviour
{
    [HideInInspector]
    public VisitStageController VisitStage;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("GroupLeader"))
        {
            GroupMovement gm = col.gameObject.GetComponent<GroupMovement>();
            if(gm != null)
            {
                if(gm.RoomsToVisit.Count == 0)
                {
                    VisitStage.GroupLeft(gm);
                }
                else if(gm.CurrentRoomType != RoomType.Entrance)
                {
                    gm.ClearPath();
                }
            }
        }
    }
}
