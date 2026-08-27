using UnityEngine;
using UnityEngine.UI;

public class PenguinMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 moveDirection;
    private int lastDirection = -1;
    private int repeatCount = 0;

    private float changeDirTime;
    private float timer;

    private float tiempoAtrapado = 0f;
    public float tiempoParaPerder = 5f;

    private bool haPerdido = false;
    [Header("Efectos visuales")]
    public ParticleSystem confetiIzquierdo;
    public ParticleSystem confetiDerecho;
    [Header("UI")]
    [SerializeField] private GameObject gameoverpanel;
    [SerializeField] private Slider barraAtrapado; // 🔹 barra de progreso

    [Header("Encierro")]
    public float radioEncierro = 2.5f;   // distancia de detección
    public int piezasMinimas = 6;        // cuántos objetos necesita alrededor

    private enum Direction { Up, Down, Left, Right }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;

        // 🔹 Resetear estado de derrota al inicio
        haPerdido = false;
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;

        // 🔹 Resetear animador para evitar quedarse en pinguino_pierde
        animator.Rebind();   // reinicia todos los parámetros
        animator.Update(0f); // fuerza actualización inmediata

        if (gameoverpanel != null)
            gameoverpanel.SetActive(false);

        if (barraAtrapado != null)
        {
            barraAtrapado.gameObject.SetActive(false);
            barraAtrapado.minValue = 0f;
            barraAtrapado.maxValue = 1f;
            barraAtrapado.value = 0f;
        }

        // 🔹 Arranca caminando
        ChooseNewDirection();
    }

    void Update()
    {
        if (haPerdido) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ChooseNewDirection();
        }

        // Chequear encierro
        if (EstaRodeado())
        {
            tiempoAtrapado += Time.deltaTime;

            if (barraAtrapado != null)
            {
                barraAtrapado.gameObject.SetActive(true);
                barraAtrapado.value = tiempoAtrapado / tiempoParaPerder;
            }

            if (tiempoAtrapado >= tiempoParaPerder)
            {
                Perder();
            }
        }
        else
        {
            tiempoAtrapado = 0f;

            if (barraAtrapado != null)
            {
                barraAtrapado.value = 0f;
                barraAtrapado.gameObject.SetActive(false);
            }
        }

        // Animaciones de movimiento
        if (moveDirection != Vector2.zero)
        {
            animator.SetBool("movimiento", true);

            if (moveDirection.x != 0)
            {
                animator.Play("pinguino_lateral");
                Vector3 scale = transform.localScale;
                scale.x = moveDirection.x > 0 ? 1 : -1;
                scale.y = 1;
                transform.localScale = scale;
            }
            else if (moveDirection.y > 0)
            {
                animator.Play("pinguino_vertical");
                transform.localScale = new Vector3(1, -1, 1);
            }
            else if (moveDirection.y < 0)
            {
                animator.Play("pinguino_vertical");
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            animator.SetBool("movimiento", false);
        }
    }

    void FixedUpdate()
    {
        if (!haPerdido)
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private void ChooseNewDirection()
    {
        int newDir;

        do
        {
            newDir = Random.Range(0, 4);
        }
        while ((newDir == lastDirection && repeatCount >= 2) ||
               (IsOppositeDirection(newDir, lastDirection)));

        if (newDir == lastDirection)
            repeatCount++;
        else
            repeatCount = 1;

        lastDirection = newDir;

        switch ((Direction)newDir)
        {
            case Direction.Up: moveDirection = Vector2.up; break;
            case Direction.Down: moveDirection = Vector2.down; break;
            case Direction.Left: moveDirection = Vector2.left; break;
            case Direction.Right: moveDirection = Vector2.right; break;
        }

        changeDirTime = Random.Range(0.5f, 1.5f);
        timer = changeDirTime;
    }

    private bool IsOppositeDirection(int dir1, int dir2)
    {
        return (dir1 == 0 && dir2 == 1) || (dir1 == 1 && dir2 == 0) ||
               (dir1 == 2 && dir2 == 3) || (dir1 == 3 && dir2 == 2);
    }

    /// Comprueba si el pingüino está rodeado
    private bool EstaRodeado()
    {
        LayerMask mask = LayerMask.GetMask("Objeto");
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, radioEncierro, mask);

        Debug.Log("Objetos alrededor: " + colisiones.Length);

        return colisiones.Length >= piezasMinimas;
    }

    private void Perder()
{
    haPerdido = true;
    animator.Play("pinguino_pierde");
    rb.velocity = Vector2.zero;
    rb.isKinematic = true;

    if (barraAtrapado != null)
        barraAtrapado.gameObject.SetActive(false);

    Debug.Log("🐧 ¡Pingüino atrapado!");

    // 🔔 Avisar al gestor de turnos
    if (GameManagerTurnos.Instance != null)
        GameManagerTurnos.Instance.PinguinoAtrapado();

    if (gameoverpanel != null)
        gameoverpanel.SetActive(true);
    // Crea confeti
    if (confetiIzquierdo != null) confetiIzquierdo.Play();
    if (confetiDerecho != null) confetiDerecho.Play();
}

    // 🔎 Visual del radio de detección
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radioEncierro);
    }
    
}
