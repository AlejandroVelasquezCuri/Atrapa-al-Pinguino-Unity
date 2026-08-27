using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] personajesPrefabs; // Arreglo de prefabs de personajes
    public Transform[] puntosSpawn; // Puntos de aparición de los 4 jugadores

    void Start()
    {
        for (int i = 0; i < PlayerData.Instance.jugadores.Length; i++)
        {
            int personajeID = PlayerData.Instance.jugadores[i].personajeID;
            if (personajeID >= 1 && personajeID <= personajesPrefabs.Length)
            {
                Vector3 posicion = puntosSpawn[i].position;
                Instantiate(personajesPrefabs[personajeID - 1], posicion, Quaternion.identity);
            }
        }
    }
}

