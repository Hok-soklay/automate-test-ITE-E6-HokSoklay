using Moq;
using SmartPark.Core.Interfaces;
using SmartPark.Core.Models;
using SmartPark.Core.Services;

namespace SmartPark.Tests;

public class ParkingSessionManagerTests
{
    private readonly Mock<IPaymentGateway> _paymentStub = new();
    private readonly Mock<INotificationService> _notificationStub = new();
    private readonly Mock<IMembershipService> _membershipStub = new();
    private readonly Mock<IParkingRepository> _repoStub = new();
    private readonly Mock<IDateTimeProvider> _dateTimeStub = new();
    private readonly ParkingFeeCalculator _feeCalculator = new();
    private readonly ParkingSessionManager _manager;

    // ✅ FIX: in-memory storage for repository behavior
    private readonly Dictionary<string, ParkingTicket> _store = new();

    public ParkingSessionManagerTests()
    {
        // 🔧 Make repo behave like real storage
        _repoStub.Setup(r => r.SaveTicketAsync(It.IsAny<ParkingTicket>()))
            .Returns((ParkingTicket t) =>
            {
                _store[t.Vehicle.LicensePlate] = t;
                return Task.CompletedTask;
            });

        _repoStub.Setup(r => r.GetActiveTicketByPlateAsync(It.IsAny<string>()))
            .ReturnsAsync((string plate) =>
            {
                return _store.TryGetValue(plate, out var ticket) ? ticket : null;
            });

        _repoStub.Setup(r => r.UpdateTicketAsync(It.IsAny<ParkingTicket>()))
            .Returns((ParkingTicket t) =>
            {
                _store[t.Vehicle.LicensePlate] = t;
                return Task.CompletedTask;
            });

        _manager = new ParkingSessionManager(
            _feeCalculator,
            _paymentStub.Object,
            _notificationStub.Object,
            _membershipStub.Object,
            _repoStub.Object,
            _dateTimeStub.Object);
    }

    [Fact]
    public async Task CheckInAsync_NewVehicle_LookUpMembership()
    {
        // Arrange
        _membershipStub.Setup(m => m.GetMembershipTier("PP-9999")).Returns(MembershipTier.Guest);
        _repoStub.Setup(r => r.GetActiveTicketByPlateAsync("PP-9999")).ReturnsAsync((ParkingTicket?)null);
        _dateTimeStub.Setup(d => d.Now).Returns(new DateTime(2026, 3, 16, 10, 0, 0));

        // Act
        var ticket = await _manager.CheckInAsync("PP-9999", VehicleType.Car);

        // Assert
        _membershipStub.Verify(m => m.GetMembershipTier("PP-9999"), Times.Once);
        Assert.Equal("PP-9999", ticket.Vehicle.LicensePlate);
    }

    #region CheckIn — Happy Path
    // Test successful vehicle check-in and verify correct interactions
    #endregion

    #region CheckIn — Validation
    // Test check-in error scenarios and verify side effects
    #endregion

    #region CheckOut — Happy Path
    // Test successful check-out with payment and notification
    #endregion

    #region CheckOut — Payment Failure
    // Test behavior when the payment step fails
    #endregion

    #region CheckOut — Notification Failure
    // Test what happens when sending the receipt fails
    #endregion

    #region CheckOut — Validation
    // Test check-out error scenarios for missing or invalid tickets
    #endregion

    #region Verify Interaction Order
    // Verify that dependencies are called in the correct sequence
    #endregion
}