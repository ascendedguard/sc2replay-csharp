// -----------------------------------------------------------------------
// <copyright file="AbilityData.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser.Version
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides version dispatch for Ability types
    /// </summary>
    public static class AbilityData
    {
        public static AbilityType GetAbilityType(int typeId, int buttonId, int buildNumber)
        {
            Dictionary<int, Dictionary<int, AbilityType>> versionData;
            if (AbilityTypeData.TryGetValue(buildNumber, out versionData))
            {
                Dictionary<int, AbilityType> cAbilData;
                if (versionData.TryGetValue(typeId, out cAbilData))
                {
                    AbilityType value;
                    if (cAbilData.TryGetValue(buttonId, out value))
                    {
                        return value;
                    }
                    else
                    {
                        // Generic cancel for simple CAbilEffect buttons, since
                        // effect cancels generally don't broadcast an event.
                        if (buttonId == 1)
                        {
                            return AbilityType.Cancel;
                        }
                        if (cAbilData.ContainsKey(0))
                        {
                            return cAbilData[0];
                        }
                    }
                }
            }
            return AbilityType.Unknown;
        }

        // Build number => CAbil Index => Button Index => AbilityType
        // Use references to 21029 in other versions by creating instances to
        // reference (i.e. Dictionary<int, AbilityType> CAbilName21029) and updating the
        // original and the new to point to that reference.
        static Dictionary<int, Dictionary<int, Dictionary<int, AbilityType>>> AbilityTypeData
            = new Dictionary<int, Dictionary<int, Dictionary<int, AbilityType>>>()
        {
            {21029, new Dictionary<int, Dictionary<int, AbilityType>>()
            {
                {0x23, new Dictionary<int, AbilityType>() // Taunt
                {
                    {0, AbilityType.Taunt},
                    {1, AbilityType.Cancel}
                }},
                {0x24, new Dictionary<int, AbilityType>() // stop
                {
                    {0, AbilityType.Stop},
                    {1, AbilityType.HoldFire},
                    {2, AbilityType.Taunt}, // /cheer
                    {3, AbilityType.Taunt} // /dance
                }},
                {0x25, new Dictionary<int, AbilityType>() // HoldFire
                {
                    {0, AbilityType.Stop},
                    {1, AbilityType.HoldFire},
                    {2, AbilityType.Taunt}, // /cheer
                    {3, AbilityType.Taunt} // /dance
                }},
                {0x26, new Dictionary<int, AbilityType>() // move
                {
                    {0, AbilityType.Move},
                    {1, AbilityType.Patrol},
                    {2, AbilityType.HoldPosition},
                    {3, AbilityType.Move}, // Scan Move
                    {4, AbilityType.Move}
                }},
                {0x2A, new Dictionary<int, AbilityType>() // attack
                {
                    {0, AbilityType.Attack}
                }},
                {0x2B, new Dictionary<int, AbilityType>() // TerranAddOns
                {
                    {0, AbilityType.BuildTechLab},
                    {1, AbilityType.BuildReactor},
                    {30, AbilityType.CancelBuilding}
                }},
                {0x32, new Dictionary<int, AbilityType>() // Corruption
                {
                    {0, AbilityType.CorruptorCorruption}
                }},
                {0x33, new Dictionary<int, AbilityType>() // GhostHoldFire
                {
                    {0, AbilityType.GhostHoldFire}
                }},
                {0x34, new Dictionary<int, AbilityType>() // GhostWeaponsFree
                {
                    {0, AbilityType.GhostWeaponsFree}
                }},
                {0x36, new Dictionary<int, AbilityType>() // Explode
                {
                    {0, AbilityType.BanelingExplode}
                }},
                {0x37, new Dictionary<int, AbilityType>() // FleetBeaconResearch
                {
                    {0, AbilityType.Unknown}, // Removed -- Flux Vanes
                    {1, AbilityType.ResearchCarrierWeaponSpeed},
                    {2, AbilityType.ResearchPhoenixRange},
                    {30, AbilityType.CancelResearch}
                }},
                {0x38, new Dictionary<int, AbilityType>() // FungalGrowth
                {
                    {0, AbilityType.InfestorFungalGrowth}
                }},
                {0x39, new Dictionary<int, AbilityType>() // Guardian Shield
                {
                    {0, AbilityType.SentryGuardianShield}
                }},
                {0x3A, new Dictionary<int, AbilityType>() // MULERepair
                {
                    {0, AbilityType.MULERepair},
                    {1, AbilityType.CancelChannel}
                }},
                {0x3B, new Dictionary<int, AbilityType>() // MorphZerglingToBaneling
                {
                    {0, AbilityType.MorphToBaneling},
                    {1, AbilityType.CancelMorph}
                }},
                {0x3C, new Dictionary<int, AbilityType>() // NexusTrainMothership
                {
                    {0, AbilityType.TrainMothership},
                    {30, AbilityType.CancelTrain}
                }},
                {0x3D, new Dictionary<int, AbilityType>() // Feedback
                {
                    {0, AbilityType.HighTemplarFeedback}
                }},
                {0x3E, new Dictionary<int, AbilityType>() // MassRecall
                {
                    {0, AbilityType.MothershipMassRecall}
                }},
                {0x3F, new Dictionary<int, AbilityType>() // PlacePointDefenseDrone
                {
                    {0, AbilityType.RavenBuildPointDefenseDrone}
                }},
                {0x40, new Dictionary<int, AbilityType>() // HallucinationArchon
                {
                    {0, AbilityType.SentryHallucinationArchon}
                }},
                {0x41, new Dictionary<int, AbilityType>() // HallucinationColossus
                {
                    {0, AbilityType.SentryHallucinationColossus}
                }},
                {0x42, new Dictionary<int, AbilityType>() // HallucinationHighTemplar
                {
                    {0, AbilityType.SentryHallucinationHighTemplar}
                }},
                {0x43, new Dictionary<int, AbilityType>() // HallucinationImmortal
                {
                    {0, AbilityType.SentryHallucinationImmortal}
                }},
                {0x44, new Dictionary<int, AbilityType>() // HallucinationPhoenix
                {
                    {0, AbilityType.SentryHallucinationPhoenix}
                }},
                {0x45, new Dictionary<int, AbilityType>() // HallucinationProbe
                {
                    {0, AbilityType.SentryHallucinationProbe}
                }},
                {0x46, new Dictionary<int, AbilityType>() // HallucinationStalker
                {
                    {0, AbilityType.SentryHallucinationStalker}
                }},
                {0x47, new Dictionary<int, AbilityType>() // HallucinationVoidRay
                {
                    {0, AbilityType.SentryHallucinationVoidRay}
                }},
                {0x48, new Dictionary<int, AbilityType>() // HallucinationWarpPrism
                {
                    {0, AbilityType.SentryHallucinationWarpPrism}
                }},
                {0x49, new Dictionary<int, AbilityType>() // HallucinationZealot
                {
                    {0, AbilityType.SentryHallucinationZealot}
                }},
                {0x4A, new Dictionary<int, AbilityType>() // MULEGather
                {
                    {0, AbilityType.Gather},
                    {1, AbilityType.ReturnCargo},
                    {2, AbilityType.Cancel}
                }},
                {0x4B, new Dictionary<int, AbilityType>() // SeekerMissile
                {
                    {0, AbilityType.RavenSeekerMissile}
                }},
                {0x4C, new Dictionary<int, AbilityType>() // CalldownMULE
                {
                    {0, AbilityType.CalldownMULE}
                }},
                {0x4D, new Dictionary<int, AbilityType>() // GravitonBeam
                {
                    {0, AbilityType.PhoenixGravitonBeam},
                    {1, AbilityType.CancelChannel}
                }},
                {0x4E, new Dictionary<int, AbilityType>() // BuildInProgressNydusCanal
                {
                    {0, AbilityType.CancelBuilding},
                    {1, AbilityType.CancelBuilding}
                }},
                {0x51, new Dictionary<int, AbilityType>() // SpawnChangeling
                {
                    {0, AbilityType.OverseerSpawnChangeling}
                }},
                {0x58, new Dictionary<int, AbilityType>() // Rally
                {
                    {0, AbilityType.SetUnitRally}
                }},
                {0x59, new Dictionary<int, AbilityType>() // ProgressRally
                {
                    {0, AbilityType.SetUnitRally}
                }},
                {0x5A, new Dictionary<int, AbilityType>() // RallyCommand
                {
                    {0, AbilityType.SetWorkerRally}
                }},
                {0x5B, new Dictionary<int, AbilityType>() // RallyNexus
                {
                    {0, AbilityType.SetUnitRally}
                }},
                {0x5C, new Dictionary<int, AbilityType>() // RallyHatchery
                {
                    {0, AbilityType.SetUnitRally},
                    {1, AbilityType.SetWorkerRally}
                }},
                {0x5D, new Dictionary<int, AbilityType>() // RoachWarrenResearch
                {
                    {0, AbilityType.Unknown}, // Removed -- Organic Carapace
                    {1, AbilityType.ResearchRoachSpeed},
                    {2, AbilityType.ResearchRoachTunnelingClaws}
                }},
                {0x5F, new Dictionary<int, AbilityType>() // InfestedTerrans
                {
                    {0, AbilityType.InfestorSpawnInfestedTerran}
                }},
                {0x60, new Dictionary<int, AbilityType>() // NeuralParasite
                {
                    {0, AbilityType.InfestorNeuralParasite}
                }},
                {0x61, new Dictionary<int, AbilityType>() // SpawnLarva
                {
                    {0, AbilityType.QueenSpawnLarva}
                }},
                {0x62, new Dictionary<int, AbilityType>() // StimpackMarauder
                {
                    {0, AbilityType.UseStimpack}
                }},
                {0x63, new Dictionary<int, AbilityType>() // SupplyDrop
                {
                    {0, AbilityType.CalldownSupply}
                }},
                {0x64, new Dictionary<int, AbilityType>() // 250mmStrikeCannons
                {
                    {0, AbilityType.ThorStrikeCannons}
                }},
                {0x66, new Dictionary<int, AbilityType>() // TimeWarp
                {
                    {0, AbilityType.NexusChronoBoost}
                }},
                {0x67, new Dictionary<int, AbilityType>() // UltraliskCavernResearch
                {
                    {0, AbilityType.Unknown}, // Removed -- Cleaving Talons?
                    {1, AbilityType.Unknown}, // Removed -- Anabolic Synthesis
                    {2, AbilityType.ResearchUltraliskArmor},
                }},
                {0x69, new Dictionary<int, AbilityType>() // SCVHarvest
                {
                    {0, AbilityType.Gather},
                    {1, AbilityType.ReturnCargo},
                    {2, AbilityType.Cancel}
                }},
                {0x6A, new Dictionary<int, AbilityType>() // ProbeHarvest
                {
                    {0, AbilityType.Gather},
                    {1, AbilityType.ReturnCargo},
                    {2, AbilityType.Cancel}
                }},
                {0x6B, new Dictionary<int, AbilityType>() // AttackWarpPrism
                {
                    {0, AbilityType.Attack}
                }},
                {0x6C, new Dictionary<int, AbilityType>() // que1
                { // Remember:  delegate event info from ability flags, not AbilityType
                    {0, AbilityType.CancelQueue}, // Cancel last (1 ?? 0)
                    {1, AbilityType.CancelQueue} // Cancel specific
                }},
                {0x6D, new Dictionary<int, AbilityType>() // que5
                {
                    {0, AbilityType.CancelQueue},
                    {1, AbilityType.CancelQueue}
                }},
                {0x6E, new Dictionary<int, AbilityType>() // que5LongBlend
                {
                    {0, AbilityType.CancelQueue},
                    {1, AbilityType.CancelQueue}
                }},
                {0x6F, new Dictionary<int, AbilityType>() // que5Passive
                {
                    {0, AbilityType.CancelQueue},
                    {1, AbilityType.CancelQueue}
                }},
                {0x70, new Dictionary<int, AbilityType>() // que5Addon
                {
                    {0, AbilityType.CancelQueue},
                    {1, AbilityType.CancelQueue}
                }},
                {0x71, new Dictionary<int, AbilityType>() // BuildInProgress
                {
                    {0, AbilityType.CancelBuilding},
                    {1, AbilityType.CancelBuilding}
                }},
                {0x72, new Dictionary<int, AbilityType>() // Repair
                {
                    {0, AbilityType.SCVRepair}
                }},
                {0x73, new Dictionary<int, AbilityType>() // TerranBuild
                {
                    {0, AbilityType.BuildCommandCenter},
                    {1, AbilityType.BuildSupplyDepot},
                    {2, AbilityType.BuildRefinery},
                    {3, AbilityType.BuildBarracks},
                    {4, AbilityType.BuildEngineeringBay},
                    {5, AbilityType.BuildMissileTurret},
                    {6, AbilityType.BuildBunker},
                    {8, AbilityType.BuildSensorTower},
                    {9, AbilityType.BuildGhostAcademy},
                    {10, AbilityType.BuildFactory},
                    {11, AbilityType.BuildStarport},
                    {13, AbilityType.BuildArmory},
                    {15, AbilityType.BuildFusionCore},
                    {30, AbilityType.CancelBuilding}
                }},
                {0x74, new Dictionary<int, AbilityType>() // RavenBuild
                {
                    {0, AbilityType.RavenBuildAutoTurret},
                    {30, AbilityType.CancelBuilding}
                }},
                {0x75, new Dictionary<int, AbilityType>() // Stimpack
                {
                    {0, AbilityType.UseStimpack}
                }},
                {0x76, new Dictionary<int, AbilityType>() // GhostCloak
                {
                    {0, AbilityType.GhostCloak},
                    {1, AbilityType.GhostDecloak}
                }},
                {0x77, new Dictionary<int, AbilityType>() // Snipe
                {
                    {0, AbilityType.GhostSnipe}
                }},
                {0x78, new Dictionary<int, AbilityType>() // MedivacHeal
                {
                    {0, AbilityType.MedivacHeal}
                }},
                {0x79, new Dictionary<int, AbilityType>() // SiegeMode
                {
                    {0, AbilityType.SiegeTankSiege}
                }},
                {0x7A, new Dictionary<int, AbilityType>() // Unsiged
                {
                    {0, AbilityType.SiegeTankUnsiege}
                }},
                {0x7B, new Dictionary<int, AbilityType>() // BansheeCloak
                {
                    {0, AbilityType.BansheeCloak},
                    {1, AbilityType.BansheeDecloak}
                }},
                {0x7C, new Dictionary<int, AbilityType>() // MedivacTransport
                {
                    {0, AbilityType.MedivacLoad},
                    {1, AbilityType.MedivacUnloadAll},
                    {2, AbilityType.MedivacUnloadAll},
                    {3, AbilityType.MedivacUnload},
                    {4, AbilityType.MedivacLoad}
                }},
                {0x7D, new Dictionary<int, AbilityType>() // ScannerSweep
                {
                    {0, AbilityType.CalldownScannerSweep}
                }},
                {0x7E, new Dictionary<int, AbilityType>() // Yamato
                {
                    {0, AbilityType.BattlecruiserYamato}
                }},
                {0x7F, new Dictionary<int, AbilityType>() // AssaultMode
                {
                    {0, AbilityType.VikingLand}
                }},
                {0x80, new Dictionary<int, AbilityType>() // FighterMode
                {
                    {0, AbilityType.VikingLift}
                }},
                {0x81, new Dictionary<int, AbilityType>() // BunkerTransport
                {
                    {0, AbilityType.BunkerLoad},
                    {1, AbilityType.BunkerUnloadAll},
                    {2, AbilityType.BunkerUnload},
                    {3, AbilityType.BunkerUnload},
                    {4, AbilityType.BunkerLoad}
                }},
                {0x82, new Dictionary<int, AbilityType>() // CommandCenterTransport
                {
                    {0, AbilityType.CommandCenterLoad},
                    {1, AbilityType.CommandCenterUnloadAll},
                    {2, AbilityType.CommandCenterUnload},
                    {3, AbilityType.CommandCenterUnload},
                    {4, AbilityType.CommandCenterLoad}
                }},
                {0x83, new Dictionary<int, AbilityType>() // CommandCenterLiftOff
                {
                    {0, AbilityType.LiftCommandCenter}
                }},
                {0x84, new Dictionary<int, AbilityType>() // CommandCenterLand
                {
                    {0, AbilityType.LandCommandCenter}
                }},
                {0x85, new Dictionary<int, AbilityType>() // BarracksAddOns
                {
                    {0, AbilityType.BuildTechLab},
                    {1, AbilityType.BuildReactor},
                    {30, AbilityType.CancelBuilding}
                }},
                {0x86, new Dictionary<int, AbilityType>() // BarracksLiftOff
                {
                    {0, AbilityType.LiftBarracks}
                }},
                {0x87, new Dictionary<int, AbilityType>() // FactoryAddOns
                {
                    {0, AbilityType.BuildTechLab},
                    {1, AbilityType.BuildReactor},
                    {30, AbilityType.CancelBuilding}
                }},
                {0x88, new Dictionary<int, AbilityType>() // FactoryLiftOff
                {
                    {0, AbilityType.LiftFactory}
                }},
                {0x89, new Dictionary<int, AbilityType>() // StarportAddOns
                {
                    {0, AbilityType.BuildTechLab},
                    {1, AbilityType.BuildReactor},
                    {30, AbilityType.CancelBuilding}
                }},
                {0x8A, new Dictionary<int, AbilityType>() // StarportLiftOff
                {
                    {0, AbilityType.LiftStarport}
                }},
                {0x8B, new Dictionary<int, AbilityType>() // FactoryLand
                {
                    {0, AbilityType.LandFactory}
                }},
                {0x8C, new Dictionary<int, AbilityType>() // StarportLand
                {
                    {0, AbilityType.LandStarport}
                }},
                {0x8D, new Dictionary<int, AbilityType>() // CommandCenterTrain
                {
                    {0, AbilityType.TrainSCV},
                    {30, AbilityType.CancelTrain}
                }},
                {0x8E, new Dictionary<int, AbilityType>() // BarracksLand
                {
                    {0, AbilityType.LandBarracks}
                }},
                {0x8F, new Dictionary<int, AbilityType>() // SupplyDepotLower
                {
                    {0, AbilityType.SupplyDepotLower}
                }},
                {0x90, new Dictionary<int, AbilityType>() // SupplyDepotRaise
                {
                    {0, AbilityType.SupplyDepotRaise}
                }},
                {0x91, new Dictionary<int, AbilityType>() // BarracksTrain
                {
                    {0, AbilityType.TrainMarine},
                    {1, AbilityType.TrainReaper},
                    {2, AbilityType.TrainGhost},
                    {3, AbilityType.TrainMarauder},
                    {30, AbilityType.CancelTrain}
                }},
                {0x92, new Dictionary<int, AbilityType>() // FactoryTrain
                {
                    {0, AbilityType.Unknown}, // Removed - ?
                    {1, AbilityType.TrainSiegeTank},
                    {4, AbilityType.TrainThor},
                    {5, AbilityType.TrainHellion},
                    {30, AbilityType.CancelTrain}
                }},
                {0x93, new Dictionary<int, AbilityType>() // StarportTrain
                {
                    {0, AbilityType.TrainMedivac},
                    {1, AbilityType.TrainBanshee},
                    {2, AbilityType.TrainRaven},
                    {3, AbilityType.TrainBattlecruiser},
                    {4, AbilityType.TrainViking},
                    {30, AbilityType.CancelTrain}
                }},
                {0x94, new Dictionary<int, AbilityType>() // EngineeringBayResearch
                {
                    {0, AbilityType.ResearchHiSecAutoTracking},
                    {1, AbilityType.ResearchTerranBuildingArmor},
                    {2, AbilityType.ResearchTerranInfantryWeapons1},
                    {3, AbilityType.ResearchTerranInfantryWeapons2},
                    {4, AbilityType.ResearchTerranInfantryWeapons3},
                    {5, AbilityType.ResearchNeosteelFrame},
                    {6, AbilityType.ResearchTerranInfantryArmors1},
                    {7, AbilityType.ResearchTerranInfantryArmors2},
                    {8, AbilityType.ResearchTerranInfantryArmors3},
                    {30, AbilityType.CancelResearch}
                }},
                {0x96, new Dictionary<int, AbilityType>() // ArmSiloWithNuke
                {
                    {0, AbilityType.ArmNuke}
                }},
                {0x97, new Dictionary<int, AbilityType>() // BarracksTechLabResearch
                {
                    {0, AbilityType.ResearchStimpack},
                    {1, AbilityType.ResearchCombatShields},
                    {2, AbilityType.ResearchPunisherGrenades},
                    {30, AbilityType.CancelResearch}
                }},
                {0x98, new Dictionary<int, AbilityType>() // FactoryTechLabResearch
                {
                    {0, AbilityType.ResearchSiegeTech},
                    {1, AbilityType.ResearchBlueFlame},
                    {2, AbilityType.ResearchStrikeCannons},
                    {30, AbilityType.CancelResearch}
                }},
                {0x99, new Dictionary<int, AbilityType>() // StarportTechLabResearch
                {
                    {0, AbilityType.ResearchBansheeCloak},
                    {2, AbilityType.ResearchMedivacEnergy},
                    {3, AbilityType.ResearchRavenEnergy},
                    {6, AbilityType.ResearchSeekerMissile},
                    {7, AbilityType.ResearchDurableMaterials},
                    {30, AbilityType.CancelResearch}
                }},
                {0x9A, new Dictionary<int, AbilityType>() // GhostAcademyResearch
                {
                    {0, AbilityType.ResearchGhostCloak},
                    {1, AbilityType.ResearchGhostEnergy},
                    {30, AbilityType.CancelResearch}
                }},
                {0x9B, new Dictionary<int, AbilityType>() // ArmoryResearch
                {
                    {2, AbilityType.ResearchTerranVehicleArmors1},
                    {3, AbilityType.ResearchTerranVehicleArmors2},
                    {4, AbilityType.ResearchTerranVehicleArmors3},
                    {5, AbilityType.ResearchTerranVehicleWeapons1},
                    {6, AbilityType.ResearchTerranVehicleWeapons2},
                    {7, AbilityType.ResearchTerranVehicleWeapons3},
                    {8, AbilityType.ResearchTerranShipArmors1},
                    {9, AbilityType.ResearchTerranShipArmors2},
                    {10, AbilityType.ResearchTerranShipArmors3},
                    {11, AbilityType.ResearchTerranShipWeapons1},
                    {12, AbilityType.ResearchTerranShipWeapons2},
                    {13, AbilityType.ResearchTerranShipWeapons3},
                    {30, AbilityType.CancelResearch}
                }},
                {0x9C, new Dictionary<int, AbilityType>() // ProtossBuild
                {
                    {0, AbilityType.BuildNexus},
                    {1, AbilityType.BuildPylon},
                    {2, AbilityType.BuildAssimilator},
                    {3, AbilityType.BuildGateway},
                    {4, AbilityType.BuildForge},
                    {5, AbilityType.BuildFleetBeacon},
                    {6, AbilityType.BuildTwilightCouncil},
                    {7, AbilityType.BuildPhotonCannon},
                    {9, AbilityType.BuildStargate},
                    {10, AbilityType.BuildTemplarArchive},
                    {11, AbilityType.BuildDarkShrine},
                    {12, AbilityType.BuildRoboticsBay},
                    {13, AbilityType.BuildRoboticsFacility},
                    {14, AbilityType.BuildCyberneticsCore},
                    {30, AbilityType.CancelBuilding}
                }},
                {0x9D, new Dictionary<int, AbilityType>() // WarpPrismTransport
                {
                    {0, AbilityType.WarpPrismLoad},
                    {1, AbilityType.WarpPrismUnloadAll},
                    {2, AbilityType.WarpPrismUnloadAll},
                    {3, AbilityType.WarpPrismUnload},
                    {4, AbilityType.WarpPrismLoad},
                }},
                {0x9E, new Dictionary<int, AbilityType>() // GatewayTrain
                {
                    {0, AbilityType.TrainZealot},
                    {1, AbilityType.TrainStalker},
                    {3, AbilityType.TrainHighTemplar},
                    {4, AbilityType.TrainDarkTemplar},
                    {5, AbilityType.TrainSentry},
                    {30, AbilityType.CancelTrain}
                }},
                {0x9F, new Dictionary<int, AbilityType>() // StargateTrain
                {
                    {0, AbilityType.TrainPhoenix},
                    {2, AbilityType.TrainCarrier},
                    {4, AbilityType.TrainVoidRay},
                    {30, AbilityType.CancelTrain}
                }},
                {0xA0, new Dictionary<int, AbilityType>() // RoboticsFacilityTrain
                {
                    {0, AbilityType.TrainWarpPrism},
                    {1, AbilityType.TrainObserver},
                    {2, AbilityType.TrainColossus},
                    {3, AbilityType.TrainImmortal},
                    {30, AbilityType.CancelTrain}
                }},
                {0xA1, new Dictionary<int, AbilityType>() // NexusTrain
                {
                    {0, AbilityType.TrainProbe},
                    {30, AbilityType.CancelTrain}
                }},
                {0xA2, new Dictionary<int, AbilityType>() // PsiStorm
                {
                    {0, AbilityType.HighTemplarPsiStorm}
                }},
                {0xA3, new Dictionary<int, AbilityType>() // HangarQueue5
                {
                    {0, AbilityType.CancelQueue},
                    {1, AbilityType.CancelQueue}
                }},
                {0xA4, new Dictionary<int, AbilityType>() // BroodLordQueue2
                {
                    {0, AbilityType.CancelQueue},
                    {1, AbilityType.CancelQueue}
                }},
                {0xA5, new Dictionary<int, AbilityType>() // CarrierHanger
                {
                    {0, AbilityType.ArmInterceptor}
                }},
                {0xA6, new Dictionary<int, AbilityType>() // ForgeResearch
                {
                    {0, AbilityType.ResearchProtossGroundWeapons1},
                    {1, AbilityType.ResearchProtossGroundWeapons2},
                    {2, AbilityType.ResearchProtossGroundWeapons3},
                    {3, AbilityType.ResearchProtossGroundArmors1},
                    {4, AbilityType.ResearchProtossGroundArmors2},
                    {5, AbilityType.ResearchProtossGroundArmors3},
                    {6, AbilityType.ResearchProtossShields1},
                    {7, AbilityType.ResearchProtossShields2},
                    {8, AbilityType.ResearchProtossShields3},
                    {30, AbilityType.CancelResearch}
                }},
                {0xA7, new Dictionary<int, AbilityType>() // RoboticsBayResearch
                {
                    {1, AbilityType.ResearchObserverSpeed},
                    {2, AbilityType.ResearchWarpPrismSpeed},
                    {5, AbilityType.ResearchExtendedThermalLance},
                    {30, AbilityType.CancelResearch}
                }},
                {0xA8, new Dictionary<int, AbilityType>() // TemplarArchivesResearch
                {
                    {4, AbilityType.ResearchPsiStorm},
                    {30, AbilityType.CancelResearch}
                }},
                {0xA9, new Dictionary<int, AbilityType>() // ZergBuild
                {
                    {0, AbilityType.BuildHatchery},
                    {2, AbilityType.BuildExtractor},
                    {3, AbilityType.BuildSpawningPool},
                    {4, AbilityType.BuildEvolutionChamber},
                    {5, AbilityType.BuildHydraliskDen},
                    {6, AbilityType.BuildSpire},
                    {7, AbilityType.BuildUltraliskCavern},
                    {8, AbilityType.BuildInfestationPit},
                    {9, AbilityType.BuildNydusNetwork},
                    {10, AbilityType.BuildBanelingNest},
                    {13, AbilityType.BuildRoachWarren},
                    {14, AbilityType.BuildSpineCrawler},
                    {15, AbilityType.BuildSporeCrawler},
                    {30, AbilityType.CancelBuilding}
                }},
                {0xAA, new Dictionary<int, AbilityType>() // DroneHarvest
                {
                    {0, AbilityType.Gather},
                    {1, AbilityType.ReturnCargo},
                    {2, AbilityType.Cancel}
                }},
                {0xAB, new Dictionary<int, AbilityType>() // EvolutionChamberResearch
                {
                    {0, AbilityType.ResearchZergMeleeWeapons1},
                    {1, AbilityType.ResearchZergMeleeWeapons2},
                    {2, AbilityType.ResearchZergMeleeWeapons3},
                    {3, AbilityType.ResearchZergGroundArmors1},
                    {4, AbilityType.ResearchZergGroundArmors2},
                    {5, AbilityType.ResearchZergGroundArmors3},
                    {6, AbilityType.ResearchZergMissileWeapons1},
                    {7, AbilityType.ResearchZergMissileWeapons2},
                    {8, AbilityType.ResearchZergMissileWeapons3},
                    {30, AbilityType.CancelResearch}
                }},
                {0xAC, new Dictionary<int, AbilityType>() // UpgradeToLair
                {
                    {0, AbilityType.MorphToLair},
                    {1, AbilityType.CancelMorph}
                }},
                {0xAD, new Dictionary<int, AbilityType>() // UpgradeToHive
                {
                    {0, AbilityType.MorphToHive},
                    {1, AbilityType.CancelMorph}
                }},
                {0xAE, new Dictionary<int, AbilityType>() // UpgradeToGreaterSpire
                {
                    {0, AbilityType.MorphToGreaterSpire},
                    {1, AbilityType.CancelMorph}
                }},
                {0xAF, new Dictionary<int, AbilityType>() // LairResearch
                {
                    {1, AbilityType.ResearchOverlordSpeed},
                    {2, AbilityType.ResearchVentralSacs},
                    {3, AbilityType.ResearchBurrow},
                    {30, AbilityType.CancelResearch}
                }},
                {0xB0, new Dictionary<int, AbilityType>() // SpawningPoolResearch
                {
                    {0, AbilityType.ResearchZerglingAttackSpeed},
                    {1, AbilityType.ResearchZerglingMovementSpeed},
                    {30, AbilityType.CancelResearch}
                }},
                {0xB1, new Dictionary<int, AbilityType>() // HydraliskDenResearch
                {
                    {2, AbilityType.ResearchHydraliskSpeed},
                    {30, AbilityType.CancelResearch}
                }},
                {0xB2, new Dictionary<int, AbilityType>() // SpireResearch
                {
                    {0, AbilityType.ResearchZergFlyerWeapons1},
                    {1, AbilityType.ResearchZergFlyerWeapons2},
                    {2, AbilityType.ResearchZergFlyerWeapons3},
                    {3, AbilityType.ResearchZergFlyerArmors1},
                    {4, AbilityType.ResearchZergFlyerArmors2},
                    {5, AbilityType.ResearchZergFlyerArmors3},
                    {30, AbilityType.CancelResearch}
                }},
                {0xB3, new Dictionary<int, AbilityType>() // LarvaTrain
                {
                    {0, AbilityType.TrainDrone},
                    {1, AbilityType.TrainZergling},
                    {2, AbilityType.TrainOverlord},
                    {3, AbilityType.TrainHydralisk},
                    {4, AbilityType.TrainMutalisk},
                    {6, AbilityType.TrainUltralisk},
                    {9, AbilityType.TrainRoach},
                    {10, AbilityType.TrainInfestor},
                    {11, AbilityType.TrainCorruptor},
                    {30, AbilityType.CancelTrain}
                }},
                {0xB4, new Dictionary<int, AbilityType>() // MorphToBroodLord
                {
                    {0, AbilityType.MorphToBroodLord},
                    {1, AbilityType.CancelMorph}
                }},
                {0xB5, new Dictionary<int, AbilityType>() // BurrowBanelingDown
                {
                    {0, AbilityType.BurrowBaneling}
                }},
                {0xB6, new Dictionary<int, AbilityType>() // BurrowBanelingUp
                {
                    {0, AbilityType.UnburrowBaneling}
                }},
                {0xB7, new Dictionary<int, AbilityType>() // BurrowDroneDown
                {
                    {0, AbilityType.BurrowDrone}
                }},
                {0xB8, new Dictionary<int, AbilityType>() // BurrowDroneUp
                {
                    {0, AbilityType.UnburrowDrone}
                }},
                {0xB9, new Dictionary<int, AbilityType>() // BurrowHydraliskDown
                {
                    {0, AbilityType.BurrowHydralisk}
                }},
                {0xBA, new Dictionary<int, AbilityType>() // BurrowHydraliskUp
                {
                    {0, AbilityType.UnburrowHydralisk}
                }},
                {0xBB, new Dictionary<int, AbilityType>() // BurrowRoachDown
                {
                    {0, AbilityType.BurrowRoach}
                }},
                {0xBC, new Dictionary<int, AbilityType>() // BurrowRoachUp
                {
                    {0, AbilityType.UnburrowRoach}
                }},
                {0xBD, new Dictionary<int, AbilityType>() // BurrowZerglingDown
                {
                    {0, AbilityType.BurrowZergling}
                }},
                {0xBE, new Dictionary<int, AbilityType>() // BurrowZerglingUp
                {
                    {0, AbilityType.UnburrowZergling}
                }},
                {0xBF, new Dictionary<int, AbilityType>() // BurrowInfestorTerranDown
                {
                    {0, AbilityType.BurrowInfestedTerran}
                }},
                {0xC0, new Dictionary<int, AbilityType>() // BurrowInfestorTerranUp
                {
                    {0, AbilityType.UnburrowInfestedTerran}
                }},
                {0xC5, new Dictionary<int, AbilityType>() // OverlordTransport
                {
                    {0, AbilityType.OverlordLoad},
                    {1, AbilityType.OverlordUnloadAll},
                    {2, AbilityType.OverlordUnloadAll},
                    {3, AbilityType.OverlordUnload},
                    {4, AbilityType.OverlordLoad}
                }},
                {0xC6, new Dictionary<int, AbilityType>() // Mergeable
                {
                    {0, AbilityType.Cancel}
                }},
                {0xC7, new Dictionary<int, AbilityType>() // Warpable
                {
                    {0, AbilityType.CancelWarpIn}
                }},
                {0xC8, new Dictionary<int, AbilityType>() // WarpGateTrain
                {
                    {0, AbilityType.WarpInZealot},
                    {1, AbilityType.WarpInStalker},
                    {3, AbilityType.WarpInHighTemplar},
                    {4, AbilityType.WarpInDarkTemplar},
                    {5, AbilityType.WarpInSentry},
                    {30, AbilityType.CancelWarpIn}
                }},
                {0xC9, new Dictionary<int, AbilityType>() // BurrowQueenDown
                {
                    {0, AbilityType.BurrowQueen}
                }},
                {0xCA, new Dictionary<int, AbilityType>() // BurrowQueenUp
                {
                    {0, AbilityType.UnburrowQueen}
                }},
                {0xCB, new Dictionary<int, AbilityType>() // NydusCanalTransport
                {
                    {0, AbilityType.NydusLoad},
                    {1, AbilityType.NydusUnloadAll},
                    {2, AbilityType.NydusUnloadAll},
                    {3, AbilityType.NydusUnload},
                    {4, AbilityType.NydusLoad}
                }},
                {0xCC, new Dictionary<int, AbilityType>() // Blink
                {
                    {0, AbilityType.StalkerBlink}
                }},
                {0xCD, new Dictionary<int, AbilityType>() // BurrowInfestorDown
                {
                    {0, AbilityType.BurrowInfestor}
                }},
                {0xCE, new Dictionary<int, AbilityType>() // BurrowInfestorUp
                {
                    {0, AbilityType.UnburrowInfestor}
                }},
                {0xCF, new Dictionary<int, AbilityType>() // MorphToOverseer
                {
                    {0, AbilityType.MorphToOverseer},
                    {1, AbilityType.CancelMorph}
                }},
                {0xD0, new Dictionary<int, AbilityType>() // UpgradeToPlanetaryFortress
                {
                    {0, AbilityType.MorphToPlanetaryFortress},
                    {1, AbilityType.CancelMorph}
                }},
                {0xD1, new Dictionary<int, AbilityType>() // InfestationPitResearch
                {
                    {2, AbilityType.ResearchInfestorEnergy},
                    {3, AbilityType.ResearchNeuralParasite},
                    {30, AbilityType.CancelResearch}
                }},
                {0xD2, new Dictionary<int, AbilityType>() // BanelingNestResearch
                {
                    {0, AbilityType.ResearchBanelingSpeed},
                    {30, AbilityType.CancelResearch}
                }},
                {0xD3, new Dictionary<int, AbilityType>() // BurrowUltraliskDown
                {
                    {0, AbilityType.BurrowUltralisk}
                }},
                {0xD4, new Dictionary<int, AbilityType>() // BurrowUltraliskUp
                {
                    {0, AbilityType.UnburrowUltralisk}
                }},
                {0xD5, new Dictionary<int, AbilityType>() // UpgradeToOrbital
                {
                    {0, AbilityType.MorphToOrbitalCommand},
                    {1, AbilityType.CancelMorph}
                }},
                {0xD6, new Dictionary<int, AbilityType>() // UpgradeToWarpGate
                {
                    {0, AbilityType.MorphToWarpGate},
                    {1, AbilityType.CancelMorph}
                }},
                {0xD7, new Dictionary<int, AbilityType>() // MorphBackToGateway
                {
                    {0, AbilityType.MorphToGateway},
                    {1, AbilityType.CancelMorph}
                }},
                {0xD8, new Dictionary<int, AbilityType>() // OrbitalLiftOff
                {
                    {0, AbilityType.LiftOrbitalCommand}
                }},
                {0xD9, new Dictionary<int, AbilityType>() // OrbitalCommandLand
                {
                    {0, AbilityType.LandOrbitalCommand}
                }},
                {0xDA, new Dictionary<int, AbilityType>() // ForceField
                {
                    {0, AbilityType.SentryForceField}
                }},
                {0xDB, new Dictionary<int, AbilityType>() // PhasingMode
                {
                    {0, AbilityType.WarpPrismPhasing}
                }},
                {0xDC, new Dictionary<int, AbilityType>() // TransportMode
                {
                    {0, AbilityType.WarpPrismTransport}
                }},
                {0xDD, new Dictionary<int, AbilityType>() // FusionCoreResearch
                {
                    {0, AbilityType.ResearchYamatoCannon},
                    {1, AbilityType.ResearchBattlecruiserEnergy},
                    {30, AbilityType.CancelResearch}
                }},
                {0xDE, new Dictionary<int, AbilityType>() // CyberneticsCoreResearch
                {
                    {0, AbilityType.ResearchProtossAirWeapons1},
                    {1, AbilityType.ResearchProtossAirWeapons2},
                    {2, AbilityType.ResearchProtossAirWeapons3},
                    {3, AbilityType.ResearchProtossAirArmors1},
                    {4, AbilityType.ResearchProtossAirArmors2},
                    {5, AbilityType.ResearchProtossAirArmors3},
                    {6, AbilityType.ResearchWarpGate},
                    {9, AbilityType.ResearchHallucination},
                    {30, AbilityType.CancelResearch}
                }},
                {0xDF, new Dictionary<int, AbilityType>() // TwilightCouncilResearch
                {
                    {0, AbilityType.ResearchCharge},
                    {1, AbilityType.ResearchBlink},
                    {30, AbilityType.CancelResearch}
                }},
                {0xE0, new Dictionary<int, AbilityType>() // TacNukeStrike
                {
                    {0, AbilityType.GhostNuke}
                }},
                {0xE2, new Dictionary<int, AbilityType>() // SalvageBunker
                {
                    {0, AbilityType.BunkerSalvage}
                }},
                {0xE3, new Dictionary<int, AbilityType>() // EMP
                {
                    {0, AbilityType.GhostEMP}
                }},
                {0xE4, new Dictionary<int, AbilityType>() // Vortex
                {
                    {0, AbilityType.MothershipVortex}
                }},
                {0xE5, new Dictionary<int, AbilityType>() // TrainQueen
                {
                    {0, AbilityType.TrainQueen},
                    {30, AbilityType.CancelTrain}
                }},
                {0xE6, new Dictionary<int, AbilityType>() // BurrowCreepTumorDown
                {
                    {0, AbilityType.CancelMorph}
                }},
                {0xE7, new Dictionary<int, AbilityType>() // Transfusion
                {
                    {0, AbilityType.QueenTransfusion}
                }},
                {0xF3, new Dictionary<int, AbilityType>() // burrowedStop
                {
                    {0, AbilityType.Stop},
                    {1, AbilityType.HoldFire},
                    {2, AbilityType.Taunt},
                    {3, AbilityType.Taunt}
                }},
                {0xF5, new Dictionary<int, AbilityType>() // GenerateCreep
                {
                    {0, AbilityType.OverlordGenerateCreep},
                    {1, AbilityType.OverlordStopGenerateCreep}
                }},
                {0xF6, new Dictionary<int, AbilityType>() // QueenBuild
                {
                    {0, AbilityType.QueenBuildCreepTumor},
                    {30, AbilityType.CancelBuilding}
                }},
                {0xF7, new Dictionary<int, AbilityType>() // SpineCrawlerUproot
                {
                    {0, AbilityType.UprootSpineCrawler}
                }},
                {0xF8, new Dictionary<int, AbilityType>() // SporeCrawlerUproot
                {
                    {0, AbilityType.UprootSporeCrawler}
                }},
                {0xF9, new Dictionary<int, AbilityType>() // SpineCrawlerRoot
                {
                    {0, AbilityType.RootSpineCrawler}
                }},
                {0xFA, new Dictionary<int, AbilityType>() // SporeCrawlerRoot
                {
                    {0, AbilityType.RootSporeCrawler}
                }},
                {0xFB, new Dictionary<int, AbilityType>() // CreepTumorBuild
                {
                    {0, AbilityType.CreepTumorBuildCreepTumor},
                    {30, AbilityType.CancelBuilding}
                }},
                {0xFC, new Dictionary<int, AbilityType>() // BuildAutoTurret
                {
                    {0, AbilityType.RavenBuildAutoTurret},
                    {1, AbilityType.CancelBuilding}
                }},
                {0xFD, new Dictionary<int, AbilityType>() // ArchonWarp
                {
                    {0, AbilityType.MergeArchon},
                    {1, AbilityType.MergeArchon}
                }},
                {0xFE, new Dictionary<int, AbilityType>() // BuildNydusCanal
                {
                    {0, AbilityType.BuildNydusCanal},
                    {30, AbilityType.CancelBuilding}
                }},
                {0xFF, new Dictionary<int, AbilityType>() // BroodLordHangar
                {
                    {0, AbilityType.ArmBroodling}
                }},
                {0x100, new Dictionary<int, AbilityType>() // Charge
                {
                    {0, AbilityType.ZealotCharge}
                }},
                {0x104, new Dictionary<int, AbilityType>() // Contaminate
                {
                    {0, AbilityType.OverseerContaminate}
                }},
                {0x106, new Dictionary<int, AbilityType>() // InfestedTerransLayEgg
                {
                    {0, AbilityType.InfestorSpawnInfestedTerran}
                }},
                {0x11C, new Dictionary<int, AbilityType>() // VolatileBurstBuilding
                {
                    {0, AbilityType.BanelingEnableBuildingAttack},
                    {1, AbilityType.BanelingDisableBuildingAttack}
                }}
            }}
        };
    }
}
