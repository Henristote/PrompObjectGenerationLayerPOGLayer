using UnityEngine;
using System.Collections.Generic;

public class InfiniteGround : MonoBehaviour
{
    public GameObject groundTilePrefab;
    public float tileSize = 10f;
    public int viewDistance = 5; // Number of tiles visible in each direction

    private Transform player;
    private Dictionary<Vector2, GameObject> activeTiles = new Dictionary<Vector2, GameObject>();
    private Vector2 lastPlayerTilePosition;

    void Start()
    {
        player = transform;
        UpdateTiles();
    }

    void Update()
    {
        // Get the current tile coordinates the player is on
        Vector2 currentTilePosition = new Vector2(
            Mathf.RoundToInt(player.position.x / tileSize),
            Mathf.RoundToInt(player.position.z / tileSize)
        );

        // If the player has moved to a new tile, update the world
        if (currentTilePosition != lastPlayerTilePosition)
        {
            lastPlayerTilePosition = currentTilePosition;
            UpdateTiles();
        }
    }

    void UpdateTiles()
    {
        //    int playerX = Mathf.RoundToInt(player.position.x / tileSize);
        //    int playerZ = Mathf.RoundToInt(player.position.z / tileSize);

        //    List<Vector2> tilesToKeep = new List<Vector2>();

        //    // Spawn or keep tiles within the view distance
        //    for (int x = -viewDistance; x  tilesToRemove = new List<Vector2>();
        //    foreach (Vector2 key in activeTiles.Keys)
        //        {
        //            if (!tilesToKeep.Contains(key))
        //            {
        //                tilesToRemove.Add(key);
        //            }
        //        }

        //    foreach (Vector2 key in tilesToRemove)
        //    {
        //        Destroy(activeTiles[key]);
        //        activeTiles.Remove(key);
        //    }
    }
}
