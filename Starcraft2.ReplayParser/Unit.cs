// -----------------------------------------------------------------------
// <copyright file="Unit.cs" company="Microsoft">
// TODO: Update copyright text.
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

        public Unit(int id, UnitType type)
        {
            this.id = id;
            this.type = type;
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
