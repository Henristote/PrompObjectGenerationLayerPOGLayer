using UnityEngine;
using System.Collections.Generic;

public class InfiniteGroundVR : MonoBehaviour
{
    public GameObject groundTilePrefab;
    [SerializeField] public float tileSize = 10f;
    [SerializeField] public int viewDistance = 5;

    private Transform centerEyeCamera; // Modifié pour suivre la vraie position du casque
    private Dictionary<Vector2, GameObject> activeTiles = new Dictionary<Vector2, GameObject>();
    private Vector2 lastPlayerTilePosition;

    void Start()
    {
        // Trouve automatiquement la caméra principale (le casque VR)
        if (Camera.main != null)
        {
            centerEyeCamera = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Aucune caméra principale trouvée ! Assurez-vous que la CenterEyeAnchor a le tag MainCamera.");
            centerEyeCamera = transform;
        }

        UpdateTiles();
    }

    void Update()
    {
        // Utilise la position réelle de la caméra dans l'espace
        Vector2 currentTilePosition = new Vector2(
            Mathf.RoundToInt(centerEyeCamera.position.x / tileSize),
            Mathf.RoundToInt(centerEyeCamera.position.z / tileSize)
        );

        if (currentTilePosition != lastPlayerTilePosition)
        {
            lastPlayerTilePosition = currentTilePosition;
            UpdateTiles();
        }
    }

    void UpdateTiles()
    {
        int playerX = Mathf.RoundToInt(centerEyeCamera.position.x / tileSize);
        int playerZ = Mathf.RoundToInt(centerEyeCamera.position.z / tileSize);

        List<Vector2> tilesToKeep = new List<Vector2>();

        // On crée la grille autour du joueur
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2 tilePosition = new Vector2(playerX + x, playerZ + z);
                tilesToKeep.Add(tilePosition);

                // Si la tuile n'existe pas encore à cet endroit, on la crée
                if (!activeTiles.ContainsKey(tilePosition))
                {
                    Vector3 worldPosition = new Vector3(tilePosition.x * tileSize, 0, tilePosition.y * tileSize);
                    GameObject newTile = Instantiate(groundTilePrefab, worldPosition, Quaternion.identity);
                    activeTiles.Add(tilePosition, newTile);
                }
            }
        }

        // On prépare la liste de suppression après avoir généré le nouveau terrain
        List<Vector2> tilesToRemove = new List<Vector2>();

        foreach (Vector2 key in activeTiles.Keys)
        {
            if (!tilesToKeep.Contains(key))
            {
                tilesToRemove.Add(key);
            }
        }

        // On détruit les tuiles trop éloignées
        foreach (Vector2 key in tilesToRemove)
        {
            Destroy(activeTiles[key]);
            activeTiles.Remove(key);
        }
    }
}
