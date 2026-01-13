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

    [Header("Configuración Reposo")]
    public float tiempoRequerido = 2.0f;
    private float contadorTiempo = 0f;
    public bool estaHorizontalYQuieto;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.2f);
        if (JoyconManager.Instance == null || JoyconManager.Instance.j == null) yield break;

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

        if (joycon != null) Recalibrate();
    }

    void Update()
    {
        if (joycon == null) return;

        ActualizarRotacion();

        ActualizarEstadoReposo();
        Debug.Log($"<color=magenta>Estado Reposo ({(isLeftController ? "Cincel" : "Martillo")}):</color> {estaHorizontalYQuieto}");

        if (Input.GetKeyDown(KeyCode.Space) || joycon.GetButtonDown(Joycon.Button.SHOULDER_2))
        {
            Recalibrate();
        }
    }

    void ActualizarRotacion()
    {
        Quaternion raw = joycon.GetVector();
        float y = invertSelfRotation ? -raw.y : raw.y;
        Quaternion fixedOrientation = new Quaternion(raw.x, raw.z, y, raw.w);
        Quaternion calibratedOrientation = Quaternion.Inverse(calibrationRotation) * fixedOrientation;
        Quaternion finalTarget = calibratedOrientation * Quaternion.Euler(rotationOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, finalTarget, Time.deltaTime * 20f);
    }

    void ActualizarEstadoReposo()
    {
        Vector3 accel = joycon.GetAccel();

        bool quieto = accel.magnitude > 0.8f && accel.magnitude < 1.2f;
        bool plano = Mathf.Abs(accel.y) > 0.85f || Mathf.Abs(accel.z) > 0.85f;

        if (quieto && plano)
        {
            contadorTiempo += Time.deltaTime;
            if (contadorTiempo >= tiempoRequerido)
            {
                estaHorizontalYQuieto = true;
            }
        }
        else
        {
            contadorTiempo = 0f;
            estaHorizontalYQuieto = false;
        }
    }

    public bool CheckEstadoReposo()
    {
        return estaHorizontalYQuieto;
    }

    public void Recalibrate()
    {
        if (joycon != null)
        {
            Quaternion raw = joycon.GetVector();
            float y = invertSelfRotation ? -raw.y : raw.y;
            calibrationRotation = new Quaternion(raw.x, raw.z, y, raw.w);
        }
    }
}