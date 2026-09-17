using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using Meta.XR.BuildingBlocks.AIBlocks;
using System.Text.RegularExpressions;

public class GenerateurShapE : MonoBehaviour
{
    // Le texte que tu souhaites envoyer à ton IA
    public string descriptionObjet = "";

    // L'adresse de ton serveur Python local

    private string urlApi = "http://" + "cio-squad-detector-craig" + ".trycloudflare.com/generer";

    public SpawnerObject3D spawner;
    public void StartGeneration(string promptText)
    {
        Debug.Log("URL de l'API : " + urlApi);
        descriptionObjet = promptText;
        Debug.Log("Démarrage de la génération de l'objet 3D avec le prompt : " + descriptionObjet);
        StartCoroutine(DemanderObjet3D());
    }

    IEnumerator DemanderObjet3D()
    {
        // 1. Formatage du texte en structure JSON
        Debug.Log("1. Formatage du texte en structure JSON");
        string contenuJson = "{\"prompt\": \"" + descriptionObjet + "\"}";

        // 2. Création de la requête POST
        Debug.Log("2. Création de la requête POST");
        using (UnityWebRequest www = UnityWebRequest.Post(urlApi, contenuJson, "application/json"))
        {
            // 3. Définition du chemin de sauvegarde (ici dans le dossier Assets/Models du projet)
            // Assure-toi que le dossier "Models" existe déjà dans tes Assets
            Debug.Log("3. Définition du chemin de sauvegarde");
            string cheminSauvegarde = Path.Combine(Application.dataPath, "Models", (Regex.Replace(descriptionObjet, @"\s", "") + ".glb"));

            // 4. Configuration du DownloadHandler pour écrire directement sur le disque
            Debug.Log("4. Configuration du DownloadHandler pour écrire directement sur le disque");
            www.downloadHandler = new DownloadHandlerFile(cheminSauvegarde);

            Debug.Log("Transmission de la demande à l'IA en cours...");

            // 5. Envoi et mise en attente sans bloquer le jeu
            Debug.Log("5. Envoi et mise en attente sans bloquer le jeu");
            yield return www.SendWebRequest();

            // 6. Vérification du résultat & spawn
            Debug.Log("6. Vérification du résultat");
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Échec de la génération : " + www.error);
            }
            else
            {
                Debug.Log("Succès ! L'objet 3D a été sauvegardé ici : " + cheminSauvegarde);

                spawner.SpawnObject(cheminSauvegarde);
            }
        }
    }
}