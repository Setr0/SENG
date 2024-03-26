using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossesManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bossNameText;
    [SerializeField] Slider healthSlider;
    [SerializeField] GameObject wall;
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] string[] text;
    [SerializeField] string nextLevel;
    List<BossLife> bossLives;
    PlayerController player;
    [HideInInspector] public bool isActive;
    bool isLoadingNextLevel;

    void Start()
    {
        bossLives = new List<BossLife>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        isActive = false;
        isLoadingNextLevel = false;

        bossNameText.text = "";

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

            bossLives.Add(transform.GetChild(i).GetComponent<BossLife>());
        }

        healthSlider.maxValue = GetTotalHealth();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > 10 || isLoadingNextLevel) return;
        else
        {
            isActive = true;
            healthSlider.transform.parent.gameObject.SetActive(true);
            wall.SetActive(true);
        }

        healthSlider.value = GetTotalHealth();

        if (GetTotalHealth() == 0)
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

    float GetTotalHealth()
    {
        float totalHealth = 0;

        foreach (BossLife bossLife in bossLives)
        {
            totalHealth += bossLife.health;
        }

        return totalHealth;
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nextLevel);
    }
}
