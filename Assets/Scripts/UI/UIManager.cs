using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject noteDisplayPanel;
    public TextMeshProUGUI noteTextUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional, solo si necesitas que persista entre escenas.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (noteDisplayPanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Return))
        {
            noteDisplayPanel.SetActive(false);
        }
    }

    public void ShowNote(string noteText)
    {
        noteTextUI.text = noteText;
        noteDisplayPanel.SetActive(true);
    }
}

