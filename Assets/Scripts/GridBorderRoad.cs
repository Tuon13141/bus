using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBorderRoad : GridRoad
{
    [SerializeField] GridBorderRoad previousBorderRoad;
    [SerializeField] GridBorderRoad nextBorderRoad;

    [SerializeField] GridMainRoad mainRoad;
}
