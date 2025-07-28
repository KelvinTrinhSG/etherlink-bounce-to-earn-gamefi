#pragma warning disable 0649

using UnityEngine;

namespace Watermelon
{
    public class AchievementValue : Achievement
    {
        [SerializeField]
        private int requiredState;
        public int RequiredState
        {
            get { return requiredState; }
        }

        [System.NonSerialized]
        private int currentState;
        public int CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        public AchievementValue()
        {
            achievementType = AchievementType.Value;
        }

        public override bool Check()
        {
            return false;
        }

        public override bool Check(int value)
        {
            return true;
        }

        public override void GetReward()
        {

        }

        public override void Forget()
        {
            base.Forget();

            currentState = 0;
        }

        public override SavableObject SaveObject()
        {
            return new AchievementSave(id, isUnlocked, currentState);
        }

        public override void LoadObject(SavableObject savableObject)
        {
            AchievementSave achievementSave = (AchievementSave)savableObject;

            if (achievementSave != null)
            {
                isUnlocked = achievementSave.isUnlocked;
                currentState = achievementSave.currentState;
            }
        }

        public override float ProgressValue()
        {
            return (float)currentState / requiredState;
        }
    }
}