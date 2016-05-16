﻿using System;
using UnityEngine;
using System.Collections;

namespace AStar
{
    public enum GirdType
    {
        TwoDimensional = 0,
        ThreeDimensional

    }

    public class Grid : MonoBehaviour
    {
        [SerializeField]
        protected GirdType _gridType;
        public GirdType GridType
        {
            get { return _gridType; }
            set { _gridType = value; }
        }
        [SerializeField]
        protected IntVector3 _gridSize;
        public virtual IntVector3 GridSize
        {
            get { return _gridSize; }
            set { _gridSize = value; }
        }
        [SerializeField]
        protected Vector3 _gridElementSize;
        [SerializeField]
        protected Vector3 _startPosition = Vector3.zero;
        [SerializeField]
        protected bool _useMyPositionAsStart = true;
        [SerializeField]
        protected IntVector3 _generateDirection = new IntVector3(1,-1,0);
        public virtual IntVector3 GenerateDirection
        {
            get { return _generateDirection; }
            set { _generateDirection = value; }
        }
        [SerializeField]
        protected GameObject _gridElementObject;

        protected GridElement[,,] _elements;
        public virtual GridElement[,,] Elements
        {
            get { return _elements; }
        }

        protected IntVector3 _elementMoveCostDirection;

        [ContextMenu("GenerateGrid")]
        public virtual void GenerateGrid()
        {
            _elementMoveCostDirection = new IntVector3(0, 
                (int)(10 * ((_gridElementSize.x + _gridElementSize.y + _gridElementSize.z) / 3f)), 
                (int)(10 * (Mathf.Sqrt(Mathf.Pow(_gridElementSize.x, 2f) + Mathf.Pow(_gridElementSize.y, 2f) + Mathf.Pow(_gridElementSize.z, 2f)))));

            switch (_gridType)
            {
                case GirdType.TwoDimensional:
                {
                    int x = _gridSize.x;
                    int y = _gridSize.y;
                    _elements = new GridElement[y, x, 1];

                    for (int i = 0; i < x; ++i)
                    {
                        for (int j = 0; j < y; ++j)
                        {
                            GameObject element;
                            if (_gridElementObject == null)
                            {
                                element = new GameObject();
                            }
                            else
                            {
                                element =
                                    Instantiate(_gridElementObject, Vector3.zero, Quaternion.identity) as GameObject;
                            }
                            element.name = string.Format("GridElement[{0}][{1}]", j, i);

                            if (_useMyPositionAsStart)
                            {
                                Vector3 startPosition = gameObject.transform.position;
                                element.transform.position =
                                    new Vector3(startPosition.x + (_gridElementSize.x*_generateDirection.x*j),
                                        startPosition.y + (_gridElementSize.y*_generateDirection.y*i),
                                        startPosition.z);
                            }
                            else
                            {
                                element.transform.position =
                                    new Vector3(_startPosition.x + (_gridElementSize.x*_generateDirection.x*j),
                                        _startPosition.y + (_gridElementSize.y*_generateDirection.y*i),
                                        _startPosition.z);
                            }
                            element.transform.parent = gameObject.transform;

                            GridElement gridElement = element.GetComponent<GridElement>();

                            if (gridElement == null)
                            {
                                _elements[j, i, 0] = element.AddComponent<GridElement>();
                            }
                            else
                            {
                                _elements[j, i, 0] = gridElement;
                            }
                            gridElement.ElementIndex = new IntVector3(j,i,0);
                            gridElement.ElementMoveCostDirection = _elementMoveCostDirection;
                        }
                    }
                }
                break;
                case GirdType.ThreeDimensional:
                {
                    int x = (int) _gridSize.x;
                    int y = (int) _gridSize.y;
                    int z = (int) _gridSize.z;
                    _elements = new GridElement[x, y, z];

                    for (int i = 0; i < x; ++i)
                    {
                        for (int j = 0; j < y; ++j)
                        {
                            for (int k = 0; k < z; ++k)
                            {
                                GameObject element;
                                if (_gridElementObject == null)
                                {
                                    element = new GameObject();
                                }
                                else
                                {
                                    element =
                                        Instantiate(_gridElementObject, Vector3.zero, Quaternion.identity) as GameObject;
                                }
                                element.name = string.Format("GridElement[{0}][{1}][{2}]", j, i, k);

                                if (_useMyPositionAsStart)
                                {
                                    Vector3 startPosition = gameObject.transform.position;
                                    element.transform.position =
                                        new Vector3(startPosition.x + (_gridElementSize.x*_generateDirection.x*j),
                                            startPosition.y + (_gridElementSize.y*_generateDirection.y*i),
                                            startPosition.z + (_gridElementSize.z * _generateDirection.z * k));
                                }
                                else
                                {
                                    element.transform.position =
                                        new Vector3(_startPosition.x + (_gridElementSize.x*_generateDirection.x*j),
                                            _startPosition.y + (_gridElementSize.y*_generateDirection.y*i),
                                            _startPosition.z + (_gridElementSize.z * _generateDirection.z * k));
                                    }
                                element.transform.parent = gameObject.transform;

                                GridElement gridElement = element.GetComponent<GridElement>();

                                if (gridElement == null)
                                {
                                    _elements[j, i, k] = element.AddComponent<GridElement>();
                                }
                                else
                                {
                                    _elements[j, i, k] = gridElement;
                                }
                                gridElement.ElementIndex = new IntVector3(j,i,k);
                            }
                        }
                    }
                }
                break;
            }
        }

        [ContextMenu("RemoveGrid")]
        public virtual void RemoveGrid()
        {
            int child = transform.childCount;

            for (int i = 0; i < child; ++i)
            {
                DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
            }
        }

        [ContextMenu("ReGenerateGrid")]
        public virtual void RegenerateGrid()
        {
            RemoveGrid();
            GenerateGrid();
        }
    }

    [Serializable]
    public class IntVector3
    {
        [SerializeField]
        protected int _x;
        public int x
        {
            get { return _x; }
            set { _x = value; }
        }
        [SerializeField]
        protected int _y;
        public int y
        {
            get { return _y; }
            set { _y = value; }
        }
        [SerializeField]
        protected int _z;
        public int z
        {
            get { return _z; }
            set { _z = value; }
        }

        public IntVector3(int x=0, int y=0, int z=0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
