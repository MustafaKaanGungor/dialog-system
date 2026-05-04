using UnityEngine;

public class NPC : MonoBehaviour
{
    public CharacterSO characterData;

    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}
