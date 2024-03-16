using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    [SerializeField] GameObject oldPanel;
    [SerializeField] GameObject newPanel;

    public void OpenPanel()
    {
        oldPanel.SetActive(false);
        newPanel.SetActive(true);
    }

    void DestroyAnimator()
    {
        Destroy(GetComponent<Animator>());
    }
}
