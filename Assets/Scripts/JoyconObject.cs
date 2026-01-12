using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconObject : MonoBehaviour
{
    public bool isLeftController;
    public Joycon joycon;
    private Quaternion calibrationRotation = Quaternion.identity;

    [Header("Ajuste de Ejes")]
    public bool invertSelfRotation = true;
    public Vector3 rotationOffset;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.2f);

        if (JoyconManager.Instance == null || JoyconManager.Instance.j == null)
        {
            Debug.LogError("Error: JoyconManager no encontrado o lista de mandos vacía.");
            yield break;
        }

        List<Joycon> joycons = JoyconManager.Instance.j;

        joycon = null;

        for (int i = 0; i < joycons.Count; i++)
        {
            bool jIsLeft = joycons[i].isLeft;

            if (isLeftController && jIsLeft)
            {
                joycon = joycons[i];
                Debug.Log($"<color=cyan>Cincel (IZQ)</color> asignado al Joy-con #{i}");
                break;
            }
            else if (!isLeftController && !jIsLeft)
            {
                joycon = joycons[i];
                Debug.Log($"<color=yellow>Martillo (DER)</color> asignado al Joy-con #{i}");
                break;
            }
        }

        if (joycon == null)
        {
            Debug.LogError($"OJO: No se encontró un Joy-con {(isLeftController ? "Izquierdo" : "Derecho")} conectado.");
        }
        else
        {
            Recalibrate();
        }
    }

    void Update()
    {
        if (joycon == null) return;

        Quaternion raw = joycon.GetVector();

        float x = raw.x;
        float y = raw.y;
        float z = raw.z;
        float w = raw.w;

        if (invertSelfRotation) y = -y;

        Quaternion fixedOrientation = new Quaternion(x, z, y, w);

        Quaternion calibratedOrientation = Quaternion.Inverse(calibrationRotation) * fixedOrientation;
        transform.rotation = calibratedOrientation * Quaternion.Euler(rotationOffset);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Recalibrate();
        }
    }

    public void Recalibrate()
    {
        if (joycon != null)
        {
            Quaternion raw = joycon.GetVector();
            float y = invertSelfRotation ? -raw.y : raw.y;
            calibrationRotation = new Quaternion(raw.x, raw.z, y, raw.w);
            Debug.Log("Joy-con recalibrado.");
        }
    }
}