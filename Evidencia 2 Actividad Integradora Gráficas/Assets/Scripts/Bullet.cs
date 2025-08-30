using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public float lifetime = 6f;
    [HideInInspector] public System.Action<Bullet> onDestroyCallback;

    private float age = 0f;
    private bool registered = false;
    private Camera cam;

    void OnEnable()
    {
        age = 0f;
        if (!registered)
        {
            BulletManager.Instance?.RegisterBullet();
            registered = true;
        }
        cam = Camera.main;
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        age += Time.deltaTime;

        // Destruir si se pasa del tiempo de vida o sale de la vista
        if (age >= lifetime || !IsVisible())
        {
            DestroySelf();
        }
    }

    private bool IsVisible()
    {
        if (cam == null) return true;
        Vector3 vp = cam.WorldToViewportPoint(transform.position);
        return vp.x > -0.1f && vp.x < 1.1f && vp.y > -0.1f && vp.y < 1.1f;
    }

    void OnDisable()
    {
        if (registered)
        {
            BulletManager.Instance?.UnregisterBullet();
            registered = false;
        }
    }

    private void DestroySelf()
    {
        onDestroyCallback?.Invoke(this);
        Destroy(gameObject);
    }
}
