using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Velocidad de la bala, modificable desde código pero no visible en el inspector
    [HideInInspector] public Vector3 velocity;

    // Tiempo de vida de la bala antes de autodestruirse
    [HideInInspector] public float lifetime = 6f;

    // Callback que se llama cuando la bala se destruye
    [HideInInspector] public System.Action<Bullet> onDestroyCallback;

    // Tiempo transcurrido desde que la bala se activó
    private float age = 0f;

    // Indica si la bala ya fue registrada en el BulletManager
    private bool registered = false;

    // Referencia a la cámara principal para comprobaciones de visibilidad
    private Camera cam;

    void OnEnable()
    {
        // Reinicia la edad al activar la bala
        age = 0f;

        // Registrar la bala en el BulletManager solo una vez
        if (!registered)
        {
            BulletManager.Instance?.RegisterBullet();
            registered = true;
        }

        // Obtener la cámara principal
        cam = Camera.main;
    }

    void Update()
    {
        // Mover la bala según su velocidad y el tiempo de frame
        transform.position += velocity * Time.deltaTime;

        // Actualizar el tiempo de vida transcurrido
        age += Time.deltaTime;

        // Destruir si se pasa del tiempo de vida o si sale de la vista de la cámara
        if (age >= lifetime || !IsVisible())
        {
            DestroySelf();
        }
    }

    // Verifica si la bala aún está dentro de la vista de la cámara
    private bool IsVisible()
    {
        if (cam == null) return true;

        // Convierte la posición de la bala a coordenadas de viewport (0-1)
        Vector3 vp = cam.WorldToViewportPoint(transform.position);

        // Permite un pequeño margen fuera de la pantalla para evitar destrucciones prematuras
        return vp.x > -0.1f && vp.x < 1.1f && vp.y > -0.1f && vp.y < 1.1f;
    }

    void OnDisable()
    {
        // Desregistrar la bala del BulletManager al desactivarla
        if (registered)
        {
            BulletManager.Instance?.UnregisterBullet();
            registered = false;
        }
    }

    // Método que destruye la bala y llama al callback si existe
    private void DestroySelf()
    {
        onDestroyCallback?.Invoke(this);
        Destroy(gameObject);
    }
}
