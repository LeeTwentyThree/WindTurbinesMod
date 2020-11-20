using System;
using System.Collections;
using UnityEngine;

namespace WindTurbinesMod.WindTurbine 
{
    public class WindTurbine : HandTarget, IHandTarget 
    {
        Constructable constructable;
        TurbineHealth health;

        public PowerSource powerSource;

        [AssertNotNull]
        public PowerRelay relay;

        public TurbineSpin spin;

        public AudioClip soundLoop;
        public AudioSource loopSource;

        public float timeEastereggEnd = 0f;

        bool NeedsMaintenance
        {
            get
            {
                if (!QPatch.config.TurbineTakesDamage) return false;
                return health.health < 10f;
            }
        }

        public void Activate()
        {
            spin = gameObject.FindChild("Blade Parent").AddComponent<TurbineSpin>();
            powerSource = gameObject.AddComponent<PowerSource>();
            powerSource.maxPower = 750f;
            relay = gameObject.AddComponent<PowerRelay>();
            relay.internalPowerSource = powerSource;
            relay.dontConnectToRelays = false;
            relay.maxOutboundDistance = 50;

            PowerFX yourPowerFX = gameObject.AddComponent<PowerFX>();
            PowerRelay powerRelay = CraftData.GetPrefabForTechType(TechType.SolarPanel).GetComponent<PowerRelay>();

            yourPowerFX.vfxPrefab = powerRelay.powerFX.vfxPrefab;
            yourPowerFX.attachPoint = gameObject.transform;
            relay.powerFX = yourPowerFX;

            Resources.UnloadAsset(powerRelay);
            relay.UpdateConnection();

            if(QPatch.config.TurbineMakesNoise) SetupAudio();
        }

        void Start()
        {
            health = gameObject.AddComponent<TurbineHealth>();
            health.SetData();
            health.health = 200f;
        }

        void SetupAudio()
        {
            loopSource = spin.gameObject.AddComponent<AudioSource>();
            loopSource.clip = soundLoop;
            loopSource.loop = true;
            if (!loopSource.isPlaying) loopSource.Play();
            loopSource.maxDistance = 15f;
            loopSource.spatialBlend = 1f;
        }

        private float GetDepthScalar()
        {
            return Mathf.Clamp(spin.transform.position.y / 35f, 0f, 1f);
        }

        private float GetSunScalar()
        {
            return Mathf.Clamp(DayNightCycle.main.GetLocalLightScalar() * 0.5f, 0.4f, 0.6f) + 0.4f;
        }

        private float GetRechargeScalar()
        {
            return this.GetDepthScalar() * this.GetSunScalar();
        }

        private void Update()
        {
            if(health == null) health = GetComponent<TurbineHealth>();
            if (constructable == null) constructable = gameObject.GetComponent<Constructable>();
            if (constructable.constructed && Time.time > timeEastereggEnd && !NeedsMaintenance)
            {
                if (!loopSource.isPlaying) loopSource.Play();
                float amount = this.GetRechargeScalar() * DayNightCycle.main.deltaTime * 40f * WindyMultiplier(new Vector2(transform.position.x, transform.position.z));
                this.relay.ModifyPower(amount / 4f, out float num);
                if(QPatch.config.TurbineTakesDamage && health.health - num > 0f) health.TakeDamage(num / 17f);
                this.spin.spinSpeed = amount * 10f;
                this.loopSource.volume = Mathf.Clamp(amount, 0.6f, 0.8f);
            }
            if (NeedsMaintenance)
            {
                this.spin.spinSpeed = 0f;
                loopSource.Stop();
            }
        }

        public static float WindyMultiplier(Vector2 position)
        {
            if (QPatch.config.PositionInfluencesPower)
            {
                return 1f + (Mathf.PerlinNoise(position.x * 0.01f, position.y * 0.01f) - 0.5f) * 0.5f;
            }
            else
            {
                return 1f;
            }
        }
        public void OnHandHover(GUIHand hand)
        {
            if(constructable == null) constructable = gameObject.GetComponent<Constructable>();
            if (constructable.constructed)
            {
                if(spin.transform.position.y > 1f)
                {
                    if(NeedsMaintenance)
                    {
                        HandReticle.main.SetInteractText("Wind Turbine: " + Mathf.RoundToInt(this.GetRechargeScalar() * 100f * WindyMultiplier(new Vector3(transform.position.x, transform.position.z))) + "% efficiency, " + Mathf.RoundToInt(this.powerSource.GetPower()).ToString() + "/" + Mathf.RoundToInt(this.powerSource.GetMaxPower()) + " power", "Needs maintenance (use repair tool)", false, false, HandReticle.Hand.None);
                        HandReticle.main.SetIcon(HandReticle.IconType.Info, 1.5f);
                    }
                    else
                    {
                        HandReticle.main.SetInteractText("Wind Turbine: " + Mathf.RoundToInt(this.GetRechargeScalar() * 100f * WindyMultiplier(new Vector3(transform.position.x, transform.position.z))) + "% efficiency, " + Mathf.RoundToInt(this.powerSource.GetPower()).ToString() + "/" + Mathf.RoundToInt(this.powerSource.GetMaxPower()) + " power", false, HandReticle.Hand.None);
                        HandReticle.main.SetIcon(HandReticle.IconType.Info, 1f);
                    }
                }
                else
                {
                    HandReticle.main.SetInteractText("Wind Turbine: 0% efficiency", "Notice: Blades are submerged, please relocate", false, false, HandReticle.Hand.None);
                    HandReticle.main.SetIcon(HandReticle.IconType.HandDeny, 1f);
                }
            }
        }

        public void OnHandClick(GUIHand hand)
        {
            spin.spinSpeed = 1000f;
            timeEastereggEnd = Time.time + 1f;
        }
    }

}