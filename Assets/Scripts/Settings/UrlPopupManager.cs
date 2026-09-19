using UnityEngine;
using TMPro;

public class UrlPopupManager : MonoBehaviour
{
    [SerializeField] private GenerateurShapE generateurShapE;
    [SerializeField] private TMP_InputField inputFieldUrl;

    [Header("Interface Manager")]
    [SerializeField] private GameObject popupPanel; // UI parent panel for the popup
    [SerializeField] private GameObject mainPanel; // UI parent panel for the main interface

    //private TouchScreenKeyboard keyboard;

    private void Start()
    {
        // Ensure the popup panel is active at the start
        if (mainPanel != null) mainPanel.SetActive(false);
        if (popupPanel != null) popupPanel.SetActive(true);

        //inputFieldUrl.onSelect.AddListener(OpenKeyboard);

    }

    //private void OpenKeyboard(string text)
    //{
    //    // Open the on-screen keyboard when the input field is selected
    //    if (TouchScreenKeyboard.isSupported)
    //    {
    //        keyboard = TouchScreenKeyboard.Open(inputFieldUrl.text, TouchScreenKeyboardType.URL, false, false, false, false, "Enter URL");
    //    }
    //}

    private void Update()
    {
        //inputFieldUrl.text = keyboard.text;

        //// If the keyboard is open, update the input field text with the keyboard text
        //if (keyboard.status == TouchScreenKeyboard.Status.Done)
        //{
        //    CheckAndClose();
        //}
    }

    public void CheckAndClose()
    {
        if (!string.IsNullOrEmpty(inputFieldUrl.text))
        {
            // send the url to the generateurShapE script
            generateurShapE.SetApiUrl(inputFieldUrl.text);

            // disable the popup panel
            popupPanel.SetActive(false);

            if (popupPanel != null) popupPanel.SetActive(false);
            if (mainPanel != null) mainPanel.SetActive(true);

            //if (keyboard != null)
            //{
            //    keyboard.active = false;
            //}
        }
        else
        {
            Debug.LogWarning("The entered URL is empty.");
        }
    }
    void OnDestroy()
    {
        // Remove the listener when the object is destroyed to prevent memory leaks
        //inputFieldUrl.onSelect.RemoveListener(OpenKeyboard);
    }
}