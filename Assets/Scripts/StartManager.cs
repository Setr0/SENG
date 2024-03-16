using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] CinemachineVirtualCamera mainCamera;
    [SerializeField] Camera[] cameras;


    void Start()
    {
        if (!levelText.transform.parent.gameObject.activeSelf)
        {
            PlayerController.canMove = true;
            return;
        }

        mainCamera.m_Lens.OrthographicSize -= 1.5f;

        foreach (Camera camera in cameras)
        {
            camera.orthographicSize -= 1.5f;
        }

        PlayerController.canMove = false;

        StartCoroutine(DecreaseOpacity());
    }

    IEnumerator DecreaseOpacity()
    {
        float opacity = 1;

        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);
            opacity -= 0.1f;
            levelText.color = new Color(1, 1, 1, opacity);
        }

        levelText.transform.parent.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.25f);

        StartCoroutine(ZoomOut());
    }

    IEnumerator ZoomOut()
    {
        for (int i = 0; i < 15; i++)
        {
            yield return new WaitForSeconds(0.02f);
            mainCamera.m_Lens.OrthographicSize += 0.1f;

            foreach (Camera camera in cameras)
            {
                camera.orthographicSize += 0.1f;
            }
        }

        PlayerController.canMove = true;
    }
}
