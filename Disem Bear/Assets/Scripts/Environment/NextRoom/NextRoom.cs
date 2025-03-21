using Game.LPlayer;
using System.Collections;
using UnityEngine;

namespace Game.Environment
{
    public class NextRoom : MonoBehaviour
    {
        private PlayerMouseMove playerMouseMove;
        //private MoveCameraAnimation moveCameraAnimation;
        private GameObject invisibleWallBetweenRooms;

        [Header("���������� ���� ������ ���� �����")]
        [SerializeField] private Vector3 positionPlayer;
        [SerializeField] private Vector3 positionCamera;
        [SerializeField] private float timeAnimationCamera = 1f;

        [SerializeField] private BoxCollider oppositeArrow;

        public void Init(PlayerMouseMove playerMouseMove, GameObject invisibleWallBetweenRooms)
        {
            this.playerMouseMove = playerMouseMove;
            this.invisibleWallBetweenRooms = invisibleWallBetweenRooms;
            //moveCameraAnimation = Camera.main.GetComponent<MoveCameraAnimation>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                invisibleWallBetweenRooms.SetActive(false);
                GetComponent<BoxCollider>().enabled = false;
                StartCoroutine(WaitBakeMesh(0.01f));
                playerMouseMove.StopPlayerMove();

                //moveCameraAnimation.startCoords = positionCamera;
                //moveCameraAnimation.needPosition = true;
                //moveCameraAnimation.needRotate = false;
                //moveCameraAnimation.TimeAnimation = timeAnimationCamera;
                //moveCameraAnimation.StartMove();

                StartCoroutine(WaitAnimCamera(timeAnimationCamera));
            }
        }

        private IEnumerator WaitAnimCamera(float f)
        {
            yield return new WaitForSeconds(f);
            playerMouseMove.ReturnPlayerMove();
            invisibleWallBetweenRooms.SetActive(true);
            oppositeArrow.enabled = true;
        }

        private IEnumerator WaitBakeMesh(float f)
        {
            yield return new WaitForSeconds(f);
            playerMouseMove.MovePlayer(positionPlayer);
        }
    }
}
