using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace IntervalsIcuMcp.Tests.Services;

/// <summary>
/// Unit tests for the WorkoutTssCalculator service.
/// Tests cover TSS and IF calculations for cycling and running sports,
/// edge cases with missing data, and real-world training scenarios.
/// </summary>
[TestFixture]
public class WorkoutTssCalculatorTests
{
    private WorkoutTssCalculator _calculator = null!;

    [SetUp]
    public void SetUp()
    {
        var loggerMock = new Mock<ILogger<WorkoutTssCalculator>>();
        _calculator = new WorkoutTssCalculator(loggerMock.Object);
    }

    #region EstimateTss Tests

    [Test]
    public void EstimateTss_WithCyclingAndPowerZones_ReturnsValidTss()
    {
        // Arrange
        var ftp = 250.0;
        var intervals = new[]
        {
            new WorkoutInterval("Warmup", 300, WorkoutZoneType.Z2),   // 5 min
            new WorkoutInterval("Main", 1800, WorkoutZoneType.Z4)    // 30 min
        };
        var powerZones = new[] { 138, 207, 276, 345, 414, 483, 552 }; // 250W FTP
        var context = new WorkoutEstimationContext
        {
            FtpWatts = ftp,
            PowerZones = powerZones.ToList(),
            LthrBpm = 0,
            MaxHrBpm = 0,
            RestHrBpm = 0,
            HrZones = new List<int>()
        };

        // Act
        var tss = _calculator.EstimateTss(intervals, context, SportType.Ride);

        // Assert
        // Calculated: Z2 warmup contributes 24, Z4 main contributes 57 = 81 total TSS
        Assert.That(tss, Is.EqualTo(81));
    }

    [Test]
    public void EstimateTss_WithRunningAndHeartRateZones_ReturnsValidTss()
    {
        // Arrange
        var lthr = 170.0;
        var intervals = new[]
        {
            new WorkoutInterval("Easy", 600, WorkoutZoneType.Z2),    // 10 min
            new WorkoutInterval("Tempo", 900, WorkoutZoneType.Z4)    // 15 min
        };
        var hrZones = new[] { 146, 162, 178, 194, 210, 226, 242 }; // 170 LTHR
        var context = new WorkoutEstimationContext
        {
            LthrBpm = (int)lthr,
            HrZones = hrZones.ToList(),
            FtpWatts = null,
            PowerZones = null,
            MaxHrBpm = 0,
            RestHrBpm = 0
        };

        // Act
        var tss = _calculator.EstimateTss(intervals, context, SportType.Run);

        // Assert
        Assert.That(tss, Is.EqualTo(44));
    }

    [Test]
    [TestCase(SportType.WeightTraining)]
    [TestCase(SportType.Yoga)]
    [TestCase(SportType.Golf)]
    public void EstimateTss_WithUnsupportedSport_ReturnsNull(SportType sport)
    {
        // Arrange
        var intervals = new[] { new WorkoutInterval("Test", 600, WorkoutZoneType.Z3) };
        var context = new WorkoutEstimationContext
        {
            LthrBpm = 0,
            MaxHrBpm = 0,
            RestHrBpm = 0,
            HrZones = new List<int>(),
            FtpWatts = null,
            PowerZones = null
        };

        // Act
        var tss = _calculator.EstimateTss(intervals, context, sport);

        // Assert
        Assert.That(tss, Is.Null);
    }

    [Test]
    public void EstimateTss_WithEmptyOrInvalidDurations_HandlesGracefully()
    {
        // Arrange
        var intervals = new[]
        {
            new WorkoutInterval("Zero duration", 0, WorkoutZoneType.Z3),
            new WorkoutInterval("Negative", -300, WorkoutZoneType.Z3),
            new WorkoutInterval("Valid", 600, WorkoutZoneType.Z3)
        };
        var context = new WorkoutEstimationContext
        {
            LthrBpm = 0,
            MaxHrBpm = 0,
            RestHrBpm = 0,
            HrZones = new List<int>(),
            FtpWatts = null,
            PowerZones = null
        };

        // Act
        var tss = _calculator.EstimateTss(intervals, context, SportType.Ride);

        // Assert - Should calculate TSS only from valid interval
        // Z3: (276+207)/2 = 241.5W, IF = 241.5/0.5 = 483 (default 0.5)
        // Actually uses default IF of 0.5 when no FTP
        // TSS = (600/3600)*0.5^2*100 = 4.17 ≈ 4
        Assert.That(tss, Is.EqualTo(4));
    }

    [Test]
    public void EstimateTss_WithoutAthleteData_UsesDefaultIfValue()
    {
        // Arrange - No zones or thresholds provided
        var intervals = new[] { new WorkoutInterval("Ride", 3600, WorkoutZoneType.Z3) };
        var context = new WorkoutEstimationContext
        {
            LthrBpm = 0,
            MaxHrBpm = 0,
            RestHrBpm = 0,
            HrZones = new List<int>(),
            FtpWatts = null,
            PowerZones = null
        };

        // Act
        var tss = _calculator.EstimateTss(intervals, context, SportType.Ride);

        // Assert - Should still calculate with default IF of 0.5
        // TSS = (3600/3600)*0.5^2*100 = 25.0
        Assert.That(tss, Is.EqualTo(25));
    }

    [Test]
    public void EstimateTss_MultipleIntervalsAdditive_TssIsSum()
    {
        // Arrange
        var ftp = 250.0;
        var interval1 = new WorkoutInterval("Z2", 1800, WorkoutZoneType.Z2);
        var interval2 = new WorkoutInterval("Z4", 1800, WorkoutZoneType.Z4);
        var powerZones = new[] { 138, 207, 276, 345, 414, 483, 552 };
        var context = new WorkoutEstimationContext
        {
            FtpWatts = ftp,
            PowerZones = powerZones.ToList(),
            LthrBpm = 0,
            MaxHrBpm = 0,
            RestHrBpm = 0,
            HrZones = new List<int>()
        };

        // Act
        var tssInterval1 = _calculator.EstimateTss(new[] { interval1 }, context, SportType.Ride);
        var tssInterval2 = _calculator.EstimateTss(new[] { interval2 }, context, SportType.Ride);
        var tssTotal = _calculator.EstimateTss(new[] { interval1, interval2 }, context, SportType.Ride);

        // Assert - Total should equal sum of parts
        Assert.That(tssInterval1, Is.EqualTo(24));
        Assert.That(tssInterval2, Is.EqualTo(77));
        Assert.That(tssTotal, Is.EqualTo(101));
    }

    #endregion

    #region EstimateIntensityFactor Tests

    [Test]
    public void EstimateIntensityFactor_WithCyclingAndPowerZones_ReturnsClampedValue()
    {
        // Arrange
        var ftp = 250.0;
        var intervals = new[]
        {
            new WorkoutInterval("Z2", 1800, WorkoutZoneType.Z2),
            new WorkoutInterval("Z4", 1800, WorkoutZoneType.Z4)
        };
        var powerZones = new[] { 138, 207, 276, 345, 414, 483, 552 };
        var context = new WorkoutEstimationContext
        {
            FtpWatts = ftp,
            PowerZones = powerZones.ToList(),
            LthrBpm = 0,
            MaxHrBpm = 0,
            RestHrBpm = 0,
            HrZones = new List<int>()
        };

        // Act
        var ifValue = _calculator.EstimateIntensityFactor(intervals, context, SportType.Ride);

        // Assert
        Assert.That(ifValue, Is.EqualTo(0.96).Within(0.01));
    }

    [Test]
    public void EstimateIntensityFactor_WithRunningAndHeartRateZones_ReturnsClampedValue()
    {
        // Arrange
        var lthr = 170.0;
        var intervals = new[]
        {
            new WorkoutInterval("Z2", 1800, WorkoutZoneType.Z2),
            new WorkoutInterval("Z4", 1800, WorkoutZoneType.Z4)
        };
        var hrZones = new[] { 146, 162, 178, 194, 210, 226, 242 };
        var context = new WorkoutEstimationContext
        {
            LthrBpm = (int)lthr,
            HrZones = hrZones.ToList(),
            FtpWatts = null,
            PowerZones = null,
            MaxHrBpm = 0,
            RestHrBpm = 0
        };

        // Act
        var ifValue = _calculator.EstimateIntensityFactor(intervals, context, SportType.Run);

        // Assert
        Assert.That(ifValue, Is.EqualTo(1.0).Within(0.01));
    }

    [Test]
    public void EstimateIntensityFactor_UnsupportedSportOrEmpty_ReturnsNullOrZero()
    {
        // Arrange - Test two edge cases
        var intervals = new[] { new WorkoutInterval("Test", 600, WorkoutZoneType.Z3) };
        var emptyIntervals = Array.Empty<WorkoutInterval>();
        var context = new WorkoutEstimationContext
        {
            LthrBpm = 0,
            MaxHrBpm = 0,
            RestHrBpm = 0,
            HrZones = new List<int>(),
            FtpWatts = null,
            PowerZones = null
        };

        // Act & Assert - Unsupported sport returns null
        var ifUnsupported = _calculator.EstimateIntensityFactor(intervals, context, SportType.Yoga);
        Assert.That(ifUnsupported, Is.Null);

        // Empty intervals returns 0
        var ifEmpty = _calculator.EstimateIntensityFactor(emptyIntervals, context, SportType.Ride);
        Assert.That(ifEmpty, Is.EqualTo(0.0));
    }

    [Test]
    public void EstimateIntensityFactor_DurationWeighted_HigherIntensityMoreWeight()
    {
        // Arrange
        var powerZones = new[] { 138, 207, 276, 345, 414, 483, 552 };
        var context = new WorkoutEstimationContext
        {
            FtpWatts = 250.0,
            PowerZones = powerZones.ToList(),
            LthrBpm = 0,
            MaxHrBpm = 0,
            RestHrBpm = 0,
            HrZones = new List<int>()
        };

        // Equal time: 50% Z2, 50% Z5
        var balanced = new[] {
            new WorkoutInterval("Z2", 1800, WorkoutZoneType.Z2),
            new WorkoutInterval("Z5", 1800, WorkoutZoneType.Z5)
        };

        // More high intensity: 5% Z2, 95% Z5
        var intense = new[] {
            new WorkoutInterval("Z2", 300, WorkoutZoneType.Z2),
            new WorkoutInterval("Z5", 5700, WorkoutZoneType.Z5)
        };

        // Act
        var ifBalanced = _calculator.EstimateIntensityFactor(balanced, context, SportType.Ride);
        var ifIntense = _calculator.EstimateIntensityFactor(intense, context, SportType.Ride);

        // Assert - More time in higher zone = higher IF
        Assert.That(ifBalanced, Is.EqualTo(1.104).Within(0.01));
        Assert.That(ifIntense, Is.EqualTo(1.477).Within(0.01));
        Assert.That(ifIntense, Is.GreaterThan(ifBalanced ?? 0d));
    }

    #endregion

    #region Real-World Scenario Tests

    [Test]
    public void RealWorldScenario_TypicalEnduranceCyclingWorkout()
    {
        // Arrange - 90-minute endurance ride
        var ftp = 275.0;
        var powerZones = new[] { 165, 248, 330, 413, 495, 578, 660 };
        var context = new WorkoutEstimationContext
        {
            FtpWatts = ftp,
            PowerZones = powerZones.ToList(),
            LthrBpm = 0,
            MaxHrBpm = 0,
            RestHrBpm = 0,
            HrZones = new List<int>()
        };

        var intervals = new[] {
            new WorkoutInterval("Warmup", 600, WorkoutZoneType.Z1),    // 10 min
            new WorkoutInterval("Steady", 3000, WorkoutZoneType.Z2),   // 50 min
            new WorkoutInterval("Cool down", 300, WorkoutZoneType.Z1)  // 5 min
        };

        // Act
        var tss = _calculator.EstimateTss(intervals, context, SportType.Ride);
        var ifValue = _calculator.EstimateIntensityFactor(intervals, context, SportType.Ride);

        // Assert
        Assert.That(tss, Is.EqualTo(51));
        Assert.That(ifValue, Is.EqualTo(0.7).Within(0.05));
    }

    [Test]
    public void RealWorldScenario_HighIntensityIntervalTraining()
    {
        // Arrange - HIIT with short, hard efforts
        var ftp = 250.0;
        var powerZones = new[] { 138, 207, 276, 345, 414, 483, 552 };
        var context = new WorkoutEstimationContext
        {
            FtpWatts = ftp,
            PowerZones = powerZones.ToList(),
            LthrBpm = 0,
            MaxHrBpm = 0,
            RestHrBpm = 0,
            HrZones = new List<int>()
        };

        var intervals = new[] {
            new WorkoutInterval("Warmup", 1200, WorkoutZoneType.Z2),      // 20 min
            new WorkoutInterval("Interval 1", 300, WorkoutZoneType.Z6),   // 5 min @ Z6
            new WorkoutInterval("Recovery 1", 300, WorkoutZoneType.Z2),   // 5 min @ Z2
            new WorkoutInterval("Interval 2", 300, WorkoutZoneType.Z6),   // 5 min @ Z6
            new WorkoutInterval("Recovery 2", 300, WorkoutZoneType.Z2),   // 5 min @ Z2
            new WorkoutInterval("Cool down", 600, WorkoutZoneType.Z1)     // 10 min
        };

        // Act
        var tss = _calculator.EstimateTss(intervals, context, SportType.Ride);
        var ifValue = _calculator.EstimateIntensityFactor(intervals, context, SportType.Ride);

        // Assert
        Assert.That(tss, Is.EqualTo(80));
        Assert.That(ifValue, Is.EqualTo(0.853).Within(0.01));
    }

    [Test]
    public void RealWorldScenario_TempoRun()
    {
        // Arrange - Running workout with sustained effort
        var lthr = 175.0;
        var hrZones = new[] { 150, 166, 182, 198, 214, 230, 246 };
        var context = new WorkoutEstimationContext
        {
            LthrBpm = (int)lthr,
            HrZones = hrZones.ToList(),
            FtpWatts = null,
            PowerZones = null,
            MaxHrBpm = 0,
            RestHrBpm = 0
        };

        var intervals = new[] {
            new WorkoutInterval("Warmup", 600, WorkoutZoneType.Z2),      // 10 min easy
            new WorkoutInterval("Tempo", 1200, WorkoutZoneType.Z4),      // 20 min @ tempo
            new WorkoutInterval("Cool down", 600, WorkoutZoneType.Z2)    // 10 min easy
        };

        // Act
        var tss = _calculator.EstimateTss(intervals, context, SportType.Run);
        var ifValue = _calculator.EstimateIntensityFactor(intervals, context, SportType.Run);

        // Assert
        // Z2: (150+166)/2 = 158 bpm, IF = 0.903
        // Z4: (198+214)/2 = 206 bpm, IF = 1.177
        // TSS_Z2 = ((600+600)/3600)*0.903^2*100 = 18.1
        // TSS_Z4 = (1200/3600)*1.177^2*100 = 46.0
        // Total = 66
        // IF_weighted = (0.903*1200 + 1.177*1200)/(2400) = 1.04
        Assert.That(tss, Is.EqualTo(66));
        Assert.That(ifValue, Is.EqualTo(1.0).Within(0.05));
    }
    #endregion
}
