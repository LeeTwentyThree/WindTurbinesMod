using System;
using System.Collections;
using UnityEngine;

namespace WindTurbinesMod.Turbine 
{
    public class WindTurbine : HandTarget, IHandTarget
    {
        Constructable constructable;

        public void Activate()
        {
            spin = gameObject.FindChild("Blades").AddComponent<TurbineSpin>();
            powerSource = gameObject.AddComponent<PowerSource>();
            powerSource.maxPower = 1500f;
            relay = gameObject.AddComponent<PowerRelay>();
            relay.internalPowerSource = powerSource;
            SetupAudio();
        }

        void SetupAudio()
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.clip = soundLoop;
            loopSource.loop = true;
            if (!loopSource.isPlaying) loopSource.Play();
            loopSource.maxDistance = 20f;
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
            if (constructable == null) constructable = gameObject.GetComponent<Constructable>();
            if (constructable.constructed && Time.time > timeEastereggEnd)
            {
                float amount = this.GetRechargeScalar() * DayNightCycle.main.deltaTime * 40f;
                float num;
                this.relay.ModifyPower(amount / 4f, out num);
                this.spin.spinSpeed = amount * 10f;
                this.loopSource.volume = Mathf.Clamp(amount, 0.6f, 0.8f);
            }
        }

        public void OnHandHover(GUIHand hand)
        {
            if(constructable == null) constructable = gameObject.GetComponent<Constructable>();
            if (constructable.constructed)
            {
                if(spin.transform.position.y > 1f)
                {
                    HandReticle.main.SetInteractText("Wind Turbine: " + Mathf.RoundToInt(this.GetRechargeScalar() * 100f) + "% (" + Mathf.RoundToInt(this.powerSource.GetPower()).ToString() + "/" + Mathf.RoundToInt(this.powerSource.GetMaxPower()) + ")", false, HandReticle.Hand.None);
                    HandReticle.main.SetIcon(HandReticle.IconType.Info, 1f);
                }
                else
                {
                    HandReticle.main.SetInteractText("Wind Turbine: 0% (0/1500)", "Warning: Blades are submerged", false, false, HandReticle.Hand.None);
                    HandReticle.main.SetIcon(HandReticle.IconType.HandDeny, 1f);
                }
            }
        }

        public void OnHandClick(GUIHand hand)
        {
            timeEastereggEnd = Time.time + 0.5f;
            spin.spinSpeed = 1000f;
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