using UnityEngine;

public class GameManager : MonoBehaviour
{
    public JoyconObject martillo;
    public JoyconObject cincel;
    public GameObject menuPausa;
    public GameObject elementosEscena;

    [Header("Referencias de Posición")]
    public Transform posBaseHerramientas;
    public Transform posPiedraResume;
    public Transform posPiedraExit;
    public GameObject grupoHerramientas;

    private bool esPausado = false;
    public MenuButton[] menuButton;
    public SculptureDirector sculptureDirector;
    private int seleccionActual = 0;

    private float proximoCambioPermitido = 0f;
    public float cooldownSeleccion = 3.0f;

    void Start() { Resume(); }

    void Update()
    {
        if (martillo == null || cincel == null || martillo.joycon == null || cincel.joycon == null) return;

        bool enReposo = martillo.CheckEstadoReposo() && cincel.CheckEstadoReposo();
        if (enReposo && !esPausado) Pause();

        if (esPausado) GestionarSeleccionMenu();
    }

    void GestionarSeleccionMenu()
    {
        // Si aún no ha pasado el tiempo de espera, no leemos la inclinación
        if (Time.unscaledTime < proximoCambioPermitido) return;

        float inclinacionZ = cincel.transform.localEulerAngles.z;
        if (inclinacionZ > 180) inclinacionZ -= 360;

        if (inclinacionZ > 40f)
        {
            if (seleccionActual != 1)
            {
                seleccionActual = 1;
                proximoCambioPermitido = Time.unscaledTime + cooldownSeleccion;
                MoverGrupo(posPiedraResume);
            }
        }
        else if (inclinacionZ < -40f)
        {
            if (seleccionActual != 2)
            {
                seleccionActual = 2;
                proximoCambioPermitido = Time.unscaledTime + cooldownSeleccion;
                MoverGrupo(posPiedraExit);
            }
        }
    }

    void MoverGrupo(Transform destino)
    {
        if (destino != null && grupoHerramientas != null)
        {
            grupoHerramientas.transform.position = destino.position;
            grupoHerramientas.transform.rotation = destino.rotation;
        }
    }

    public void Pause()
    {
        esPausado = true;
        foreach (var menuButton in menuButton)
        {
            menuButton.Reload();
        }
        sculptureDirector.enabled = false;
        //Time.timeScale = 0f;
        if (menuPausa) menuPausa.SetActive(true);
        if (elementosEscena) elementosEscena.SetActive(false);
    }

    public void Resume()
    {
        esPausado = false;
        foreach (var menuButton in menuButton)
        {
            menuButton.Reload();
        }
        sculptureDirector.enabled = true;
        //Time.timeScale = 1f;
        if (menuPausa) menuPausa.SetActive(false);
        if (elementosEscena) elementosEscena.SetActive(true);

        grupoHerramientas.transform.position = posBaseHerramientas.position;
        //grupoHerramientas.transform.rotation = posBaseHerramientas.rotation;
    }

    public void QuitGame()
    {
        Debug.Log("<color=red> Saliendo del juego...</color>");
        Application.Quit();
        foreach (var menuButton in menuButton)
        {
            menuButton.Reload();
        }
    }
}