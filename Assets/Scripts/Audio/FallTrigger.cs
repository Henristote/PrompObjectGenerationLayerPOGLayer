using UnityEngine;

public class FallTrigger : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    // Ce paramètre bloque le son si l'impact est trop faible
    [SerializeField] float seuilImpact = 1.0f;

    void Start()
    {
        Debug.Log("FallTrigger script started.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("!!! IMPACT !!!");
        // On déclenche le clip uniquement si la force du choc dépasse notre seuil
        if (collision.relativeVelocity.magnitude > seuilImpact && !collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("SOUND TRIGGER");
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}