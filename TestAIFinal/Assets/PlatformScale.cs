using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScale : MonoBehaviour
    {
    public float minSize;
    public float shrinkFactor;
    public float waitTime;

    void Start()
    {
        StartCoroutine(Scale());
    }

    IEnumerator Scale()
    {
        float timer = 0;

        while (true) //"alive or dead"
        {
            while (minSize < transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale -= new Vector3(1, 0, 1) * Time.deltaTime * shrinkFactor;
                yield return null;
            }
            // reset the timer

            yield return new WaitForSeconds(waitTime);

            timer = 0;
            while (1 > transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale -= new Vector3(1, 0, 1) * Time.deltaTime * shrinkFactor;
                yield return null;
            }

            timer = 0;
            yield return new WaitForSeconds(waitTime);
        }
    }
}