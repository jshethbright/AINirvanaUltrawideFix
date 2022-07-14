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
            //LoggerInstance.Msg("Application started.");

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
                Cursor.visible = CursorVisible.Value;

                QualitySettings.antiAliasing = MSAA.Value;
                //QualitySettings.antiAliasing = 0;
            }

            

            
        }

        // Fix in-game MSAA
        [HarmonyPatch]
        public class MSAAFix
        {


            // Adjust pipeline settings to match MSAA value on creation
            [HarmonyPatch(typeof(UniversalRenderPipelineAsset), "CreatePipeline")]
            [HarmonyPrefix]
            public static void FixPipeline(UniversalRenderPipelineAsset __instance)
            {
                

                switch (QualitySettings.antiAliasing)
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

            

            //[HarmonyPatch(typeof(RenderPipeline), "BeginCameraRendering")]
            //[HarmonyPrefix]
            //public static void test(ref Camera camera)
            //{
            //    if (camera != null)
            //    {
            //        if (camera.activeTexture.name.Contains("AiSight"))
            //        {
            //            RenderTexture currTex = camera.activeTexture;
            //            camera.activeTexture.antiAliasing = 0;
            //        }
            //    }
            //}



            //[HarmonyPatch(typeof(Game.CameraController), "OnDisable")]
            //[HarmonyPostfix]
            //public static void reEnableAfterSomCamera(Game.CameraController __instance)
            //{
            //    Camera currCam = __instance._camera;

            //    if (currCam != null)
            //    {
            //        //MelonLogger.Msg(currCam.name);
            //        if (currCam.name.Contains("Som"))
            //        {
            //            MelonLogger.Msg($"error: {currCam.name}");
            //            MelonLogger.Msg(currCam.name);
            //            QualitySettings.antiAliasing = MSAA.Value;
            //        }
            //    }
            //}


            // Check AiSight mode and disable MSAA calculations if mode is a filter
            [HarmonyPatch(typeof(Game.AiSight), "SetMode")]
            [HarmonyPrefix]
            public static void FixAiSightFilter(Game.AiSight __instance, ref Special special)
            {
                //MelonLogger.Msg(special);
                if (special != Special.Info && special != Special.Normal && special != Special.Zoom)
                {
                    QualitySettings.antiAliasing = 0;

                }
                
            }

            // Disable MSAA in Somniums and reenable afterwards.
            [HarmonyPatch(typeof(Game.SceneController), "Load")]
            [HarmonyPrefix]
            public static void AdjustMSAAByScene(ref string sceneName)
            {
                MelonLogger.Msg(sceneName);
                if (sceneName.StartsWith("M"))
                {
                    QualitySettings.antiAliasing = 0;
                }

                if (
                    !sceneName.Contains("Option") && 
                    !sceneName.StartsWith("M") && 
                    !sceneName.StartsWith("aiba") &&
                    !sceneName.Contains("evaluation") &&
                    !sceneName.Contains("file")
                    )
                {
                    QualitySettings.antiAliasing = MSAA.Value;
                }
                
            }

            //[HarmonyPatch(typeof(Game.AiSight), "Update")]
            //[HarmonyPostfix]
            //public static void reenableAfterFilter(Game.AiSight __instance)
            //{
            //    if (QualitySettings.antiAliasing != MSAA.Value)
            //    {
            //        if (!__instance.IsActive() && !__instance.IsVisible() && !__instance.IsFullOpen())
            //        {
            //            MelonLogger.Msg("Filter inactive, AA now on");
            //            QualitySettings.antiAliasing = 8;
            //            RenderPipelineAsset currRenderPipe = GraphicsSettings.renderPipelineAsset;
            //            QualitySettings.renderPipeline = RenderPipelineAsset.Instantiate(currRenderPipe);
            //        }
            //    }

            //}
        }

        [HarmonyPatch]
        public class UIFixes
        {
            public static float NewAspectRatio = (float)DesiredResolutionX.Value / DesiredResolutionY.Value;



            //Implement various UI scaling fixed by changing ScreenMatchMode and scaling filters for specific canvases.
            [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
            [HarmonyPostfix]
            public static void FixUIScaling(CanvasScaler __instance)
            {
                if (NewAspectRatio > 1.8 && UIFix.Value)
                {
                    string currName = __instance.gameObject.name;
                    //MelonLogger.Msg(currName);

                    if (currName == "CanvasBrightness")
                    {
                        __instance.m_ReferenceResolution = new Vector2(512, 512);
                        __instance.m_ScreenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                        __instance.matchWidthOrHeight = 0;
                    }

                    else if (currName == "#Canvas")
                    {


                        var baseChildren = GameObject.FindObjectsOfType<Game.FilterController>();

                        foreach (Game.FilterController item in baseChildren)
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

                        var baseChildren = GameObject.FindObjectsOfType<Game.FilterController>();

                        foreach (Game.FilterController item in baseChildren)
                        {
                            GameObject currObj = item.gameObject;
                            string itemName = currObj.name;
                            if (itemName.Contains("Filter"))
                            {
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
