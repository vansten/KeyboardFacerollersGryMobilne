using UnityEngine;
using System.Collections;

public class GroupMember : MonoBehaviour
{
    public GroupMovement GroupLeader;
    public Transform PreviousMemberHook;
	
	void FixedUpdate ()
	{
	    if (Vector3.Distance(transform.position, PreviousMemberHook.position) <= 0.2f)
	    {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, transform.position - PreviousMemberHook.parent.position), 8f * Time.deltaTime);
            return;
	    }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, transform.position - PreviousMemberHook.position), 8f * Time.deltaTime);
	    transform.position += -transform.up * GroupLeader.MovementDelta.magnitude;

	}
}
