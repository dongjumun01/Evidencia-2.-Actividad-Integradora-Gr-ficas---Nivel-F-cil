using UnityEngine;
using System.Collections;
using TMPro;

public class JugadorController : MonoBehaviour
{
    public SpriteRenderer bossSprite;      
    public ParticleSystem patternFlash;      
    public TextMeshProUGUI patternNameText;    
    public TextMeshProUGUI patternTimerText;   
    public Color ringColor = Color.cyan;
    public Color sweepColor = new Color(1f, 0.4f, 0.7f);
    public Color turnColor = Color.green;
    public enum Pattern { RingSpin, SweepingLines, TurnMidFlight }
    public float moveSpeed = 5f;

    
    private Color currentPatternColor = Color.white;
    [Header("General")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float patternDuration = 10f; // 10 s c/u, total 30 s
    [SerializeField] private float bulletLifetime = 6f;

    [Header("Pattern 1: RingSpin")]
    [SerializeField] private int ringBullets = 24;
    [SerializeField] private float ringFireRate = 0.15f;
    [SerializeField] private float ringBaseSpeed = 5f;
    [SerializeField] private float spinSpeedDeg = 120f; // grados/seg para girar el patrón

    [Header("Pattern 2: SweepingLines")]
    [SerializeField] private int emitters = 3;          // emisores laterales virtuales
    [SerializeField] private float sweepAmplitude = 3f; // amplitud seno
    [SerializeField] private float sweepFrequency = 1f; // Hz del movimiento
    [SerializeField] private float sweepFireRate = 0.12f;
    [SerializeField] private float sweepBulletSpeed = 7f;

    [Header("Pattern 3: TurnMidFlight")]
    [SerializeField] private float turnFireRate = 0.18f;
    [SerializeField] private float turnBulletSpeed = 6f;
    [SerializeField] private float turnAfterSeconds = 0.7f; // al cambiar trayectoria
    [SerializeField] private float turnAngleDeg = 90f;      // curva

    private void Start()
    {
        StartCoroutine(RunPatternsSequence());
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // A/D or ← →
        float v = Input.GetAxis("Vertical");   // W/S or ↑ ↓
        
        Vector3 dir = new Vector3(h, v, 0f);
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private IEnumerator RunPatternsSequence()
    {
        yield return StartCoroutine(RunPattern(Pattern.RingSpin, PatternRingSpin()));
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(RunPattern(Pattern.SweepingLines, PatternSweepingLines()));
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(RunPattern(Pattern.TurnMidFlight, PatternTurnMidFlight()));
    }

    private IEnumerator RunPattern(Pattern p, IEnumerator routine)
    {
        ApplyPatternVisuals(p);
        StartCoroutine(UpdatePatternTimer(patternDuration));
        Coroutine c = StartCoroutine(routine);
        yield return new WaitForSeconds(patternDuration);
        if(c != null) StopCoroutine(c);
    }

    // ---------- PATTERN 1: anillo giratorio ----------
    private IEnumerator PatternRingSpin()
    {
        float angleOffset = 0f;
        WaitForSeconds wait = new WaitForSeconds(ringFireRate);
        while (true)
        {
            float angleStep = 360f / ringBullets;
            for (int i = 0; i < ringBullets; i++)
            {
                float ang = angleOffset + i * angleStep;
                Vector3 dir = Quaternion.Euler(0, 0, ang) * Vector3.up; // ver tips de Quaternion del enunciado
                SpawnBullet(transform.position, dir * ringBaseSpeed, bulletLifetime);
            }
            angleOffset += spinSpeedDeg * ringFireRate; // el círculo va girando
            yield return wait;
        }
    }

    // ---------- PATTERN 2: líneas en barrido con emisores móviles ----------
    private IEnumerator PatternSweepingLines()
    {
        float t = 0f;
        WaitForSeconds wait = new WaitForSeconds(sweepFireRate);
        while (true)
        {
            for (int e = 0; e < emitters; e++)
            {
                // Emisores “flotando” lateralmente
                float phase = (e / (float)Mathf.Max(1, emitters - 1)) * Mathf.PI; // desfases
                float x = Mathf.Sin(2f * Mathf.PI * sweepFrequency * t + phase) * sweepAmplitude;
                Vector3 emitPos = transform.position;

                // Línea de 5 balas hacia abajo (o hacia donde quieras)
                int lineCount = 5;
                float spread = 15f; // apertura vertical
                for (int i = 0; i < lineCount; i++)
                {
                    float ang = -90f + (i - (lineCount - 1) / 2f) * (spread / (lineCount - 1));
                    Vector3 dir = Quaternion.Euler(0, 0, ang) * Vector3.up;
                    SpawnBullet(emitPos, dir * sweepBulletSpeed, bulletLifetime);
                }
            }
            t += sweepFireRate;
            yield return wait;
        }
    }

    // ---------- PATTERN 3: cambia de dirección en vuelo ----------
    private IEnumerator PatternTurnMidFlight()
    {
        WaitForSeconds wait = new WaitForSeconds(turnFireRate);
        while (true)
        {
            int fan = 9;       // número de balas por oleada
            float spread = 90f; // apertura del abanico

            for (int i = 0; i < fan; i++)
            {
                // Calculamos la dirección inicial
                float ang = -spread / 2f + (spread / (fan - 1)) * i;
                Vector3 dir = Quaternion.Euler(0, 0, ang) * Vector3.up;

                // SpawnTurningBullet ahora hará que cada bala gire
                SpawnTurningBullet(
                    transform.position, 
                    dir * turnBulletSpeed, 
                    bulletLifetime, 
                    turnAfterSeconds, 
                    turnAngleDeg
                );
            }
            yield return wait;
        }
    }

    // ---------- helpers de spawn ----------
    private void SpawnBullet(Vector3 pos, Vector3 vel, float life)
    {
        if(bulletPrefab == null) return;
        GameObject go = Instantiate(bulletPrefab, pos, Quaternion.identity);
        if(go == null) {
            Debug.LogError("Instancia de bala falló, prefab nulo o destruido");
            return;
        }
        Bullet b = go.GetComponent<Bullet>();
        b.velocity = vel;
        b.lifetime = life;

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = currentPatternColor;
    }

    private void SpawnTurningBullet(Vector3 pos, Vector3 vel, float life, float after, float angleDeg)
    {
        if(bulletPrefab == null) return;
        GameObject go = Instantiate(bulletPrefab, pos, Quaternion.identity);
        if(go == null) {
            Debug.LogError("Instancia de bala falló, prefab nulo o destruido");
            return;
        }
        // obtenemos el SpriteRenderer de la bala
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = currentPatternColor;
        }

        BulletTurning bt = go.AddComponent<BulletTurning>(); // componente ligero para este patrón
        bt.Init(vel, life, after, angleDeg);
    }

    private void ApplyPatternVisuals(Pattern p)
    {
        Color c = Color.white;
        string name = "";
        switch (p)
        {
            case Pattern.RingSpin:
                c = ringColor; name = "Círculo giratorio"; break;
            case Pattern.SweepingLines:
                c = sweepColor; name = "Líneas rectas"; break;
            case Pattern.TurnMidFlight:
                c = turnColor; name = "Cambio en vuelo"; break;
        }

        currentPatternColor = c;

        if (patternNameText != null) patternNameText.text = name;
        if (patternFlash != null) patternFlash.Play();
        if (bossSprite != null) StartCoroutine(BossPulseColorAndScale(c, 0.12f));
        Debug.Log($"[Boss] Iniciando patrón {name} en t={Time.time}");
    }
    
    private IEnumerator BossPulseColorAndScale(Color target, float time)
    {
        if (bossSprite == null) yield break;

        Color initial = bossSprite.color;
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.06f;

        float t = 0f;
        // Pulsar hacia el target
        while (t < time)
        {
            if (bossSprite == null) yield break; // <-- salir si el sprite ya no existe
            float k = t / time;
            bossSprite.color = Color.Lerp(initial, target, k);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, k);
            t += Time.deltaTime;
            yield return null;
        }

        if (bossSprite != null)
            bossSprite.color = target;

        // volver al tamaño original suavemente
        t = 0f;
        while (t < time)
        {
            if (bossSprite == null) yield break; // <-- salir si el sprite ya no existe
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t / time);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }


    private IEnumerator UpdatePatternTimer(float duration)
    {
        float t = duration;
        while (t > 0f)
        {
            if (patternTimerText != null) patternTimerText.text = $"Tiempo: {t:F1}s";
            yield return null;
            t -= Time.deltaTime;
        }
        if (patternTimerText != null) patternTimerText.text = "Tiempo: 0.0s";
    }
}
