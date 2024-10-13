using System;
using System.Collections.Generic;

[Serializable]
public class PassengerWave
{
    public int numberOfPassenger;
    public ColorType colorType;
}

[Serializable]
public class GridPassengerList
{
    public List<GridPassenger> gridPassengers;
}
