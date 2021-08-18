using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Rendering : MonoBehaviour
{
    public Shader display;
    public Transform domain;
    public int NumberOfStep = 5;
    public float threashold = 0.3f;
    public Physics physics;
    Material material;
    // Start is called before the first frame update
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material == null) material = new Material(display);

        //set the bound of the fluids domain for rendering
        material.SetVector("minBounds", domain.position - domain.localScale / 2);
        material.SetVector("maxBounds", domain.position + domain.localScale / 2);
        material.SetInt("nbStep", NumberOfStep);
        material.SetFloat("threashold", Mathf.Max(0.0f,threashold));
        material.SetInt("unitPerSide", physics.GRID_COUNT);
        material.SetBuffer("smokeDensity", physics.GPUdomain.smokeDensity);

        Graphics.Blit(source, destination, material);
    }
}
