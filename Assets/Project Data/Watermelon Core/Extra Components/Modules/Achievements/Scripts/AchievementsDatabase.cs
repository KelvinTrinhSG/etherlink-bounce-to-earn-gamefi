using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Watermelon
{
    public class AchievementsDatabase : ScriptableObject
    {
        [SerializeField] Achievement[] achievements;
        public Achievement[] Achievements { get { return achievements; } }

        public Dictionary<int, int> achievementsLink = new Dictionary<int, int>();

        public void Init()
        {
            //Link achievements IDs
            for (int i = 0; i < achievements.Length; i++)
            {
                achievementsLink.Add(achievements[i].ID, i);
            }
        }
    }
}