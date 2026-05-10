using SmartPark.Core.Models;

namespace SmartPark.Core.Services;

public class ParkingFeeCalculator
{
    // ── Rates ─────────────────────────────
    private const decimal MotorcycleRate = 500m;
    private const decimal CarRate = 1000m;
    private const decimal SuvRate = 1500m;

    private const decimal MotorcycleCap = 4000m;
    private const decimal CarCap = 8000m;
    private const decimal SuvCap = 12000m;

    private const int GraceMinutes = 30;

    private const decimal OvernightFee = 2000m;
    private const int OvernightHour = 22;

    private const decimal WeekendRate = 0.20m;
    private const decimal HolidayRate = 0.50m;

    private const decimal Silver = 0.10m;
    private const decimal Gold = 0.25m;
    private const decimal Platinum = 0.40m;

    private const decimal LostTicket = 20000m;

    public ParkingFeeResult CalculateFee(
        VehicleType vehicleType,
        MembershipTier membership,
        DateTime checkIn,
        DateTime checkOut,
        bool isLostTicket = false,
        bool isHoliday = false)
    {
        // 1. Validate
        if (checkOut < checkIn)
            throw new ArgumentException("checkOut cannot be before checkIn");

        var totalMinutes = (checkOut - checkIn).TotalMinutes;

        // 2. Grace period
        if (totalMinutes <= GraceMinutes)
        {
            return new ParkingFeeResult
            {
                TotalFee = isLostTicket ? LostTicket : 0m
            };
        }

        // 3. Duration
        var billableMinutes = totalMinutes - GraceMinutes;
        var hours = Math.Ceiling(billableMinutes / 60.0);
        if (hours < 1) hours = 1;

        // 4. Base fee
        decimal rate = vehicleType switch
        {
            VehicleType.Motorcycle => MotorcycleRate,
            VehicleType.Car => CarRate,
            VehicleType.SUV => SuvRate,
            _ => 0m
        };

        decimal baseFee = (decimal)hours * rate;

        // Apply cap
        baseFee = vehicleType switch
        {
            VehicleType.Motorcycle => Math.Min(baseFee, MotorcycleCap),
            VehicleType.Car => Math.Min(baseFee, CarCap),
            VehicleType.SUV => Math.Min(baseFee, SuvCap),
            _ => baseFee
        };

        // 5. Overnight
        decimal overnightFee = IsOvernight(checkIn, checkOut)
            ? OvernightFee
            : 0m;

        // 6. Surcharge (holiday overrides weekend)
        decimal surcharge = 0m;

        if (isHoliday)
        {
            surcharge = baseFee * HolidayRate;
        }
        else if (IsWeekend(checkIn))
        {
            surcharge = baseFee * WeekendRate;
        }

        // 7. Membership discount
        decimal discountRate = membership switch
        {
            MembershipTier.Silver => Silver,
            MembershipTier.Gold => Gold,
            MembershipTier.Platinum => Platinum,
            _ => 0m
        };

        decimal discount = (baseFee + surcharge) * discountRate;

        // 8. Lost ticket
        decimal penalty = isLostTicket ? LostTicket : 0m;

        // 9. Total
        decimal total = baseFee + surcharge - discount + overnightFee + penalty;

        return new ParkingFeeResult
        {
            TotalFee = Math.Max(0, total)
        };
    }

    // ── Helpers ─────────────────────────────

    private bool IsWeekend(DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday ||
               date.DayOfWeek == DayOfWeek.Sunday;
    }

    private bool IsOvernight(DateTime checkIn, DateTime checkOut)
    {
        return checkIn.Hour < OvernightHour &&
               checkOut.Hour >= OvernightHour;
    }
}