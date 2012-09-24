// -----------------------------------------------------------------------
// <copyright file="AbilityType.cs">
// Copyright 2012 Robert Nix, Will Eddins
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    /// <summary>
    /// The type of an ability.
    /// </summary>
    public enum AbilityType
    {
        // This is meant to describe every ability type, i.e. CAbil * Buttons.
        // Hand-made with love.

        // Unknown
        Unknown = 0,

        // Basic movement
        Attack,
        Move,
        Stop,
        HoldPosition,
        HoldFire,
        Patrol,

        // Rallies
        SetWorkerRally,
        SetUnitRally,

        // Cancels
        CancelQueue,
        CancelResearch,
        CancelTrain,
        CancelBuilding,
        CancelWarpIn,
        CancelChannel,
        CancelMorph,
        Cancel,

        // Harvest
        Gather,
        ReturnCargo,

        // Build - Terran
        BuildCommandCenter,
        BuildSupplyDepot,
        BuildRefinery,
        BuildBarracks,
        BuildFactory,
        BuildStarport,
        BuildEngineeringBay,
        BuildArmory,
        BuildGhostAcademy,
        BuildFusionCore,
        BuildBunker,
        BuildMissileTurret,
        BuildSensorTower,
        BuildReactor,
        BuildTechLab,
        // Build - Protoss
        BuildNexus,
        BuildPylon,
        BuildAssimilator,
        BuildGateway,
        BuildForge,
        BuildFleetBeacon,
        BuildTwilightCouncil,
        BuildPhotonCannon,
        BuildStargate,
        BuildTemplarArchive,
        BuildDarkShrine,
        BuildRoboticsBay,
        BuildRoboticsFacility,
        BuildCyberneticsCore,
        // Build - Zerg
        BuildHatchery,
        BuildExtractor,
        BuildSpawningPool,
        BuildEvolutionChamber,
        BuildHydraliskDen,
        BuildSpire,
        BuildUltraliskCavern,
        BuildInfestationPit,
        BuildNydusNetwork,
        BuildNydusCanal,
        BuildBanelingNest,
        BuildRoachWarren,
        BuildSpineCrawler,
        BuildSporeCrawler,

        // Unit Training - Terran
        TrainSCV,
        TrainGhost,
        TrainMarauder,
        TrainMarine,
        TrainReaper,
        TrainHellion,
        TrainSiegeTank,
        TrainThor,
        TrainBanshee,
        TrainBattlecruiser,
        TrainMedivac,
        TrainRaven,
        TrainViking,
        // Unit Training - Protoss
        TrainProbe, // NexusTrain
        TrainZealot, // GatewayTrain
        TrainStalker,
        TrainSentry,
        TrainHighTemplar,
        TrainDarkTemplar,
        TrainColossus, // RoboticsFacilityTrain
        TrainImmortal,
        TrainObserver,
        TrainWarpPrism,
        TrainCarrier, // StargateTrain
        TrainPhoenix,
        TrainVoidRay,
        WarpInZealot, // WarpGateTrain
        WarpInStalker,
        WarpInSentry,
        WarpInHighTemplar,
        WarpInDarkTemplar,
        TrainMothership,
        // Unit Training - Zerg
        TrainDrone,
        TrainOverlord,
        TrainQueen,
        TrainZergling,
        TrainRoach,
        TrainHydralisk,
        TrainInfestor,
        TrainMutalisk,
        TrainCorruptor,
        TrainUltralisk,

        // Terran Flying Buildings
        LiftCommandCenter,
        LiftOrbitalCommand,
        LiftBarracks,
        LiftFactory,
        LiftStarport,
        LandCommandCenter,
        LandOrbitalCommand,
        LandBarracks,
        LandFactory,
        LandStarport,

        // Burrow
        BurrowBaneling,
        BurrowDrone,
        BurrowHydralisk,
        BurrowInfestor,
        BurrowInfestedTerran,
        BurrowQueen,
        BurrowRoach,
        BurrowUltralisk,
        BurrowZergling,
        UprootSpineCrawler,
        UprootSporeCrawler,
        // Unburrow
        UnburrowBaneling,
        UnburrowDrone,
        UnburrowHydralisk,
        UnburrowInfestor,
        UnburrowInfestedTerran,
        UnburrowQueen,
        UnburrowRoach,
        UnburrowUltralisk,
        UnburrowZergling,
        RootSpineCrawler,
        RootSporeCrawler,

        // Morph - Terran
        SiegeTankSiege,
        SiegeTankUnsiege,
        VikingLand,
        VikingLift,
        SupplyDepotLower,
        SupplyDepotRaise,
        MorphToOrbitalCommand,
        MorphToPlanetaryFortress,
        // Morph - Protoss
        MorphToWarpGate,
        MorphToGateway,
        WarpPrismPhasing,
        WarpPrismTransport,
        // Morph - Zerg
        MorphToLair,
        MorphToHive,
        MorphToGreaterSpire,
        MorphToBaneling,
        MorphToOverseer,
        MorphToBroodLord,

        // Upgrade Research - Terran
        ResearchStimpack, // Barracks Tech Lab
        ResearchCombatShields,
        ResearchPunisherGrenades,
        ResearchReaperSpeed,
        ResearchBlueFlame, // Factory Tech Lab
        ResearchSiegeTech,
        ResearchStrikeCannons,
        ResearchBansheeCloak, // Starport Tech Lab
        ResearchDurableMaterials,
        ResearchSeekerMissile,
        ResearchMedivacEnergy,
        ResearchRavenEnergy,
        ResearchGhostEnergy, // Ghost Academy
        ResearchGhostCloak,
        ResearchHiSecAutoTracking, // Engineering Bay
        ResearchNeosteelFrame,
        ResearchTerranBuildingArmor,
        ResearchTerranInfantryArmors1,
        ResearchTerranInfantryArmors2,
        ResearchTerranInfantryArmors3,
        ResearchTerranInfantryWeapons1,
        ResearchTerranInfantryWeapons2,
        ResearchTerranInfantryWeapons3,
        ResearchTerranShipArmors1, // Armory
        ResearchTerranShipArmors2,
        ResearchTerranShipArmors3,
        ResearchTerranShipWeapons1,
        ResearchTerranShipWeapons2,
        ResearchTerranShipWeapons3,
        ResearchTerranVehicleArmors1,
        ResearchTerranVehicleArmors2,
        ResearchTerranVehicleArmors3,
        ResearchTerranVehicleWeapons1,
        ResearchTerranVehicleWeapons2,
        ResearchTerranVehicleWeapons3,
        ResearchBattlecruiserEnergy, // Fusion Core
        ResearchYamatoCannon,
        // Upgrade Research - Protoss
        ResearchWarpGate, // Cybernetics Core
        ResearchHallucination,
        ResearchProtossAirArmors1,
        ResearchProtossAirArmors2,
        ResearchProtossAirArmors3,
        ResearchProtossAirWeapons1,
        ResearchProtossAirWeapons2,
        ResearchProtossAirWeapons3,
        ResearchProtossGroundArmors1, // Forge
        ResearchProtossGroundArmors2,
        ResearchProtossGroundArmors3,
        ResearchProtossGroundWeapons1,
        ResearchProtossGroundWeapons2,
        ResearchProtossGroundWeapons3,
        ResearchProtossShields1,
        ResearchProtossShields2,
        ResearchProtossShields3,
        ResearchBlink, // Twilight Council
        ResearchCharge,
        ResearchPsiStorm, // Templar Archives
        ResearchKhaydarinAmulet, // Templar Archives - Removed
        ResearchExtendedThermalLance, // Robotics Bay
        ResearchWarpPrismSpeed,
        ResearchObserverSpeed,
        ResearchFluxVanes, // Fleet Beacon - Removed
        ResearchPhoenixRange, // Fleet Beacon
        ResearchCarrierWeaponSpeed,
        // Upgrade Research - Zerg
        ResearchZerglingAttackSpeed, // Spawning Pool
        ResearchZerglingMovementSpeed,
        ResearchRoachSpeed, // Roach Warren
        ResearchRoachTunnelingClaws,
        ResearchBurrow, // Lair
        ResearchOverlordSpeed,
        ResearchVentralSacs,
        ResearchZergFlyerArmors1, // Spire
        ResearchZergFlyerArmors2,
        ResearchZergFlyerArmors3,
        ResearchZergFlyerWeapons1,
        ResearchZergFlyerWeapons2,
        ResearchZergFlyerWeapons3,
        ResearchZergGroundArmors1, // Evolution Chamber
        ResearchZergGroundArmors2,
        ResearchZergGroundArmors3,
        ResearchZergMeleeWeapons1,
        ResearchZergMeleeWeapons2,
        ResearchZergMeleeWeapons3,
        ResearchZergMissileWeapons1,
        ResearchZergMissileWeapons2,
        ResearchZergMissileWeapons3,
        ResearchInfestorEnergy, // Infestation Pit
        ResearchNeuralParasite,
        ResearchHydraliskSpeed, // Hydralisk Den
        ResearchBanelingSpeed, // Baneling Nest
        ResearchUltraliskArmor, // Ultralisk Cavern

        // Spell Casts - Terran
        RavenBuildAutoTurret,
        RavenBuildPointDefenseDrone,
        RavenSeekerMissile,
        GhostCloak,
        GhostDecloak,
        GhostHoldFire,
        GhostWeaponsFree,
        GhostSnipe,
        GhostNuke,
        GhostEMP,
        BansheeCloak,
        BansheeDecloak,
        UseStimpack,
        SCVRepair,
        MULERepair,
        MedivacHeal,
        BattlecruiserYamato,
        CalldownMULE,
        CalldownSupply,
        CalldownScannerSweep,
        ThorStrikeCannons,
        // Spell Casts - Protoss
        ZealotCharge,
        StalkerBlink,
        SentryForceField,
        SentryGuardianShield,
        SentryHallucinationArchon, // Perhaps consolidate these?
        SentryHallucinationColossus,
        SentryHallucinationHighTemplar,
        SentryHallucinationImmortal,
        SentryHallucinationPhoenix,
        SentryHallucinationProbe,
        SentryHallucinationStalker,
        SentryHallucinationVoidRay,
        SentryHallucinationWarpPrism,
        SentryHallucinationZealot,
        HighTemplarFeedback,
        HighTemplarPsiStorm,
        MothershipMassRecall,
        MothershipVortex,
        PhoenixGravitonBeam,
        NexusChronoBoost,
        // Spell Casts - Zerg
        BanelingExplode,
        BanelingEnableBuildingAttack,
        BanelingDisableBuildingAttack,
        /// <summary> &lt; 1.4.0 </summary>
        BanelingBuildingAttack,
        CorruptorCorruption,
        QueenSpawnLarva,
        QueenTransfusion,
        QueenBuildCreepTumor,
        CreepTumorBuildCreepTumor,
        OverlordGenerateCreep,
        OverlordStopGenerateCreep,
        OverseerSpawnChangeling,
        OverseerContaminate,
        InfestorSpawnInfestedTerran,
        InfestorFungalGrowth,
        InfestorNeuralParasite,

        // Transport - Terran
        CommandCenterLoad,
        CommandCenterUnload,
        CommandCenterUnloadAll,
        MedivacLoad,
        MedivacUnload,
        MedivacUnloadAll,
        BunkerLoad,
        BunkerUnload,
        BunkerUnloadAll,
        // Transport - Protoss
        WarpPrismLoad,
        WarpPrismUnload,
        WarpPrismUnloadAll,
        // Transport - Zerg
        OverlordLoad,
        OverlordUnload,
        OverlordUnloadAll,
        NydusLoad,
        NydusUnload,
        NydusUnloadAll,

        // Misc - Terran
        ArmNuke,
        BunkerSalvage,
        // Misc - Protoss
        ArmInterceptor,
        MergeArchon,
        // Misc - Zerg
        ArmBroodling,

        // Misc
        Taunt // /dance, /cheer
    }
}
