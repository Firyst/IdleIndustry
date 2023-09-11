using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] List<Animation> animations;
    [SerializeField] List<Button> buttons;
    // Start is called before the first frame update
    void Start()
    {
        // hook all buttons and animations
        for (int buttonI=0; buttonI<buttons.Count; buttonI++) 
        {
            int buttonId = buttonI;
            buttons[buttonI].onClick.AddListener(() => ManageAnimation(buttonId));
            animations[buttonI]["Open"].speed = 0;
        }
    }

    void ManageAnimation(int index)
    {
        var anim = animations[index];
        if (anim["Open"].time == 0 && anim["Open"].speed <= 0)
        {
            // from close to open
            anim["Open"].speed = 1;
            anim.Play();
        } else if (anim["Open"].time == 0 && anim["Open"].speed > 0)
        {
            // from full open to closed
            anim["Open"].speed = -1;
            anim["Open"].time = anim["Open"].length;
            anim.Play();
        }

    }
}
