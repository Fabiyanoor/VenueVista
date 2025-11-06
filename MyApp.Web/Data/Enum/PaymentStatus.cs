namespace MyApp.Web.Data.Enum;


public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded,
    Canceled
}

public enum PackageTier
{
    Basic = 1,
    Intermediate = 2,
    Advance = 3,
    Custom = 4 // Added for custom packages
}

public enum BookingType
{
    PredefinedPackage,
    CustomPackage
}