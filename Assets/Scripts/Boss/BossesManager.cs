using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossesManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bossNameText;
    public Slider healthSlider;
    [SerializeField] GameObject wall;
    [SerializeField] GameObject enemies;
    [SerializeField] GameObject platforms;
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] string[] text;
    [SerializeField] string nextLevel;
    PlayerController player;
    [HideInInspector] public bool isActive;
    bool isLoadingNextLevel;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        isActive = false;
        isLoadingNextLevel = false;

        bossNameText.text = "";

        float totalHealth = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i != transform.childCount - 1)
            {
                bossNameText.text += transform.GetChild(i).GetComponent<Boss>().bossName + ", ";
            }
            else
            {
                bossNameText.text += transform.GetChild(i).GetComponent<Boss>().bossName;
            }

            totalHealth += transform.GetChild(i).GetComponent<BossLife>().health;
        }

        healthSlider.maxValue = totalHealth;
        healthSlider.value = totalHealth;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > 10 || isLoadingNextLevel) return;
        else
        {
            if (!isActive)
            {
                isActive = true;
                healthSlider.transform.parent.gameObject.SetActive(true);
                Destroy(enemies);
                Destroy(platforms);
                wall.SetActive(true);
            }
        }

        if (healthSlider.value == 0)
        {
            if (!DialogueManager.isDialogueActive && !DialogueManager.isDialogueCompleted)
            {
                healthSlider.transform.parent.gameObject.SetActive(false);
                dialogueManager.npcName = bossNameText.text;
                dialogueManager.text = text;
                dialogueManager.gameObject.SetActive(true);

                return;
            }

            if (DialogueManager.isDialogueCompleted)
            {
                StartCoroutine(LoadNextLevel());
                isLoadingNextLevel = true;
            }
        }
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nextLevel);
    }
}
