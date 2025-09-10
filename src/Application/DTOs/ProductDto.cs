namespace Application.DTOs;

public record ProductDto(
    int Id,
    string ProductName,
    string CreatedBy,
    DateTime CreatedOn,
    string? ModifiedBy,
    DateTime? ModifiedOn
);

public record CreateProductDto(
    string ProductName,
    string CreatedBy
);

public record UpdateProductDto(
    string? ProductName,
    string? ModifiedBy
);

public record ProductListDto(
    List<ProductDto> Products,
    int TotalCount,
    int PageNumber,
    int PageSize
);

public record ItemDto(
    int Id,
    int ProductId,
    int Quantity,
    ProductDto Product
);

public record CreateItemDto(
    int ProductId,
    int Quantity
);

public record UpdateItemDto(
    int? Quantity
);
