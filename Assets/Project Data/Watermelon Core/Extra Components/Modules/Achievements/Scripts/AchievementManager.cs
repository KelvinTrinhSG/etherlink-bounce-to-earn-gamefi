#pragma warning disable 0649

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Watermelon
{
    public class AchievementManager : MonoBehaviour
    {
        private static AchievementManager instance;

        [SerializeField]
        private AchievementsDatabase database;

        private bool isRequiredSave = false;

        private AchievementCallback onAchievementUnlocked;
        public static AchievementCallback OnAchievementUnlocked
        {
            get { return instance.onAchievementUnlocked; }
            set { instance.onAchievementUnlocked = value; }
        }

        private const string FILE_NAME = "Achievements.dat";

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private void OnDestroy()
        {
            Save();
        }

        private void OnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (isRequiredSave)
                Save();
        }

        private void Awake()
        {
            instance = this;

            database.Init();

            if (!Serializer.FileExistsAtPDP(FILE_NAME))
            {
                Save();
            }
            else
            {
                Load();
            }
        }

        public static void UnlockAchievement(int achievementID)
        {
            Achievement achievement = GetAchievement(achievementID);

            if (achievement != null && !achievement.IsUnlocked)
            {
                achievement.Unlock();

                instance.isRequiredSave = true;
            }
        }

        public static void IncrementProgress(int achievementID, int value)
        {
            Achievement achievement = GetAchievement(achievementID);

            if (achievement != null && !achievement.IsUnlocked && achievement.AchievementType == AchievementType.Progress)
            {
                AchievementProgress progressAchievement = (AchievementProgress)achievement;

                progressAchievement.CurrentState += value;
                if (progressAchievement.CurrentState >= progressAchievement.RequiredState)
                {
                    achievement.Unlock();
                }

                instance.isRequiredSave = true;
            }
        }

        public static void SetProgress(int achievementID, int value)
        {
            Achievement achievement = GetAchievement(achievementID);

            if (achievement != null && !achievement.IsUnlocked && achievement.AchievementType == AchievementType.Value)
            {
                AchievementValue valueAchievement = (AchievementValue)achievement;

                if (value > valueAchievement.CurrentState)
                {
                    valueAchievement.CurrentState = value;

                    if (value >= valueAchievement.RequiredState)
                    {
                        achievement.Unlock();
                    }

                    instance.isRequiredSave = true;
                }
            }
        }

        public static void ForgetAchievement(int achievementID)
        {
            Achievement achievement = GetAchievement(achievementID);

            if (achievement != null)
            {
                achievement.Forget();

                instance.isRequiredSave = true;
            }
        }

        public static bool IsAchievementUnlocked(int achievementID)
        {
            if (instance.database.achievementsLink.ContainsKey(achievementID))
            {
                return instance.database.Achievements[instance.database.achievementsLink[achievementID]].IsUnlocked;
            }

            return false;
        }

        public static Achievement GetAchievement(int id)
        {
            if (instance.database.achievementsLink.ContainsKey(id))
            {
                return instance.database.Achievements[instance.database.achievementsLink[id]];
            }

            return null;
        }

        public static Achievement[] GetAchievements()
        {
            return instance.database.Achievements;
        }

        public void Load()
        {
            Achievement[] achievements = database.Achievements;

            SaveContainer saveContainer = Serializer.DeserializeFromPDP<SaveContainer>(FILE_NAME, logIfFileNotExists: false);

            for (int i = 0; i < saveContainer.achievementSave.Length; i++)
            {
                if (database.achievementsLink.ContainsKey(saveContainer.achievementSave[i].id))
                    achievements[database.achievementsLink[saveContainer.achievementSave[i].id]].LoadObject(saveContainer.achievementSave[i]);
            }
        }

        public void Save()
        {
            Achievement[] achievements = database.Achievements;

            AchievementSave[] achievementSaves = new AchievementSave[achievements.Length];
            for (int i = 0; i < achievementSaves.Length; i++)
            {
                achievementSaves[i] = (AchievementSave)achievements[i].SaveObject();
            }

            Serializer.SerializeToPDP(new SaveContainer(achievementSaves), FILE_NAME);
        }

        [System.Serializable]
        public class SaveContainer
        {
            public AchievementSave[] achievementSave;

            public SaveContainer()
            {

            }

            public SaveContainer(AchievementSave[] achievementSave)
            {
                this.achievementSave = achievementSave;
            }
        }

        public delegate void AchievementCallback(Achievement achievement);
        public delegate void AchievementMessageCallback(string title, string message, Sprite icon = null);
    }

    [System.Serializable]
    public class AchievementSave : SavableObject
    {
        public int id;
        public bool isUnlocked = false;
        public int currentState;

        public AchievementSave(int id, bool isUnlocked, int currentState)
        {
            this.id = id;
            this.isUnlocked = isUnlocked;
            this.currentState = currentState;
        }
    }
}
