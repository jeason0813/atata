﻿using System;

namespace Atata
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Assembly, AllowMultiple = true)]
    public abstract class TriggerAttribute : Attribute
    {
        protected TriggerAttribute(TriggerEvents on, TriggerPriority priority = TriggerPriority.Medium, TriggerScope appliesTo = TriggerScope.Self)
        {
            On = on;
            Priority = priority;
            AppliesTo = appliesTo;
        }

        public TriggerEvents On { get; internal set; }
        public TriggerPriority Priority { get; set; }
        public TriggerScope AppliesTo { get; set; }

        public abstract void Run(TriggerContext context);
    }
}