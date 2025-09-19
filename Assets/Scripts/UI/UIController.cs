using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public List<Image> heartImages;

    public Sprite heartFull, heartEmpty;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHealthUI()
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < Character.Instance.maxHealth)
            {
                heartImages[i].gameObject.SetActive(true);
                heartImages[i].sprite = i < Character.Instance.currentHealth ? heartFull : heartEmpty;
            }
            else
            {
                heartImages[i].gameObject.SetActive(false);
            }
        }
    }
}
