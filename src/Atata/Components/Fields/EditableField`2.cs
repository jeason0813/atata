﻿namespace Atata
{
    public abstract class EditableField<T, TOwner> : Field<T, TOwner>
        where TOwner : PageObject<TOwner>
    {
        protected EditableField()
        {
        }

        protected abstract void SetValue(T value);

        public TOwner Set(T value)
        {
            RunTriggers(TriggerEvents.BeforeSet);
            Log.StartSettingSection(ComponentName, ConvertValueToString(value));

            SetValue(value);

            Log.EndSection();
            RunTriggers(TriggerEvents.AfterSet);

            return Owner;
        }
    }
}