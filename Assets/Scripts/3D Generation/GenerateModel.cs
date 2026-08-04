using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;
public class GenerateModel : MonoBehaviour
{

    [SerializeField] public string LAYER_TOKEN = "your-pat-token";
    [SerializeField] public string WORKSPACE_ID = "your-workspace-id";
    public string modelId = "TON_MODEL_ID";

    void Start()
    {
        StartCoroutine(GenerateAssetCoroutine("A medieval castle on a hilltop at sunset, game concept art"));
    }

    private IEnumerator GenerateAssetCoroutine(string prompt)
    {
        string baseUrl = $"https://api.app.layer.ai/api/v1/workspaces/{WORKSPACE_ID}/inferences";

        string jsonBody = $@"{{
            ""model_id"": ""{modelId}"",
            ""parameters"": {{
                ""prompt"": ""{prompt}"",
                ""width"": 1024,
                ""height"": 1024
            }}
        }}";

        using (UnityWebRequest request = new UnityWebRequest(baseUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {LAYER_TOKEN}");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur lors de la demande de génération : " + request.error);
                yield break;
            }

            JObject responseJson = JObject.Parse(request.downloadHandler.text);
            string inferenceId = responseJson["inference_id"]?.ToString();

            if (!string.IsNullOrEmpty(inferenceId))
            {
                Debug.Log($"Génération lancée avec l'ID: {inferenceId}. Début du polling...");
                yield return StartCoroutine(PollForResultsCoroutine(inferenceId));
            }
        }
    }

    private IEnumerator PollForResultsCoroutine(string inferenceId)
    {
        string url = $"https://api.app.layer.ai/api/v1/workspaces/{WORKSPACE_ID}/inferences/{inferenceId}";
        bool isCompleted = false;

        while (!isCompleted)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", $"Bearer {LAYER_TOKEN}");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Erreur lors de la vérification : " + request.error);
                    yield break;
                }

                JObject responseJson = JObject.Parse(request.downloadHandler.text);
                string status = responseJson["status"]?.ToString();

                if (status == "COMPLETED")
                {
                    isCompleted = true;
                    Debug.Log("Génération terminée ! Voici la réponse JSON : \n" + request.downloadHandler.text);
                }
                else
                {
                    Debug.Log($"Statut actuel : {status}. Nouvelle vérification dans 3 secondes...");
                    yield return new WaitForSeconds(3f);
                }
            }
        }
    }
}
