using System;
using UnityEngine;
using System.Collections.Generic;

namespace AStar
{
    public class AStar : MonoBehaviour
    {
        public Grid MyGrid;

        protected List<GridElement> _openElements;
        protected List<GridElement> _closedElements; 

        protected GridElement _currentElement;
        protected GridElement _startElement;
        protected GridElement _targetElement;

        protected string _gridDirection = string.Empty;

        public virtual void OnEnable()
        {
            try
            {
                switch (MyGrid.GridType)
                {
                    case GirdType.TwoDimensional:
                        {
                            if (MyGrid.GenerateDirection.x >= 0)
                            {
                                if (MyGrid.GenerateDirection.y >= 0)
                                {
                                    _gridDirection = "ru";
                                }
                                else
                                {
                                    _gridDirection = "rd";
                                }
                            }
                            else
                            {
                                if (MyGrid.GenerateDirection.y >= 0)
                                {
                                    _gridDirection = "lu";
                                }
                                else
                                {
                                    _gridDirection = "ld";
                                }
                            }
                        }
                        break;
                    case GirdType.ThreeDimensional:
                        {
                            throw new NotImplementedException();
                        }
                        break;
                }
            }
            catch (NotImplementedException)
            {
                Debug.LogError("A* 3D not implemented");
                this.enabled = false;
            }
        }

        public virtual void GetStaringElement(Vector3 position)
        {
            _startElement = FindClosestElement(position);
        }

        public virtual void GetTargetElement(Vector3 position)
        {
            _targetElement = FindClosestElement(position);
        }

        protected virtual GridElement FindClosestElement(Vector3 position)
        {
            GridElement closestElement = null;
            try
            {
                int xMax, yMax, zMax,xMin,yMin,zMin,i,j;
                i = xMax = MyGrid.GridSize.x;
                j = yMax = MyGrid.GridSize.y;
                zMax = MyGrid.GridSize.z;

                xMin = 0;
                yMin = 0;
                zMin = 0;

                switch (MyGrid.GridType)
                {
                    case GirdType.TwoDimensional:
                        {
                            
                            List<GridElement> leftPoints = new List<GridElement>();
                            List<GridElement> middlePoints = new List<GridElement>();
                            List<GridElement> rightPoints = new List<GridElement>();

                            leftPoints.Add(MyGrid.Elements[xMin, yMin, 0]);
                            leftPoints.Add(MyGrid.Elements[xMin, yMax / 2, 0]);
                            leftPoints.Add(MyGrid.Elements[xMin, yMax - 1, 0]);

                            middlePoints.Add(MyGrid.Elements[xMax / 2, yMin, 0]);
                            middlePoints.Add(MyGrid.Elements[xMax / 2, yMax / 2, 0]);
                            middlePoints.Add(MyGrid.Elements[xMax / 2, yMax - 1, 0]);

                            rightPoints.Add(MyGrid.Elements[xMax - 1, 0, 0]);
                            rightPoints.Add(MyGrid.Elements[xMax - 1, yMax / 2, 0]);
                            rightPoints.Add(MyGrid.Elements[xMax - 1, yMax - 1, 0]);

                            while (i > 1 || j > 1)
                            {
                                i /= 2;
                                j /= 2;
                                if (FragmentsContaisPosition(leftPoints[0],middlePoints[1], position))
                                {
                                    leftPoints.Clear();
                                    middlePoints.Clear();
                                    rightPoints.Clear();

                                    xMax /= 2;
                                    yMax /= 2;

                                    leftPoints.Add(MyGrid.Elements[xMin, yMin, 0]);
                                    leftPoints.Add(MyGrid.Elements[xMin, yMax / 2, 0]);
                                    leftPoints.Add(MyGrid.Elements[xMin, yMax - 1, 0]);

                                    middlePoints.Add(MyGrid.Elements[xMax / 2, yMin, 0]);
                                    middlePoints.Add(MyGrid.Elements[xMax / 2, yMax / 2, 0]);
                                    middlePoints.Add(MyGrid.Elements[xMax / 2, yMax - 1, 0]);

                                    rightPoints.Add(MyGrid.Elements[xMax - 1, 0, 0]);
                                    rightPoints.Add(MyGrid.Elements[xMax - 1, yMax / 2, 0]);
                                    rightPoints.Add(MyGrid.Elements[xMax - 1, yMax - 1, 0]);
                                    continue;
                                }
                                if (FragmentsContaisPosition(middlePoints[0], rightPoints[1], position))
                                {
                                    leftPoints.Clear();
                                    middlePoints.Clear();
                                    rightPoints.Clear();

                                    xMin = xMax / 2;
                                    yMax /= 2;

                                    leftPoints.Add(MyGrid.Elements[xMin, yMin, 0]);
                                    leftPoints.Add(MyGrid.Elements[xMin, yMax / 2, 0]);
                                    leftPoints.Add(MyGrid.Elements[xMin, yMax - 1, 0]);

                                    middlePoints.Add(MyGrid.Elements[xMax / 2, yMin, 0]);
                                    middlePoints.Add(MyGrid.Elements[xMax / 2, yMax / 2, 0]);
                                    middlePoints.Add(MyGrid.Elements[xMax / 2, yMax - 1, 0]);

                                    rightPoints.Add(MyGrid.Elements[xMax - 1, 0, 0]);
                                    rightPoints.Add(MyGrid.Elements[xMax - 1, yMax / 2, 0]);
                                    rightPoints.Add(MyGrid.Elements[xMax - 1, yMax - 1, 0]);
                                    continue;
                                }
                                if (FragmentsContaisPosition(leftPoints[1], middlePoints[2], position))
                                {
                                    leftPoints.Clear();
                                    middlePoints.Clear();
                                    rightPoints.Clear();

                                    xMax /= 2;
                                    yMin = yMax / 2;

                                    leftPoints.Add(MyGrid.Elements[xMin, yMin, 0]);
                                    leftPoints.Add(MyGrid.Elements[xMin, yMax / 2, 0]);
                                    leftPoints.Add(MyGrid.Elements[xMin, yMax - 1, 0]);

                                    middlePoints.Add(MyGrid.Elements[xMax / 2, yMin, 0]);
                                    middlePoints.Add(MyGrid.Elements[xMax / 2, yMax / 2, 0]);
                                    middlePoints.Add(MyGrid.Elements[xMax / 2, yMax - 1, 0]);

                                    rightPoints.Add(MyGrid.Elements[xMax - 1, 0, 0]);
                                    rightPoints.Add(MyGrid.Elements[xMax - 1, yMax / 2, 0]);
                                    rightPoints.Add(MyGrid.Elements[xMax - 1, yMax - 1, 0]);
                                    continue;
                                }
                                if (FragmentsContaisPosition(middlePoints[1], rightPoints[2], position))
                                {
                                    leftPoints.Clear();
                                    middlePoints.Clear();
                                    rightPoints.Clear();

                                    xMin = xMax / 2;
                                    yMin = yMax / 2;

                                    leftPoints.Add(MyGrid.Elements[xMin, yMin, 0]);
                                    leftPoints.Add(MyGrid.Elements[xMin, yMax / 2, 0]);
                                    leftPoints.Add(MyGrid.Elements[xMin, yMax - 1, 0]);

                                    middlePoints.Add(MyGrid.Elements[xMax / 2, yMin, 0]);
                                    middlePoints.Add(MyGrid.Elements[xMax / 2, yMax / 2, 0]);
                                    middlePoints.Add(MyGrid.Elements[xMax / 2, yMax - 1, 0]);

                                    rightPoints.Add(MyGrid.Elements[xMax - 1, 0, 0]);
                                    rightPoints.Add(MyGrid.Elements[xMax - 1, yMax / 2, 0]);
                                    rightPoints.Add(MyGrid.Elements[xMax - 1, yMax - 1, 0]);
                                    continue;
                                }
                            }

                            float minDist, curDist;
                            minDist= Vector3.Distance(leftPoints[0].transform.position, position);
                            closestElement = leftPoints[0];
                            curDist = Vector3.Distance(rightPoints[0].transform.position, position);
                            if (minDist > curDist)
                            {
                                minDist = curDist;
                                closestElement = rightPoints[0];
                            }
                            curDist = Vector3.Distance(leftPoints[2].transform.position, position);
                            if (minDist > curDist)
                            {
                                minDist = curDist;
                                closestElement = leftPoints[2];
                            }
                            curDist = Vector3.Distance(rightPoints[2].transform.position, position);
                            if (minDist > curDist)
                            {
                                minDist = curDist;
                                closestElement = rightPoints[2];
                            }
                            return closestElement;
                        }
                    case GirdType.ThreeDimensional:
                        {
                            Debug.LogError("A* 3D not implemented");
                            throw new NotImplementedException();
                        }
                        break;
                }
            }
            catch (NotImplementedException)
            {
                this.enabled = false;
                return null;
            }
            return null;
        }

        protected virtual bool FragmentsContaisPosition(GridElement leftUp, GridElement rightDown, Vector3 position)
        {
            switch (_gridDirection)
            {
                case "ru":
                    if (position.x > leftUp.transform.position.x && position.x < rightDown.transform.position.x
                        && position.y > rightDown.transform.position.y && position.y < leftUp.transform.position.y)
                        return true;
                    else
                        return false;
                case "rd":
                    if (position.x > leftUp.transform.position.x && position.x < rightDown.transform.position.x
                        && position.y > leftUp.transform.position.y && position.y < rightDown.transform.position.y)
                        return true;
                    else
                        return false;
                case "lu":
                    if (position.x > rightDown.transform.position.x && position.x < leftUp.transform.position.x
                        && position.y > rightDown.transform.position.y && position.y < leftUp.transform.position.y)
                        return true;
                    else
                        return false;
                case "ld":
                    if (position.x > rightDown.transform.position.x && position.x < leftUp.transform.position.x
                         && position.y > leftUp.transform.position.y && position.y < rightDown.transform.position.y)
                        return true;
                    else
                        return false;
            }
            return false;
        }

        public virtual void Process(ref List<GridElement> path)
        {
            bool complete = false;
            if (_startElement != null)
            {
                _openElements.Add(_startElement);
                _currentElement = _startElement;
                _currentElement.CalculateHeuristic(_targetElement);
            }
            while (_openElements.Count > 0)
            {
                int openSize = _openElements.Count;
                int lowestCost = Int32.MaxValue;
                for (int i = 0; i < openSize; ++i)
                {
                    if (_openElements[i].TotalFieldMoveCost < lowestCost)
                    {
                        _currentElement = _openElements[i];
                        lowestCost = _currentElement.TotalFieldMoveCost;
                    }
                }
                _openElements.Remove(_currentElement);
                _closedElements.Add(_currentElement);

                if (_currentElement.ElementIndex == _targetElement.ElementIndex)
                {
                    complete = true;
                    break;
                }
                CheckNeighbours();
            }

            if (complete)
            {
                GridElement step = _currentElement;
                path.Clear();
                do
                {
                    path.Add(step);
                    step = step.PathParentField;
                } while (step != null);

                path.Reverse();
            }
        }

        protected virtual void CheckNeighbours()
        {
            IntVector3 index = _currentElement.ElementIndex;
            index.x -= 1;
            CheckNeighbourHelper(index);

            index = _currentElement.ElementIndex;
            index.x += 1;
            CheckNeighbourHelper(index);

            index = _currentElement.ElementIndex;
            index.y -= 1;
            CheckNeighbourHelper(index);

            index = _currentElement.ElementIndex;
            index.y += 1;
            CheckNeighbourHelper(index);

            index = _currentElement.ElementIndex;
            index.x -= 1;
            index.y -= 1;
            CheckNeighbourHelper(index);

            index = _currentElement.ElementIndex;
            index.x -= 1;
            index.y += 1;
            CheckNeighbourHelper(index);

            index = _currentElement.ElementIndex;
            index.x += 1;
            index.y -= 1;
            CheckNeighbourHelper(index);

            index = _currentElement.ElementIndex;
            index.x += 1;
            index.y += 1;
            CheckNeighbourHelper(index);
        }

        protected virtual void CheckNeighbourHelper( IntVector3 index)
        {
            if (IsIndexInGrid(index))
            {
                GridElement element = MyGrid.Elements[index.x, index.y, index.z];
                if (element.Walkable)
                {
                    if (!_openElements.Contains(element))
                    {
                        element.PathParentField = _currentElement;
                        element.CalculateHeuristic(_targetElement);
                        _openElements.Add(element);
                    }
                    else
                    {
                        if (GridElement.CheckMoveDistance(element) >
                            GridElement.CheckMoveDistance(element, _currentElement))
                        {
                            element.PathParentField = _currentElement;
                            element.CalculateHeuristic(_targetElement);
                        }
                    }
                }
            }
        }

        protected bool IsIndexInGrid(IntVector3 index)
        {
            if (index.x >= 0 && index.x < MyGrid.GridSize.x &&
                index.y >= 0 && index.y < MyGrid.GridSize.y)
                return true;
            else
                return false;
        }
    }
}
