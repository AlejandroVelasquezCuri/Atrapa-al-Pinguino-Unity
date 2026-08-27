using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManagerTurnos : MonoBehaviour
{
    
    public static GameManagerTurnos Instance;

    [Header("Turnos")]
    [Tooltip("Jugador actual (1..4)")]
    public int jugadorActual = 1;

    [Tooltip("Duración de cada turno en segundos")]
    public float duracionTurno = 30f;

    [Header("UI")]
    [Tooltip("Texto que dice: Es turno del Jugador X")]
    public TMP_Text textoTurno;

    [Tooltip("Círculos de tiempo (Image con Fill = Radial 360)")]
    public Image tiempo1;
    public Image tiempo2;
    public Image tiempo3;
    public Image tiempo4;

    [Header("Opcional")]
    public bool comenzarAlIniciar = true;

    private Image[] tiempos;
    private float tiempoRestante;
    private Coroutine rutinaTiempo;
    private bool juegoTerminado = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
{
    tiempos = new Image[] { tiempo1, tiempo2, tiempo3, tiempo4 };
    OcultarTodosLosTiempos();

    // 🎵 Cambiar música al entrar al juego
    if (AudioManager.instance != null)
    {
        AudioManager.instance.PlayGameMusic();
    }

    if (comenzarAlIniciar)
        IniciarTurno(1); // arranca en Jugador 1
}

    private void OcultarTodosLosTiempos()
    {
        foreach (var img in tiempos)
            if (img) img.gameObject.SetActive(false);
    }

    public void IniciarTurno(int jugador)
    {
        if (juegoTerminado) return;

        jugadorActual = Mathf.Clamp(jugador, 1, 4);

        // ✅ Mostrar texto SOLO al inicio del turno
        if (textoTurno)
        {
            textoTurno.gameObject.SetActive(true);
            textoTurno.text = $"Es turno del Jugador {jugadorActual}";
            StartCoroutine(EsconderTexto());
        }

        // ✅ Mostrar solo el círculo del jugador actual y resetearlo
        for (int i = 0; i < tiempos.Length; i++)
        {
            if (tiempos[i] == null) continue;
            tiempos[i].gameObject.SetActive(i == jugadorActual - 1);
            tiempos[i].fillAmount = 1f;
        }

        tiempoRestante = duracionTurno;
        if (rutinaTiempo != null) StopCoroutine(rutinaTiempo);
        rutinaTiempo = StartCoroutine(ContarTiempo());
    }

    private IEnumerator EsconderTexto()
    {
        yield return new WaitForSeconds(5f); // ⏳ 5 segundos visible
        if (textoTurno) textoTurno.gameObject.SetActive(false);
    }

    private IEnumerator ContarTiempo()
    {
        while (tiempoRestante > 0f && !juegoTerminado)
        {
            tiempoRestante -= Time.deltaTime;

            var img = tiempos[jugadorActual - 1];
            if (img) img.fillAmount = tiempoRestante / duracionTurno;

            yield return null;
        }

        if (!juegoTerminado)
            SiguienteTurno();
    }

    public void SiguienteTurno()
    {
        int siguiente = jugadorActual + 1;
        if (siguiente > 4) siguiente = 1;
        IniciarTurno(siguiente);
    }

    public bool EsTurnoDelJugador(int numeroJugador)
    {
        return jugadorActual == numeroJugador;
    }

    /// Llamar cuando el pingüino sea atrapado
    public void PinguinoAtrapado()
    {
        juegoTerminado = true;
        if (rutinaTiempo != null) StopCoroutine(rutinaTiempo);

        OcultarTodosLosTiempos();
        if (textoTurno)
        {
            textoTurno.gameObject.SetActive(true);
            textoTurno.text = "";
        }
    }
}
