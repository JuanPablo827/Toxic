using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidadCaminar = 5f;
    public float velocidadCorrer = 8f;
    public float fuerzaSalto = 8f;

    [Header("Estadísticas de Vida")]
    public float vidaMax = 100f;
    private float vidaActual;

    [Header("Configuración de Suelo")]
    public LayerMask capaSuelo;
    public float distanciaDeteccion = 0.1f;
    public float tiempoEntreSaltos = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private float movimientoHorizontal;
    private bool enElSuelo;
    private bool estaCorriendo;
    private bool estaMuerto = false;
    private float siguienteTiempoSalto = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        vidaActual = vidaMax;
    }

    void Update()
    {
        if (estaMuerto) return;

        var teclado = Keyboard.current;
        if (teclado == null) return;

        enElSuelo = ChequearSuelo();

        // Entrada horizontal (A/D y flechas)
        movimientoHorizontal = 0f;
        if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) movimientoHorizontal -= 1f;
        if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) movimientoHorizontal += 1f;

        // Correr
        bool presionandoShift = teclado.leftShiftKey.isPressed || teclado.rightShiftKey.isPressed;
        bool estaMoviendose = movimientoHorizontal != 0;
        estaCorriendo = estaMoviendose && presionandoShift;

        // Voltear sprite
        if (movimientoHorizontal > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (movimientoHorizontal < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // Salto (Espacio)
        if (teclado.spaceKey.wasPressedThisFrame && enElSuelo && Time.time >= siguienteTiempoSalto)
        {
            float velocidadActual = estaCorriendo ? velocidadCorrer : velocidadCaminar;
            rb.linearVelocity = new Vector2(movimientoHorizontal * velocidadActual, fuerzaSalto);
            siguienteTiempoSalto = Time.time + tiempoEntreSaltos;
            enElSuelo = false;
        }

        if (anim != null)
        {
            anim.SetBool("isMoving", estaMoviendose);
            anim.SetBool("isRunning", estaCorriendo);
            anim.SetBool("isGrounded", enElSuelo);
        }
    }

    void FixedUpdate()
    {
        if (estaMuerto) return;

        float velocidadActual = estaCorriendo ? velocidadCorrer : velocidadCaminar;
        rb.linearVelocity = new Vector2(movimientoHorizontal * velocidadActual, rb.linearVelocity.y);

        Debug.Log($"input: {movimientoHorizontal} | velocidad: {rb.linearVelocity} | posicion: {transform.position}");
    }

    private bool ChequearSuelo()
    {
        if (boxCollider == null) return false;

        Vector2 tamañoCaja = new Vector2(boxCollider.bounds.size.x * 0.9f, boxCollider.bounds.size.y);

        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            tamañoCaja,
            0f,
            Vector2.down,
            distanciaDeteccion,
            capaSuelo
        );

        return hit.collider != null;
    }

    public void RecibirDanio(float cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;

        if (vidaActual > 0)
        {
            if (anim != null) anim.SetTrigger("Hurt");
        }
        else
        {
            Morir();
        }
    }

    private void Morir()
    {
        estaMuerto = true;
        rb.linearVelocity = Vector2.zero;

        if (anim != null) anim.SetTrigger("Die");

        if (boxCollider != null) boxCollider.enabled = false;
    }
}