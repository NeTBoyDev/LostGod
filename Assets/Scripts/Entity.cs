using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IIenteractable
{
    [SerializeField] [TextArea(4, 8)] public string CharacterDescription;
    [SerializeField] private bool isFemale;
    [SerializeField] private string Name;

    public void Interact()
    {
        var dialogue = FindObjectOfType<Dialogue>();
        dialogue.CharacterInfo = CharacterDescription;
        dialogue.isFemale = isFemale;
        dialogue.CharacterName.text = Name;
    }
}
