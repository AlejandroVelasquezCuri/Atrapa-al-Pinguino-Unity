using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class menuinicial : MonoBehaviour
{
     void Start()
    {
        // Al iniciar la escena de menú, reproducimos la música de menú
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMenuMusic();
        }
    }
    public void Jugar(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    public void Salir(){
        Debug.Log("Salir...");
        Application.Quit();
    }
}
