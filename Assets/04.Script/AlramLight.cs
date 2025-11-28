using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlramLight : MonoBehaviour
{

    float goalInten = 1f;
    float currInten = 0.0f;
    byte blinkGoal = 4;
    byte blinkCount = 0;
    bool intenForward = true;
    Light light;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (blinkGoal < blinkCount) { light.intensity = goalInten; Destroy(this); return; }
        

        currInten += intenForward ? Time.deltaTime : -Time.deltaTime ;

        light.intensity = currInten;

        if (intenForward && goalInten < currInten)
        {
            blinkCount++;
            intenForward = false;

            currInten = goalInten;
            if (blinkCount == blinkGoal)
            {
                goalInten /= 2f;
            }
        }
        else if (!intenForward && 0f > currInten)
        {
            intenForward = true;
            currInten = 0f;
        }
    }
}
