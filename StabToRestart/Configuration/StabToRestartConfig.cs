using System.Runtime.CompilerServices;
using UnityEngine.XR;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace StabToRestart.Configuration
{
    internal class StabToRestartConfig
    {
        public enum TriggerCondition
        {
            Instant = 0,
            Button = 1,
            Hold = 2,
            Both = 3,
        }

        public static StabToRestartConfig Instance { get; set; }
        public virtual bool IsEnabled { get; set; } = true;
        [UseConverter(typeof(EnumConverter<TriggerCondition>))]
        public virtual TriggerCondition Mode { get; set; } = TriggerCondition.Instant;
        public virtual float MaxHorizontal { get; set; } = 20f;
        public virtual float MaxVertical { get; set; } = 10f;
        public virtual string SelectedButton { get; set; } = "PrimaryButton";

        public virtual float MinStabTime { get; set; } = 0.5f;
        public virtual float MinSpeed { get; set; } = 2f;

        public virtual void CopyFrom(StabToRestartConfig other)
        {
            IsEnabled = other.IsEnabled;
            Mode = other.Mode;
            MaxHorizontal = other.MaxHorizontal;
            MaxVertical = other.MaxVertical;
            SelectedButton = other.SelectedButton;
            MinStabTime = other.MinStabTime;
        }
    }
}