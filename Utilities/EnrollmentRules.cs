using G2AspNet.Models;

namespace G2AspNet.Utilities;

public static class EnrollmentRules
{
    public static readonly EnrollmentStatus[] PlaceTakingStatuses =
        { EnrollmentStatus.Pending, EnrollmentStatus.Active };

    public static bool TakesPlace(Enrollment enrollment) =>
        PlaceTakingStatuses.Contains(enrollment.Status);
}
