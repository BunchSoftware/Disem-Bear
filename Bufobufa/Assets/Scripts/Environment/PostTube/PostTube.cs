using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment.PostTube
{
    public class PostTube : MonoBehaviour
    {
        [SerializeField] private Vector3 ejectionPosition;
        [SerializeField] private GameObject exitPoint;
        [SerializeField] private ParticleSystem particleSystem;

        public void ObjectFall(MoveAnimation prefab)
        {
            GameObject currentFallObject = Instantiate(prefab.gameObject, ejectionPosition, prefab.transform.rotation);
            currentFallObject.GetComponent<GetItemFromTable>().isTube = true;

            MoveAnimation moveAnimation = currentFallObject.GetComponent<MoveAnimation>();
            moveAnimation.needPosition = true;
            moveAnimation.TimeAnimation = 1f;
            moveAnimation.startCoords = exitPoint.transform.position;
            moveAnimation.StartMove();

            StartCoroutine(ParticleFall(moveAnimation.TimeAnimation));
        }
        IEnumerator ParticleFall(float t)
        {
            yield return new WaitForSeconds(t);
            particleSystem.Play();
            yield return new WaitForSeconds(0.2f);
            particleSystem.Stop();
        }
    }
}
