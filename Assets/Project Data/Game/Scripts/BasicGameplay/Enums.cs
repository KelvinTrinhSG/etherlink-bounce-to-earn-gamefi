
public enum PlatformType
{
    Normal = 0, // normal platform should be always at 0 index
    Jump = 1,
    Spiked = 2,
    Mob = 3,
    Moving = 4,
    Shield = 5,
    Invisible = 6,
    Background = 7,
}

public enum Location
{
    JumpCanyon = 0,
    EnergyFieldCanyon = 1,
    Portal = 2,
    None = 3,
}

public enum Zone
{
    Jump = 0,
    LongDist = 1,
    Columns = 2,
    Spiked = 3,
    Mob = 4,
    Invis = 5,
    Appearing = 6,
    None = 7,
}

public enum PowerUpType
{
    EnergyField = 0,
    Shield = 1,
    Booster = 2,
}

public enum DeathReason
{
    Falling = 0,
    Spikes = 1,
    Mob = 2,
    ChalangeRule = 3,
}

public enum ComboState
{
    None = 0,
    Basic = 1,
    Middle = 2,
    Ultra = 3,
}