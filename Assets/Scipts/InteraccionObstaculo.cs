using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteraccionObstaculo : MonoBehaviour
{
    public Transform jugador;
    public float distanciaInteraccion = 1.5f;
    public float duracionPenalizacion = 2f;
    public float duracionPenalizacionExtra = 4f;
    public int minNPCs = 3;
    private static int contadorNPCs = 0;
    private bool enInteraccion = false;
    private float tiempoInteraccion = 0f;

    private MovimientoJugador movimientoJugador;
    private AudioSource audioSource;

    // Variables para el tambaleo
    public float amplitudTambaleo = 5f;
    public float frecuenciaTambaleo = 5f;
    public float frecuenciaTambaleoRapido = 10f; // Frecuencia cuando el jugador está acompañado
    private float anguloInicial;

    private void Start()
    {
        movimientoJugador = jugador.GetComponent<MovimientoJugador>();
        anguloInicial = transform.rotation.eulerAngles.z;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= distanciaInteraccion && !enInteraccion)
        {
            IniciarInteraccion();
        }

        if (enInteraccion)
        {
            ControlarInteraccion();
            TambalearObstaculo();
        }
    }

    private void IniciarInteraccion()
    {
        enInteraccion = true;
        tiempoInteraccion = 0f;

        // Verificar si cumple con el requisito mínimo de NPCs
        if (contadorNPCs >= minNPCs)
        {
            movimientoJugador.ActivarAnimacionObstaculoConNPCs(); // Animación de interacción exitosa
            movimientoJugador.DeshabilitarMovimiento(duracionPenalizacion);
        }
        else
        {
            movimientoJugador.ActivarAnimacionObstaculoSinNPCs(); // Animación de interacción fallida
            movimientoJugador.DeshabilitarMovimiento(duracionPenalizacionExtra);
        }

        // Reproducir sonido
        audioSource.pitch = 1.0f;
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }



    private void ControlarInteraccion()
    {
        tiempoInteraccion += Time.deltaTime;

        float penalizacionActual = (contadorNPCs < minNPCs) ? duracionPenalizacionExtra : duracionPenalizacion;
        if (tiempoInteraccion >= penalizacionActual)
        {
            FinalizarInteraccion();
        }
    }

    private void FinalizarInteraccion()
    {
        enInteraccion = false;
        movimientoJugador.EstablecerInteraccion(false);
        movimientoJugador.DesactivarAnimacionesObstaculo(); // Desactivar ambas animaciones
        transform.rotation = Quaternion.Euler(0, 0, anguloInicial);
        Destroy(gameObject);
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }



    private void TambalearObstaculo()
    {
        // Selecciona la frecuencia del tambaleo en función del número de NPCs acompañantes
        float frecuenciaActual = (contadorNPCs > 0) ? frecuenciaTambaleoRapido : frecuenciaTambaleo;

        // Aplica una rotación oscilante usando Mathf.Sin para el tambaleo
        float anguloTambaleo = amplitudTambaleo * Mathf.Sin(Time.time * frecuenciaActual);
        transform.rotation = Quaternion.Euler(0, 0, anguloInicial + anguloTambaleo);
    }

    public static void IncrementarNPCContador()
    {
        contadorNPCs++;
        Debug.Log("Contador de NPCs: " + contadorNPCs);
    }

    public static void ReiniciarContadorNPCs()
    {
        contadorNPCs = 0;
        Debug.Log("Contador de NPCs reiniciado.");
    }
}
