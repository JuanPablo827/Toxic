using JetBrains.Annotations;
using UnityEngine;

public class Movimientoprueba : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidadCaminar = 5f;
    public float velocidadCorrer = 8f;
    public float fuerzaSalto = 8f;
    public float movimientoHorizontal;


    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Entrada horizontal (A/D y flechas)
        movimientoHorizontal = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(velocidadCaminar * movimientoHorizontal, 0);
    }
}
