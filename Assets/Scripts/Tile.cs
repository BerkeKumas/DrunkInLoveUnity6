using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{
    private TextMeshProUGUI tileText;

    private void Awake()
    {
        tileText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetLetter(char letter)
    {
        tileText.text = letter.ToString();
    }
}
