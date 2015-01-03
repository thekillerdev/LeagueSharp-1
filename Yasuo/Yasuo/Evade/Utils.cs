﻿// Copyright 2014 - 2015 Esk0r
// Utils.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Yasuo.Evade
{
    public static class Utils
    {
        public static List<Vector2> To2DList(this Vector3[] v)
        {
            return v.Select(point => point.To2D()).ToList();
        }

        public static void SendMovePacket(this Obj_AI_Base v, Vector2 point)
        {
            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(point.X, point.Y)).Send();
        }

        public static Obj_AI_Base Closest(List<Obj_AI_Base> targetList, Vector2 from)
        {
            var dist = float.MaxValue;
            Obj_AI_Base result = null;

            foreach (var target in targetList)
            {
                var distance = Vector2.DistanceSquared(from, target.ServerPosition.To2D());
                if (!(distance < dist))
                {
                    continue;
                }

                dist = distance;
                result = target;
            }

            return result;
        }

        /// <summary>
        ///     Returns when the unit will be able to move again
        /// </summary>
        public static int ImmobileTime(Obj_AI_Base unit)
        {
            var result = (from buff in unit.Buffs
                where
                    buff.IsActive && Game.Time <= buff.EndTime &&
                    (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                     buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare)
                select buff.EndTime).Concat(new[] { 0f }).Max();

            return (Math.Abs(result) < float.Epsilon) ? -1 : (int) (Environment.TickCount + (result - Game.Time) * 1000);
        }

        public static void DrawLineInWorld(Vector3 start, Vector3 end, int width, Color color)
        {
            var segmentStart = Drawing.WorldToScreen(start);
            var segmentEnd = Drawing.WorldToScreen(end);
            Drawing.DrawLine(segmentStart[0], segmentStart[1], segmentEnd[0], segmentEnd[1], width, color);
        }
    }

    internal class SpellList<T> : List<T>
    {
        public event EventHandler OnAdd;

        public new void Add(T item)
        {
            if (OnAdd != null)
            {
                OnAdd(this, null);
            }

            base.Add(item);
        }
    }
}