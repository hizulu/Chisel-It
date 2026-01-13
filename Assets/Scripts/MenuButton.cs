using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class MenuButton : MonoBehaviour
{
    public UnityEvent onPiedraRota;
    private bool activado = false;
    private Rigidbody[] trozos;
    private Vector3[] posicionesOriginales;
    private Quaternion[] rotacionesOriginales;

    private void Awake()
    {
        InicializarTrozos();
    }

    private void InicializarTrozos()
    {
        if (trozos == null)
        {
            trozos = GetComponentsInChildren<Rigidbody>(true);
            posicionesOriginales = new Vector3[trozos.Length];
            rotacionesOriginales = new Quaternion[trozos.Length];

            for (int i = 0; i < trozos.Length; i++)
            {
                posicionesOriginales[i] = trozos[i].transform.localPosition;
                rotacionesOriginales[i] = trozos[i].transform.localRotation;
            }
        }
    }

    public void TriggerAction()
    {
        if (activado) return;
        activado = true;

        InicializarTrozos();

        foreach (Rigidbody rb in trozos)
        {
            if (rb == null) continue;
            rb.gameObject.SetActive(true);
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(new Vector3(0, 5, -5), ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 10, ForceMode.Impulse);
            rb.gameObject.tag = "Untagged";
            StartCoroutine(DesactivarDespuesDeEspera(rb.gameObject, 4f));
        }
        onPiedraRota.Invoke();
    }

    public void Reload()
    {
        InicializarTrozos();
        activado = false;

        for (int i = 0; i < trozos.Length; i++)
        {
            if (trozos[i] == null) continue;

            StopAllCoroutines();

            trozos[i].gameObject.SetActive(true);
            trozos[i].isKinematic = true;
            trozos[i].useGravity = false;
            trozos[i].transform.localPosition = posicionesOriginales[i];
            trozos[i].transform.localRotation = rotacionesOriginales[i];
            trozos[i].gameObject.tag = "PauseStone";
        }
    }

    private IEnumerator DesactivarDespuesDeEspera(GameObject pieza, float segundos)
    {
        yield return new WaitForSecondsRealtime(segundos);
        if (pieza != null) pieza.SetActive(false);
    }
}