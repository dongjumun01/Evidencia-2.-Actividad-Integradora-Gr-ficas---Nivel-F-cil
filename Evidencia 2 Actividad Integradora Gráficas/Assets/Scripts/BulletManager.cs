using UnityEngine;
using TMPro;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI bulletCounterText;

    private int activeBullets = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void RegisterBullet()
    {
        activeBullets++;
        UpdateUI();
    }

    public void UnregisterBullet()
    {
        activeBullets = Mathf.Max(0, activeBullets - 1);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (bulletCounterText != null)
            bulletCounterText.text = $"Balas activas: {activeBullets}";
    }
}
