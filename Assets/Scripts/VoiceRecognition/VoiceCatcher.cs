using UnityEngine;
using TMPro; // Nécessaire pour manipuler le texte
using Meta.WitAi.Dictation; // Namespace pour DictationExperience
using System.IO;
using System.Collections;

public class VoiceCatcher : MonoBehaviour
{
    [SerializeField] private DictationService dictationExperience;
    [SerializeField] private TextMeshProUGUI uiTextDisplay; // Glissez votre texte ici

    private string logPath;
    private Coroutine clearTextCoroutine;

    void Start()
    {
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
        }

        if(OVRInput.GetUp(OVRInput.Button.One)) // Vérifie si le bouton A est pressé
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
        using (StreamWriter sw = File.AppendText(logPath))
        {
            sw.WriteLine($"[{System.DateTime.Now}] : {fullText}");
        }

        // Optionnel : on peut ajouter un feedback visuel ou réinitialiser le texte
        if (uiTextDisplay != null)
        {
            uiTextDisplay.text = fullText;
            clearTextCoroutine = StartCoroutine(ClearTextAfterDelay(20f));
        }
    }

    private IEnumerator ClearTextAfterDelay(float delay) {
        yield return new WaitForSeconds(20f);
        if (uiTextDisplay.text != null)
        {
            uiTextDisplay.text = "";
        }
    }
}