using System.Collections;
using System.Collections.Generic;
using Game.Environment;
using UnityEngine;

namespace Game.Environment.LMixTable
{
    public class PutCellInMix : MonoBehaviour, ILeftMouseDownClickable
    {
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform mixTransform;

        private Transform cellTransform;

        private float timeFromDownClickToUpClick = 0f;
        public float timeCellMoveToMix = 0.5f;
        public void OnMouseLeftClickDownObject()
        {
            cellTransform = Instantiate(cellPrefab, transform.position, transform.rotation, transform.parent).transform;
            timeFromDownClickToUpClick = 0f;
        }

        public void OnMouseLeftClickDownOtherObject()
        {
            throw new System.NotImplementedException();
        }
        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (timeFromDownClickToUpClick < 0.1f && cellTransform != null && cellTransform.TryGetComponent(out MovePointToPoint movePointToPoint))
                {
                    movePointToPoint.point2 = mixTransform;
                    movePointToPoint.StartMoveTo(timeCellMoveToMix);
                }
                cellTransform = null;
            }
            if (cellTransform != null)
            {
                timeFromDownClickToUpClick += Time.deltaTime;
                
            }
        }
    }
}
