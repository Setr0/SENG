using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] Door door;
    [SerializeField] Sprite leverLeft;
    [SerializeField] Sprite leverRight;
    Image commadImage;
    bool isPlayerNear;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        commadImage = GameObject.FindGameObjectWithTag("FKey").GetComponent<Image>();
        isPlayerNear = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isPlayerNear)
        {
            if (!door.isOpened) spriteRenderer.sprite = leverRight;
            else spriteRenderer.sprite = leverLeft;

            door.HandleDoor();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
            commadImage.enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            commadImage.enabled = false;
        }
    }
}
