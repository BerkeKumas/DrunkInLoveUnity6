using UnityEngine;

public class Drunk : MonoBehaviour
{
    [SerializeField] private Material shaderMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, shaderMaterial);
    }
}
