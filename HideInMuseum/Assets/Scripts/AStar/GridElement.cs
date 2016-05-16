﻿using UnityEngine;
using System.Collections;

namespace AStar
{
    public class GridElement : MonoBehaviour
    {
        [SerializeField]
        protected bool _walkable = true;
        public virtual bool Walkable
        {
            get { return _walkable; }
            set { _walkable = value; }
        }

        [SerializeField]
        protected int _fieldMoveCost;
        public virtual int FieldMoveCost
        {
            get { return _fieldMoveCost; }
            set { _fieldMoveCost = value; }
        }

        protected int _heuristicCost;

        protected IntVector3 _elementIndex;
        public virtual IntVector3 ElementIndex
        {
            get { return _elementIndex; }
            set { _elementIndex = value; }
        }

        protected GridElement _pathParentField;
        public virtual GridElement PathParentField 
        {
            get { return _pathParentField; }
            set
            {
                _pathParentField = value;
                int dist = CheckMoveDistance(this);
                if (dist <= 0)
                {
                    _fieldMoveCost = _elementMoveCostDirection.x;
                }
                else if (dist == 1)
                {
                    _fieldMoveCost = _elementMoveCostDirection.y;
                }
                else
                {
                    _fieldMoveCost = _elementMoveCostDirection.z;
                }
            }
        }

        public virtual int TotalFieldMoveCost
        {
            get
            {
                GridElement element = this;
                int totalCost = _heuristicCost;
                while (element != null)
                {
                    totalCost += FieldMoveCost;
                    element = PathParentField;
                }
                return totalCost;
            }
        }

        protected IntVector3 _elementMoveCostDirection;
        public virtual IntVector3 ElementMoveCostDirection
        {
            get { return _elementMoveCostDirection; }
            set { _elementMoveCostDirection = value; }
        }


        public static int CheckMoveDistance(GridElement element)
        {
            if (element.PathParentField != null)
            {
                return Mathf.Abs(element.ElementIndex.x - element.PathParentField.ElementIndex.x) +
                    Mathf.Abs(element.ElementIndex.y - element.PathParentField.ElementIndex.y) +
                    Mathf.Abs(element.ElementIndex.z - element.PathParentField.ElementIndex.z);               
            }
            else
            {
                return 0;
            }
        }

        public static int CheckMoveDistance(GridElement element, GridElement newParent)
        {
            if (element.PathParentField != null)
            {
                return Mathf.Abs(element.ElementIndex.x - newParent.ElementIndex.x) +
                    Mathf.Abs(element.ElementIndex.y - newParent.ElementIndex.y) +
                    Mathf.Abs(element.ElementIndex.z - newParent.ElementIndex.z);
            }
            else
            {
                return 0;
            }
        }

        public virtual void CalculateHeuristic(GridElement element)
        {
            _heuristicCost = Mathf.FloorToInt(Vector3.Distance(gameObject.transform.position, element.transform.position));
        }
    }
}
