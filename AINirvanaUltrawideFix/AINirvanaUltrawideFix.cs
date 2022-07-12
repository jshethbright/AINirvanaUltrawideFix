using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

[assembly: MelonInfo(typeof(AINirvanaUltrawideFix.AINirvanaUltrawideFix), "AI: Somnium Files - nirvanA Initiative", "1.0.0", "jshethbright")]
[assembly: MelonGame("SpikeChunsoft", "AI_TheSomniumFiles2")]
namespace AINirvanaUltrawideFix
{
    public class AINirvanaUltrawideFix : MelonMod
    {
        public static MelonPreferences_Category Fixes;
        public static MelonPreferences_Entry<int> DesiredResolutionX;
        public static MelonPreferences_Entry<int> DesiredResolutionY;
        public static MelonPreferences_Entry<bool> Fullscreen;
        public static MelonPreferences_Entry<bool> UIFix;
        public static MelonPreferences_Entry<bool> CursorVisible;
        public static MelonPreferences_Entry<int> MSAA;

        public override void OnApplicationStart()
        {
            LoggerInstance.Msg("Application started.");

            Fixes = MelonPreferences.CreateCategory("AINirvanaUltrawideFix");
            Fixes.SetFilePath("Mods/AINirvanaUltrawideFix.cfg");
            DesiredResolutionX = Fixes.CreateEntry("Resolution_Width", Display.main.systemWidth, "", "Custom resolution width"); // Set default to something safe
            DesiredResolutionY = Fixes.CreateEntry("Resolution_Height", Display.main.systemHeight, "", "Custom resolution height"); // Set default to something safe
            Fullscreen = Fixes.CreateEntry("Fullscreen", true, "", "Set to true for fullscreen or false for windowed");
            UIFix = Fixes.CreateEntry("UI_Fixes", true, "", "Fixes UI issues at ultrawide/wider");
            CursorVisible = Fixes.CreateEntry("CursorVisible", false, "", "Hides cursor with default (false) value.");
            MSAA = Fixes.CreateEntry("MSAA", 8, "", "Enable Multisample anti-aliasing (MSAA), [Accepted values are: 0, 2, 4, 8]");
        }


        [HarmonyPatch]
        public class CustomResolution
        {
            [HarmonyPatch(typeof(Game.LauncherArgs), nameof(Game.LauncherArgs.OnRuntimeMethodLoad))]
            [HarmonyPostfix]
            public static void SetResolution()
            {
                if (!Fullscreen.Value)
                {
                    Screen.SetResolution(DesiredResolutionX.Value, DesiredResolutionY.Value, FullScreenMode.Windowed);
                }
                else
                {
                    Screen.SetResolution(DesiredResolutionX.Value, DesiredResolutionY.Value, FullScreenMode.FullScreenWindow);
                }

                
                QualitySettings.antiAliasing = 8;
                var pipes = GameObject.FindObjectsOfType<UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset>(true);
                foreach (var pipe in pipes)
                {
                    MelonLogger.Msg(pipe.m_MSAA);
                    pipe.m_MSAA = UnityEngine.Rendering.Universal.MsaaQuality._8x;
                    //GraphicsSettings.renderPipelineAsset = pipe;

                }
            }

            
        }

        // Fix in-game MSAA
        [HarmonyPatch]
        public class MSAAFix
        {
            [HarmonyPatch(typeof(UniversalRenderPipelineAsset), "CreatePipeline")]
            [HarmonyPrefix]
            public static void fixPipeline(UniversalRenderPipelineAsset __instance)
            {
                //MelonLogger.Msg($"{__instance.name}: {__instance.GetInstanceID()}");
                switch (MSAA.Value)
                {
                    case 0:
                        __instance.m_MSAA = MsaaQuality.Disabled;
                        break;
                    case 2:
                        __instance.m_MSAA = MsaaQuality._2x;
                        break;
                    case 4:
                        __instance.m_MSAA = MsaaQuality._4x;
                        break;
                    case 8:
                        __instance.m_MSAA = MsaaQuality._8x;
                        break;
                    default:
                        __instance.m_MSAA = MsaaQuality.Disabled;
                        break;
                }
            }
        }

        [HarmonyPatch]
        public class UIFixes
        {
            public static float NewAspectRatio = (float)DesiredResolutionX.Value / DesiredResolutionY.Value;

            // Leave cursor visible at main menu
            [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
            [HarmonyPostfix]
            public static void test()
            {
                GameObject uiActive = GameObject.Find("Canvas2");

                if (uiActive != null && Cursor.visible == true)
                {
                    if (!CursorVisible.Value)
                    {
                        Cursor.visible = false;
                    }
                }
            }


            // Implement various UI scaling fixed by changing ScreenMatchMode and scaling filters for specific canvases.
            [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
            [HarmonyPostfix]
            public static void FixUIScaling(CanvasScaler __instance)
            {
                if (NewAspectRatio > 1.8 && UIFix.Value)
                {
                    string currName = __instance.gameObject.name;

                    if (currName == "CanvasBrightness")
                    {
                        __instance.m_ReferenceResolution = new Vector2(512, 512);
                        __instance.m_ScreenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                        __instance.matchWidthOrHeight = 0;
                    }

                    else if (currName == "#Canvas")
                    {
                        

                        Canvas currCanvas = __instance.gameObject.GetComponent<Canvas>();
                        Transform[] baseChildren = currCanvas.GetComponentsInChildren<Transform>(false);
                        
                        foreach (Transform item in baseChildren)
                        {
                            GameObject currObj = item.gameObject;
                            string itemName = currObj.name;
                            if (itemName.Contains("Filter"))
                            {
                                
                                currObj.transform.localScale = new Vector3(1 * NewAspectRatio, 1f, 1f);
                            }
                        }
                    }

                    else if (currName == "Canvas2")
                    {
                        __instance.m_ScreenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                        __instance.matchWidthOrHeight = 1;
                        Canvas currCanvas = __instance.gameObject.GetComponent<Canvas>();


                        Transform[] allChildren = currCanvas.GetComponentsInChildren<Transform>(false);

                        foreach (Transform item in allChildren)
                        {
                            GameObject currObj = item.gameObject;
                            string itemName = currObj.name;
                            if (itemName.Contains("Filter"))
                            {
                                //MelonLogger.Msg(itemName);
                                currObj.transform.localScale = new Vector3(1 * NewAspectRatio, 1f, 1f);
                            }
                        }
                    }

                    else
                    {
                        __instance.m_ScreenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                    }
                    
                }
            }

            // Fix letterboxing to span screen
            [HarmonyPatch(typeof(Game.CinemaScope), "Show")]
            [HarmonyPostfix]
            public static void LetterboxFix()
            {
                if (NewAspectRatio > 1.8 && UIFix.Value)
                {
                    var GameObjects = GameObject.FindObjectsOfType<Game.CinemaScope>();
                    foreach (var GameObject in GameObjects)
                    {
                        GameObject.transform.localScale = new Vector3(1 * NewAspectRatio, 1f, 1f);
                    }
                }
            }
        }
    }
}
