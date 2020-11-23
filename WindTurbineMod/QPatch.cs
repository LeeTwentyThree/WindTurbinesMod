using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using SMLHelper.V2.Crafting;
using QModManager.API.ModLoading;

namespace WindTurbinesMod
{
    [QModCore]
    public class QPatch
    {
        public const string modName = "WindTurbinesMod";

        public static TechType turbineBlade;
        public static TechType turbineGenerator;
        public static TechType turbinePole;
        public static AssetBundle bundle;
        public static WindTurbineConfig config;
        public static string mainDirectory;
        public static string assetsFolder;

        static void LoadAssetBundle()
        {
            bundle = AssetBundle.LoadFromFile(Path.Combine(assetsFolder, "windturbineassets"));
        }

        static void LoadConfig()
        {
            string configPath = Path.Combine(mainDirectory, "config.txt");
            if (File.Exists(configPath))
            {
                using(StreamReader sr = new StreamReader(configPath))
                {
                    string json = sr.ReadToEnd();
                    config = JsonUtility.FromJson<WindTurbineConfig>(json);
                }
            }
        }

        [QModPatch]
        public static void Patch()
        {
            mainDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            assetsFolder = Path.Combine(mainDirectory, "Assets");
            LoadAssetBundle();
            LoadConfig();
            //patch crafting recipes
            //is there a more efficient way of doing this?
            turbineBlade = TechTypeHandler.AddTechType("TurbineBlade", "Turbine Blade", "Necessary component in constructing a wind turbine. Large and lightweight for maximum aerodynamics.", GetSprite("TurbineBlade"));
            CraftDataHandler.SetItemSize(turbineBlade, new Vector2int(2, 1));
            var techDataBlade = new TechData()
            {
                craftAmount = 3,
                Ingredients = new List<Ingredient>()
                {
                  new Ingredient(TechType.Titanium, 3)
                }
            };
            CraftDataHandler.SetTechData(turbineBlade, techDataBlade);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, turbineBlade, "Resources", "Electronics");

            turbineGenerator = TechTypeHandler.AddTechType("TurbineGenerator", "Turbine Generator", "Necessary component in constructing a wind turbine. Converts mechanical energy of the blades into usable electricity.", GetSprite("Generator"));
            CraftDataHandler.SetItemSize(turbineGenerator, new Vector2int(2, 2));
            var techDataGen = new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                  new Ingredient(TechType.WiringKit, 1),
                  new Ingredient(TechType.PowerCell, 1),
                  new Ingredient(TechType.Lubricant, 1)
                }
            };
            CraftDataHandler.SetTechData(turbineGenerator, techDataGen);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, turbineGenerator, "Resources", "Electronics");

            turbinePole = TechTypeHandler.AddTechType("TurbinePole", "Turbine Base", "Necessary component in constructing a wind turbine. Supports the large structure.", GetSprite("TurbinePole"));
            CraftDataHandler.SetItemSize(turbinePole, new Vector2int(1, 2));
            var techDataPole = new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                  new Ingredient(TechType.Titanium, 4)
                }
            };
            CraftDataHandler.SetTechData(turbinePole, techDataPole);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, turbinePole, "Resources", "Electronics");

            var turbine = new WindTurbine.TurbinePatch();
            turbine.Patch();

            //Add the databank entry.
            LanguageHandler.SetLanguageLine("Ency_WindTurbine", "Wind Turbine");
            LanguageHandler.SetLanguageLine("EncyDesc_WindTurbine", string.Format("A large generator suspended by 17.5 meter tall pole. The lightweight blades are rotated by the planet's strong air currents and efficiently converts the force into electrical energy. The generator contains a large internal battery that can hold up to {0} units of power. Unlike solar panels, these operate at roughly the same efficiency throughout the day. Orientation does not appear to affect power output. However certain places seem to simply have more wind than others. Power output also increases with altitude.", config.MaxPower));

            //This just isn't working for now. Maybe another update?
            //var windTool = new WindTool.WindToolPatch();
            //windTool.Patch();
        }

        public static Atlas.Sprite GetSprite(string name)
        {
            return ImageUtils.LoadSpriteFromFile(@"./QMods/" + modName + "/Assets/" + name + ".png");
        }
    }
}