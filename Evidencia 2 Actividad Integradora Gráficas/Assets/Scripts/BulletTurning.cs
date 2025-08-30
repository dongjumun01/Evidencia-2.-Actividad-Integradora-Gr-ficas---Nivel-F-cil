using UnityEngine;
using System.Collections;

public class BulletTurning : MonoBehaviour
{
    // Referencia al componente Bullet que controla la bala
    private Bullet core;

    // Velocidad inicial de la bala
    private Vector3 initialVelocity;

    // Tiempo después del cual la bala empieza a girar
    private float turnAfter;

    // Ángulo en grados al que la bala girará
    private float turnAngleDeg;

    // Duración del giro de la bala (en segundos)
    [SerializeField] private float turnDuration = 0.5f; // tiempo que tarda en girar

    // Inicializa la bala con parámetros y comienza la rutina de giro
    public void Init(Vector3 vel, float life, float after, float angleDeg)
    {
        initialVelocity = vel;
        turnAfter = after;
        turnAngleDeg = angleDeg;

        // Añade el componente Bullet dinámicamente y configura sus parámetros
        core = gameObject.AddComponent<Bullet>();
        core.velocity = initialVelocity;
        core.lifetime = life;

        // Inicia la corutina que hará que la bala gire después del tiempo especificado
        StartCoroutine(TurnRoutine());
    }

    // Corutina que controla el giro de la bala
    private IEnumerator TurnRoutine()
    {
        // Espera el tiempo antes de comenzar a girar
        yield return new WaitForSeconds(turnAfter);

        Vector3 startVelocity = core.velocity;

        // Calcula la velocidad final después de aplicar el ángulo de giro
        Vector3 endVelocity = Quaternion.Euler(0, 0, turnAngleDeg) * startVelocity;

        float t = 0f;
        while (t < turnDuration)
        {
            t += Time.deltaTime;
            float k = t / turnDuration;

            // Interpola suavemente la velocidad de la bala durante el giro
            core.velocity = Vector3.Lerp(startVelocity, endVelocity, k);

            // Ajusta la rotación del sprite para que apunte en la dirección de movimiento
            if (core.TryGetComponent<SpriteRenderer>(out var sr))
            {
                float angle = Mathf.Atan2(core.velocity.y, core.velocity.x) * Mathf.Rad2Deg - 90f;
                sr.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            yield return null; // espera al siguiente frame
        }

        // Asegura que la bala termine exactamente en la dirección final
        core.velocity = endVelocity;
        if (core.TryGetComponent<SpriteRenderer>(out var srFinal))
        {
            float angle = Mathf.Atan2(core.velocity.y, core.velocity.x) * Mathf.Rad2Deg - 90f;
            srFinal.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
