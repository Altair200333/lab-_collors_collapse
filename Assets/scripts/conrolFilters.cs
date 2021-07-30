using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class conrolFilters : MonoBehaviour
{
    public List<Renderer> renderers;
    public Slider aToB;
    public Slider bToA;

    Texture2D buffer;

    public Renderer preview;
    private RenderTexture tex;
    private int size = 1024;
    void Start()
    {
        tex = new RenderTexture(size, size, 16, RenderTextureFormat.ARGB32);
        buffer = new Texture2D(tex.width, tex.height);

        aToB.onValueChanged.AddListener(onChange);
        bToA.onValueChanged.AddListener(onChange);
    }
    
    private bool called = false;

    void onChange(float value)
    {
        foreach (var renderer in renderers)
        {
            renderer.material.SetFloat("a_to_b", aToB.value);
            renderer.material.SetFloat("b_to_a", bToA.value);
        }

        toTexture2D(tex, buffer);
    }

    void Update()
    {
        Blit(tex, renderers[0].material);//color mixer

        if(!called)
        {
            onChange(0);
            called = true;
        }

        preview.material.mainTexture = buffer;
    }
    void toTexture2D(RenderTexture rTex, Texture2D buf)
    {
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        buf.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        buf.Apply();
    }
    void Blit(RenderTexture destination, Material mat)
    {
        RenderTexture.active = destination;
        GL.PushMatrix();
        GL.LoadOrtho();
        GL.invertCulling = true;
        mat.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 0.0f);
        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 0.0f);
        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 0.0f);
        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);
        GL.End();
        GL.invertCulling = false;
        GL.PopMatrix();
    }
    
}
