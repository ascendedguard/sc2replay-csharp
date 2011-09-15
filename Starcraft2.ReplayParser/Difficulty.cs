// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Difficulty.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Describes the difficulty of a computer AI opponent.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    /// <summary>
    /// Describes the difficulty of a computer AI opponent.
    /// </summary>
    public enum Difficulty
    {
        /// <summary>
        /// Unknown AI difficulty
        /// </summary>
        Unknown, 

        /// <summary>
        /// Very Easy AI difficulty
        /// </summary>
        VeryEasy, 

        /// <summary>
        /// Easy AI difficulty
        /// </summary>
        Easy, 

        /// <summary>
        /// Medium AI difficulty
        /// </summary>
        Medium, 

        /// <summary>
        /// Hard AI difficulty
        /// </summary>
        Hard, 

        /// <summary>
        /// Very Hard AI difficulty
        /// </summary>
        VeryHard, 

        /// <summary>
        /// Insane AI difficulty
        /// </summary>
        Insane, 
    }
}