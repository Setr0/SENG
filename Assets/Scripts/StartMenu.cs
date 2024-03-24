using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject levelsPanel;

    void Update()
    {
        if (Input.anyKey)
        {
            startMenu.SetActive(false);
            levelsPanel.SetActive(true);
        }
    }
}
