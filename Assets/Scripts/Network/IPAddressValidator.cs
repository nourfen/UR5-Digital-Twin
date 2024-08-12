using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class IPAddressValidator : MonoBehaviour
{
    public TMP_InputField inputField;

    // Regex pattern to allow only digits and dots
    private string allowedCharactersPattern = @"[^0-9.]";

    void Start()
    {
        // Add listener to filter input as it changes
        inputField.onValueChanged.AddListener(FilterInput);
    }

    void OnDestroy()
    {
        // Remove listener when the object is destroyed to prevent memory leaks
        inputField.onValueChanged.RemoveListener(FilterInput);
    }

    void FilterInput(string input)
    {
        // Store the current caret position
        int caretPosition = inputField.caretPosition;

        // Remove any characters that do not match the allowed pattern
        string filteredInput = Regex.Replace(input, allowedCharactersPattern, "");

        // If input was modified, update the text and reset the caret position
        if (!filteredInput.Equals(input))
        {
            inputField.text = filteredInput;
            // Restore the caret position
            inputField.caretPosition = caretPosition - (input.Length - filteredInput.Length);
        }
    }
}
