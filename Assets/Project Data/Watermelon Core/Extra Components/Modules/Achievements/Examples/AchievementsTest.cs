using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class AchievementsTest : MonoBehaviour
    {
        private Achievement[] achievements;

        private void Start()
        {
            achievements = AchievementManager.GetAchievements();
        }

        private void OnGUI()
        {
            if (achievements != null)
            {
                GUILayout.BeginArea(new Rect(0, 200, Screen.width, Screen.height));
                for (int i = 0; i < achievements.Length; i++)
                {
                    GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.ExpandWidth(true));
                    GUILayout.Label("ID: " + achievements[i].ID);
                    GUILayout.Label(" | ");
                    GUILayout.Label("Name: " + achievements[i].name);
                    GUILayout.Label(" | ");
                    if (achievements[i].IsUnlocked)
                    {
                        GUILayout.Label("Status: " + achievements[i].IsUnlocked.ToString());
                    }
                    else
                    {
                        switch (achievements[i].AchievementType)
                        {
                            case AchievementType.Progress:
                                AchievementProgress achievementProgress = (AchievementProgress)achievements[i];
                                GUILayout.Label("Status: " + achievementProgress.CurrentState + "/" + achievementProgress.RequiredState);
                                break;
                            case AchievementType.Single:
                                GUILayout.Label("Status: " + achievements[i].IsUnlocked.ToString());
                                break;
                            case AchievementType.Value:
                                AchievementValue achievementValue = (AchievementValue)achievements[i];
                                GUILayout.Label("Status: " + achievementValue.CurrentState + "/" + achievementValue.RequiredState);
                                break;
                        }
                    }
                    GUILayout.Label(" | ");
                    GUILayout.Label("Type: " + achievements[i].AchievementType.ToString());

                    switch (achievements[i].AchievementType)
                    {
                        case AchievementType.Progress:
                            if (achievements[i].IsUnlocked)
                            {
                                if (GUILayout.Button("Forget"))
                                {
                                    AchievementManager.ForgetAchievement(achievements[i].ID);
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Increment"))
                                {
                                    AchievementManager.IncrementProgress(achievements[i].ID, 1);
                                }
                            }
                            break;
                        case AchievementType.Single:
                            if (achievements[i].IsUnlocked)
                            {
                                if (GUILayout.Button("Forget"))
                                {
                                    AchievementManager.ForgetAchievement(achievements[i].ID);
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Unlock"))
                                {
                                    AchievementManager.UnlockAchievement(achievements[i].ID);
                                }
                            }
                            break;
                        case AchievementType.Value:
                            if (achievements[i].IsUnlocked)
                            {
                                if (GUILayout.Button("Forget"))
                                {
                                    AchievementManager.ForgetAchievement(achievements[i].ID);
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Unlock"))
                                {
                                    AchievementValue achievementValue = (AchievementValue)achievements[i];
                                    AchievementManager.SetProgress(achievements[i].ID, achievementValue.RequiredState);
                                }
                            }
                            break;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
        }
    }
}