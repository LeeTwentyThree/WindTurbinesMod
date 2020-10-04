﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindTurbinesMod.Turbine
{
    class TurbineHealth : LiveMixin
    {
        static LiveMixinData dataAsset;

        public void SetData()
        {
            if(dataAsset == null)
            {
                dataAsset = ScriptableObject.CreateInstance(typeof(LiveMixinData)) as LiveMixinData;
                dataAsset.maxHealth = 200f;
                dataAsset.destroyOnDeath = false;
                dataAsset.minDamageForSound = 50f;
                dataAsset.weldable = true;
                dataAsset.knifeable = true;
            }
            data = dataAsset;
        }

        void Update()
        {
            if(health <= 0f)
            {
                health = 1f;
            }
        }
    }
}
