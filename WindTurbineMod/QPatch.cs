using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Harmony;
using UnityEngine;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using SMLHelper.V2.Crafting;

namespace WindTurbinesMod
{
    public class QPatch
    {
        public const string modName = "WindTurbinesMod";

        public static TechType turbineBlade;
        public static TechType turbineGenerator;
        public static TechType turbinePole;
        public static AssetBundle bundle;

        static void LoadAssetBundle()
        {
            string mainDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assetsFolder = Path.Combine(mainDirectory, "Assets");
            bundle = AssetBundle.LoadFromFile(Path.Combine(assetsFolder, "assets"));
        }
        public static void Patch()
        {
            LoadAssetBundle();
            //patch crafting recipes
            //is there a more efficient way of doing this?
            turbineBlade = TechTypeHandler.AddTechType("TurbineBlade", "Turbine Blade", "Necessary component in constructing a wind turbine. Large and lightweight to be easily pushed by wind.", GetSprite("TurbineBlade"));
            CraftDataHandler.SetItemSize(turbineBlade, new Vector2int(3, 1));
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

            turbineGenerator = TechTypeHandler.AddTechType("TurbineGenerator", "Turbine Generator", "Necessary component in constructing a wind turbine. Converts rotation of the blades into electricity.", GetSprite("Generator"));
            CraftDataHandler.SetItemSize(turbineGenerator, new Vector2int(2, 2));
            var techDataGen = new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                  new Ingredient(TechType.WiringKit, 1),
                  new Ingredient(TechType.Magnetite, 1),
                  new Ingredient(TechType.Lubricant, 1)
                }
            };
            CraftDataHandler.SetTechData(turbineGenerator, techDataGen);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, turbineGenerator, "Resources", "Electronics");

            turbinePole = TechTypeHandler.AddTechType("TurbinePole", "Turbine Base", "Necessary component in constructing a wind turbine. Supports the massive structure.", GetSprite("TurbinePole"));
            CraftDataHandler.SetItemSize(turbinePole, new Vector2int(1, 3));
            var techDataPole = new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                  new Ingredient(TechType.PlasteelIngot, 1),
                  new Ingredient(TechType.CopperWire, 1),
                }
            };
            CraftDataHandler.SetTechData(turbinePole, techDataPole);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, turbinePole, "Resources", "Electronics");

            var turbine = new Turbine.TurbinePatch();
            turbine.Patch();
        }

        public static Atlas.Sprite GetSprite(string name)
        {
            return ImageUtils.LoadSpriteFromFile(@"./QMods/" + modName + "/Assets/" + name + ".png");
        }
    }
}