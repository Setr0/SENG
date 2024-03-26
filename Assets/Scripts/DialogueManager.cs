using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI npcNameText;
    [SerializeField] TextMeshProUGUI dialogueText;
    public string npcName;
    public string[] text;
    int textIndex;
    bool isWritingText;
    public static bool isDialogueActive;
    public static bool isDialogueCompleted;

    void OnEnable()
    {
        npcNameText.text = npcName;
        textIndex = 0;
        isWritingText = false;
        StartCoroutine(WriteText(text[textIndex]));
        isDialogueActive = true;
        isDialogueCompleted = false;
        PlayerController.canMove = false;
    }

    void OnDisable()
    {
        isDialogueActive = false;
        isDialogueCompleted = true;
        PlayerController.canMove = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isWritingText)
            {
                StopAllCoroutines();
                dialogueText.text = text[textIndex];
                isWritingText = false;
                return;
            }

            if (textIndex < text.Length - 1)
            {
                textIndex++;
                StartCoroutine(WriteText(text[textIndex]));
            }
            else
            {
                dialogueText.transform.parent.parent.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator WriteText(string _text)
    {
        dialogueText.text = "";
        isWritingText = true;

        for (int i = 0; i < _text.Length; i++)
        {
            yield return new WaitForSeconds(0.0625f);
            dialogueText.text += _text[i];
        }

        isWritingText = false;
    }
}
