
namespace HatFeather.HealthControl
{
    public class GeneralTargetableInfo
    {
        public int? previousMaxHealth { get; set; } = null;
        public int? previousHealth { get; set; } = null;
        public bool? previouslyDead { get; set; } = null;
    }
}
