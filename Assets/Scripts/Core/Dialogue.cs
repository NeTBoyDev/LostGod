using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KinematicCharacterController.Examples;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
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

    [SerializeField] private LayerMask EntityMask;
    
    public TMP_InputField _Input;
    public TMP_Text AnswerText;
    public TMP_Text CharacterName;
    public Entity Character;

    public Image Crosshair;
    public Sprite Default, Active;

    private AudioSource _source;
    private CanvasGroup _canvas;

    private Camera Camera;

    private Coroutine WriteCoroutine;
    private Coroutine AudioCoroutine;

    private bool isDialogueOpen;

    private void Start()
    {
        _source = gameObject.AddComponent<AudioSource>();
        _canvas = GetComponentInChildren<CanvasGroup>();
        Camera = GetComponent<Camera>();


        var volume = FindObjectOfType<Volume>();
        AnimateEyeOpening(volume.profile, 1);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CloseDialogue();
        }

        
            var ray = new Ray(Camera.main.transform.position,Camera.main.transform.forward);
            if (Physics.Raycast(ray, out var hit, 2.5f,EntityMask))
            {
                Crosshair.sprite = Active;
                if (Input.GetKeyDown(KeyCode.Mouse0) && !isDialogueOpen)
                {
                    if (hit.collider.gameObject.TryGetComponent(out IIenteractable i))
                    {
                        i.Interact();
                        OpenDialogue();
                    }
                }
            }
            else
            {
                Crosshair.sprite = Default;
            }
            
        
    }

    public void OpenDialogue()
    {
        isDialogueOpen = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        FindObjectOfType<ExamplePlayer>().MayMove = false;
        DOTween.To(() => _canvas.alpha, x => _canvas.alpha = x, 1, .25f);

        if (Character != null && !string.IsNullOrWhiteSpace(Character.StartText))
        {
            SendText(Character.StartText);
        }
    }

    public void CloseDialogue()
    {
        isDialogueOpen = false;
        Character = null;
        AnswerText.text = string.Empty;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        FindObjectOfType<ExamplePlayer>().MayMove = true;
        History.Clear();
        DOTween.To(() => _canvas.alpha, x => _canvas.alpha = x, 0, .25f);
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

            if (result.Contains("[Event]"))
            {
                result = result.Replace("Event", "");
                Character.Invoke(InvokeType.Event);
            }
            if (result.Contains("[Aggressive]"))
            {
                result = result.Replace("[Aggressive]", "");
                Character.Invoke(InvokeType.Aggressive);
            }
            if (result.Contains("[Stop]"))
            {
                result = result.Replace("[Stop]", "");
                Character.Invoke(InvokeType.Stop);
            }
            if (result.Contains("[Quest]"))
            {
                result = result.Replace("[Quest]", "");
                Character.Invoke(InvokeType.Quest);
            }
            
            WriteCoroutine = StartCoroutine(WriteText(result));
            AudioCoroutine = StartCoroutine(PlaySounds());
            
            History.Add(new Replique(){Sender = "Ты",Text = result});
        }));
    }
    
    public void SendText(string text)
    {
        if (!_canvas.enabled)
            return;
        History.Add(new Replique(){Sender = "Я",Text = text});
        StartCoroutine(GroqAPI.SendRequest(text, CharacterInfo,History, result =>
        {
            if (WriteCoroutine != null) 
                StopCoroutine(WriteCoroutine);
            if (AudioCoroutine != null) 
                StopCoroutine(AudioCoroutine);

            if (result.Contains("[Event]"))
            {
                result.Replace("Event", "");
                Character.Invoke(InvokeType.Event);
            }
            if (result.Contains("[Aggressive]"))
            {
                result.Replace("[Aggressive]", "");
                Character.Invoke(InvokeType.Aggressive);
            }
            if (result.Contains("[Stop]"))
            {
                result.Replace("[Stop]", "");
                Character.Invoke(InvokeType.Stop);
            }
            if (result.Contains("[Quest]"))
            {
                result.Replace("[Quest]", "");
                Character.Invoke(InvokeType.Quest);
            }
            
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
    public void AnimateEyeOpening(VolumeProfile postProcessingProfile, float duration = 1f)
    {
        // Находим компонент EyeOpeningEffect в профиле постобработки
        if (postProcessingProfile.TryGet<EyeOpeningEffectVolume>(out var eyeEffect))
        {
            // Устанавливаем начальное значение EyeOpenness
            eyeEffect._EyeOpenness.value = 0f;
            
            // Создаем анимацию с помощью DOTween
            DOTween.To(
                () => eyeEffect._EyeOpenness.value,           // Геттер
                x => eyeEffect._EyeOpenness.value = x,       // Сеттер
                1f,                                         // Конечное значение
                duration                                    // Длительность анимации
            ).SetEase(Ease.InOutSine);                      // Тип смягчения анимации
        }
        else
        {
            Debug.LogWarning("EyeOpeningEffect не найден в профиле постобработки!");
        }
    }
}
