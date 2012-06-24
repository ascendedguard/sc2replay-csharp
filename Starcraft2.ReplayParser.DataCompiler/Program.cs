using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Xml;
using System.IO;

using Starcraft2.ReplayParser;

namespace Starcraft2.ReplayParser.DataCompiler
{
    /// <summary>
    /// Compiles sqlite db tables into binary version info pages
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var dbFile = "sc2replay.sqlite";
            if (args.Length > 0)
            {
                dbFile = args[0];
            }

            using (var conn = new SQLiteConnection(String.Format("Data Source=.\\{0};", dbFile)))
            {
                conn.Open();

                CompileSubgroups(conn);

                CompileEvents(conn);

                var builds = CompileBuilds(conn);
                foreach (var build in builds)
                {
                    CompileAbilities(conn, build);
                    CompileUnits(conn, build);
                }
            }
        }


        static void CompileEvents(SQLiteConnection conn)
        {
            var result = new Dictionary<AbilityType, GameEventType>();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM event_type;";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add((AbilityType)Enum.Parse(typeof(AbilityType), reader.GetString(0)),
                        (GameEventType)Enum.Parse(typeof(GameEventType), reader.GetString(1)));
                }
            }

            using (var file = File.Open("events.dat", FileMode.Create))
            {
                var typesLength = Enum.GetNames(typeof(AbilityType)).Length;

                var headerMagic = BitConverter.GetBytes(magicWord);
                file.Write(headerMagic, 0, headerMagic.Length);

                // 'evts'
                var headerEvts = BitConverter.GetBytes(0x65767473);
                file.Write(headerEvts, 0, headerEvts.Length);

                var headerBuild = BitConverter.GetBytes((int)0);
                file.Write(headerBuild, 0, headerBuild.Length);

                // AbilityType count * 1 byte
                var headerLength = BitConverter.GetBytes(typesLength);
                file.Write(headerLength, 0, headerLength.Length);

                for (var i = 0; i < typesLength; i++)
                {
                    GameEventType eventType;
                    if (result.TryGetValue((AbilityType)i, out eventType))
                    {
                        file.WriteByte((byte)eventType);
                    }
                    else
                    {
                        file.WriteByte((byte)GameEventType.Unknown);
                    }
                }
            }
        }


        static void CompileSubgroups(SQLiteConnection conn)
        {
            var result = new Dictionary<UnitType, int>();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM unit_subgroup_priority;";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add((UnitType)Enum.Parse(typeof(UnitType), reader.GetString(0)), reader.GetInt32(1));
                }
            }

            using (var file = File.Open("subgroups.dat", FileMode.Create))
            {
                var typesLength = Enum.GetNames(typeof(UnitType)).Length;

                var headerMagic = BitConverter.GetBytes(magicWord);
                file.Write(headerMagic, 0, headerMagic.Length);

                // 'subg'
                var headerSubg = BitConverter.GetBytes(0x73756267);
                file.Write(headerSubg, 0, headerSubg.Length);

                var headerBuild = BitConverter.GetBytes((int)0);
                file.Write(headerBuild, 0, headerBuild.Length);

                // UnitType count * 1 byte
                var headerLength = BitConverter.GetBytes(typesLength);
                file.Write(headerLength, 0, headerLength.Length);

                for (var i = 0; i < typesLength; i++)
                {
                    var priority = 0;
                    if (result.TryGetValue((UnitType)i, out priority))
                    {
                        file.WriteByte((byte)priority);
                    }
                    else
                    {
                        file.WriteByte(0);
                    }
                }
            }
        }


        static List<int> CompileBuilds(SQLiteConnection conn)
        {
            var result = new Dictionary<int, int>();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM build_info;";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetInt32(0), reader.GetInt32(1));
                }
            }

            var effectiveBuildList = new List<int>();

            using (var file = File.Open("builds.dat", FileMode.Create))
            {
                var buildsLength = result.Count * 4;

                var headerMagic = BitConverter.GetBytes(magicWord);
                file.Write(headerMagic, 0, headerMagic.Length);

                // 'blds'
                var headerBlds = BitConverter.GetBytes(0x626C6473);
                file.Write(headerBlds, 0, headerBlds.Length);

                var headerBuild = BitConverter.GetBytes((int)0);
                file.Write(headerBuild, 0, headerBuild.Length);

                var headerLength = BitConverter.GetBytes(buildsLength);
                file.Write(headerLength, 0, headerLength.Length);

                foreach (var pair in result)
                {
                    var build = BitConverter.GetBytes((ushort)pair.Key);
                    var effectiveBuild = BitConverter.GetBytes((ushort)pair.Value);
                    if (!effectiveBuildList.Contains(pair.Value))
                    {
                        effectiveBuildList.Add(pair.Value);
                    }

                    file.Write(build, 0, build.Length);
                    file.Write(effectiveBuild, 0, effectiveBuild.Length);
                }
            }

            return effectiveBuildList;
        }


        static void CompileAbilities(SQLiteConnection conn, int build)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = String.Format("select typeId, CAbil from ability_type where build={0}", build);

            var result = new Dictionary<int, string>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetInt32(0), reader.GetString(1));
                }
            }
            
            using (var file = File.Open(String.Format("abil{0}.dat", build), FileMode.Create))
            {
                cmd = conn.CreateCommand();
                cmd.CommandText = String.Format("select max(typeId) from ability_type where build={0}", build);
                // 32 ushorts, 0..max -- throw if DBNull
                var abilsLength = (Convert.ToInt32(cmd.ExecuteScalar()) + 1);

                var headerMagic = BitConverter.GetBytes(magicWord);
                file.Write(headerMagic, 0, headerMagic.Length);

                // 'abil'
                var headerAbil = BitConverter.GetBytes(0x6162696C);
                file.Write(headerAbil, 0, headerAbil.Length);

                var headerBuild = BitConverter.GetBytes(build);
                file.Write(headerBuild, 0, headerBuild.Length);

                var headerLength = BitConverter.GetBytes(abilsLength * 64);
                file.Write(headerLength, 0, headerLength.Length);

                for (var i = 0; i < abilsLength; i++)
                {
                    var failed = false;

                    string CAbil;
                    if (result.TryGetValue(i, out CAbil) && CAbil.Length > 0) // i.e. name isn't String.Empty
                    {
                        // Try to get corresponding row from abil_info
                        cmd = conn.CreateCommand();
                        cmd.CommandText = String.Format("select * from abil_info where CAbil='{0}'", CAbil);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                // info0..info31
                                for (var j = 1; j <= 32; j++)
                                {
                                    ushort value = (ushort)AbilityType.Unknown;
                                    if (!reader.IsDBNull(j))
                                    {
                                        value = (ushort)(AbilityType)Enum.Parse(typeof(AbilityType), reader.GetString(j));
                                    }
                                    var valueBytes = BitConverter.GetBytes(value);
                                    file.Write(valueBytes, 0, valueBytes.Length);
                                }
                            }
                            else
                            {
                                failed = true;
                            }
                        }
                    }
                    else 
                    {
                        failed = true;
                    }

                    if (failed)
                    {
                        for (var j = 0; j < 32; j++)
                        {
                            var unknown = BitConverter.GetBytes((ushort)AbilityType.Unknown);
                            file.Write(unknown, 0, unknown.Length);
                        }
                    }
                }
            }
        }


        static void CompileUnits(SQLiteConnection conn, int build)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = String.Format("select typeId, CUnit from unit_type where build={0}", build);

            var result = new Dictionary<int, string>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetInt32(0), reader.GetString(1));
                }
            }

            using (var file = File.Open(String.Format("unit{0}.dat", build), FileMode.Create))
            {
                cmd = conn.CreateCommand();
                cmd.CommandText = String.Format("select max(typeId) from unit_type where build={0}", build);
                // 1 ushort, 0..max -- throw if DBNull
                var unitsLength = (Convert.ToInt32(cmd.ExecuteScalar()) + 1);

                var headerMagic = BitConverter.GetBytes(magicWord);
                file.Write(headerMagic, 0, headerMagic.Length);

                // 'unit'
                var headerAbil = BitConverter.GetBytes(0x756E6974);
                file.Write(headerAbil, 0, headerAbil.Length);

                var headerBuild = BitConverter.GetBytes(build);
                file.Write(headerBuild, 0, headerBuild.Length);

                var headerLength = BitConverter.GetBytes(unitsLength * 2);
                file.Write(headerLength, 0, headerLength.Length);

                for (var i = 0; i < unitsLength; i++)
                {
                    var failed = false;

                    string CUnit;
                    if (result.TryGetValue(i, out CUnit) && CUnit.Length > 0) // i.e. name isn't String.Empty
                    {
                        // Try to get corresponding row from abil_info
                        cmd = conn.CreateCommand();
                        cmd.CommandText = String.Format("select UnitType from unit_info where CUnit='{0}'", CUnit);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                ushort value = (ushort)UnitType.Unknown;
                                if (!reader.IsDBNull(0))
                                {
                                    value = (ushort)(UnitType)Enum.Parse(typeof(UnitType), reader.GetString(0));
                                }
                                var valueBytes = BitConverter.GetBytes(value);
                                file.Write(valueBytes, 0, valueBytes.Length);
                            }
                            else
                            {
                                failed = true;
                            }
                        }
                    }
                    else
                    {
                        failed = true;
                    }

                    if (failed)
                    {
                        var unknown = BitConverter.GetBytes((ushort)UnitType.Unknown);
                        file.Write(unknown, 0, unknown.Length);
                    }
                }
            }
        }

        // 'sc2r'
        const int magicWord = 0x73633272;
    }
}
