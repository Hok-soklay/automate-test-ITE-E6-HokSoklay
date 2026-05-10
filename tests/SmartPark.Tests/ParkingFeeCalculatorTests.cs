using SmartPark.Core.Models;
using SmartPark.Core.Services;
using FsCheck;
using FsCheck.Xunit;

namespace SmartPark.Tests;

public class ParkingFeeCalculatorTests
{
    private readonly ParkingFeeCalculator _calculator = new();

    // ────────────────────────────────────────────────────────────
    //  EXAMPLE TEST — shows the naming convention and AAA pattern.
    //  Delete or keep this; it does not count toward your grade.
    // ────────────────────────────────────────────────────────────

    [Fact]
public void CalculateFee_Car_1Hour_Returns1000()
{
    // Arrange
    var checkIn = new DateTime(2026, 5, 10, 10, 0, 0);
    var checkOut = checkIn.AddHours(1);

    // Act
    var result = _calculator.CalculateFee(
        VehicleType.Car,
        MembershipTier.Guest,
        checkIn,
        checkOut);

    // Assert
    Assert.Equal(1000m, result.TotalFee);
}

    #region Basic Fee Calculation
    // Test basic hourly rates for each vehicle type
    // Consider using [Theory] with [InlineData] for multiple scenarios
    #endregion

    #region Grace Period
    // Test the free parking window and its boundaries
    #endregion

    #region Duration Rounding
    // Test how partial hours are rounded for billing
    #endregion

    #region Daily Cap
    [Fact]
public void CalculateFee_Car_LongDuration_ShouldNotExceedDailyCap()
{
    // Arrange
    var checkIn = new DateTime(2026, 5, 10, 8, 0, 0);
    var checkOut = checkIn.AddHours(24);

    // Act
    var result = _calculator.CalculateFee(
        VehicleType.Car,
        MembershipTier.Guest,
        checkIn,
        checkOut);

    // Assert
    Assert.True(result.TotalFee <= 15000m);
}
    #endregion

    #region Overnight Fee
     [Fact]
public void CalculateFee_Overnight_ShouldBeHigherThanSameDurationSameDay()
{
    // Arrange
    var checkIn = new DateTime(2026, 5, 10, 21, 0, 0);

    var sameDayOut = checkIn.AddHours(4); // same day
    var overnightOut = new DateTime(2026, 5, 11, 1, 0, 0); // next day

    // Act
    var normal = _calculator.CalculateFee(
        VehicleType.Car,
        MembershipTier.Guest,
        checkIn,
        sameDayOut);

    var overnight = _calculator.CalculateFee(
        VehicleType.Car,
        MembershipTier.Guest,
        checkIn,
        overnightOut);

    // Assert (THIS MUST FAIL FIRST)
    Assert.True(overnight.TotalFee > normal.TotalFee);
}
    #endregion

    #region Weekend Surcharge
    // Test the percentage-based surcharge on specific days
    #endregion

    #region Holiday Surcharge
    // Test holiday pricing and its interaction with weekend pricing
    #endregion

    #region Membership Discounts
    // Test discount tiers and what amounts they apply to
    #endregion

    #region Lost Ticket
    // Test the penalty and how it interacts with other fee modifiers
    #endregion

    #region Edge Cases
    // Test invalid inputs and boundary conditions
    #endregion

    #region Property-Based Tests
    // Write at least 5 FsCheck properties that must hold for ALL valid inputs
    // You may need custom Arbitrary<T> for generating valid DateTime pairs
    #endregion
}
