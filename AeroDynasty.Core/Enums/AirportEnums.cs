﻿namespace AeroDynasty.Core.Enums
{
    public enum AirportType
    {
        International,
        Regional,
        Domestic
    }

    public enum AirportSize
    {
        Largest,
        VeryLarge,
        Large,
        Medium,
        Small,
        VerySmall,
        Smallest
    }

    public enum AirportExpansionType
    {
        NewRunway,
        ChangeRunwayLength,
        ChangeRunwaySurface,
        ChangeRunwayName,
        CloseRunway,
        NewTerminal,
    }

    public enum RunwaySurface
    {
        Asphalt,
        Gravel,
        Water,
        Grass,
        Concrete,
        Sand,
        Bitumen,
        Dirt,
        Turf,
        PSP,
        Soil,
        Clay,
        Coral,
        Wood
    }
}
