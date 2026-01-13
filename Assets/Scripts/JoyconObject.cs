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
    private bool yaRecalibradoEnEsteReposo = false;

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
                break;
            }
            else if (!isLeftController && !jIsLeft)
            {
                joycon = joycons[i];
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

        if (Input.GetKeyDown(KeyCode.Space) || joycon.GetButtonDown(Joycon.Button.SHOULDER_2))
        {
            Recalibrate();
        }

        if (estaHorizontalYQuieto && !yaRecalibradoEnEsteReposo)
        {
            Recalibrate();
            yaRecalibradoEnEsteReposo = true;
        }
    }

    void ActualizarRotacion()
    {
        Quaternion raw = joycon.GetVector();

        float y = invertSelfRotation ? -raw.y : raw.y;
        Quaternion fixedOrientation = new Quaternion(raw.x, raw.z, y, raw.w);

        Quaternion finalTarget = Quaternion.Inverse(calibrationRotation) * fixedOrientation;
        finalTarget *= Quaternion.Euler(rotationOffset);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalTarget, Time.unscaledDeltaTime * 25f);
    }

    void ActualizarEstadoReposo()
    {
        Vector3 accel = joycon.GetAccel();

        bool quieto = accel.magnitude > 0.95f && accel.magnitude < 1.05f;
        bool plano = Mathf.Abs(accel.y) > 0.9f || Mathf.Abs(accel.z) > 0.9f;

        if (quieto && plano)
        {
            contadorTiempo += Time.unscaledDeltaTime;
            if (contadorTiempo >= tiempoRequerido)
            {
                estaHorizontalYQuieto = true;
            }
        }
        else
        {
            contadorTiempo = 0f;
            estaHorizontalYQuieto = false;
            yaRecalibradoEnEsteReposo = false;
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

            transform.localRotation = Quaternion.identity;
        }
    }
}