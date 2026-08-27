using UnityEngine;
using UnityEngine.UI; // si usas UI
using TMPro; 
public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public JugadorData[] jugadores = new JugadorData[4];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
