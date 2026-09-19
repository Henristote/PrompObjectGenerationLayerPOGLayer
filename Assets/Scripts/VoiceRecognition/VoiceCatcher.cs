using UnityEngine;
using TMPro; 
using Meta.WitAi.Dictation; 
using System.IO;
using System.Collections;


#if UNITY_ANDROID
using UnityEngine.Android; 
#endif


public class VoiceCatcher : MonoBehaviour
{
    [SerializeField] private DictationService dictationExperience;
    [SerializeField] private TextMeshProUGUI uiTextDisplay; 

    private string logPath;
    private Coroutine clearTextCoroutine;

    [SerializeField] private GenerateurShapE generateModel;


    void Start()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif
        Debug.Log("Voice Catcher initialized. Log path: " + logPath);
        //generateModel.LAYER_TOKEN = LAYER_TOKEN;
        //generateModel.WORKSPACE_ID = WORKSPACE_ID;

        //StartCoroutine(generateModel.GetAvailableModelsCoroutine());

        logPath = Path.Combine(Application.persistentDataPath, "voice_logs.txt");

        // Événement pour le texte qui s'affiche au fur et à mesure
        dictationExperience.DictationEvents.OnPartialTranscription.AddListener(UpdateUI);

        // Événement pour la phrase finale enregistrée
        dictationExperience.DictationEvents.OnFullTranscription.AddListener(SaveAndFinalize);
    }

    private void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.One)) // Vérifie si le bouton A est pressé
        {
            if (clearTextCoroutine != null) StopCoroutine(clearTextCoroutine);
            dictationExperience.Activate();
            Debug.Log("Dictation Activate statut : " + (dictationExperience.Active == true));
        }

        if(OVRInput.GetUp(OVRInput.Button.One)) // Vérifie si le bouton A est relâché
        {
            dictationExperience.Deactivate();
            Debug.Log("Dictation Desactivate statut : " + (dictationExperience.Active == false));
        }

        if(OVRInput.GetDown(OVRInput.Button.Two)) // Vérifie si le bouton B est pressé
        {
            //if (clearTextCoroutine != null) StopCoroutine(clearTextCoroutine)
                //GenerateModel.Instantiate();
        }

        if(OVRInput.GetUp(OVRInput.Button.Two)) // Vérifie si le bouton B est relâché
        {
            dictationExperience.Deactivate();
        }
    }

    private void UpdateUI(string partialText)
    {
        if (uiTextDisplay != null)
            uiTextDisplay.text = partialText;
    }

    private void SaveAndFinalize(string fullText)
    {
        // On sauvegarde le résultat final dans les logs
        Debug.Log("SaveAndFinalize Start");
        using (StreamWriter sw = File.AppendText(logPath))
        {
            Debug.Log("Writing to log file: " + logPath);
            sw.WriteLine($"[{System.DateTime.Now}] : {fullText}");
            Debug.Log("Text saved to log file: " + fullText);
        }
        Debug.Log("generateModel null ? : " + generateModel != null);
        Debug.Log("Text loggé ? : " + string.IsNullOrEmpty(fullText) + "text : " + fullText);

        // Optionnel : on peut ajouter un feedback visuel ou réinitialiser le texte
        if (uiTextDisplay != null)
        {
            uiTextDisplay.text = fullText;
            clearTextCoroutine = StartCoroutine(ClearTextAfterDelay(20f));
        }
        if (generateModel != null && !string.IsNullOrEmpty(fullText))
        {
            Debug.Log("Starting model generation with prompt: " + fullText);
            generateModel.StartGeneration(fullText);
        }
    }

    private IEnumerator ClearTextAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        if (uiTextDisplay.text != null)
        {
            uiTextDisplay.text = "";
        }
    }
}