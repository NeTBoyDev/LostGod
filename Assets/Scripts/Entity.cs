
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IIenteractable,IInvokable<InvokeType>
{
    [SerializeField] [TextArea(4, 8)] public string CharacterDescription;
    [SerializeField] [TextArea(4, 8)] public string StartText;
    [SerializeField] private bool isFemale;
    [SerializeField] private string Name;

    private Animator Animator;

    private void Start()
    {
        Animator = GetComponent<Animator>();
    }

    public void Pray()
    {
        StartCoroutine(PrayCoroutine());
    }

    private IEnumerator PrayCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.5f));
        Animator.SetTrigger("Pray");
    }

    public void Interact()
    {
        var dialogue = FindObjectOfType<Dialogue>();
        dialogue.CharacterInfo = CharacterDescription;
        dialogue.isFemale = isFemale;
        dialogue.CharacterName.text = Name;
        dialogue.Character = this;
    }

    public void Invoke(InvokeType parameter)
    {
        print(parameter.ToString());
    }
}
