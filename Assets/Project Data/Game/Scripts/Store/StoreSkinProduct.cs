using UnityEngine;
using Watermelon;

public class StoreSkinProduct : StoreProduct
{
    [Space]
    public int achievementID;

    private Achievement achievement;
    public Achievement Achievement
    {
        get { return achievement; }
    }

    public override void Init()
    {
        if(achievementID != -1)
        {
            achievement = AchievementManager.GetAchievement(achievementID);
        }
    }

    public StoreSkinProduct()
    {
        productBehaviourType = BehaviourType.Achievement;
    }

    public override bool IsOpened()
    {
        if(achievement != null)
        {
            return achievement.IsUnlocked;
        }

        return false;
    }

    public override void Buy()
    {
        //Do nothing
    }

    public override bool Check()
    {
        return true;
    }

    public override float Progress()
    {
        if (achievement != null)
        {
            if(achievement.IsUnlocked)
            {
                return 1;
            }
            else
            {
                return achievement.ProgressValue();
            }
        }

        return 0;
    }
}
