using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
public class GenerateModel : MonoBehaviour
{

    public string LAYER_TOKEN;
    public string WORKSPACE_ID;
    private string modelId;
    public void StartGeneration(string promptText)
    {
        Debug.Log("Start of the generation with this prompt : " + promptText);
        StartCoroutine(GenerateFlowCoroutine(promptText));
    }

    private IEnumerator GenerateFlowCoroutine(string prompt)
    {
        // Stage 1 : Check and retrieve available models
        if (string.IsNullOrEmpty(modelId) || modelId == "TON_MODEL_ID")
        {
            Debug.Log("No ModelId in memory. Fetching in progress...");
            yield return StartCoroutine(GetAvailableModelsCoroutine());
        }

        //Stage 2 : Security Check
        //If is still Empty, that mean JSON request failed
        if(string.IsNullOrEmpty(modelId) || modelId == "TON_MODEL_ID")
        {
            Debug.LogError("ModelId is still empty after fetching. Aborting generation.");
            yield break;
        }

        //Stage 3 : Id is available, proceed to generate the asset
        yield return StartCoroutine(GenerateAssetCoroutine(prompt));
    }

    private IEnumerator GenerateAssetCoroutine(string prompt)
    {
        Debug.Log("Sending generation request...");
        string baseUrl = $"https://api.app.layer.ai/api/v1/workspaces/{WORKSPACE_ID}/inferences";

        JObject jsonBodyObj = new JObject
        {
            ["model_id"] = modelId,
            ["parameters"] = new JObject
            {
                ["prompt"] = prompt,
            }
        };
        string jsonBody = jsonBodyObj.ToString();

        Debug.Log("Prompt used : " + prompt);
        Debug.Log("JSON request body : " + jsonBody);
        using (UnityWebRequest request = new UnityWebRequest(baseUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.uploadHandler.contentType = "application/json";
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {LAYER_TOKEN}");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error during generation request : " + request.error);
                Debug.LogError("Prompt back : " + jsonBodyObj["parameters"]["prompt"]);

                if ( request.downloadHandler != null)
                {
                    Debug.LogError("API answer : " + request.downloadHandler.text);
                    Debug.LogError("API modelId: " + modelId);
                }

                yield break;
            }

            JObject responseJson = JObject.Parse(request.downloadHandler.text);
            string inferenceId = responseJson["inference_id"]?.ToString();

            if (!string.IsNullOrEmpty(inferenceId))
            {
                Debug.Log($"Generation with this id : {inferenceId}. Polling start...");
                yield return StartCoroutine(PollForResultsCoroutine(inferenceId));
            }
        }
        Debug.Log("End of generation Coroutine.");
    }

    private IEnumerator PollForResultsCoroutine(string inferenceId)
    {
        Debug.Log("Vérification du statut de la génération...");
        string url = $"https://api.app.layer.ai/api/v1/workspaces/{WORKSPACE_ID}/inferences/{inferenceId}";
        bool isCompleted = false;

        while (!isCompleted)
        {
            Debug.Log ("Sending status check request...");
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", $"Bearer {LAYER_TOKEN}");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error during status check : " + request.error);
                    yield break;
                }

                JObject responseJson = JObject.Parse(request.downloadHandler.text);
                string status = responseJson["status"]?.ToString();

                if (status == "COMPLETED")
                {
                    isCompleted = true;
                    Debug.Log("Generation completed! Here is the JSON response : \n" + request.downloadHandler.text);
                }
                else
                {
                    //Debug.Log($"Current status : {status}. Checking again in 3 seconds...");
                    Debug.Log($"Current status : {status}. text : {request.downloadHandler.text}");
                    yield return new WaitForSeconds(3f);
                }
            }
        }
    }
    public IEnumerator GetAvailableModelsCoroutine()
    {
        Debug.Log("Retrieving list of available models...");

        bool modelFound = false;
        bool hasMoreResults = true;
        string currentCursor = "";

        while (!modelFound && hasMoreResults)
        {
            string url = $"https://api.app.layer.ai/api/v1/workspaces/{WORKSPACE_ID}/models";
            if (!string.IsNullOrEmpty(currentCursor))
            {
                url += $"?cursor={UnityWebRequest.EscapeURL(currentCursor)}";
            }

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", $"Bearer {LAYER_TOKEN}");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Unable to retrieve models : " + request.error);
                    yield break;
                }

                JObject responseJson = JObject.Parse(request.downloadHandler.text);
                JArray modelsArray = responseJson["models"] as JArray;

                if (modelsArray != null)
                {
                    foreach (JToken model in modelsArray)
                    {
                        if (model["modality"] != null && model["modality"].ToString().Trim().Equals("three_d", System.StringComparison.OrdinalIgnoreCase)) // && model["capabilities"].ToString() == %5B"textTo3D"%5D)
                        {
                        modelId = model["model_id"]?.ToString();
                        Debug.Log($"3D model founded ! ID : {modelId} (Name: {model["name"]})");
                        modelFound = true;
                        break;
                        }
                    }
                }

                //if no results on this page , check if there is a next page
                if (!modelFound) 
                { 
                    JToken pagination = responseJson["pagination"];
                    if (pagination != null && pagination["has_more_results"]?.ToObject<bool>() == true)
                    {
                        currentCursor = pagination["next_cursor"]?.ToString();
                        Debug.LogError("API answer : " + request.downloadHandler.text);
                        Debug.Log("No results on this page. Moving to the next page with cursor : " + currentCursor);
                    }
                    else
                    {
                        //If no more pages, exit the loop
                        hasMoreResults = false;
                        Debug.LogWarning("Search end : No 3D model found in the available models.");
                    }
                }
            }
        }
    }
}
