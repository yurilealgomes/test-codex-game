using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public static class SkillTargeting
    {
        public static IDamageable FindNearestEnemy(Vector3 origin, float range)
        {
            IDamageable best = null;
            float bestDistance = range * range;

            for (int i = 0; i < EnemyController.ActiveEnemies.Count; i++)
            {
                EnemyController enemy = EnemyController.ActiveEnemies[i];
                if (enemy == null || !enemy.IsAlive)
                {
                    continue;
                }

                float distance = (enemy.transform.position - origin).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = enemy.Health;
                }
            }

            for (int i = 0; i < BossController.ActiveBosses.Count; i++)
            {
                BossController boss = BossController.ActiveBosses[i];
                if (boss == null || !boss.IsAlive)
                {
                    continue;
                }

                float distance = (boss.transform.position - origin).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = boss;
                }
            }

            for (int i = 0; i < BreakableObject.ActiveBreakables.Count; i++)
            {
                BreakableObject breakable = BreakableObject.ActiveBreakables[i];
                if (breakable == null || !breakable.IsAlive)
                {
                    continue;
                }

                float distance = (breakable.transform.position - origin).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = breakable;
                }
            }

            return best;
        }

        public static List<IDamageable> FindEnemies(Vector3 origin, float range, int maxCount)
        {
            List<IDamageable> results = new List<IDamageable>();
            float rangeSqr = range * range;

            for (int i = 0; i < EnemyController.ActiveEnemies.Count && results.Count < maxCount; i++)
            {
                EnemyController enemy = EnemyController.ActiveEnemies[i];
                if (enemy != null && enemy.IsAlive && (enemy.transform.position - origin).sqrMagnitude <= rangeSqr)
                {
                    results.Add(enemy.Health);
                }
            }

            for (int i = 0; i < BossController.ActiveBosses.Count && results.Count < maxCount; i++)
            {
                BossController boss = BossController.ActiveBosses[i];
                if (boss != null && boss.IsAlive && (boss.transform.position - origin).sqrMagnitude <= rangeSqr)
                {
                    results.Add(boss);
                }
            }

            for (int i = 0; i < BreakableObject.ActiveBreakables.Count && results.Count < maxCount; i++)
            {
                BreakableObject breakable = BreakableObject.ActiveBreakables[i];
                if (breakable != null && breakable.IsAlive && (breakable.transform.position - origin).sqrMagnitude <= rangeSqr)
                {
                    results.Add(breakable);
                }
            }

            results.Sort((a, b) =>
                (a.Transform.position - origin).sqrMagnitude.CompareTo((b.Transform.position - origin).sqrMagnitude));
            return results;
        }

        public static IDamageable FindNearestEnemyExcluding(Vector3 origin, float range, List<IDamageable> excluded)
        {
            IDamageable best = null;
            float bestDistance = range * range;
            List<IDamageable> candidates = FindEnemies(origin, range, 64);

            for (int i = 0; i < candidates.Count; i++)
            {
                if (excluded.Contains(candidates[i]))
                {
                    continue;
                }

                float distance = (candidates[i].Transform.position - origin).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = candidates[i];
                }
            }

            return best;
        }
    }
}
