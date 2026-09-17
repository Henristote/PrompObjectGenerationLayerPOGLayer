using UnityEngine;
using GLTFast;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Grab;
using Oculus.Interaction.GrabAPI;
using System;
using System.IO;
using Unity.VisualScripting;

public class SpawnerObject3D : MonoBehaviour
{
    public float distanceDevantJoueur = 2.0f;

    [Tooltip("put  (avec le script Grabbable)")]
    public GameObject prefab;

    public async void SpawnObject(string filePath)
    {
        string nomObjet = Path.GetFileNameWithoutExtension(filePath);

        Transform cameraJoueur = Camera.main.transform;
        Vector3 positionSpawn = cameraJoueur.position + cameraJoueur.forward * distanceDevantJoueur;

        Vector3 directionRegard = cameraJoueur.forward;
        directionRegard.y = Math.Abs(directionRegard.y);
        Quaternion rotationSpawn = directionRegard.sqrMagnitude > 0.001f ? Quaternion.LookRotation(directionRegard) : Quaternion.identity;

        if (prefab == null) return;

        // On instancie le parent (le Model3D) et on le désactive
        GameObject model3D = Instantiate(prefab, positionSpawn, rotationSpawn);
        model3D.name = nomObjet;
        model3D.SetActive(false);

        // On récupère le Grabbable du parent
        Grabbable grabbable = model3D.GetComponent<Grabbable>();
        if (grabbable == null)
        {
            Debug.LogError("Le prefab Model3D doit contenir un Grabbable !");
            Destroy(model3D);
            return;
        }

        var gltf = new GltfImport();
        bool succes = await gltf.Load($"file://{filePath}");

        if (succes)
        {
            await gltf.InstantiateMainSceneAsync(model3D.transform);

            // On récupère tous les éléments de rendu générés
            MeshRenderer[] renderers = model3D.GetComponentsInChildren<MeshRenderer>(true);

            // Pour l'exemple, on considère qu'il n'y a qu'un seul objet principal généré
            // Si le modèle est composé de plusieurs parties, tu pourrais vouloir appliquer
            // un Rigidbody global sur le parent. Ici on l'applique sur chaque sous-partie.
            foreach (MeshRenderer rend in renderers)
            {
                GameObject objetFils = rend.gameObject;

                // --- Ajout du Collider ---
                MeshCollider mc = objetFils.AddComponent<MeshCollider>();
                mc.convex = true;

                // --- Ajout du Rigidbody sur l'ENFANT ---
                Rigidbody rbFils = objetFils.GetComponent<Rigidbody>();
                if (rbFils == null)
                {
                    rbFils = objetFils.AddComponent<Rigidbody>();
                }

                // --- ON INJECTE LE RIGIDBODY DANS LE GRABBABLE DU PARENT ---
                // C'est cette ligne qui relie la physique de l'enfant à la logique de grab du parent
                grabbable.InjectOptionalRigidbody(rbFils);

                // --- Ajout des scripts d'interaction sur l'ENFANT ---
                // Ces scripts gèrent l'interaction directe (toucher)
                GrabInteractable grab = objetFils.AddComponent<GrabInteractable>();
                grab.InjectAllGrabInteractable(rbFils);
                grab.InjectOptionalPointableElement(grabbable);

                HandGrabInteractable handGrab = objetFils.AddComponent<HandGrabInteractable>();
                handGrab.InjectAllHandGrabInteractable(GrabTypeFlags.All, rbFils, GrabbingRule.DefaultPinchRule, GrabbingRule.DefaultPalmRule);
                handGrab.InjectOptionalPointableElement(grabbable);

                // Ces scripts gèrent l'interaction à distance (pointer)
                DistanceGrabInteractable distGrab = objetFils.AddComponent<DistanceGrabInteractable>();
                distGrab.InjectAllGrabInteractable(rbFils);
                distGrab.InjectOptionalPointableElement(grabbable);

                DistanceHandGrabInteractable distHandGrab = objetFils.AddComponent<DistanceHandGrabInteractable>();
                distHandGrab.InjectAllDistanceHandGrabInteractable(GrabTypeFlags.All, rbFils, GrabbingRule.DefaultPinchRule, GrabbingRule.DefaultPalmRule);
                distHandGrab.InjectOptionalPointableElement(grabbable);
            }

            // On réactive l'objet pour initialiser les scripts Oculus
            model3D.SetActive(true);

#if UNITY_EDITOR
            string dossierPrefabs = "Assets/Models_Generes";
            if (!Directory.Exists(dossierPrefabs))
            {
                Directory.CreateDirectory(dossierPrefabs);
            }

            string cheminPrefab = $"{dossierPrefabs}/{nomObjet}.prefab";
            UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(model3D, cheminPrefab, UnityEditor.InteractionMode.UserAction);

            Debug.Log($"Le modèle a spawn et a été sauvegardé en Prefab ici : {cheminPrefab}");
#else
            Debug.Log("Le modèle a spawn avec succès !");
#endif
        }
        else
        {
            Debug.LogError("Échec du chargement du fichier GLB.");
            Destroy(model3D);
        }
    }
}