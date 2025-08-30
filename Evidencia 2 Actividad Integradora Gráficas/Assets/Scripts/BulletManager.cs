using UnityEngine;
using TMPro;

public class BulletManager : MonoBehaviour
{
    // Instancia singleton para acceder fácilmente desde otras clases
    public static BulletManager Instance { get; private set; }

    [Header("UI")]
    // Texto de la UI que muestra el número de balas activas
    [SerializeField] private TextMeshProUGUI bulletCounterText;

    // Contador de balas actualmente activas en la escena
    private int activeBullets = 0;

    void Awake()
    {
        // Implementación del patrón singleton
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); // Evita duplicados de BulletManager
            return; 
        }
        Instance = this;
    }

    // Llamado cuando se crea/activa una bala
    public void RegisterBullet()
    {
        activeBullets++;
        UpdateUI(); // Actualiza el contador en la UI
    }

    // Llamado cuando una bala se destruye/desactiva
    public void UnregisterBullet()
    {
        // Asegura que el contador no sea negativo
        activeBullets = Mathf.Max(0, activeBullets - 1);
        UpdateUI(); // Actualiza el contador en la UI
    }

    // Actualiza el texto de la UI mostrando el número de balas activas
    private void UpdateUI()
    {
        if (bulletCounterText != null)
            bulletCounterText.text = $"Balas activas: {activeBullets}";
    }
}
