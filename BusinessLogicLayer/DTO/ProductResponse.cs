using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer.DTO
{
    public record ProductResponse
    (
        Guid ProductID,
        string ProductName,
        CategoryOptions Category,
        double? UnitPrice,
        int? QuantityInStock
    );
}
