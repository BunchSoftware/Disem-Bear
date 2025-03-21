using UnityEngine;

namespace Game.Environment
{
    public class MoveAnimation : MonoBehaviour
    {
        [Header("Куда полетит объект")]
        public bool needPosition = false;
        private Vector3 endCoords = new();
        public Vector3 startCoords = new();
        [Header("Как повернеться объект")]
        public bool needRotate = false;
        private Quaternion endRotate = new();
        public Quaternion startRotate = new();
        private float timer = 0f;
        public float TimeAnimation = 1f;
        private bool MoveOn = false;
        private bool FinallyMove = false;

        public void StartMove()
        {
            endCoords = startCoords;
            startCoords = transform.localPosition;
            endRotate = startRotate;
            startRotate = transform.localRotation;
            timer = 0f;
            MoveOn = true;
        }
        public void EndMove()
        {
            endCoords = startCoords;
            startCoords = transform.localPosition;
            endRotate = startRotate;
            startRotate = transform.localRotation;
            timer = 0f;
            MoveOn = true;
        }

        private void Update()
        {
            if (MoveOn)
            {
                if (timer <= TimeAnimation)
                {
                    FinallyMove = true;
                    if (needPosition)
                        transform.localPosition = Vector3.Lerp(startCoords, endCoords, timer / TimeAnimation);
                    if (needRotate)
                        transform.localRotation = Quaternion.Lerp(startRotate, endRotate, timer / TimeAnimation);
                    timer += Time.deltaTime;
                }
                else if (FinallyMove)
                {
                    if (needPosition)
                        transform.localPosition = endCoords;
                    if (needRotate)
                        transform.localRotation = endRotate;
                    FinallyMove = false;
                }
                else
                    MoveOn = false;
            }
        }
    }
}
