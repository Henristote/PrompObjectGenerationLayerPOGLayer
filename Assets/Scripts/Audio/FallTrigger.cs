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
        // On déclenche le clip uniquement si la force du choc dépasse notre seuil
        if (collision.relativeVelocity.magnitude > seuilImpact)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}