using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public class GenerateurShapE : MonoBehaviour
{
    // The text you want to send to your AI
    public string objectDescription = "";

    // The address of your local Python server

    private string urlApi = "";

    public SpawnerObject3D spawner;
    public void StartGeneration(string promptText)
    {
        Debug.Log("URL of API : " + urlApi);
        objectDescription = promptText;
        Debug.Log("Starting 3D object generation with prompt: " + objectDescription);
        StartCoroutine(DemanderObjet3D());
    }

    public void SetApiUrl(string url)
    {
        urlApi = url;
        Debug.Log("API URL updated: " + urlApi);
    }

    IEnumerator DemanderObjet3D()
    {
        // 1. Formatting the text into JSON structure
        Debug.Log("1. Formatting text into JSON structure");
        string jsonContent = "{\"prompt\": \"" + objectDescription + "\"}";

        // 2. Creating the POST request
        Debug.Log("2. Creating POST request");
        using (UnityWebRequest www = UnityWebRequest.Post(urlApi, jsonContent, "application/json"))
        {
            // 3. Defining the save path (here in the Assets/Models folder of the project)
            // Make sure the "Models" folder already exists in your Assets
            Debug.Log("3. Defining the save path");
            string savePath = Path.Combine(Application.dataPath, "Models", (Regex.Replace(objectDescription, @"\s", "") + ".glb"));

            // 4. Configuration of DownloadHandler to write directly to disk
            Debug.Log("4. Configuring the DownloadHandler to write directly to disk");
            www.downloadHandler = new DownloadHandlerFile(savePath);
            Debug.Log("Sending request to AI...");

            // 5. Sending and waiting without blocking the game
            Debug.Log("5. Sending and waiting without blocking the game");
            yield return www.SendWebRequest();

            // 6. Checking the result & spawn
            Debug.Log("6. Checking the result");
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Generation failed: " + www.error);
            }
            else
            {
                Debug.Log("Success! The 3D object has been saved here: " + savePath);
                spawner.SpawnObject(savePath);
            }
        }
    }
}