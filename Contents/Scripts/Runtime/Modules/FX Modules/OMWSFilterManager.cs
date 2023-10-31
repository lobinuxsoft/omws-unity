namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [System.Serializable]
    public class OMWSFilterManager : OMWSFXModule
    {
        public override void OnFXEnable() { }

        public override void OnFXUpdate()
        {
            if (!isEnabled)
                return;
        }

        public override void OnFXDisable() { }

        public override void SetupFXParent() { }
    }
}