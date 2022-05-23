using System.Runtime.CompilerServices;
using UnityEngine.XR;
using IPA.Config.Stores;

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
        public virtual TriggerCondition Mode { get; set; } = TriggerCondition.Instant;
        public virtual float MaxHorizontal { get; set; } = 20f;
        public virtual float MaxVertical { get; set; } = 10f;
        public virtual InputFeatureUsage<bool> SelectedButton { get; set; } = CommonUsages.primaryButton;

        public virtual float MinStabTime { get; set; } = 0.5f;

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        //public virtual void OnReload()
        //{
        //   // Do stuff after config is read from disk.
        //}

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        //public virtual void Changed()
        //{
        //    // Do stuff when the config is changed.
        //}

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(StabToRestartConfig other)
        {
            IsEnabled = other.IsEnabled;
            Mode = other.Mode;
        }
    }
}