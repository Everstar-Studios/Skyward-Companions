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
        if (controller == null)
        {
            Debug.LogError("CharacterController bulunamadı! Kayma çalışmaz.");
        }
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
                Debug.Log("🛑 Engel var, kayma durduruluyor.");
                StopSliding();
                return;
            }

            // Hareketi uygula
            controller.Move(moveVector * Time.deltaTime);
            Debug.Log($"⚡ Character Controller ile kayıyorum: {moveVector}");

            // Eğer yere değerse kaymayı durdur
            if (controller.isGrounded)
            {
                Debug.Log("🛑 Yere temas edildi, kayma durduruluyor.");
                StopSliding();
            }
        }
    }

    public void StartSliding()
    {
        isSliding = true;
        Debug.Log("🛑 InvisibleWall'a çarptım! Kayma başlıyor...");

        // ✅ Hedef olarak belirlenen GameObject'i kullan
        GameObject hedefObje = GameObject.Find("HedefNokta"); // "HedefNokta" adında bir GameObject bul

        if (hedefObje != null)
        {
            hedefNokta = hedefObje.transform.position; // GameObject'in pozisyonunu hedef nokta olarak ayarla
        }
        else
        {
            Debug.LogError("❌ Hedef GameObject bulunamadı! Varsayılan noktaya kayıyor.");
            hedefNokta = new Vector3(450f, 19f, 446f); // Eğer GameObject bulunamazsa varsayılan noktayı kullan
        }

        // ✅ Kayma yönünü hesapla
        Vector3 slideDirection = (hedefNokta - transform.position).normalized;

        // ✅ Kayma hızını uygula
        slideVelocity = slideDirection * slideSpeed;

        Debug.Log($"⚡ Güncellenmiş kayma yönü: {slideDirection}");
    }

    public void StopSliding()
    {
        isSliding = false;
        slideVelocity = Vector3.zero; // Kayma hızını tamamen sıfırla
        Debug.Log("🛑 Kayma durduruldu.");
    }
}
