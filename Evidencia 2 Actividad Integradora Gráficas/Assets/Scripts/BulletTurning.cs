using UnityEngine;
using System.Collections;

public class BulletTurning : MonoBehaviour
{
    private Bullet core;
    private Vector3 initialVelocity;
    private float turnAfter;
    private float turnAngleDeg;
    [SerializeField] private float turnDuration = 0.5f; // tiempo que tarda en girar

    public void Init(Vector3 vel, float life, float after, float angleDeg)
    {
        initialVelocity = vel;
        turnAfter = after;
        turnAngleDeg = angleDeg;

        core = gameObject.AddComponent<Bullet>();
        core.velocity = initialVelocity;
        core.lifetime = life;

        StartCoroutine(TurnRoutine());
    }

    private IEnumerator TurnRoutine()
    {
        yield return new WaitForSeconds(turnAfter);

        Vector3 startVelocity = core.velocity;
        Vector3 endVelocity = Quaternion.Euler(0, 0, turnAngleDeg) * startVelocity;

        float t = 0f;
        while (t < turnDuration)
        {
            t += Time.deltaTime;
            float k = t / turnDuration;
            core.velocity = Vector3.Lerp(startVelocity, endVelocity, k);

            if (core.TryGetComponent<SpriteRenderer>(out var sr))
            {
                float angle = Mathf.Atan2(core.velocity.y, core.velocity.x) * Mathf.Rad2Deg - 90f;
                sr.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            yield return null;
        }

        // Aseguramos que termine exactamente en la dirección final
        core.velocity = endVelocity;
        if (core.TryGetComponent<SpriteRenderer>(out var srFinal))
        {
            float angle = Mathf.Atan2(core.velocity.y, core.velocity.x) * Mathf.Rad2Deg - 90f;
            srFinal.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
