using External.DI;
using UnityEngine;

namespace Game.Environment
{
    public class ScaleChooseObject : MonoBehaviour, IMouseOver
    {
        public bool on = true;
        public float coefficient = 1.08f;

        private Vector3 maxScale;
        private Vector3 minScale;
        private bool increaseScale = false;
        private AudioClip audioClip;
        private GameBootstrap gameBootstrap;
        private float timerForSound = 0f;

        private void Awake()
        {
            gameBootstrap = GameObject.Find("Bootstrap").GetComponent<GameBootstrap>();
            audioClip = gameBootstrap.ScaleChooseObjectSound;
            minScale = transform.localScale;
            maxScale = new Vector3(minScale.x * coefficient, minScale.y * coefficient, minScale.z * coefficient);
        }


        private void Update()
        {
            if (timerForSound < 0.3f)
            {
                timerForSound += Time.deltaTime;
            }
            if (on && increaseScale && transform.localScale.x < maxScale.x)
            {
                Vector3 scale = new Vector3(transform.localScale.x + (Time.deltaTime * minScale.x), transform.localScale.y + (Time.deltaTime * minScale.y), transform.localScale.z + (Time.deltaTime * minScale.z));
                if (scale.x > maxScale.x)
                {
                    scale = maxScale;
                }
                transform.localScale = scale;
            }
            else if (transform.localScale.x > minScale.x && (!increaseScale || !on))
            {
                Vector3 scale = new Vector3(transform.localScale.x - (Time.deltaTime * minScale.x), transform.localScale.y - (Time.deltaTime * minScale.y), transform.localScale.z - (Time.deltaTime * minScale.z));
                if (scale.x < minScale.x)
                {
                    scale = minScale;
                }
                transform.localScale = scale;
            }
        }

        public void RemoveComponent()
        {
            transform.localScale = minScale;
            Destroy(this);
        }

        public void OnMouseEnterObject()
        {
            if (on && timerForSound >= 0.3f)
            {
                timerForSound = 0f;
                gameBootstrap.OnPlayOneShotSound(audioClip);
            }
            increaseScale = true;
        }

        public void OnMouseExitObject()
        {
            if (on && timerForSound >= 0.3f)
            {
                timerForSound = 0f;
                gameBootstrap.OnPlayOneShotSound(audioClip);
            }
            increaseScale = false;
        }
    }
}
