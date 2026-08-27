using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [Header("Puntos de Spawn de los jugadores")]
    public Transform esquinaSupIzq;
    public Transform esquinaSupDer;
    public Transform esquinaInfIzq;
    public Transform esquinaInfDer;

    [Header("Prefabs de Personajes (en orden 1-4)")]
    public GameObject[] personajesPrefabs; // Arrastra aquí tus prefabs

    void Start()
    {
        // Recorremos los 4 jugadores que guardaste en PlayerData
        for (int i = 0; i < PlayerData.Instance.jugadores.Length; i++)
        {
            JugadorData jd = PlayerData.Instance.jugadores[i];
            if (jd == null) continue; // por si no eligió alguien

            int personajeID = jd.personajeID; // 1 al 4
            if (personajeID < 1 || personajeID > personajesPrefabs.Length) continue;

            GameObject prefab = personajesPrefabs[personajeID - 1];

            Transform spawnPos = null;
            switch (jd.numeroJugador)
            {
                case 1: spawnPos = esquinaSupIzq; break;
                case 2: spawnPos = esquinaSupDer; break;
                case 3: spawnPos = esquinaInfIzq; break;
                case 4: spawnPos = esquinaInfDer; break;
            }

            if (spawnPos != null && prefab != null)
            {
                Instantiate(prefab, spawnPos.position, spawnPos.rotation);
            }
        }
    }
}
