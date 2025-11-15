using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.Models.IntervalsIcu;
using IntervalsIcuMcp.Services;
using Moq;

namespace IntervalsIcuMcp.Tests.Services;

/// <summary>
/// Unit tests for the IntervalsIcuWorkoutTextService.
/// Tests cover cycling and running workouts with power/HR zones,
/// duration formatting, and various workout scenarios.
/// </summary>
[TestFixture]
public class IntervalsIcuWorkoutTextServiceTests
{
    private Mock<IAthleteProfileRetriever> _athleteProfileRetrieverMock = null!;
    private IntervalsIcuWorkoutTextService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _athleteProfileRetrieverMock = new Mock<IAthleteProfileRetriever>();
        _service = new IntervalsIcuWorkoutTextService(_athleteProfileRetrieverMock.Object);
    }

    #region Cycling Workouts Tests

    [Test]
    public async Task ToIntervalsIcuTextAsync_CyclingWithPowerZones_ReturnsFormattedText()
    {
        // Arrange
        var athleteProfile = CreateCyclingAthleteProfile(ftp: 250);
        _athleteProfileRetrieverMock.Setup(x => x.GetAsync()).ReturnsAsync(athleteProfile);

        var intervals = new List<WorkoutInterval>
        {
            new WorkoutInterval("Warmup", 300, WorkoutZoneType.Z2),   // 5 min
            new WorkoutInterval("Main", 1800, WorkoutZoneType.Z4)    // 30 min
        };
        var workout = new GenerateWorkoutRequest(
            Sport: SportType.Ride,
            Title: "Test Ride",
            Description: "A test cycling workout",
            Intervals: intervals
        );

        // Act
        var result = await _service.ToIntervalsIcuTextAsync(workout);

        // Assert
        var lines = result.Trim().Split(Environment.NewLine);
        Assert.That(lines[0], Is.EqualTo("- 5m @ 55-75%"));
        Assert.That(lines[1], Is.EqualTo("- 30m @ 90-105%"));
    }

    [Test]
    public async Task ToIntervalsIcuTextAsync_TypicalCyclingEnduranceWorkout_ReturnsCorrectFormat()
    {
        // Arrange - 90-minute endurance ride
        var athleteProfile = CreateCyclingAthleteProfile(ftp: 275);
        _athleteProfileRetrieverMock.Setup(x => x.GetAsync()).ReturnsAsync(athleteProfile);

        var intervals = new List<WorkoutInterval>
        {
            new WorkoutInterval("Warmup", 600, WorkoutZoneType.Z1),    // 10 min
            new WorkoutInterval("Steady", 3000, WorkoutZoneType.Z2),   // 50 min
            new WorkoutInterval("Cool down", 300, WorkoutZoneType.Z1)  // 5 min
        };
        var workout = new GenerateWorkoutRequest(
            Sport: SportType.Ride,
            Title: "Endurance Ride",
            Description: "Long, steady-state endurance workout",
            Intervals: intervals
        );

        // Act
        var result = await _service.ToIntervalsIcuTextAsync(workout);

        // Assert
        var lines = result.Trim().Split(Environment.NewLine);
        Assert.That(lines[0], Is.EqualTo("- 10m @ 40-55%"));
        Assert.That(lines[1], Is.EqualTo("- 50m @ 55-75%"));
        Assert.That(lines[2], Is.EqualTo("- 5m @ 40-55%"));
    }

    [Test]
    public async Task ToIntervalsIcuTextAsync_HighIntensityCyclingIntervals_ReturnsHighPowerZones()
    {
        // Arrange - HIIT workout with Z6 efforts
        var athleteProfile = CreateCyclingAthleteProfile(ftp: 250);
        _athleteProfileRetrieverMock.Setup(x => x.GetAsync()).ReturnsAsync(athleteProfile);

        var intervals = new List<WorkoutInterval>
        {
            new WorkoutInterval("Warmup", 1200, WorkoutZoneType.Z2),      // 20 min
            new WorkoutInterval("Interval 1", 300, WorkoutZoneType.Z6),   // 5 min @ Z6
            new WorkoutInterval("Recovery 1", 300, WorkoutZoneType.Z2),   // 5 min @ Z2
            new WorkoutInterval("Cool down", 600, WorkoutZoneType.Z1)     // 10 min
        };
        var workout = new GenerateWorkoutRequest(
            Sport: SportType.Ride,
            Title: "HIIT",
            Description: "High-intensity interval training",
            Intervals: intervals
        );

        // Act
        var result = await _service.ToIntervalsIcuTextAsync(workout);

        // Assert
        var lines = result.Trim().Split(Environment.NewLine);
        Assert.That(lines[0], Is.EqualTo("- 20m @ 55-75%"));
        Assert.That(lines[1], Is.EqualTo("- 5m @ 120-150%"));
        Assert.That(lines[2], Is.EqualTo("- 5m @ 55-75%"));
        Assert.That(lines[3], Is.EqualTo("- 10m @ 40-55%"));
    }

    #endregion

    #region Running Workouts Tests

    [Test]
    public async Task ToIntervalsIcuTextAsync_RunningWithHeartRateZones_ReturnsFormattedText()
    {
        // Arrange
        var athleteProfile = CreateRunningAthleteProfile(lthr: 170);
        _athleteProfileRetrieverMock.Setup(x => x.GetAsync()).ReturnsAsync(athleteProfile);

        var intervals = new List<WorkoutInterval>
        {
            new WorkoutInterval("Easy", 600, WorkoutZoneType.Z2),    // 10 min
            new WorkoutInterval("Tempo", 900, WorkoutZoneType.Z4)    // 15 min
        };
        var workout = new GenerateWorkoutRequest(
            Sport: SportType.Run,
            Title: "Test Run",
            Description: "A test running workout",
            Intervals: intervals
        );

        // Act
        var result = await _service.ToIntervalsIcuTextAsync(workout);

        // Assert
        var lines = result.Trim().Split(Environment.NewLine);
        Assert.That(lines[0], Is.EqualTo("- 10m @ 150-166bpm"));
        Assert.That(lines[1], Is.EqualTo("- 15m @ 182-198bpm"));
    }

    [Test]
    public async Task ToIntervalsIcuTextAsync_TempoRun_ReturnsCorrectHeartRateRanges()
    {
        // Arrange - Running workout with sustained effort
        var athleteProfile = CreateRunningAthleteProfile(lthr: 175);
        _athleteProfileRetrieverMock.Setup(x => x.GetAsync()).ReturnsAsync(athleteProfile);

        var intervals = new List<WorkoutInterval>
        {
            new WorkoutInterval("Warmup", 600, WorkoutZoneType.Z2),      // 10 min easy
            new WorkoutInterval("Tempo", 1200, WorkoutZoneType.Z4),      // 20 min @ tempo
            new WorkoutInterval("Cool down", 600, WorkoutZoneType.Z2)    // 10 min easy
        };
        var workout = new GenerateWorkoutRequest(
            Sport: SportType.Run,
            Title: "Tempo Run",
            Description: "Sustained tempo effort",
            Intervals: intervals
        );

        // Act
        var result = await _service.ToIntervalsIcuTextAsync(workout);

        // Assert
        var lines = result.Trim().Split(Environment.NewLine);
        Assert.That(lines[0], Is.EqualTo("- 10m @ 150-166bpm"));
        Assert.That(lines[1], Is.EqualTo("- 20m @ 182-198bpm"));
        Assert.That(lines[2], Is.EqualTo("- 10m @ 150-166bpm"));
    }

    #endregion

    #region Duration Formatting Tests

    [Test]
    [TestCase(60, "1m")]
    [TestCase(300, "5m")]
    [TestCase(600, "10m")]
    [TestCase(1800, "30m")]
    [TestCase(3600, "1h")]
    [TestCase(3660, "1h 1m")]
    [TestCase(5400, "1h 30m")]
    [TestCase(45, "45s")]
    [TestCase(75, "1m 15s")]
    public async Task ToIntervalsIcuTextAsync_VariousDurations_FormattedCorrectly(int seconds, string expectedDuration)
    {
        // Arrange
        var athleteProfile = CreateCyclingAthleteProfile(ftp: 250);
        _athleteProfileRetrieverMock.Setup(x => x.GetAsync()).ReturnsAsync(athleteProfile);

        var intervals = new List<WorkoutInterval>
        {
            new WorkoutInterval("Test", seconds, WorkoutZoneType.Z3)
        };
        var workout = new GenerateWorkoutRequest(
            Sport: SportType.Ride,
            Title: "Test",
            Description: "Test",
            Intervals: intervals
        );

        // Act
        var result = await _service.ToIntervalsIcuTextAsync(workout);

        // Assert
        Assert.That(result, Contains.Substring(expectedDuration));
    }

    #endregion

    #region Error Handling Tests

    [Test]
    public void ToIntervalsIcuTextAsync_NoAthleteProfile_ThrowsInvalidOperationException()
    {
        // Arrange
        _athleteProfileRetrieverMock.Setup(x => x.GetAsync()).ReturnsAsync((AthleteProfile?)null);

        var intervals = new List<WorkoutInterval>
        {
            new WorkoutInterval("Test", 600, WorkoutZoneType.Z3)
        };
        var workout = new GenerateWorkoutRequest(
            Sport: SportType.Ride,
            Title: "Test",
            Description: "Test",
            Intervals: intervals
        );

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.ToIntervalsIcuTextAsync(workout),
            "Should throw when athlete profile cannot be retrieved"
        );
    }

    [Test]
    public async Task ToIntervalsIcuTextAsync_EmptyIntervals_ReturnsEmptyString()
    {
        // Arrange
        var athleteProfile = CreateCyclingAthleteProfile(ftp: 250);
        _athleteProfileRetrieverMock.Setup(x => x.GetAsync()).ReturnsAsync(athleteProfile);

        var workout = new GenerateWorkoutRequest(
            Sport: SportType.Ride,
            Title: "Empty",
            Description: "Empty workout",
            Intervals: new List<WorkoutInterval>()
        );

        // Act
        var result = await _service.ToIntervalsIcuTextAsync(workout);

        // Assert
        Assert.That(result, Is.Empty);
    }

    #endregion

    #region Helper Methods

    private static AthleteProfile CreateCyclingAthleteProfile(int ftp = 250)
    {
        var powerZones = new[] { 55, 75, 90, 105, 120, 150 }; // Power zones are in % of FTP from the Intervals API
        var hrZones = new[] { 146, 162, 178, 194, 210, 226, 242 };    // 170 LTHR

        var sportSetting = new SportSetting(
            Id: 1,
            AthleteId: "athlete-1",
            Types: [SportType.Ride],
            Ftp: ftp,
            IndoorFtp: null,
            WPrime: null,
            PMax: null,
            PowerZones: powerZones,
            PowerZoneNames: null,
            Lthr: 170,
            MaxHr: 200,
            HrZones: hrZones,
            HrZoneNames: null,
            ThresholdPace: null,
            PaceUnits: null
        );

        return new AthleteProfile(
            Id: "athlete-1",
            Sex: "M",
            City: "Test City",
            State: "Test State",
            Country: "Test Country",
            Timezone: "UTC",
            MeasurementPreference: "Metric",
            IcuDateOfBirth: new DateTime(1990, 1, 1),
            IcuRestingHr: 60,
            IcuWeight: 75,
            SportSettings: new[] { sportSetting }
        );
    }

    private static AthleteProfile CreateRunningAthleteProfile(int lthr = 170)
    {
        var hrZones = new[] { 150, 166, 182, 198, 214, 230, 246 }; // 175 LTHR

        var sportSetting = new SportSetting(
            Id: 2,
            AthleteId: "athlete-1",
            Types: new[] { SportType.Run },
            Ftp: null,
            IndoorFtp: null,
            WPrime: null,
            PMax: null,
            PowerZones: null,
            PowerZoneNames: null,
            Lthr: lthr,
            MaxHr: 200,
            HrZones: hrZones,
            HrZoneNames: null,
            ThresholdPace: null,
            PaceUnits: null
        );

        return new AthleteProfile(
            Id: "athlete-1",
            Sex: "M",
            City: "Test City",
            State: "Test State",
            Country: "Test Country",
            Timezone: "UTC",
            MeasurementPreference: "Metric",
            IcuDateOfBirth: new DateTime(1990, 1, 1),
            IcuRestingHr: 60,
            IcuWeight: 75,
            SportSettings: new[] { sportSetting }
        );
    }

    #endregion
}
