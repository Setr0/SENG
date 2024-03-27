using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] Door door;
    [SerializeField] Sprite leverLeft;
    [SerializeField] Sprite leverRight;
    bool isPlayerNear;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            isPlayerNear = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isPlayerNear = false;
    }
}
