using System.Collections.Generic;
using UnityEngine;

public class TestJoycon : MonoBehaviour
{
    private List<Joycon> joycons;
    private Joycon joyconL;
    private Joycon joyconR;

    void Start()
    {
        joycons = JoyconManager.Instance.j;

        if (joycons.Count < 1)
        {
            Debug.Log("No Joy-Cons detected.");
            return;
        }

        foreach (Joycon j in joycons)
        {
            if (j.isLeft)
                joyconL = j;
            else
                joyconR = j;
        }
    }

    void Update()
    {
        if (joyconL != null)
        {
            Debug.Log("LEFT Joy-Con Accel: " + joyconL.GetAccel());
            Debug.Log("LEFT Joy-Con Gyro: " + joyconL.GetGyro());
        }

        if (joyconR != null)
        {
            Debug.Log("RIGHT Joy-Con Accel: " + joyconR.GetAccel());
            Debug.Log("RIGHT Joy-Con Gyro: " + joyconR.GetGyro());
        }
    }
}
