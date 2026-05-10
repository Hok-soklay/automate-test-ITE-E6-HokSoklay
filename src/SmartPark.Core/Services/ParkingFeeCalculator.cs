using SmartPark.Core.Models;

namespace SmartPark.Core.Services;

/// <summary>
/// Core pricing engine. Pure calculation service with no external dependencies.
/// Students: implement this class using TDD (Red-Green-Refactor).
/// </summary>
public class ParkingFeeCalculator
{
    // ── Pricing constants (from spec §4) ────────────────────────

    // Hourly rates (KHR)
    private const decimal MotorcycleRatePerHour = 500m;
    private const decimal CarRatePerHour = 1_000m;
    private const decimal SuvRatePerHour = 1_500m;

    // Daily caps (KHR)
    private const decimal MotorcycleDailyCap = 4_000m;
    private const decimal CarDailyCap = 8_000m;
    private const decimal SuvDailyCap = 12_000m;

    // Time-based rules
    private const int GracePeriodMinutes = 30;
    private const decimal OvernightFlatFee = 2_000m;
    private const int OvernightHourThreshold = 22; // 10 PM

    // Surcharges
    private const decimal WeekendSurchargeRate = 0.20m;
    private const decimal HolidaySurchargeRate = 0.50m;

    // Membership discounts
    private const decimal SilverDiscountRate = 0.10m;
    private const decimal GoldDiscountRate = 0.25m;
    private const decimal PlatinumDiscountRate = 0.40m;

    // Penalties
    private const decimal LostTicketPenalty = 20_000m;

    /// <summary>
    /// Calculates the parking fee following the 9-step flow in the spec.
    /// </summary>
    public ParkingFeeResult CalculateFee(
    VehicleType vehicleType,
    MembershipTier membership,
    DateTime checkIn,
    DateTime checkOut,
    bool isLostTicket = false,
    bool isHoliday = false)
{
    // 1. Validate (refactored)
    if (IsInvalidTimeRange(checkIn, checkOut))
        return new ParkingFeeResult { TotalFee = 0 };

    var totalMinutes = (checkOut - checkIn).TotalMinutes;

    // 2. Grace period
    if (totalMinutes <= 30)
        return new ParkingFeeResult { TotalFee = 1000m };

    // 3. Duration
    var billableMinutes = Math.Max(0, totalMinutes - 30);
    var billableHours = Math.Ceiling(billableMinutes / 60);

    // 4. Base rate
    decimal rate = vehicleType switch
    {
        VehicleType.Motorcycle => MotorcycleRatePerHour,
        VehicleType.Car => CarRatePerHour,
        VehicleType.SUV => SuvRatePerHour,
        _ => 0m
    };

    var baseFee = rate * (decimal)billableHours;

    // Apply daily cap
    decimal dailyCap = GetDailyCap(vehicleType);

    if (baseFee > dailyCap)
        baseFee = dailyCap;

    // Holiday surcharge
    decimal surcharge = CalculateHolidaySurcharge(baseFee, isHoliday);

    // Membership discount
    decimal discount = CalculateMembershipDiscount(
        baseFee + surcharge,
        membership);

    // Lost ticket penalty
    decimal lostTicketPenalty = isLostTicket ? 20000m : 0m;

    // =========================
    // OVERNIGHT (FIXED POSITION)
    // =========================
    decimal overnightFee = CalculateOvernightFee(checkIn, checkOut, vehicleType);

    return new ParkingFeeResult
    {
        TotalFee = baseFee + surcharge - discount + lostTicketPenalty + overnightFee
    };
}
private decimal CalculateHolidaySurcharge(decimal baseFee, bool isHoliday)
{
    if (!isHoliday)
        return 0m;

    return baseFee * 0.5m;
}

private decimal CalculateMembershipDiscount(
    decimal amount,
    MembershipTier membership)
{
    return membership switch
    {
        MembershipTier.Silver => amount * 0.10m,
        MembershipTier.Gold => amount * 0.25m,
        MembershipTier.Platinum => amount * 0.40m,
        _ => 0m
    };
}

private decimal GetDailyCap(VehicleType vehicleType)
{
    return vehicleType switch
    {
        VehicleType.Motorcycle => MotorcycleDailyCap,
        VehicleType.Car => CarDailyCap,
        VehicleType.SUV => SuvDailyCap,
        _ => 0m
    };
}

private bool IsInvalidTimeRange(DateTime checkIn, DateTime checkOut)
{
    return checkOut < checkIn;
}

private decimal CalculateOvernightFee(DateTime checkIn, DateTime checkOut, VehicleType vehicleType)
{
    // If same day → no overnight fee
    if (checkIn.Date == checkOut.Date)
        return 0m;

    return vehicleType switch
    {
        VehicleType.Car => 2000m,
        VehicleType.Motorcycle => 1000m,
        VehicleType.SUV => 3000m,
        _ => 0m
    };
}
}