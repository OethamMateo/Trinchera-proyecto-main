using System.Collections;
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Configuración de Velocidad")]
    public float velocidad = 5f;
    public float velocidadAcelerada = 10f;

    [Header("Configuración de Interacción")]
    public float tiempoInmovilizacion = 3f;

    private float tiempoPresionado = 0f;
    private float velocidadActual;
    private bool puedeMoverse = true;
    private bool estaInteraccionando = false;

    private AudioSource audioSource;
    private Animator animator;

    void Start()
    {
        velocidadActual = velocidad;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!puedeMoverse) return;

        // Verificar si se está presionando la tecla de movimiento
        bool teclaPresionada = Input.GetKey(KeyCode.RightArrow);

        if (teclaPresionada)
        {
            tiempoPresionado += Time.deltaTime;

            // Aumentar la velocidad si se mantiene presionado por más de 3 segundos
            if (tiempoPresionado >= 3f)
            {
                velocidadActual = velocidadAcelerada;
                audioSource.pitch = 1.5f; // Aumentar la velocidad del sonido
                animator.SetBool("corriendo", true); // Activar animación de correr
            }
            else
            {
                velocidadActual = velocidad;
                audioSource.pitch = 1.0f;
                animator.SetBool("corriendo", false); // Desactivar animación de correr
            }

            // Reproducir sonido si no está ya reproduciéndose
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            // Mover el jugador
            transform.Translate(Vector2.right * velocidadActual * Time.deltaTime);

            // Activar la animación de caminar
            animator.SetBool("moviendo", true);
        }
        else
        {
            // Restablecer velocidad y tiempo cuando se suelta la tecla
            tiempoPresionado = 0f;
            velocidadActual = 0f;
            audioSource.pitch = 1.0f;

            // Detener el sonido cuando se suelta la tecla
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            // Desactivar la animación de caminar y correr
            animator.SetBool("moviendo", false);
            animator.SetBool("corriendo", false);
        }
    }

    // Método para deshabilitar el movimiento temporalmente
    public void DeshabilitarMovimiento(float duracion)
    {
        if (!estaInteraccionando)
        {
            StartCoroutine(DeshabilitarMovimientoCoroutine(duracion));
        }
    }

    private IEnumerator DeshabilitarMovimientoCoroutine(float duracion)
    {
        puedeMoverse = false;
        audioSource.Stop(); // Detener el sonido al inmovilizar al jugador
        animator.SetBool("moviendo", false);
        animator.SetBool("interactuando", true); // Activar la animación de interacción
        yield return new WaitForSeconds(duracion);
        puedeMoverse = true;
        animator.SetBool("interactuando", false); // Volver a la animación normal
    }

    // Método para cambiar el estado de interacción
    public void EstablecerInteraccion(bool estado)
    {
        estaInteraccionando = estado;
        if (estado)
        {
            DeshabilitarMovimiento(tiempoInmovilizacion);
        }
    }
}
