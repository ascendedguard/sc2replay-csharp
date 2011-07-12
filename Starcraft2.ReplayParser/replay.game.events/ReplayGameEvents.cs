namespace Starcraft2.ReplayParser
{
    using System.Diagnostics;
    using System.IO;
    using System;
    using System.Collections.Generic;

    public class ReplayGameEvents
    {
        public static List<IGameEvent> Parse(Replay replay, byte[] buffer)
        {
            // The GameEvent file changes significantly after 16561.
            // This is sometime around the first patch after release. Since
            // parsing replays this old should be extremely rare, I don't believe
            // its worth the effort to try to support both. If it is, it should be
            // done in another method.
            if (replay.ReplayBuild < 16561)
            {
                throw new NotSupportedException(
                    "Replay builds under 16561 are not supported for parsing GameEvent log.");
            }

            var events = new List<IGameEvent>();

            using (var stream = new MemoryStream(buffer))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var currentTime = 0;
                    var numEvents = 0;

                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        bool knownEvent = true;

                        var timestamp = Timestamp.Parse(reader).Value;
                        var nextByte = reader.ReadByte();

                        var eventType = nextByte >> 5; // 3 lowest bits
                        var globalEventFlag = nextByte & 16; // 4th bit
                        var playerId = nextByte & 15; // bits 5-8

                        Player player = null;
                        if (playerId > 0)
                        {
                            player = replay.GetPlayerById(playerId);
                        }

                        var eventCode = reader.ReadByte();

                        currentTime += timestamp;

                        var time = Timestamp.Create(currentTime);

                        numEvents++;

                        switch (eventType)
                        {
                            case 0x00: // initialization
                                switch (eventCode) 
                                {
                                    case 0x0B: // Player enters game
                                    case 0x0C: // for build >= 17326
                                    case 0x17:
                                    case 0x1B:
                                    case 0x2B:
                                    case 0x2C:
                                        break;
                                    case 0x05: // game starts
                                        break;
                                    default:
                                        knownEvent = false;
                                        break;
                                }

                                break;

                            case 0x01: // Action
                                switch (eventCode)
                                {
                                    case 0x09: // player quits the game
                                        events.Add(new PlayerLeftEvent(player, time));
                                        break;
                                    case 0x1B:
                                    case 0x2B:
                                    case 0x3B:
                                    case 0x4B:
                                    case 0x5B:
                                    case 0x6B:
                                    case 0x7B:
                                    case 0x8B:
                                    case 0x9B:
                                    case 0x0B: // player uses an ability
                                        int ability = -1;
                                        byte firstByte = reader.ReadByte();
                                        byte temp = reader.ReadByte();

                                        if (replay.ReplayBuild >= 18317)
                                        {
                                            byte lastTemp;

                                            ability = reader.Read() << 16 | reader.ReadByte() << 8
                                                      | (lastTemp = reader.ReadByte());

                                            // 18574 should be the correct build? not sure
                                            if ((firstByte & 0x0c) == 0x0c && (firstByte & 1) == 0)
                                            {
                                                reader.ReadBytes(4);
                                            }
                                            else if (temp == 64 || temp == 66)
                                            {
                                                if (lastTemp > 14)
                                                {
                                                    if ((lastTemp & 0x40) != 0)
                                                    {
                                                        reader.ReadBytes(2);
                                                        reader.ReadBytes(4);

                                                        reader.ReadBytes(2);
                                                    }
                                                    else
                                                    {
                                                        reader.ReadBytes(6);
                                                    }
                                                }
                                            }
                                            else if (temp == 8 || temp == 10)
                                            {
                                                reader.ReadBytes(7);
                                            }
                                            else if (temp == 136 || temp == 138)
                                            {
                                                reader.ReadBytes(15);
                                            }
                                            /*
                                            {
                                                    if ((temp & 0x80) == 0x80)
                                                    {
                                                        reader.ReadBytes(8);
                                                    }

                                                    reader.ReadBytes(10);
                                                    ability = 0;
                                                }
                                                else
                                                {
                                                    byte lastTemp;

                                                    ability = reader.Read() << 16 | reader.ReadByte() << 8
                                                              | (lastTemp = reader.ReadByte());

                                                    if ((temp & 0x60) == 0x60)
                                                    {
                                                        reader.ReadBytes(4);
                                                    }
                                                    else
                                                    {
                                                        var flaga = ability & 0xF0; // some kind of flag
                                                        if ((flaga & 0x20) == 0x20)
                                                        {
                                                            reader.ReadBytes(9);
                                                            if ((firstByte & 8) == 8)
                                                            {
                                                                reader.ReadBytes(9);
                                                            }
                                                        }
                                                        else if ((flaga & 0x10) == 0x10)
                                                        {
                                                            reader.ReadBytes(9);
                                                        }
                                                        else if ((flaga & 0x40) == 0x40)
                                                        {
                                                            reader.ReadBytes(18);
                                                        }
                                                    }
                                                }
                                            }
                                            */
                                            if (ability != -1)
                                            {
                                                events.Add(
                                                    new GameEventBase(
                                                        replay.GetPlayerById(playerId), Timestamp.Create(currentTime)));
                                            }
                                        }

                                        if (ability == -1)
                                        {
                                            ability = (reader.ReadByte() << 16) |
                                                            (reader.ReadByte() << 8) |
                                                            (reader.ReadByte() & 0x3F);

                                            if (temp == 0x20 || temp == 0x22)
                                            {
                                                var nByte = ability & 0xFF;

                                                if (nByte > 0x07)
                                                {
                                                    if (firstByte == 0x29 || firstByte == 0x19)
                                                    {
                                                        reader.ReadBytes(4); // Advance 4 bytes.
                                                        break;
                                                    }

                                                    reader.ReadBytes(9);

                                                    if ((nByte & 0x20) > 0)
                                                    {
                                                        reader.ReadBytes(9);
                                                    }
                                                }
                                            }
                                            else if (temp == 0x48 || temp == 0x4A)
                                            {
                                                reader.ReadBytes(7);
                                            }
                                            else if (temp == 0x88 || temp == 0x8A)
                                            {
                                                reader.ReadBytes(15);
                                            }

                                            if ((temp & 0x20) != 0)
                                            {
                                                // TODO: Record player ability.
                                                // This is wrong, right?
                                                events.Add(new GameEventBase(replay.GetPlayerById(playerId), Timestamp.Create(currentTime)));
                                            }

                                            events.Add(new GameEventBase(replay.GetPlayerById(playerId), Timestamp.Create(currentTime)));
                                        }
                                        
                                        break;
                                    case 0x0C: // automatic update of hotkey?
                                    case 0x1C:
                                    case 0x2C:
                                    case 0x3C: // 01 01 01 01 11 01 03 02 02 38 00 01 02 3c 00 01 00
                                    case 0x4C: // 01 02 02 01 0d 00 02 01 01 a8 00 00 01
                                    case 0x5C: // 01 01 01 01 16 03 01 01 03 18 00 01 00
                                    case 0x6C: // 01 04 08 01 03 00 02 01 01 34 c0 00 01
                                    case 0x7C: // 01 05 10 01 01 10 02 01 01 1a a0 00 01
                                    case 0x8C:
                                    case 0x9C:
                                    case 0xAC: // player changes selection
                                        if (replay.ReplayBuild >= 16561)
                                        {
                                            int bitmask = 0;
                                            byte nByte = 0;
                                            reader.ReadByte(); // skip flag byte
                                            var deselectFlags = reader.ReadByte();
                                            if ((deselectFlags & 3) == 1)
                                            {
                                                nByte = reader.ReadByte();
                                                var deselectionBits = (deselectFlags & 0xFC) | (nByte & 3);
                                                while (deselectionBits > 6)
                                                {
                                                    nByte = reader.ReadByte();
                                                    deselectionBits -= 8;
                                                }
                                                deselectionBits += 2;
                                                deselectionBits = deselectionBits % 8;

                                                bitmask = (int)Math.Pow(2, deselectionBits) - 1;
                                            }
                                            else if ((deselectFlags & 3) == 2 || (deselectFlags & 3) == 3)
                                            {
                                                nByte = reader.ReadByte();
                                                var deselectionBytes = (deselectFlags & 0xFC) | (nByte & 3);
                                                while (deselectionBytes > 0)
                                                {
                                                    nByte = reader.ReadByte();
                                                    deselectionBytes--;
                                                }

                                                bitmask = 3;
                                            }
                                            else if ((deselectFlags & 3) == 0)
                                            {
                                                bitmask = 3;
                                                nByte = deselectFlags;
                                            }

                                            int numUnitTypeIDs = 0;

                                            var prevByte = nByte;
                                            nByte = reader.ReadByte();

                                            if (bitmask > 0)
                                            {
                                                numUnitTypeIDs = (prevByte & (0xFF - bitmask)) | (nByte & bitmask);
                                            }
                                            else
                                            {
                                                numUnitTypeIDs = nByte;
                                            }

                                            for (int i = 0; i < numUnitTypeIDs; i++)
                                            {
                                                int unitTypeID = 0;
                                                int unitTypeCount = 0;

                                                for (int j = 0; j < 3; j++)
                                                {
                                                    byte by = 0;

                                                    prevByte = nByte;
                                                    nByte = reader.ReadByte();

                                                    if (bitmask > 0) // Line 610 of sc2replay.php
                                                    {
                                                        by = (byte)((prevByte & (0xFF - bitmask)) | (nByte & bitmask));
                                                    }
                                                    else
                                                    {
                                                        by = nByte;
                                                    }

                                                    unitTypeID = by << ((2 - j) * 8) | unitTypeID;
                                                }

                                                prevByte = nByte;
                                                nByte = reader.ReadByte();

                                                if (bitmask > 0)
                                                {
                                                    unitTypeCount = (prevByte & (0xFF - bitmask)) | (nByte & bitmask);
                                                }
                                                else
                                                {
                                                    unitTypeCount = nByte;
                                                }

                                                // $uType[$i + 1]['count'] = $unitTypeCount;
                                                // $uType[$i + 1]['id'] = $unitTypeID;
                                            }

                                            var numUnits = 0;
                                            prevByte = nByte;
                                            nByte = reader.ReadByte();
                                            if (bitmask > 0)
                                            {
                                                numUnits = (prevByte & (0xFF - bitmask)) | (nByte & bitmask);
                                            }
                                            else
                                            {
                                                numUnits = nByte;
                                            }

                                            for (int i = 0; i < numUnits; i++)
                                            {
                                                var unitID = 0;
                                                byte by = 0;

                                                for (int j = 0; j < 4; j++)
                                                {
                                                    prevByte = nByte;
                                                    nByte = reader.ReadByte();

                                                    if (bitmask > 0)
                                                    {
                                                        by =
                                                            (byte)((prevByte & (0xFF - bitmask)) | (nextByte & bitmask));
                                                    }
                                                    else
                                                    {
                                                        by = nByte;
                                                    }

                                                    if (j < 2)
                                                    {
                                                        unitID = (by << ((1 - j) * 8)) | unitID;
                                                    }
                                                }

                                                // TODO: Record unitID
                                                // unitIDs[] = unitID;
                                            }

                                            var a = 0;

                                            //foreach($uType as $unitType){
                                            //for($i = 1; $i <= $unitType['count']; $i++){
                                            //    $uid = $unitIDs[$a];
                                            //    //Bytes 3 + 4 contain flag info (perhaps same as in 1.00)
                                            //    $this->addSelectedUnit($uid, $unitType['id'], $playerId, floor($time / 16));
                                            //    if ($this->debug) {
                                            //        $this->debug(sprintf("  0x%06X -> 0x%04X", $unitType['id'], $uid));
                                            //    }
                                            //    $a++;
                                            //}

                                            if (eventCode == 0xAC)
                                            {
                                                events.Add(new GameEventBase(replay.GetPlayerById(playerId), Timestamp.Create(currentTime)));
                                                // $this->addPlayerAction($playerId, floor($time / 16));
                                            }

                                            break;
                                        } // sc2replay.php: Line 666

                                        throw new NotSupportedException("Event logic for builds < 16561 not implemented");
                                    case 0x0D: // manually uses hotkey
                                    case 0x1D:
                                    case 0x2D:
                                    case 0x3D:
                                    case 0x4D:
                                    case 0x5D:
                                    case 0x6D:
                                    case 0x7D:
                                    case 0x8D:
                                    case 0x9D: // sc2replay.php: Line 802
                                        HotkeyEvent hotkey = new HotkeyEvent(player, time);
                                        hotkey.Key = eventCode >> 4;
                                        var byte1 = reader.ReadByte();

                                        int flag = byte1 & 0x03;
                                        hotkey.Action = flag;

                                        if (flag == 2)
                                        {
                                            hotkey.EventType = GameEventType.Selection;
                                        }

                                        byte byte2 = 0;
                                        
                                        if ((byte1 < 16) && ((byte1 & 0x8) == 8))
                                        {
                                            byte b2 = (byte)(reader.ReadByte() & 0xF);
                                            reader.ReadBytes(b2);
                                        }
                                        else if (byte1 > 4)
                                        {
                                            int j;
                                            if (byte1 < 8)
                                            {
                                                j = reader.ReadByte();
                                                if ((j & 0x7) > 4)
                                                {
                                                    reader.ReadByte();
                                                }
                                                if ((j & 0x8) != 0)
                                                {
                                                    reader.ReadByte();
                                                }
                                            }
                                            else
                                            {
                                                j = reader.ReadByte();
                                                int shift = (byte1 >> 3) + ((j & 0xF) > 4 ? 1 : 0) +
                                                            ((j & 0xF) > 12 ? 1 : 0);
                                                reader.ReadBytes(shift);

                                                if (replay.ReplayBuild >= 18574)
                                                {
                                                    if (byte1 == 30 && j == 1)
                                                    {
                                                        reader.ReadBytes(14);
                                                    }
                                                }
                                            }
                                        }

                                        events.Add(hotkey);
                                        break;
                                    case 0x1F: // send resources
						            case 0x2F: 
						            case 0x3F: 
						            case 0x4F:
						            case 0x5F:
						            case 0x6F:
						            case 0x7F:
						            case 0x8F:
                                        reader.ReadByte(); // 0x84
                                        var sender = playerId;
                                        var receiver = (eventCode & 0xF0) >> 4;
                                        // sent minerals
                                        var bytes = reader.ReadBytes(4);
                                        var mineralValue = (((bytes[0] << 20) | (bytes[1] << 12) | bytes[2] << 4 ) >> 1) + (bytes[3] & 0x0F);

                                        // sent gas
                                        bytes = reader.ReadBytes(4);
                                        var gasValue = (((bytes[0] << 20) | (bytes[1] << 12) | bytes[2] << 4 ) >> 1) + (bytes[3] & 0x0F);

                                        // last 8 bytes are unknown
                                        reader.ReadBytes(8);
                                        break;
                                    default:
                                        knownEvent = false;
                                        break;
                                }
                                break;
                            case 0x02: // weird
                                switch (eventCode)
                                {
                                    case 0x06:
                                        reader.ReadBytes(8);
                                        break;
                                    case 0x07:
                                        reader.ReadBytes(4);
                                        break;
                                    case 0x49:
                                        // Unknown...
                                        break;
                                    default:
                                        knownEvent = false;
                                        break;
                                }
                                break;
                            case 0x03: // replay
                                switch (eventCode)
                                {
                                    case 0x87:
                                        reader.ReadBytes(8);
                                        break;
                                    case 0x08:
                                        reader.ReadBytes(10);
                                        break;
                                    case 0x18:
                                        reader.ReadBytes(162);
                                        break;
                                    case 0x01: // camera movement
						            case 0x11:						
						            case 0x21:
						            case 0x31:
						            case 0x41:
						            case 0x51:
						            case 0x61:
						            case 0x71:
						            case 0x81:
						            case 0x91:
						            case 0xA1:
						            case 0xB1:
						            case 0xC1:
						            case 0xD1:
						            case 0xE1:
						            case 0xF1:
                                        reader.ReadBytes(3);
                                        var nByte = reader.ReadByte();

                                        var aByte = nByte & 0x70;
                                        switch (aByte)
                                        {
                                            case 0x10: // zoom camera up or down
                                            case 0x20:
								            case 0x30: // only 0x10 matters, but due to 0x70 mask in comparison, check for this too
                                            case 0x40: // rotate camera
								            case 0x50:

                                                if (aByte == 0x10 || aByte == 0x30 || aByte == 0x50)
                                                {
                                                    reader.ReadByte();
                                                    nByte = reader.ReadByte();                                                    
                                                }

                                                if (aByte != 0x40)
                                                {
                                                    if ((nByte & 0x20) > 0)
                                                    {
                                                        // zooming, if comparison is 0 max/min zoom reached
                                                        reader.ReadByte();
                                                        nByte = reader.ReadByte();
                                                    }

                                                    if ((nByte & 0x40) == 0) break;
                                                }

                                                reader.ReadBytes(2);
                                                //events.Add(new PlayerEvent(playerId, currentTime));
                                                break;
                                        }
                                        break;
                                    default:
                                        knownEvent = false;
                                        break;
                                }
                                break;
                            case 0x04: // inaction
                                if ((eventCode & 0x0F) == 2)
                                {
                                    reader.ReadBytes(2);
                                    break;
                                }
                                
                                if ((eventCode & 0x0C) == 2)
                                {
                                    break;
                                }

                                if ((eventCode & 0x0F) == 12)
                                {
                                    break;
                                }
                                
                                switch(eventCode)
                                {
                                    case 0x16:
                                        reader.ReadBytes(24);
                                        break;
                                    case 0xC6:
                                        reader.ReadBytes(16);
                                        break;
                                    case 0x18:
                                        reader.ReadBytes(4);
                                        break;
                                    case 0x87:
                                        reader.ReadBytes(4);
                                        break;
                                    default:
                                        knownEvent = false;
                                        break;
                                }

                                break;
                            case 0x05: // system
                                switch (eventCode)
                                {
                                    case 0x89: // automatic sync?
                                        reader.ReadBytes(4);
                                        break;
                                    default:
                                        knownEvent = false;
                                        break;
                                }

                                break;
                            default:
                                knownEvent = false;
                                break;
                        }

                        if (knownEvent == false)
                        {
                            Debug.WriteLine("Unknown Event: " + eventCode + "," + currentTime + "," + eventType);
                            throw new FormatException("An unknown event prevented the events file from being correctly parsed.");
                        }
                    }
                }
            }

            return events;
        }

        private static void TrackLeavingPlayer(List<Player> remainingPlayers, Player player)
        {
            if (remainingPlayers.Contains(player))
            {
                remainingPlayers.Remove(player);
            }
        }
    }
}
