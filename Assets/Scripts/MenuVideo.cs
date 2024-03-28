using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuVideo : MonoBehaviour
{
    Image image;
    [SerializeField] Sprite[] frames;
    int frameIndex;

    void Start()
    {
        image = GetComponent<Image>();
        frameIndex = 0;
        InvokeRepeating(nameof(UpdateFrame), 1f / 10f, 1f / 10f);
    }

    void UpdateFrame()
    {
        frameIndex = (frameIndex + 1) % frames.Length;
        image.sprite = frames[frameIndex];
    }
}
