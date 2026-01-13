using UnityEngine;
using System.Collections;

public class HammerImpactFX : MonoBehaviour
{
    public JoyconObject hammerJoyconScript;
    public Transform hammerHeadVisual;
    public Transform chiselHeadVisual;
    public Transform puntoDeteccionPunta;

    public float impactDistance = 1.70f;
    public float minForce = 1.15f;
    public float radioDeRotura = 0.5f;

    private float cooldown = 0.4f;
    private float nextHit;

    void Update()
    {
        if (hammerHeadVisual == null || chiselHeadVisual == null || hammerJoyconScript?.joycon == null) return;

        float dist = Vector3.Distance(hammerHeadVisual.position, chiselHeadVisual.position);
        float force = hammerJoyconScript.joycon.GetAccel().magnitude;

        if (dist <= impactDistance && force >= minForce && Time.unscaledTime > nextHit)
        {
            EjecutarImpacto(force);
            nextHit = Time.unscaledTime + cooldown;
        }
    }

    void EjecutarImpacto(float force)
    {
        hammerJoyconScript.joycon.SetRumble(100, 200, 100);
        Invoke("StopRumble", 0.1f);

        Collider[] hitColliders = Physics.OverlapSphere(puntoDeteccionPunta.position, radioDeRotura);

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("PauseStone"))
            {
                MenuButton btn = col.GetComponentInParent<MenuButton>();
                if (btn != null)
                {
                    btn.TriggerAction();
                }
                break;
            }
            else if (col.CompareTag("Stone") && Time.timeScale != 0)
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
            StartCoroutine(DesactivarDespuesDeEspera(pieza, 4f));
        }
    }

    private IEnumerator DesactivarDespuesDeEspera(GameObject pieza, float segundos)
    {
        yield return new WaitForSeconds(segundos);
        pieza.SetActive(false);
    }

    void StopRumble() { if (hammerJoyconScript.joycon != null) hammerJoyconScript.joycon.SetRumble(0, 0, 0); }
}