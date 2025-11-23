using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    public void TriggerAction()
    {
        PlayerBasic.Instance.SaveGame();
        AudioManager.Instance.PlayCoinSFX();
    }
}
