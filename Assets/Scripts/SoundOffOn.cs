using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundOffOn : MonoBehaviour
{
    private Sprite SoundOnImage;
    public Sprite SoundOffImage;
    public Button button;
    private bool isOn = true;

    public AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        SoundOnImage = button.image.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ButtonClicked()
    {
        if (isOn)
        {
            button.image.sprite = SoundOffImage;
            isOn = false;
            audioSource.mute = true;
        }
        else
        {
            button.image.sprite = SoundOnImage;
            isOn = true;
            audioSource.mute = false;
        }
    }
}
