using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaArrowButton : MonoBehaviour
{
    public ExitArea ExitArea { get; set; }
    public void OnClick()
    {
        ExitArea.OnArrowClick();
    }
}
