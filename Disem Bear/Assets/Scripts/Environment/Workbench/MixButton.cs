using External.DI;
using UnityEngine;
using UnityEngine.Events;


namespace Game.Environment.LMixTable
{

    public class MixButton : MonoBehaviour
    {
        [SerializeField] private Material deactiveMaterial;
        [SerializeField] private MeshRenderer meshRendererBase;
        [SerializeField] private AudioClip audioClip;
        private float timerPressButton = 25f / 60f;
        private float timePressButton = 25f / 60f;

        private MeshRenderer meshRenderer;
        private Material activeMaterial;
        private Workbench workbench;
        private GameBootstrap gameBootstrap;
        public UnityEvent pressButton;

        private bool isActive = true;

        public void Init(Workbench workbench, GameBootstrap gameBootstrap)
        {
            this.gameBootstrap = gameBootstrap;
            this.workbench = workbench;
            meshRenderer = GetComponent<MeshRenderer>();
            activeMaterial = meshRenderer.material;
        }
        public void OnUpdate(float deltaTime)
        {
            if (timerPressButton < timePressButton)
            {
                timerPressButton += deltaTime;
            }
        }
        public void SetActive(bool isActive)
        {
            if (isActive)
            {
                meshRenderer.material = activeMaterial;
                meshRendererBase.material = activeMaterial;
            }
            else
            {
                meshRenderer.material = deactiveMaterial;
                meshRendererBase.material = deactiveMaterial;
            }

            this.isActive = isActive;
        }

        private void OnMouseDown()
        {
            if (isActive)
            {
                if (timerPressButton >= timePressButton)
                {
                    gameBootstrap.OnPlayOneShotSound(audioClip);
                    timerPressButton = 0;
                }
                workbench.MixIngradients();
                transform.parent.gameObject.GetComponent<Animator>().Play("ButtonPress");
                pressButton?.Invoke();
            }
        }
    }
}
