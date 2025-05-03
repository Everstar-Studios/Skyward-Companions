using UnityEngine;
using UnityEngine.Playables;

public class JoystickTutorialTrigger : MonoBehaviour
{
    public PlayableDirector tutorialTimeline;
    public GameObject player; // sahnedeki player referansı

    private void Start()
    {
        tutorialTimeline.gameObject.SetActive(false); // ya da ilgili UI nesneleri
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != player) return;

        tutorialTimeline.Play();
        gameObject.SetActive(false); // tekrar tetiklenmesin diye
    }
}
