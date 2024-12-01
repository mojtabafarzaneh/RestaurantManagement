using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace RestaurantManagement.Models.ValueGenerator;

public class GuidValueGenerator : ValueGenerator<Guid>
{
    public override Guid Next(EntityEntry entry)
    {
        return Guid.NewGuid();
    }

    public override bool GeneratesTemporaryValues => false;
}