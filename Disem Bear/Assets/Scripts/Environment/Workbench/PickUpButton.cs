using UnityEngine;


namespace Game.Environment.LMixTable
{
    public class PickUpButton : MonoBehaviour
    {
        [SerializeField] private Material deactiveMaterial;
        [SerializeField] private MeshRenderer meshRendererBase;

        private MeshRenderer meshRenderer;
        private Material activeMaterial;
        private Workbench workbench;

        private bool isActive = true;

        public void Init(Workbench workbench)
        {
            this.workbench = workbench;
            meshRenderer = GetComponent<MeshRenderer>();
            activeMaterial = meshRenderer.material;
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
            if(isActive)
            {
                //workbench.MixIngradients();
                transform.parent.gameObject.GetComponent<Animator>().Play("ButtonPress");
            }
        }
    }
}
