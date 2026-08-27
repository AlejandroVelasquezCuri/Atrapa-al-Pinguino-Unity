using UnityEngine;
using System.Collections.Generic;

public class SpawnObjetos : MonoBehaviour
{
    public GameObject[] objetos;        // Prefabs de objetos
    public int cantidad = 10;
    public BoxCollider2D zonaSpawn;     // Arrastra aquí tu BoxCollider2D
    public float distanciaMinima = 20f;  // Separación mínima entre objetos

    private List<Vector2> posicionesUsadas = new List<Vector2>();

    void Start()
    {
        for (int i = 0; i < cantidad; i++)
        {
            Vector2 spawnPos = GetRandomPointSeparated();
            int index = Random.Range(0, objetos.Length);
            Instantiate(objetos[index], spawnPos, Quaternion.identity);
        }
    }

    Vector2 GetRandomPointSeparated()
    {
        Bounds bounds = zonaSpawn.bounds;
        Vector2 punto;
        int intentos = 0;

        do
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            punto = new Vector2(x, y);
            intentos++;

            // Si no encuentra espacio tras muchos intentos, se sale igual
            if (intentos > 50)
                break;

        } while (!EsValido(punto));

        posicionesUsadas.Add(punto);
        return punto;
    }

    bool EsValido(Vector2 nuevoPunto)
    {
        foreach (var pos in posicionesUsadas)
        {
            if (Vector2.Distance(nuevoPunto, pos) < distanciaMinima)
                return false;
        }
        return true;
    }
}
