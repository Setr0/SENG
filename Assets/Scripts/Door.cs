using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] Door nextDoor;
    [SerializeField] Sprite doorClosed;
    [SerializeField] Sprite doorOpened;
    Transform player;
    Image commadImage;
    bool isPlayerNear;
    public bool isOpened;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        commadImage = GameObject.FindGameObjectWithTag("FKey").GetComponent<Image>();
        isPlayerNear = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isPlayerNear && isOpened)
        {
            if (nextDoor.isOpened) player.position = nextDoor.transform.position;
        }
    }

    public void HandleDoor()
    {
        if (!isOpened)
        {
            spriteRenderer.sprite = doorOpened;
            isOpened = true;
        }
        else
        {
            spriteRenderer.sprite = doorClosed;
            isOpened = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (isOpened) commadImage.enabled = true;
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
