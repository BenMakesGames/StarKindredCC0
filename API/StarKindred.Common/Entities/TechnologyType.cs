namespace StarKindred.Common.Entities;

public enum TechnologyType
{
    // economy

    WoodWorking,
        FinishingI,
            FinishingII,
                FinishingIII,
        ArchitectureI,
            ArchitectureII,
                ArchitectureIII,

    Blacksmithing,
        HushingI, // get 50 gold from marble pit power
            HushingII, // +50 gold
        CupronickleI, // fishery power: get 200 gold
            CupronickleII, // +50 gold
                CupronickleIII, // +50 gold
        ScrappingI,
            ScrappingII,

    FreeTrade,
        GemsI,
            GemsII,
        MapMakingI,
            MapMakingII,
                MapMakingIII,
                    MapMakingIV,

    // cosmology

    Ritual,
        ShamanismI,
            ShamanismII,
        TithesI,
            TithesII,
                TithesIII,
                    TithesIV,

    Divination,
        SacrificeI,
            SacrificeII,
                SacrificeIII,
        AstrologyI,
            AstrologyII,
                AstrologyIII,
                    AstrologyIV,

    // defense
    Militarization,
        FoundryI,
            FoundryII,
        MilitiaI,
            MilitiaII,

    Expansion,
        TrackingI,
            TrackingII,
                TrackingIII,
        RunnerNetworkI,
            RunnerNetworkII,
                RunnerNetworkIII,
                    RunnerNetworkIV,

    // culture
    TownBeautification, // +2 decorations
        PublicArtI, // +4 more decorations (total of +6)
            PublicArtII, // +6
                PublicArtIII, // +8
                    PublicArtIV, // +10 (total of +30)
        TourismI, // new palace power: new settlers
            TourismII, // settler missions are +5 levels higher, max 100
                TourismIII, // settler missions are another +5 levels higher (total of +10), max 100

    Music, // palace can make lyres
        // ???
}