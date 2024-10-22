using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridExitArea : Grid
{
    [SerializeField] ExitArea exitArea;
    public int IndexInLevelDesigner { get; set; }
    public override void OnStart()
    {
        exitArea = GetComponent<ExitArea>();
        exitArea.GridExitArea = this;
    }
}
