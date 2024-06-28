using FluentAssertions;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using Xunit;

namespace StarKindred.API.Tests.Utility.Missions.Recruit;

public class ComputeBaseRecruitLevelTests
{
    private const AstrologicalSign AnySignThatDoesntAffectRecruitLevels = AstrologicalSign.Cat;

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(10)]
    public void ComputeRecruitLevel_DoesntFreakOut_WhenVassalLevelsAreAll0(int numberOfLevel0Vassals)
    {
        // Arrange
        var vassals = new List<Vassal>();

        for (int i = 0; i < numberOfLevel0Vassals; i++)
            vassals.Add(new() { Level = 0, Sign = AnySignThatDoesntAffectRecruitLevels });

        // Act
        var rng = new Random();

        var levels = Enumerable.Range(1, 1000)
            .Select(_ => StarKindred.API.Utility.Missions.Recruit.ComputeRecruitLevel(rng, vassals))
            .ToList();

        // Assert
        levels.Should().AllSatisfy(l => l.Should().Be(0));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(10, 0, 10)]
    [InlineData(0, 10, 10)]
    [InlineData(10, null, 10)]
    [InlineData(10, 10, 10)]
    [InlineData(100, null, 100)]
    public void ComputeRecruitLevel_NeverGeneratesLevelsAboveMax(int vassalLevel1, int? vassalLevel2, int maxExpectedLevel)
    {
        // Arrange
        var vassals = new List<Vassal>();

        vassals.Add(new() { Level = vassalLevel1, Sign = AnySignThatDoesntAffectRecruitLevels });

        if(vassalLevel2.HasValue)
            vassals.Add(new() { Level = vassalLevel2.Value, Sign = AnySignThatDoesntAffectRecruitLevels });

        // Act
        var rng = new Random();

        var levels = Enumerable.Range(1, 1000)
            .Select(_ => StarKindred.API.Utility.Missions.Recruit.ComputeRecruitLevel(rng, vassals))
            .ToList();

        // Assert
        levels.Should().AllSatisfy(l => l.Should().BeLessThanOrEqualTo(maxExpectedLevel));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(10, 0, 5)]
    [InlineData(0, 10, 5)]
    [InlineData(10, null, 5)]
    [InlineData(10, 10, 5)]
    [InlineData(100, null, 50)]
    public void ComputeRecruitLevel_NeverGeneratesLevelsBelowHalfMax(int vassalLevel1, int? vassalLevel2, int minExpectedLevel)
    {
        // Arrange
        var vassals = new List<Vassal>();

        vassals.Add(new() { Level = vassalLevel1, Sign = AnySignThatDoesntAffectRecruitLevels });

        if(vassalLevel2.HasValue)
            vassals.Add(new() { Level = vassalLevel2.Value, Sign = AnySignThatDoesntAffectRecruitLevels });

        // Act
        var rng = new Random();

        var levels = Enumerable.Range(1, 1000)
            .Select(_ => StarKindred.API.Utility.Missions.Recruit.ComputeRecruitLevel(rng, vassals))
            .ToList();

        // Assert
        levels.Should().AllSatisfy(l => l.Should().BeGreaterThanOrEqualTo(minExpectedLevel));
    }
}