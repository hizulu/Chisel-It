using UnityEngine;
using System.Collections;

public class SculptureDirector : MonoBehaviour
{
    [Header("Configuración de Puntos")]
    public int totalPuntos = 5;
    private int puntoActual = 0;
    public float anguloPorPunto = 72f;

    [Header("Detección (Punto B)")]
    public Transform puntoDeteccionPunta;
    public float radioDeteccion = 0.4f;

    private bool estaRotando = false;
    private float tiempoEsperaDeteccion = 2f;

    public GameObject[] piedras;
    public Material materialPiedras;
    public ParticleSystem particulas;

    private void Awake()
    {
        materialPiedras.color = new Color(materialPiedras.color.r, materialPiedras.color.g, materialPiedras.color.b, 1f);
    }

    void Start()
    {
        InvokeRepeating("CheckPiedrasRestantes", tiempoEsperaDeteccion, 3f);
        particulas.Stop();
    }

    void CheckPiedrasRestantes()
    {
        if (estaRotando || puntoDeteccionPunta == null) return;

        Collider[] hitColliders = Physics.OverlapSphere(puntoDeteccionPunta.position, radioDeteccion);

        foreach (var col in hitColliders)
        {
            Debug.Log("Esfera verde tocando a: " + col.gameObject.name + " | Tag: " + col.tag);
        }

        int contadorPiedras = 0;
        foreach (var col in hitColliders)
        {
            if (col.CompareTag("Stone"))
            {
                contadorPiedras++;
            }
        }

        if (contadorPiedras == 0 && puntoActual < totalPuntos)
        {
            StartCoroutine(RotarAlSiguientePunto());
        }
    }

    IEnumerator RotarAlSiguientePunto()
    {
        estaRotando = true;
        puntoActual++;

        Debug.Log($"<color=orange>¡ZONA LIMPIA! Rotando al punto {puntoActual + 1}</color>");

        yield return new WaitForSeconds(3f);

        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, anguloPorPunto, 0);
        float elapsed = 0f;
        float duration = 2.5f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        estaRotando = false;

        if (puntoActual >= totalPuntos)
        {
            Debug.Log("<color=cyan>¡ESCULTURA FINALIZADA!</color>");
            CancelInvoke("CheckPiedrasRestantes");

            StartCoroutine(DesactivarParticulasGradualmente());
        }
    }

    IEnumerator DesactivarParticulasGradualmente()
    {
        particulas.Play();
        yield return new WaitForSeconds(5f);

        float duracion = 2f;
        float tiempo = 0f;
        Vector3 escalaInicial = particulas.transform.localScale;

        var main = particulas.main;
        float startSizeInicial = main.startSize.constant;

        while (tiempo < duracion)
        {
            //El material se vuelve transparente
            float progreso = tiempo / duracion;
            float transparente = Mathf.Lerp(1f, 0f, progreso);
            materialPiedras.color = new Color(materialPiedras.color.r, materialPiedras.color.g, materialPiedras.color.b, transparente);
            tiempo += Time.deltaTime;
            yield return null;
        }

        foreach (var i in piedras)
        {
            i.SetActive(false);
        }
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracion;
            main.startSize = Mathf.Lerp(startSizeInicial, 0f, progreso);
            yield return null;
        }

        particulas.Stop();
        particulas.transform.localScale = escalaInicial;
        particulas.gameObject.SetActive(false);
    }

    void OnDrawGizmos()
    {
        if (puntoDeteccionPunta != null)
        {
            Gizmos.color = Color.green; // Verde para el sensor del Director
            Gizmos.DrawWireSphere(puntoDeteccionPunta.position, radioDeteccion);
        }
    }
}