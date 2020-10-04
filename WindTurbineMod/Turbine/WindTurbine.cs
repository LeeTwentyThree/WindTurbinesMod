using System;
using System.Collections;
using UnityEngine;

namespace WindTurbinesMod.Turbine 
{
    public class WindTurbine : HandTarget, IHandTarget 
    {
        Constructable constructable;
        TurbineHealth health;
        bool NeedsMaintenance
        {
            get
            {
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
            relay.UpdateConnection();
            SetupAudio();
            health = gameObject.AddComponent<TurbineHealth>();
            health.SetData();
            health.health = 100f;
        }

        void SetupAudio()
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.clip = soundLoop;
            loopSource.loop = true;
            if (!loopSource.isPlaying) loopSource.Play();
            loopSource.maxDistance = 10f;
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
            if (health == null) health = GetComponent<TurbineHealth>();
            if (constructable == null) constructable = gameObject.GetComponent<Constructable>();
            if (constructable.constructed && Time.time > timeEastereggEnd && !NeedsMaintenance)
            {
                if (!loopSource.isPlaying) loopSource.Play();
                float amount = this.GetRechargeScalar() * DayNightCycle.main.deltaTime * 40f * WindyMultiplier();
                float num;
                this.relay.ModifyPower(amount / 4f, out num);
                if(health.health - num > 0f) health.TakeDamage(num / 20f);
                this.spin.spinSpeed = amount * 10f;
                this.loopSource.volume = Mathf.Clamp(amount, 0.6f, 0.8f);
            }
            if (NeedsMaintenance)
            {
                this.spin.spinSpeed = 0f;
                loopSource.Stop();
            }
        }

        float WindyMultiplier()
        {
            return (1f + (Mathf.PerlinNoise(0f, Time.time * 0.05f) * 1.5f));
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
                        HandReticle.main.SetInteractText("Wind Turbine: " + Mathf.RoundToInt(this.GetRechargeScalar() * 100f * WindyMultiplier()) + "% efficiency, " + Mathf.RoundToInt(this.powerSource.GetPower()).ToString() + "/" + Mathf.RoundToInt(this.powerSource.GetMaxPower()) + " power", "Needs maintenance (use repair tool)", false, false, HandReticle.Hand.None);
                        HandReticle.main.SetIcon(HandReticle.IconType.Info, 1.5f);
                    }
                    else
                    {
                        HandReticle.main.SetInteractText("Wind Turbine: " + Mathf.RoundToInt(this.GetRechargeScalar() * 100f * WindyMultiplier()) + "% efficiency, " + Mathf.RoundToInt(this.powerSource.GetPower()).ToString() + "/" + Mathf.RoundToInt(this.powerSource.GetMaxPower()) + " power", false, HandReticle.Hand.None);
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

        public PowerSource powerSource;

        [AssertNotNull]
        public PowerRelay relay;

        public TurbineSpin spin;

        public AudioClip soundLoop;
        public AudioSource loopSource;

        public float timeEastereggEnd = 0f;
    }

}