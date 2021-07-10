using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateTexture : MonoBehaviour
{
    // Scroll main texture based on time

    public float scrollSpeed;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    private void FixedUpdate()
    {
        float offset = Time.time * scrollSpeed;
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }

}
