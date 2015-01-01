using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Yasuo
{
    public static class YasuoUtility
    {
        private const int Delay = 150;
        private const float MinDistance = 400;
        private static int _lastMoveCommandT;
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        ///     Checks if the object has the whirlwind passive.
        /// </summary>
        /// <param name="objAiBase">The object to check if has whirlwind</param>
        /// <returns></returns>
        public static bool HasWhirlwind(this Obj_AI_Base objAiBase)
        {
            return objAiBase.HasBuff("YasuoQ3W");
        }

        /// <summary>
        ///     Checks if the targetted objet is able to be dashed on.
        /// </summary>
        /// <param name="objAiBase">The object of the dasher</param>
        /// <param name="targetObjAiBase">The object of the one who is being dashed on</param>
        /// <returns>True/False</returns>
        public static bool IsDashable(this Obj_AI_Base objAiBase, Obj_AI_Base targetObjAiBase)
        {
            return objAiBase.Distance(targetObjAiBase.Position) <= 475f && !targetObjAiBase.HasBuff("YasuoDashWrapper");
        }

        /// <summary>
        ///     Calculates the Vector2 position of the dashing end position.
        /// </summary>
        /// <param name="objAiBase">The object of the dasher</param>
        /// <param name="unitObjAiBase">The object of the one who is being dashed on</param>
        /// <returns>Vector2 position</returns>
        public static Vector2 GetDashingEnd(this Obj_AI_Base objAiBase, Obj_AI_Base unitObjAiBase)
        {
            var vector3 = new Vector2(
                unitObjAiBase.Position.X - objAiBase.Position.X, unitObjAiBase.Position.Y - objAiBase.Position.Y);
            var abs = Math.Sqrt(vector3.X * vector3.X + vector3.Y * vector3.Y);
            return new Vector2(
                (float) (objAiBase.Position.X + (475f * (vector3.X / abs))),
                (float) (objAiBase.Position.Y + (475f * (vector3.Y / abs))));
        }

        /// <summary>
        ///     Calculates the real auto attack range of the object.
        /// </summary>
        /// <param name="objAiBase">The object to be checked of</param>
        /// <returns>Float Attack Range</returns>
        public static float GetAutoAttackRange(this Obj_AI_Base objAiBase)
        {
            return (objAiBase.AttackRange + objAiBase.BoundingRadius);
        }

        /// <summary>
        ///     Returns Yasuo's correct Steel Tempest (Q) mode
        /// </summary>
        /// <param name="objAiBase">Base Object</param>
        /// <returns>Steel Tempest Spell Instance</returns>
        public static Spell GetYasuoQState(this Obj_AI_Base objAiBase)
        {
            return (objAiBase.HasWhirlwind()) ? YasuoSpells.QWind : YasuoSpells.Q;
        }

        /// <summary>
        ///     Returns if the base object is knocked up.
        /// </summary>
        /// <param name="objAiBase">Base Object</param>
        /// <param name="onlySelf">Only yasuo knock ups</param>
        /// <returns>True/False</returns>
        public static bool IsKnockedUp(this Obj_AI_Base objAiBase, bool onlySelf = false)
        {
            return (onlySelf)
                ? (objAiBase.HasBuff("yasuoq3mis"))
                : (objAiBase.HasBuffOfType(BuffType.Knockup) || objAiBase.HasBuffOfType(BuffType.Knockback));
        }

        /// <summary>
        ///     Returns Dash Data
        /// </summary>
        /// <param name="base">Base Object</param>
        /// <param name="vector3">Vector3 Position</param>
        /// <param name="focusTarget">If to focus target</param>
        /// <param name="ignoreTower">Ignore Towers</param>
        /// <returns>Object[index=0 is Vector3],[index=1 is Obj_AI_Base]</returns>
        public static object[] GetDashData(this Obj_AI_Base @base,
            Vector3 vector3,
            Obj_AI_Hero focusTarget = null,
            bool ignoreTower = true)
        {
            var returnObjects = new object[2];
            returnObjects[0] = Vector3.Zero;
            returnObjects[1] = null;

            if (!vector3.IsValid())
            {
                return null;
            }

            var minions =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(m => m.IsValidTarget() && m.Distance(@base) <= YasuoSpells.E.Range && @base.IsDashable(m));
            foreach (var m in minions)
            {
                var point = @base.Position + (m.Position - @base.Position).Normalized() * YasuoSpells.E.Range;

                if (!ignoreTower && point.UnderTurret(true))
                {
                    continue;
                }

                if (!((Vector3) returnObjects[0]).IsValid())
                {
                    returnObjects[0] = point;
                    returnObjects[1] = m;
                }
                else if (point.Distance(vector3) < ((Vector3) returnObjects[0]).Distance(vector3))
                {
                    returnObjects[0] = point;
                    returnObjects[1] = m;
                }
            }

            var enemies =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        e =>
                            e.IsValidTarget() && e.Distance(@base.Position) <= YasuoSpells.E.Range &&
                            @base.IsDashable(e));
            foreach (var e in enemies)
            {
                if (focusTarget.IsValidTarget() && e == focusTarget)
                {
                    continue;
                }

                var point = @base.Position + (e.Position - @base.Position).Normalized() * YasuoSpells.E.Range;

                if (ignoreTower && point.UnderTurret(true))
                {
                    continue;
                }

                if (!((Vector3) returnObjects[0]).IsValid())
                {
                    returnObjects[0] = point;
                    returnObjects[1] = e;
                }
                else if (point.Distance(vector3) < ((Vector3) returnObjects[0]).Distance(vector3))
                {
                    returnObjects[0] = point;
                    returnObjects[1] = e;
                }
            }

            return returnObjects;
        }

        /// <summary>
        ///     Quick usage of moving a hero
        /// </summary>
        /// <param name="player">Hero Object</param>
        /// <param name="pVector3">Position to Move</param>
        /// <param name="holdAreaRadius">Hold Radius</param>
        public static void MoveTo(this Obj_AI_Hero @player, Vector3 pVector3, float holdAreaRadius = 0)
        {
            if (Environment.TickCount - _lastMoveCommandT < Delay)
            {
                return;
            }

            _lastMoveCommandT = Environment.TickCount;

            if (@player.ServerPosition.Distance(pVector3) < holdAreaRadius)
            {
                if (@player.Path.Count() <= 1)
                {
                    return;
                }

                @player.IssueOrder(GameObjectOrder.HoldPosition, @player.ServerPosition);

                return;
            }

            var point = @player.ServerPosition +
                        ((Random.NextFloat(0.6f, 1) + 0.2f) * MinDistance) *
                        (pVector3.To2D() - @player.ServerPosition.To2D()).Normalized().To3D();

            @player.IssueOrder(GameObjectOrder.MoveTo, point);
        }
    }
}