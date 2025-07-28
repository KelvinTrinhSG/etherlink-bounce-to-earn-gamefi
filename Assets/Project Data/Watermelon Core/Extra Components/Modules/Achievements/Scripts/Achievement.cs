#pragma warning disable 0649

using UnityEngine;

namespace Watermelon
{
    public abstract class Achievement : ScriptableObject, ISavable
    {
        [SerializeField]
        protected int id;
        public int ID
        {
            get { return id; }
#if UNITY_EDITOR
            set { id = value; }
#endif
        }

        [SerializeField]
        [MultilanguageWord("ui.achievements")]
        protected string description;
        public string Description
        {
            get { return description; }
        }

        [SerializeField]
        private Sprite icon;
        public Sprite Icon
        {
            get { return icon; }
        }

        protected AchievementType achievementType;
        public AchievementType AchievementType
        {
            get { return achievementType; }
        }

        [System.NonSerialized]
        protected bool isUnlocked = false;
        public bool IsUnlocked
        {
            get { return isUnlocked; }
        }

        public abstract bool Check();
        public abstract bool Check(int value);
        public abstract void GetReward();

        public virtual SavableObject SaveObject()
        {
            return new AchievementSave(id, isUnlocked, 0);
        }

        public virtual void LoadObject(SavableObject savableObject)
        {
            AchievementSave achievementSave = (AchievementSave)savableObject;

            if (achievementSave != null)
            {
                isUnlocked = achievementSave.isUnlocked;
            }
        }

        public void Unlock()
        {
            isUnlocked = true;

            if (AchievementManager.OnAchievementUnlocked != null)
                AchievementManager.OnAchievementUnlocked.Invoke(this);
        }

        public virtual string Progress(bool format = false)
        {
            return "";
        }

        public virtual float ProgressValue()
        {
            return 0;
        }

        public virtual void Forget()
        {
            isUnlocked = false;
        }
    }

    public enum AchievementType
    {
        Single,
        Progress,
        Value
    }
}