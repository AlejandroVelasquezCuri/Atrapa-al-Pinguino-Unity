using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ObjetoInteractivo : MonoBehaviour
{
    private bool seleccionado = false;
    private Vector3 offset;
    private Rigidbody2D rb;

    // 🔥 Triple toque
    private int contadorTaps = 0;
    private float tiempoUltimoTap = 0f;
    public float ventanaTiempoTap = 0.5f; // segundos para contar taps

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // =========================
        // PC (Mouse)
        // =========================
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;

                if (TieneColliderEn(mousePos)) // ✅ ahora revisa todos los colliders
                {
                    offset = transform.position - mousePos;
                    seleccionado = true;

                    DetectarTripleTap();
                }
            }
            else if (Input.GetMouseButton(0) && seleccionado)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                rb.MovePosition(mousePos + offset);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                seleccionado = false;
            }

            if (seleccionado && Input.GetMouseButtonDown(1))
            {
                RotarGrupo(90f);
            }
        }
        // =========================
        // Tablet / Móvil (Touch)
        // =========================
        else if (Application.platform == RuntimePlatform.Android ||
                 Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                touchPos.z = 0;

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (TieneColliderEn(touchPos)) // ✅ revisa todos los colliders
                        {
                            offset = transform.position - touchPos;
                            seleccionado = true;

                            DetectarTripleTap();
                        }
                        break;

                    case TouchPhase.Moved:
                        if (seleccionado)
                        {
                            rb.MovePosition(touchPos + offset);
                        }
                        break;

                    case TouchPhase.Ended:
                        seleccionado = false;
                        break;
                }
            }

            if (Input.touchCount == 2 && seleccionado)
            {
                RotarGrupo(90f);
            }
        }
    }

    // =========================
    // ✅ Revisa todos los colliders
    // =========================
    private bool TieneColliderEn(Vector2 pos)
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            if (col.OverlapPoint(pos)) return true;
        }
        return false;
    }

    // =========================
    // 👆 Detectar triple tap
    // =========================
    private void DetectarTripleTap()
    {
        if (Time.time - tiempoUltimoTap < ventanaTiempoTap)
        {
            contadorTaps++;
        }
        else
        {
            contadorTaps = 1;
        }

        tiempoUltimoTap = Time.time;

        if (contadorTaps >= 3)
        {
            SoltarCadena();
            contadorTaps = 0;
        }
    }

    // =========================
    // 🔓 Soltar toda la cadena
    // =========================
    private void SoltarCadena()
    {
        GameObject[] objetos = GameObject.FindGameObjectsWithTag("Objeto");

        foreach (GameObject obj in objetos)
        {
            FixedJoint2D[] joints = obj.GetComponents<FixedJoint2D>();
            foreach (var joint in joints)
            {
                Destroy(joint);
            }

            Rigidbody2D rbObj = obj.GetComponent<Rigidbody2D>();
            if (rbObj != null)
            {
                Vector2 dir = Random.insideUnitCircle.normalized;
                rbObj.AddForce(dir * 100f, ForceMode2D.Impulse);
            }
        }

        Debug.Log("🔓 Toda la cadena se soltó por TRIPLE TAP!");
    }

    // =========================
    // 🔄 Rotación en grupo
    // =========================
    private void RotarGrupo(float angulo)
    {
        transform.Rotate(0, 0, angulo);

        FixedJoint2D[] joints = GetComponents<FixedJoint2D>();
        foreach (FixedJoint2D joint in joints)
        {
            if (joint.connectedBody != null)
            {
                joint.connectedBody.transform.Rotate(0, 0, angulo);
            }
        }
    }

    // =========================
    // 🔗 Unión con otros objetos
    // =========================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objeto"))
        {
            Vector2 contacto = collision.contacts[0].point;

            FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = collision.rigidbody;

            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = transform.InverseTransformPoint(contacto);
            joint.connectedAnchor = collision.transform.InverseTransformPoint(contacto);

            joint.breakForce = Mathf.Infinity;
            joint.breakTorque = Mathf.Infinity;

            Debug.Log($"{gameObject.name} se unió a {collision.gameObject.name} en {contacto}");
        }
    }
}
