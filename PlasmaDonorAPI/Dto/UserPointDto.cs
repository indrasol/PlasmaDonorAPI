namespace NewPlasmaDonorsAPI.Dto
{
    public class UserPointDto
    {
        public List<long> Id { get; set; } = new List<long>();
        public long RewardId { get; set; }

        public UserPointDto() { }

        public UserPointDto(List<long> id, long rewardId)
        {
            Id = id;
            RewardId = rewardId;
        }
    }
}
