namespace Domain.Entities
{
    public enum MaterialType
    {
        Paper = 1,
        Plastic = 2,
        Metal = 3,
        Glass = 4,
        Electronic = 5
    }

    public enum Unit
    {
        Kg = 1,
        Tons = 2,
        Pieces = 3
    }

    public enum PickupStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        PickedUp = 4,
        Published = 5
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Refunded = 4
    }

    public enum NotificationType
    {
        PickupApproved = 1,
        PaymentReceived = 2,
        ItemSold = 3,
        PurchaseConfirmed = 4
    }
}
