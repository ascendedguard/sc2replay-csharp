// -----------------------------------------------------------------------
// <copyright file="CameraEvent.cs">
// Copyright 2012 Robert Nix, Will Eddins
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using Streams;

    /// <summary> A camera movement or update </summary>
    public class CameraEvent : GameEventBase
    {
        public CameraEvent(BitReader bitReader, Replay replay)
        {
            TargetX = CFixedToDouble(bitReader.Read(16));
            TargetY = CFixedToDouble(bitReader.Read(16));

            HasDistance = bitReader.Read(1) != 0;
            if (HasDistance)
            {
                Distance = CFixedToDouble(bitReader.Read(16));
            }

            HasPitch = bitReader.Read(1) != 0;
            if (HasPitch)
            {
                Pitch = RotationAmountToDegrees(bitReader.Read(16));
            }

            HasYaw = bitReader.Read(1) != 0;
            if (HasYaw)
            {
                Yaw = RotationAmountToDegrees(bitReader.Read(16));
            }

            HasHeightOffset = bitReader.Read(1) != 0;
            if (HasHeightOffset)
            {
                // Debug since we're unsure
                HeightOffset = CFixedToDouble(bitReader.Read(16));
            }

            this.EventType = GameEventType.Other;
        }

        /// <summary>
        /// Convert 8.8 cfixed (i.e. serialized map coord cfixed) to double
        /// </summary>
        double CFixedToDouble(uint cfixedAmount)
        {
            return (double)cfixedAmount / 256;
        }

        /// <summary>
        /// Convert 16 bit rotation amount to degrees
        /// </summary>
        double RotationAmountToDegrees(uint rotationAmount)
        {
            var amount = (int)rotationAmount;
            return 45 * (((((amount * 0x10 - 0x2000) << 17) - 1) >> 17) + 1) / 4096d;
        }

        /// <summary> The x coordinate of the camera's target </summary>
        public double TargetX { get; private set; }

        /// <summary> The y coordinate of the camera's target </summary>
        public double TargetY { get; private set; }

        /// <summary> Whether the event contains an update of the camera's distance from the target </summary>
        public bool HasDistance { get; private set; }

        /// <summary> Whether the event contains an update of the camera's pitch amount </summary>
        public bool HasPitch { get; private set; }

        /// <summary> Whether the event contains an update of the camera's yaw amount </summary>
        public bool HasYaw { get; private set; }

        /// <summary> Whether the event contains an update of the camera's height offset from the ground plane </summary>
        public bool HasHeightOffset { get; private set; }

        /// <summary> The distance of the camera from its target </summary>
        public double Distance { get; private set; }

        /// <summary> The pitch of the camera in degrees </summary>
        public double Pitch { get; private set; }

        /// <summary> The yaw of the camera in degrees </summary>
        public double Yaw { get; private set; }

        /// <summary> The height offset of the camera from the ground plane </summary>
        public double HeightOffset { get; private set; }
    }
}
