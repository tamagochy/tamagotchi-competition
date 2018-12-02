using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Competition.Context
{
    public partial class Score
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public int Value { get; set; }
    }
}
