namespace WindTurbinesMod.Turbine
{
    using System;
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    public class TurbinePatch : ModPrefab
    {
        public const string NameID = "turbine";

        public const string FriendlyName = "Wind Turbine";

        private GameObject prefab;

        internal TurbinePatch() : base(NameID, $"{NameID}PreFab")
        {

        }

        public override GameObject GetGameObject()
        {
            if (prefab == null)
            {
                prefab = QPatch.bundle.LoadAsset<GameObject>("turbine");

                var techTag = prefab.AddComponent<TechTag>();
                techTag.type = TechType;

                // Set prefab identifier
                var prefabId = prefab.AddComponent<PrefabIdentifier>();
                prefabId.ClassId = ClassID;

                var collider = prefab.AddComponent<BoxCollider>();
                collider.size = new Vector3(2f, 17f, 2f);
                collider.center = new Vector3(0f, 7.5f, 0f);

                var renderers = prefab.GetComponentsInChildren<Renderer>();
                var shader = Shader.Find("MarmosetUBER");

                foreach (var renderer in renderers)
                {
                    foreach (Material mat in renderer.materials)
                    {
                        if(mat.name != "Mat5") mat.shader = shader;
                    }
                }

                var skyApplier = prefab.AddComponent<SkyApplier>();
                skyApplier.renderers = renderers;
                skyApplier.anchorSky = Skies.Auto;

                // Add constructable - This prefab normally isn't constructed.
                Constructable constructible = prefab.AddComponent<Constructable>();
                constructible.allowedInBase = false;
                constructible.allowedInSub = false;
                constructible.allowedOutside = true;
                constructible.allowedOnCeiling = false;
                constructible.allowedOnGround = true;
                constructible.allowedOnWall = false;
                constructible.allowedOnConstructables = true;
                constructible.techType = this.TechType;
                constructible.rotationEnabled = true;
                constructible.placeDefaultDistance = 6f;
                constructible.placeMinDistance = 0.5f;
                constructible.placeMaxDistance = 15f;
                constructible.model = prefab.FindChild("Pole");

                var bounds = prefab.AddComponent<ConstructableBounds>();
                WindTurbine turbine = prefab.AddComponent<WindTurbine>();
                turbine.soundLoop = QPatch.bundle.LoadAsset<AudioClip>("turbineloop");
                turbine.Activate();

                AddLight(prefab.transform, 1f, 25f, new Vector3(-6f, 15f, 0f));
            }
            return prefab;
        }

        void AddLight(Transform parent, float intensity, float range, Vector3 position)
        {
            var obj = new GameObject();
            obj.transform.parent = parent;
            Light light = obj.AddComponent<Light>();
            light.intensity = intensity;
            light.range = range;
            light.transform.localPosition = position;
        }

        public void Patch()
        {
            // Create a new TechType for new fabricator
            this.TechType = TechTypeHandler.AddTechType(
                         internalName: NameID,
                         displayName: FriendlyName,
                         tooltip: "High efficiency generator that runs on wind. Only works above water. Must be placed on a base piece to function.",
                         sprite: QPatch.GetSprite("TurbineIcon"),
                         unlockAtStart: true);

            // Create a Recipie for the new TechType
            var customRecipe = new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[3]
                             {
                                 new Ingredient(QPatch.turbinePole, 1),
                                 new Ingredient(QPatch.turbineBlade, 3),
                                 new Ingredient(QPatch.turbineGenerator, 1)
                             })
            };

            // Add the new TechType to the buildables
            CraftDataHandler.AddBuildable(this.TechType);

            // Add the new TechType to the group of Interior Module buildables
            CraftDataHandler.AddToGroup(TechGroup.ExteriorModules, TechCategory.ExteriorModule, this.TechType);

            // Set the buildable prefab
            PrefabHandler.RegisterPrefab(this);

            // Associate the recipie to the new TechType
            CraftDataHandler.SetTechData(this.TechType, customRecipe);
        }
    }
}
