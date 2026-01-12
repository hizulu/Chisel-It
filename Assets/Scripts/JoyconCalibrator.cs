using System.Collections.Generic;
using UnityEngine;

public class JoyconCalibrator : MonoBehaviour
{
    private Joycon leftJoycon, rightJoycon;

    [Header("Modelos 3D")]
    public Transform leftModel;   // Cincel
    public Transform rightModel;  // Martillo

    private Quaternion leftReference = Quaternion.identity;
    private Quaternion rightReference = Quaternion.identity;

    private bool leftCalibrated = false;
    private bool rightCalibrated = false;

    void Start()
    {
        var joycons = JoyconManager.Instance.j;

        foreach (var j in joycons)
        {
            if (j.isLeft) leftJoycon = j;
            else rightJoycon = j;
        }

        Debug.Log("JoyconCalibrator listo. Pulsa SL (izq) o SR (der) para calibrar.");
    }

    void Update()
    {
        if (leftJoycon != null)
        {
            if (leftJoycon.GetButtonDown(Joycon.Button.SL))
            {
                Quaternion raw = leftJoycon.GetVector();
                leftReference = Quaternion.Inverse(raw);
                leftCalibrated = true;
                Debug.Log("Joy-Con IZQUIERDO calibrado.");
            }

            if (leftCalibrated && leftModel != null)
            {
                Quaternion raw = leftJoycon.GetVector();
                Quaternion corrected = leftReference * raw;
                leftModel.rotation = corrected;
            }
        }

        if (rightJoycon != null)
        {
            if (rightJoycon.GetButtonDown(Joycon.Button.SR))
            {
                Quaternion raw = rightJoycon.GetVector();
                rightReference = Quaternion.Inverse(raw);
                rightCalibrated = true;
                Debug.Log("Joy-Con DERECHO calibrado.");
            }

            if (rightCalibrated && rightModel != null)
            {
                Quaternion raw = rightJoycon.GetVector();
                Quaternion corrected = rightReference * raw;
                rightModel.rotation = corrected;
            }
        }
    }
}
