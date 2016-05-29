using UnityEngine;
using System.Collections;
using AStar;

public class GroupMember : MonoBehaviour
{
    public GroupMovement GroupLeader;
    public Transform PreviousMemberHook;

    public int MemberIndex;

    protected AStarAgent _myAgent;
    protected GridElement _currentElement;

    public AStarAgent MyAgent
    {
        get { return _myAgent; }
    }

    protected void Awake()
    {
        _myAgent = GetComponent<AStarAgent>();
        if (_myAgent == null)
        {
            _myAgent = gameObject.AddComponent<AStarAgent>();
            _myAgent.UseMyPositionAsStart = true;
        }

        if (GameManager.Instance.MainGrid == null)
        {
            Debug.LogError("There is no grid");
            gameObject.SetActive(false);
            return;
        }
        _myAgent.MyGrid = GameManager.Instance.MainGrid;
    }
	
	void FixedUpdate ()
	{
	    //if (GroupLeader.IsMoving) ControlByAStar();

	    //if (MyAgent.Path.Count == 0) return;

     //   if (_currentElement != null)
     //   {
     //       transform.rotation = Quaternion.Lerp(transform.rotation,
     //           Quaternion.LookRotation(Vector3.forward, _currentElement.transform.position - gameObject.transform.position), 1f/* * 2f */ );
     //       if (Vector3.Distance(transform.position, _currentElement.transform.position) < 0.2f) NextGridElement();
        //}

	    if (Vector3.Distance(transform.position, PreviousMemberHook.position) <= 0.5f) return;  

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, transform.position - PreviousMemberHook.position), 8f * Time.deltaTime);
        transform.position += -transform.up * 0.06f;
	}

    public void ControlByAStar()
    {
        //_useAStar = true;
        MyAgent.TargetObject = PreviousMemberHook;
        MyAgent.CalculatePath();
        NextGridElement();
    }

    protected void NextGridElement()
    {
        if (_myAgent.Path.Count == 0) return;
        _currentElement = _myAgent.Path[0];
        _myAgent.Path.RemoveAt(0);
    }
}
