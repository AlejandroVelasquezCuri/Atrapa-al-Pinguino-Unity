using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    
    public void Reintentar()
    {
        // Recarga completamente la escena de juego
        SceneManager.LoadScene("EscenaJuego", LoadSceneMode.Single);
    }

    public void IrAlMenu()
    {
        // Cargar menú inicial
        SceneManager.LoadScene("menuinicial", LoadSceneMode.Single);
    }
}