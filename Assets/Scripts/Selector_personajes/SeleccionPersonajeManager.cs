using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class SeleccionPersonajeManager : MonoBehaviour
{
    public TMP_Text textoJugadorActual; // Texto que muestra "Jugador X elige su personaje"
    public Button BotonIniciar;     // Botón que inicia el juego (debe estar desactivado al inicio)
    
    // Referencias a los botones de personajes
    public Button Personaje_1;
    public Button Personaje_2;
    public Button Personaje_3;
    public Button Personaje_4;

    private int jugadorActual = 1; 
    private Dictionary<int, int> seleccionPersonajes = new Dictionary<int, int>();
    private HashSet<int> personajesSeleccionados = new HashSet<int>();

    void Start()
    {
        if (textoJugadorActual == null)
    {
        Debug.LogError("Falta asignar 'textoJugadorActual' en el Inspector.");
        return;
    }
        BotonIniciar.gameObject.SetActive(false);
        textoJugadorActual.text = "Jugador 1: Elige tu personaje";

        Personaje_1.onClick.AddListener(() => SeleccionarPersonaje(1));
        Personaje_2.onClick.AddListener(() => SeleccionarPersonaje(2));
        Personaje_3.onClick.AddListener(() => SeleccionarPersonaje(3));
        Personaje_4.onClick.AddListener(() => SeleccionarPersonaje(4));
    }

    public void SeleccionarPersonaje(int personajeID)
    {
        if (personajesSeleccionados.Contains(personajeID))
        {
            Debug.Log("Ese personaje ya fue elegido.");
            return;
        }

        seleccionPersonajes[jugadorActual] = personajeID;
        personajesSeleccionados.Add(personajeID);

        
        PlayerData.Instance.jugadores[jugadorActual - 1] = new JugadorData
        {
            numeroJugador = jugadorActual,
            personajeID = personajeID
        };

        jugadorActual++;

        if (jugadorActual > 4)
        {
            Debug.Log("¡Todos listos! Activando botón...");
            textoJugadorActual.text = "¡Todos listos!";
            BotonIniciar.gameObject.SetActive(true);
        }
        else
        {
            textoJugadorActual.text = "Jugador " + jugadorActual + ": Elige tu personaje";
        }
    }

    public void IniciarJuego()
    {
        SceneManager.LoadScene("EscenaJuego");
    }
}
