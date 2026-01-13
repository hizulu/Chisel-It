using UnityEngine;
using UnityEngine.SceneManagement;

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
    public MenuButton[] menuButtons;
    public SculptureDirector sculptureDirector;
    private int seleccionActual = 0;

    private float proximoCambioPermitido = 0f;
    public float cooldownSeleccion = 1.5f;
    public bool menuInicio;

    void Start()
    {
        if (menuInicio)
        {
            ConfigurarEstadoMenu(true);
        }
        else
        {
            Resume();
        }
    }

    void Update()
    {
        if (martillo == null || cincel == null || martillo.joycon == null || cincel.joycon == null) return;

        if (!menuInicio)
        {
            bool enReposo = martillo.CheckEstadoReposo() && cincel.CheckEstadoReposo();
            if (enReposo && !esPausado) Pause();
        }

        if (esPausado) GestionarSeleccionMenu();
    }

    void GestionarSeleccionMenu()
    {
        if (Time.unscaledTime < proximoCambioPermitido) return;

        float inclinacionZ = cincel.transform.localEulerAngles.z;
        if (inclinacionZ > 180) inclinacionZ -= 360;

        Transform destino = null;
        if (inclinacionZ > 40f)
        {
            if (seleccionActual != 1)
            {
                seleccionActual = 1;
                destino = posPiedraResume;
            }
        }
        else if (inclinacionZ < -40f)
        {
            if (seleccionActual != 2)
            {
                seleccionActual = 2;
                destino = posPiedraExit;
            }
        }

        if (destino != null)
        {
            proximoCambioPermitido = Time.unscaledTime + cooldownSeleccion;
            MoverGrupo(destino);
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

    private void ConfigurarEstadoMenu(bool pausar)
    {
        esPausado = pausar;

        if (menuButtons != null)
        {
            foreach (var btn in menuButtons) if (btn != null) btn.Reload();
        }

        //if (sculptureDirector != null) sculptureDirector.enabled = !pausar;
        if (menuPausa) menuPausa.SetActive(pausar);
        if (elementosEscena) elementosEscena.SetActive(!pausar);
    }

    public void StartGame()
    {
        menuInicio = false;
        cincel.joycon.SetRumble(0, 0, 0);
        martillo.joycon.SetRumble(0, 0, 0);
        SceneManager.LoadScene(1);
    }

    public void Pause()
    {
        if (menuInicio) return;
        ConfigurarEstadoMenu(true);
    }

    public void Resume()
    {
        ConfigurarEstadoMenu(false);
        menuInicio = false;
        seleccionActual = 0;

        if (menuButtons != null)
        {
            foreach (var btn in menuButtons) if (btn != null) btn.Reload();
        }

        if (grupoHerramientas != null && posBaseHerramientas != null)
        {
            MoverGrupo(posBaseHerramientas);
        }
    }

    public void QuitToInitialScene()
    {
        cincel.joycon.SetRumble(0, 0, 0);
        martillo.joycon.SetRumble(0, 0, 0);
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("<color=red>Saliendo del juego...</color>");
        Application.Quit();
    }
}