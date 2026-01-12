using UnityEngine;

public class HammerImpactFinal : MonoBehaviour
{
    [Header("Referencias")]
    public JoyconObject hammerJoyconScript;
    public Transform hammerHeadVisual;
    public Transform chiselHeadVisual;

    [Header("Ajustes basados en tus Logs")]
    public float impactDistance = 1.70f;
    public float minForce = 1.15f;

    private float cooldown = 0.4f;
    private float nextHit;

    void Update()
    {
        if (hammerHeadVisual == null || chiselHeadVisual == null || hammerJoyconScript?.joycon == null) return;

        float dist = Vector3.Distance(hammerHeadVisual.position, chiselHeadVisual.position);
        float force = hammerJoyconScript.joycon.GetAccel().magnitude;

        if (dist <= impactDistance)
        {
            if (force >= minForce)
            {
                if (Time.time > nextHit)
                {
                    EjecutarImpacto(force);
                    nextHit = Time.time + cooldown;
                }
            }
        }
    }

    void EjecutarImpacto(float force)
    {
        Debug.Log($"<color=green>¡GOLPE DETECTADO!</color> Distancia: {Vector3.Distance(hammerHeadVisual.position, chiselHeadVisual.position)} | Fuerza: {force}");

        hammerJoyconScript.joycon.SetRumble(160, 320, 150);
        Invoke("StopRumble", 0.2f);
    }

    void StopRumble()
    {
        hammerJoyconScript.joycon.SetRumble(0, 0, 0);
    }
}
