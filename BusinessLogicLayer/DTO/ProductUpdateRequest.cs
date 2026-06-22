using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer.DTO
{
    public record ProductUpdateRequest
    (
        Guid ProductID,
        string ProductName,
        CategoryOptions Category,
        double? UnitPrice,
        int? QuantityInStock
    );
}
