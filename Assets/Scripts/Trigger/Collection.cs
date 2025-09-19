using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour, IInteractable

{
    public int collectionId;
    public void TriggerAction() 
    {
        PlayerBasic.Instance.UnlockCollection(collectionId);
        AudioManager.Instance.PlayCoinSFX();
        Destroy(gameObject);
    }
}
