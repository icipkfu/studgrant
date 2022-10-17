namespace Grant.WebApi.Request.Achievement
{
    using Core.Enum;
    using System.Collections.Generic;

    public class AchievementData
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public AchievementLevel? Level { get; set; }
        public AchievementState? State { get; set; }
        public AchievementSubject Subject { get; set; }
        public AchievementCriterion? Criterion { get; set; }
        public int Year { get; set; }
        public string Files { get; set; }
        public string ProofFile { get; set; }
    }
}