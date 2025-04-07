using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlePlayer : MonoBehaviour
{
    [SerializeField]
    float aceleracao, forcaPulo, velocidadeMaxima;
    [SerializeField]
    LayerMask mascaraDeLayers;
    [SerializeField] Image barraPulo;

    bool noChao = false;
    bool jumping = false;
    Rigidbody2D rb;

    InputAction move;
    InputAction jump;

    int itensColetados = 0;
    int qntVidas = 2;

    [SerializeField] TextMeshProUGUI pontos, vidas;

    private void Start()
    {
        StartCoroutine(BarraRegenera());
        AtualizaPontos();
        AtualizaVidas();

        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void Update()
    {
        if (jump.WasPressedThisFrame() && noChao && barraPulo.fillAmount > 0)
        {
            jumping = true;
            barraPulo.fillAmount -= 0.1f;
        }


    }
    private void FixedUpdate()
    {
        Vector2 direcao = move.ReadValue<Vector2>();

        if (direcao != Vector2.zero)
        {
            rb.AddForce(Vector2.right * direcao.x * aceleracao, ForceMode2D.Force);
            if (rb.linearVelocity.magnitude > velocidadeMaxima)
            {
                rb.linearVelocityX = velocidadeMaxima * direcao.x;
            }
        }
        else
        {
            rb.AddForce(new Vector2(rb.linearVelocityX * -aceleracao, 0), ForceMode2D.Force);
        }

        if (jumping)
        {
            rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
            jumping=false;
        }

       Vector2 baseObjeto = new Vector2(transform.position.x, 
            GetComponent<BoxCollider2D>().bounds.min.y);

        noChao = Physics2D.OverlapCircle(baseObjeto, 0.1f, mascaraDeLayers);
    }

    IEnumerator BarraRegenera()
    {
        yield return new WaitForSeconds(0.1f);
        barraPulo.fillAmount += 0.001f;
        StartCoroutine(BarraRegenera());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Penis"))
        {
            itensColetados++;
            AtualizaPontos();
            Destroy(collision.gameObject);
        }
    }

    private void AtualizaPontos()
    {
        pontos.text = "Coletado: " + itensColetados.ToString("000");

        if (itensColetados%3 == 0)
        {
            qntVidas++;
            AtualizaVidas();
        }
    }
    private void AtualizaVidas()
    {
        vidas.text = "Vidas: " + qntVidas.ToString("000");
    }
}
