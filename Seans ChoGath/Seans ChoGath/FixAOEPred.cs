using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aimtec;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Prediction.Skillshots;
using Aimtec.SDK.Util.Cache;

namespace Seans_ChoGath
{
    class FixAOEPred
    {
        internal class cPossibleTarget
        {
            /// <summary>
            ///     Gets or sets the unit position.
            /// </summary>
            public Vector2 Position { get; set; }

            /// <summary>
            ///     Gets or sets the unit.
            /// </summary>
            public Obj_AI_Base Unit { get; set; }

        }

        public static PredictionOutput GetCirclePrediction(PredictionInput input)
        {
            var mainTargetPrediction = Prediction.GetPrediction(input);
            var posibleTargets = new List<cPossibleTarget>
            {
                new cPossibleTarget
                {
                    Position = mainTargetPrediction.UnitPosition.To2D(),
                    Unit = input.Unit
                }
            };

            if (mainTargetPrediction.HitChance >= HitChance.High)
            {
                //Add the posible targets  in range:
                foreach (var h in ObjectManager.Get<Obj_AI_Hero>().Where(e => e.IsEnemy && e.IsValidTarget(input.Radius, false, true, mainTargetPrediction.UnitPosition) && e != input.Unit))
                {
                    var prediction = Prediction.GetPrediction(Spells.Q.GetPredictionInput(h));
                    if (prediction.HitChance >= HitChance.Medium)
                    {
                        posibleTargets.Add(new cPossibleTarget { Position = prediction.UnitPosition.To2D(), Unit = h });
                    }
                }
            }

            if (posibleTargets.Count == 1)
            {
                return new PredictionOutput
                {
                    AoeTargetsHit = posibleTargets.Select(h => (Obj_AI_Hero)h.Unit).ToList(),
                    CastPosition = mainTargetPrediction.CastPosition,
                    UnitPosition = mainTargetPrediction.UnitPosition,
                    HitChance = mainTargetPrediction.HitChance,
                    AoeHitCount = posibleTargets.Count
                };
            }

            while (posibleTargets.Count > 1)
            {
                var mecCircle = Mec.GetMec(posibleTargets.Select(h => h.Position).ToList());

                if (mecCircle.Radius <= input.Radius + input.Unit.BoundingRadius - 10
                    && Vector2.DistanceSquared(mecCircle.Center, input.RangeCheckFrom.To2D())
                    < input.Range * input.Range)
                {
                    return new PredictionOutput
                    {
                        AoeTargetsHit = posibleTargets.Select(h => (Obj_AI_Hero)h.Unit).ToList(),
                        CastPosition = mecCircle.Center.To3D(),
                        UnitPosition = mainTargetPrediction.UnitPosition,
                        HitChance = mainTargetPrediction.HitChance,
                        AoeHitCount = posibleTargets.Count
                    };
                }

                float maxdist = -1;
                var maxdistindex = 1;
                for (var i = 1; i < posibleTargets.Count; i++)
                {
                    var distance = Vector2.DistanceSquared(posibleTargets[i].Position, posibleTargets[0].Position);
                    if (distance > maxdist || maxdist.CompareTo(-1) == 0)
                    {
                        maxdistindex = i;
                        maxdist = distance;
                    }
                }
                posibleTargets.RemoveAt(maxdistindex);
            }

            return mainTargetPrediction;
        }


        public static PredictionOutput GetConePrediction(PredictionInput input)
        {
            var mainTargetPrediction = Prediction.GetPrediction(input);

            var posibleTargets = new List<cPossibleTarget>
            {
                new cPossibleTarget { Position = (Vector2) mainTargetPrediction.UnitPosition, Unit = input.Unit }
            };

            if (mainTargetPrediction.HitChance >= HitChance.Medium)
            {
                //Add the posible targets  in range:
                foreach (var h in ObjectManager.Get<Obj_AI_Hero>().Where(e => e.IsEnemy && e.IsValidTarget(input.Radius, false, true, mainTargetPrediction.UnitPosition) && e != input.Unit))
                {
                    var prediction = Prediction.GetPrediction(Spells.W.GetPredictionInput(h));
                    if (prediction.HitChance >= HitChance.Medium)
                    {
                        posibleTargets.Add(new cPossibleTarget { Position = prediction.UnitPosition.To2D(), Unit = h });
                    }
                }
            }

            if (posibleTargets.Count <= 1)
            {
                return new PredictionOutput
                {
                    HitChance = mainTargetPrediction.HitChance,
                    AoeHitCount = posibleTargets.Count,
                    UnitPosition = mainTargetPrediction.UnitPosition,
                    CastPosition = (Vector3)mainTargetPrediction.CastPosition,
                };
            }


            var candidates = new List<Vector2>();

            foreach (var target in posibleTargets)
            {
                target.Position = target.Position - (Vector2)input.From;
            }

            for (var i = 0; i < posibleTargets.Count; i++)
            {
                for (var j = 0; j < posibleTargets.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var p = (posibleTargets[i].Position + posibleTargets[j].Position) * 0.5f;

                    if (!candidates.Contains(p))
                    {
                        candidates.Add(p);
                    }
                }
            }

            var bestCandidateHits = -1;
            var bestCandidate = default(Vector2);
            var positionsList = posibleTargets.Select(t => t.Position).ToList();

            foreach (var candidate in candidates)
            {
                var hits = GetHits(candidate, input.Range, input.Radius, positionsList);

                if (hits <= bestCandidateHits)
                {
                    continue;
                }

                bestCandidate = candidate;
                bestCandidateHits = hits;
            }

            if (bestCandidateHits > 1 && input.From.DistanceSquared(bestCandidate) > 50 * 50)
            {
                return new PredictionOutput
                {
                    HitChance = mainTargetPrediction.HitChance,
                    AoeHitCount = bestCandidateHits,
                    UnitPosition = mainTargetPrediction.UnitPosition,
                    CastPosition = (Vector3)bestCandidate,
                };
            }

            return mainTargetPrediction;
        }

        internal static int GetHits(Vector2 end, double range, float angle, List<Vector2> points)
        {
            return points.Select(x => new { point = x, edge1 = end.Rotated(-angle / 2) })
                .Select(x => new { t = x, edge2 = x.edge1.Rotated(angle) })
                .Where(
                    x => x.t.point.DistanceSquared(default(Vector2)) < range * range
                         && x.t.edge1.CrossProduct(x.t.point) > 0 && x.t.point.CrossProduct(x.edge2) > 0)
                .Select(x => x.t.point).Count();
        }

    }
}
