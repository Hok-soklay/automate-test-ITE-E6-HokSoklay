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

    #region Basic Fee Calculation
    // Test basic hourly rates for each vehicle type
    // Consider using [Theory] with [InlineData] for multiple scenarios
    #endregion

    #region Grace Period
    [Fact]
public void CalculateFee_Motorcycle_1Hour_Returns500()
{
    // Arrange
    var checkIn = new DateTime(2026, 5, 10, 10, 0, 0);
    var checkOut = checkIn.AddHours(1);

    // Act
    var result = _calculator.CalculateFee(
        VehicleType.Motorcycle,
        MembershipTier.Guest,
        checkIn,
        checkOut);

    // Assert
    Assert.Equal(500m, result.TotalFee);
}
    #endregion

    #region Duration Rounding
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
    #endregion

    #region Daily Cap
    // Test that fees respect maximum daily limits per vehicle type
    #endregion

    #region Overnight Fee
    // Test the flat fee applied for sessions that extend into late hours
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
   [Fact]
public void CalculateFee_LostTicket_AddsPenalty()
{
    // Arrange
    var checkIn = new DateTime(2026, 5, 10, 10, 0, 0);
    var checkOut = checkIn.AddHours(1);

    // Act
    var result = _calculator.CalculateFee(
        VehicleType.Car,
        MembershipTier.Guest,
        checkIn,
        checkOut,
        isLostTicket: true);

    // Assert
    Assert.True(result.TotalFee >= 20000m);
}
    #endregion

    #region Property-Based Tests
    // Write at least 5 FsCheck properties that must hold for ALL valid inputs
    // You may need custom Arbitrary<T> for generating valid DateTime pairs
    #endregion
}
