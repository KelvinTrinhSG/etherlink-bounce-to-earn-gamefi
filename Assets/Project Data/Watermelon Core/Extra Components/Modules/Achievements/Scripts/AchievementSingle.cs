using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class AchievementSingle : Achievement, ISavable
    {
        public AchievementSingle()
        {
            achievementType = AchievementType.Single;
        }

        public override bool Check()
        {
            return false;
        }

        public override bool Check(int value)
        {
            return false;
        }

        public override void GetReward()
        {

        }
    }
}