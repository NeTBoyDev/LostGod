using System;
using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController.Examples;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct Replique
{
    public string Sender;
    public string Text;
}

public class Dialogue : MonoBehaviour
{
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private string CharacterInfo;
    [SerializeField] public List<Replique> History = new();

    public TMP_InputField _Input;
    public TMP_Text AnswerText;

    private AudioSource _source;
    private Canvas _canvas;

    private void Start()
    {
        _source = gameObject.AddComponent<AudioSource>();
        _canvas = GetComponent<Canvas>();
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _canvas.enabled = !_canvas.enabled;

            Cursor.visible = _canvas.enabled;
            Cursor.lockState = _canvas.enabled ? CursorLockMode.None : CursorLockMode.Locked;

            FindObjectOfType<ExamplePlayer>().MayMove = !_canvas.enabled;
        }
    }

    public void SendText()
    {
        if (!_canvas.enabled)
            return;
        var text = _Input.text;
        StartCoroutine(GroqAPI.SendRequest(text, CharacterInfo,History, result =>
        {
            StartCoroutine(WriteText(result));
            StartCoroutine(PlaySounds());
            History.Add(new Replique(){Sender = "Я",Text = text});
            History.Add(new Replique(){Sender = "Ты",Text = result});
        }));
    }

    private bool isWriting;
    private IEnumerator WriteText(string text)
    {
        AnswerText.text = string.Empty;
        _Input.text = string.Empty;
        isWriting = true;
        foreach (var c in text)
        {
            yield return new WaitForSeconds(0.05f);
            AnswerText.text += c;
        }
        isWriting = false;
    }

    private IEnumerator PlaySounds()
    {
        while (isWriting)
        {
            var clip = _clips[Random.Range(0, _clips.Length)];
            _source.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }
    }
}
