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
    [SerializeField] private AudioClip[] _femaleclips;
    public bool isFemale;
    [SerializeField] public string CharacterInfo;
    [SerializeField] public List<Replique> History = new();

    public TMP_InputField _Input;
    public TMP_Text AnswerText;
    public TMP_Text CharacterName;

    private AudioSource _source;
    private Canvas _canvas;

    private Camera Camera;

    private Coroutine WriteCoroutine;
    private Coroutine AudioCoroutine;

    private void Start()
    {
        _source = gameObject.AddComponent<AudioSource>();
        _canvas = GetComponent<Canvas>();
        Camera = GetComponent<Camera>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CloseDialogue();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var ray = new Ray(Camera.main.transform.position,Camera.main.transform.forward);
            if (Physics.Raycast(ray, out var hit, 2))
            {
                if (hit.collider.gameObject.TryGetComponent(out IIenteractable i))
                {
                    i.Interact();
                    OpenDialogue();
                }
            }
            
        }
    }

    public void OpenDialogue()
    {
        _canvas.enabled = true;

        Cursor.visible = _canvas.enabled;
        Cursor.lockState = _canvas.enabled ? CursorLockMode.None : CursorLockMode.Locked;

        FindObjectOfType<ExamplePlayer>().MayMove = !_canvas.enabled;
        
    }

    public void CloseDialogue()
    {
        _canvas.enabled = false;
        AnswerText.text = string.Empty;

        Cursor.visible = _canvas.enabled;
        Cursor.lockState = _canvas.enabled ? CursorLockMode.None : CursorLockMode.Locked;

        FindObjectOfType<ExamplePlayer>().MayMove = !_canvas.enabled;
        History.Clear();
    }

    public void SendText()
    {
        if (!_canvas.enabled)
            return;
        var text = _Input.text;
        History.Add(new Replique(){Sender = "Я",Text = text});
        StartCoroutine(GroqAPI.SendRequest(text, CharacterInfo,History, result =>
        {
            if (WriteCoroutine != null) 
                StopCoroutine(WriteCoroutine);
            if (AudioCoroutine != null) 
                StopCoroutine(AudioCoroutine);
            
            WriteCoroutine = StartCoroutine(WriteText(result));
            AudioCoroutine = StartCoroutine(PlaySounds());
            
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
            var clip = isFemale ? _femaleclips[Random.Range(0, _femaleclips.Length)] : _clips[Random.Range(0, _clips.Length)];
            _source.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }
    }
}
