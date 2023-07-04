using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraUnderwaterEffect : MonoBehaviour
{
    [SerializeField] Shader shader;
    [SerializeField] LayerMask waterLayers;

    [Header("Depth Effect")]
    public Color depthColor = new Color(0, 0.42f, 0.87f);
    public float depthStart = -12, depthEnd = 98;

    Camera cam;
    Material material;
    bool inWater;

    void Start()
    {
        cam = GetComponent<Camera>();

        //make our camera send depth information (ie how far a pixel is from the screen)
        //to the shader as well
        cam.depthTextureMode = DepthTextureMode.Depth;

        //create a material using the shader
        if(shader) material = new Material(shader);
    }
    private void FixedUpdate()
    {
        Collider[] c = Physics.OverlapSphere(transform.position, 0.01f, waterLayers);
    }
    //automatically finds and assigned inspector variables so that the script can be used automatically when attached to an object
    private void Reset()
    {
        //look for the shader we made
        Shader[] shaders = Resources.FindObjectsOfTypeAll<Shader>();
        foreach(Shader s in shaders)
        {
            if (s.name.Contains(this.GetType().Name))
            {
                shader = s;
                return;
            }
        }
    }

    //this is where the image effect is applied
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material && inWater)
        {
            material.SetColor("_DepthColor", depthColor);
            material.SetFloat("_DepthStart", depthStart);
            material.SetFloat("_DepthEnd", depthEnd);

            //apply to the image using blit
            Graphics.Blit(source, destination, material);

        }
        else
        {
            Graphics.Blit (source, destination);
        }


    }
}
