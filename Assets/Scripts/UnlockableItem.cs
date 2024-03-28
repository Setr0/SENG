using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class unlockableItem : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] string[] text;
    [SerializeField] string nextLevel;
    bool isPickedUp;

    void Start()
    {
        isPickedUp = false;
    }

    void Update()
    {
        if (DialogueManager.isDialogueCompleted && isPickedUp)
        {
            SceneManager.LoadScene(nextLevel);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueManager.npcName = "Narrator";
            dialogueManager.text = text;
            dialogueManager.gameObject.SetActive(true);

            isPickedUp = true;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
