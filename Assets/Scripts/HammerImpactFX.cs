using UnityEngine;
using System.Collections;

public class HammerImpactFX : MonoBehaviour
{
    [Header("Referencias")]
    public JoyconObject hammerJoyconScript;
    public Transform hammerHeadVisual;
    public Transform chiselHeadVisual;
    public Transform puntoDeteccionPunta;

    [Header("Ajustes")]
    public float impactDistance = 1.70f;
    public float minForce = 1.15f;
    public float radioDeRotura = 0.4f;

    private float cooldown = 0.4f;
    private float nextHit;

    void Update()
    {
        if (hammerHeadVisual == null || chiselHeadVisual == null || hammerJoyconScript?.joycon == null) return;

        float dist = Vector3.Distance(hammerHeadVisual.position, chiselHeadVisual.position);
        float force = hammerJoyconScript.joycon.GetAccel().magnitude;

        if (dist <= impactDistance)
        {
            if (force >= minForce && Time.time > nextHit)
            {
                EjecutarImpacto(force);
                nextHit = Time.time + cooldown;
            }
        }
    }

    void EjecutarImpacto(float force)
    {
        Debug.Log($"<color=green>¡GOLPE!</color> Fuerza: {force}");

        hammerJoyconScript.joycon.SetRumble(160, 320, 150);
        Invoke("StopRumble", 0.2f);

        Collider[] piedras = Physics.OverlapSphere(puntoDeteccionPunta.position, radioDeRotura);

        foreach (Collider col in piedras)
        {
            if (col.CompareTag("Stone"))
            {
                LiberarPieza(col.gameObject);
                break;
            }
        }
    }

    void LiberarPieza(GameObject pieza)
    {
        Rigidbody rb = pieza.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            pieza.tag = "Untagged";

            rb.AddExplosionForce(200f, puntoDeteccionPunta.position, 1f);
            StartCoroutine(DesactivarDespuesDeEspera(pieza, 4f));
        }
    }

    private IEnumerator DesactivarDespuesDeEspera(GameObject pieza, float segundos)
    {
        yield return new WaitForSeconds(segundos);
        pieza.SetActive(false);
    }

    void StopRumble()
    {
        if (hammerJoyconScript.joycon != null)
            hammerJoyconScript.joycon.SetRumble(0, 0, 0);
    }

    void OnDrawGizmos()
    {
        if (puntoDeteccionPunta != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(puntoDeteccionPunta.position, radioDeRotura);
        }
    }
}