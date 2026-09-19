using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VRLaser : MonoBehaviour
{
    private LineRenderer laser;

    void Start()
    {
        laser = GetComponent<LineRenderer>();
        laser.startWidth = 0.005f; // Un rayon très fin
        laser.endWidth = 0.005f;
        laser.material = new Material(Shader.Find("Sprites/Default"));
        laser.startColor = Color.red;
        laser.endColor = Color.red;
    }

    void Update()
    {
        // Dessine un rayon droit de 2 mètres depuis la manette
        laser.SetPosition(0, transform.position);
        laser.SetPosition(1, transform.position + transform.forward * 2f); 
    }
}