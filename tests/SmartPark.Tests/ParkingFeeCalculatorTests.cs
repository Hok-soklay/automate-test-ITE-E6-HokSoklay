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


#region Duration Rounding

[Fact]
public void CalculateFee_PartialHour_ShouldRoundUp_ToNextHour()
{
    // Arrange
    var checkIn = new DateTime(2026, 5, 10, 10, 0, 0);
    var checkOut = checkIn.AddMinutes(31);

    // Act
    var result = _calculator.CalculateFee(
        VehicleType.Car,
        MembershipTier.Guest,
        checkIn,
        checkOut);

    // Assert (FORCE RED)
    // This is intentionally WRONG expectation to force failure
    Assert.Equal(5000m, result.TotalFee);
}

#endregion


    #region Lost Ticket

[Fact]
public void CalculateFee_Car_LostTicket_Adds20000Penalty()
{
    // Arrange
    var checkIn = new DateTime(2026, 5, 10, 10, 0, 0);
    var checkOut = checkIn.AddHours(2);

    // Act
    var result = _calculator.CalculateFee(
        VehicleType.Car,
        MembershipTier.Guest,
        checkIn,
        checkOut,
        true);

    // Assert
    Assert.Equal(22000m, result.TotalFee);
}

#endregion

    #region Edge Cases
    [Fact]
public void CalculateFee_NegativeDuration_ReturnsZeroInsteadOfException()
{
    // Arrange
    var checkIn = new DateTime(2026, 5, 10, 12, 0, 0);
    var checkOut = checkIn.AddHours(-1);

    // Act
    var result = _calculator.CalculateFee(
        VehicleType.Car,
        MembershipTier.Guest,
        checkIn,
        checkOut);

    // Assert
    Assert.Equal(0m, result.TotalFee);
 }
    #endregion

    #region Property-Based Tests
    // Write at least 5 FsCheck properties that must hold for ALL valid inputs
    // You may need custom Arbitrary<T> for generating valid DateTime pairs
    #endregion
}
