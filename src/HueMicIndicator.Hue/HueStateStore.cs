namespace HueMicIndicator.Hue
{
    public class HueStateStore
    {
        public IHueState GetState(bool isActive)
        {
            // TODO: Load from settings
            if (isActive)
                return new HueState(false, null, "6");
            
            return new HueState(true, null, "6");
        }
    }
}