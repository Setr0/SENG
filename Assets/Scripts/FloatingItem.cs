using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [SerializeField] float speed = 0.25f;
    [SerializeField] float distanceY = 0.25f;
    float startPositionY;
    float endPositionY;
    bool hasChangedSpeed;

    void Start()
    {
        startPositionY = transform.position.y;
        endPositionY = transform.position.y + distanceY;
        hasChangedSpeed = false;
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        if ((transform.position.y > endPositionY || transform.position.y < startPositionY) && !hasChangedSpeed)
        {
            speed *= -1;
            hasChangedSpeed = true;
        }

        if (transform.position.y < endPositionY && transform.position.y > startPositionY && hasChangedSpeed)
            hasChangedSpeed = false;
    }
}
