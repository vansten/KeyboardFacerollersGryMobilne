using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entrance : MonoBehaviour
{
    [HideInInspector]
    public VisitStageController VisitStage;

    private List<GroupMovement> _groupsInEntrance = new List<GroupMovement>();

    void Update()
    {
        for(int i = 0; i < _groupsInEntrance.Count; ++i)
        {
            if(_groupsInEntrance[i].RoomsToVisit.Count == 0)
            {
                VisitStage.GroupLeft(_groupsInEntrance[i]);
                _groupsInEntrance.RemoveAt(i);
                i -= 1;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("GroupLeader"))
        {
            GroupMovement gm = col.gameObject.GetComponent<GroupMovement>();
            if(gm != null)
            {
                if (gm.RoomsToVisit.Count == 0)
                {
                    VisitStage.GroupLeft(gm);
                }
                else
                {
                    _groupsInEntrance.Add(gm);
                    if (gm.CurrentRoomType != RoomType.Entrance)
                    {
                        gm.ClearPath();
                    }
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("GroupLeader"))
        {
            GroupMovement gm = col.gameObject.GetComponent<GroupMovement>();
            if (gm != null)
            {
                _groupsInEntrance.Remove(gm);
                ExclamationMark.Instance.RemoveGroupWaiting(gm);
            }
        }
    }
}
