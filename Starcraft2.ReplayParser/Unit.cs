// -----------------------------------------------------------------------
// <copyright file="Unit.cs">
// Copyright 2012 Robert Nix, Will Eddins
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A game unit
    /// </summary>
    public class Unit   
    {
        int id;
        UnitType type;

        /// <summary>
        /// Version-specific id, used as a tie-breaker in wireframe subgroups.
        /// </summary>
        internal int typeId;

        public Unit(int id, UnitType type)
        {
            this.id = id;
            this.type = type;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public Unit(Unit u)
        {
            this.id = u.id;
            this.type = u.type;
            this.typeId = u.typeId;
        }

        internal void UpdateType(UnitType type)
        {
            this.type = type;
        }

        public int Id
        {
            get
            {
                return id >> 18;
            }
        }

        public UnitType Type
        {
            get
            {
                return type;
            }
        }
    }
}
