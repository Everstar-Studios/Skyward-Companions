using System.Collections;
using UnityEngine;

public class PlayerSliding : MonoBehaviour
{
    private CharacterController controller;
    private bool isSliding = false;
    private Vector3 slideVelocity;
    private float slideSpeed = 10f;
    private Vector3 hedefNokta;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isSliding)
        {
            Vector3 gravityEffect = Vector3.down * 5f; // Yerçekimi etkisi
            Vector3 moveVector = slideVelocity + gravityEffect;

            // Eğer oyuncunun önünde bir engel varsa kaymayı durdur
            if (Physics.CapsuleCast(controller.bounds.center, controller.bounds.center + Vector3.up * controller.height,
                                    controller.radius, moveVector.normalized, 1f, LayerMask.GetMask("Ground")))
            {
                StopSliding();
                return;
            }

            // Hareketi uygula
            controller.Move(moveVector * Time.deltaTime);

            // Eğer yere değerse kaymayı durdur
            if (controller.isGrounded)
            {
                StopSliding();
            }
        }
    }

    public void StartSliding()
    {
        isSliding = true;

        // ✅ Hedef olarak belirlenen GameObject'i kullan
        GameObject hedefObje = GameObject.Find("HedefNokta"); // "HedefNokta" adında bir GameObject bul

        if (hedefObje != null)
        {
            hedefNokta = hedefObje.transform.position; // GameObject'in pozisyonunu hedef nokta olarak ayarla
        }
        else
        {
            hedefNokta = new Vector3(450f, 19f, 446f); // Eğer GameObject bulunamazsa varsayılan noktayı kullan
        }

        // ✅ Kayma yönünü hesapla
        Vector3 slideDirection = (hedefNokta - transform.position).normalized;

        // ✅ Kayma hızını uygula
        slideVelocity = slideDirection * slideSpeed;
    }

    public void StopSliding()
    {
        isSliding = false;
        slideVelocity = Vector3.zero; // Kayma hızını tamamen sıfırla
    }
}
