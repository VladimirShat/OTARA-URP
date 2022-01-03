using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Outliner : MonoBehaviour
{
    [Header("Outline Settings")]
    [Tooltip("Should the outline be solid or fade out")]
    public bool solidOutline = false;

    [Tooltip("Strength override multiplier")]
    [Range(0, 10)]
    public float outlineStrength = 1f;

    [Tooltip("Which layers should this outline system display on")]
    public LayerMask outlineLayer;

    [Tooltip("What color should the outline be")]
    public Color outlineColor;

    [Tooltip("How many times should the render be downsampled")]
    [Range(0, 4)]
    public int downsampleAmount = 2;

    [Tooltip("How big should the outline be")]
    [Range(0.0f, 10.0f)]
    public float outlineSize = 1.5f;

    [Tooltip("How many times should the blur be performed")]
    [Range(1, 10)]
    public int outlineIterations = 2;

    Camera sourceCam;
    Camera outlineCam;

    RenderTexture renTexInput;
    RenderTexture renTexRecolor;
    RenderTexture renTexDownsample;
    RenderTexture renTexBlur;
    RenderTexture renTexOut;

    Material blurMaterial;
    Material outlineMaterial;
    Material overlayMaterial;
    Vector2 prevSize;

    static HashSet<GameObject> highlightedObjects;

    static Outliner()
    {
        highlightedObjects = new HashSet<GameObject>();
    }

    public static void RegisterObject(GameObject _go)
    {
        highlightedObjects.Add(_go);
    }

    public static void UnregisterObject(GameObject _go)
    {
        highlightedObjects.Remove(_go);
    }

    void PrepareRenderTextures()
    {
        if (sourceCam == null) return;

        int w = Screen.width;
        int h = Screen.height;
        renTexInput = new RenderTexture(w, h, 1);
        renTexInput.Create();
        renTexDownsample = new RenderTexture(w, h, 1);
        renTexDownsample.Create();
        renTexRecolor = new RenderTexture(w, h, 1);
        renTexRecolor.Create();
        renTexOut = new RenderTexture(w, h, 1);
        renTexOut.Create();
        renTexBlur = new RenderTexture(w, h, 1);
        renTexBlur.Create();

        if (outlineCam == null)
        {
            GameObject camGO = new GameObject("Outline Camera");
            camGO.transform.SetParent(sourceCam.transform);
            camGO.transform.SetPositionAndRotation(sourceCam.transform.position, sourceCam.transform.rotation);
            outlineCam = camGO.AddComponent<Camera>();
            outlineCam.enabled = false;
        }

        outlineCam.CopyFrom(sourceCam);
        outlineCam.cullingMask = outlineLayer.value;
        outlineCam.targetTexture = renTexInput;
        outlineCam.clearFlags = CameraClearFlags.SolidColor;
        outlineCam.backgroundColor = new Color(1f, 0f, 1f, 1f);
    }

    void OnEnable()
    {
        sourceCam = GetComponent<Camera>();

        Shader blurShader = Shader.Find("Hidden/FastBlur");
        if (blurShader == null)
        {
            enabled = false;
            return;
        }
        blurMaterial = new Material(blurShader);

        Shader outlineShader = Shader.Find("Hidden/ScreenSpaceOutlineShader");
        if (outlineShader == null)
        {
            enabled = false;
            return;
        }
        outlineMaterial = new Material(outlineShader);

        Shader overlayShader = Shader.Find("Unlit/Transparent");
        if (overlayShader == null)
        {
            enabled = false;
            return;
        }
        overlayMaterial = new Material(overlayShader);

        PrepareRenderTextures();

        overlayMaterial.mainTexture = renTexOut;
    }

    void OnDisable()
    {
        highlightedObjects.Clear();
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (highlightedObjects.Count != 0)
        {
            outlineCam.CopyFrom(sourceCam);
            outlineCam.cullingMask = outlineLayer.value;
            outlineCam.targetTexture = renTexInput;
            outlineCam.clearFlags = CameraClearFlags.SolidColor;
            outlineCam.backgroundColor = new Color(1f, 0f, 1f, 1f);

            outlineMaterial.SetColor("_OutlineCol", outlineColor);
            outlineMaterial.SetFloat("_GradientStrengthModifier", outlineStrength);

            outlineCam.Render();

            float widthMod = 1.0f / (1.0f * (1 << downsampleAmount));
            blurMaterial.SetVector("_Parameter", new Vector4(outlineSize * widthMod, -outlineSize * widthMod, 0.0f, 0.0f));

            renTexRecolor.DiscardContents();
            Graphics.Blit(renTexInput, renTexRecolor, outlineMaterial, 0);

            renTexDownsample.DiscardContents();
            Graphics.Blit(renTexRecolor, renTexDownsample, blurMaterial, 0);

            for (int i = 0; i < outlineIterations; i++)
            {
                float iterationOffs = (i * 1.0f);
                blurMaterial.SetVector("_Parameter", new Vector4(outlineSize * widthMod + iterationOffs, -outlineSize * widthMod - iterationOffs, 0.0f, 0.0f));

                renTexBlur.DiscardContents();
                Graphics.Blit(renTexDownsample, renTexBlur, blurMaterial, 1);
                renTexDownsample.DiscardContents();
                Graphics.Blit(renTexBlur, renTexDownsample, blurMaterial, 2);
            }
            outlineMaterial.SetFloat("_Solid", solidOutline ? 1f : 0f);
            outlineMaterial.SetTexture("_BlurTex", renTexDownsample);
            renTexOut.DiscardContents();
            Graphics.Blit(renTexRecolor, renTexOut, outlineMaterial, 1);

            Graphics.Blit(renTexOut, source, overlayMaterial);
        }

        Graphics.Blit(source, destination);
    }
}
