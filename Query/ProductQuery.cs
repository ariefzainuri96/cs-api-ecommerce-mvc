using Ecommerce.Data;
using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;

namespace Ecommerce.Query;

public static class ProductQuery
{
    public static IQueryable<Product> GetQuery(EcommerceDbContext context, PaginationRequestDto request)
    {        
        IQueryable<Product> query = context.Products;

        if (!string.IsNullOrWhiteSpace(request.SearchAll))
        {
            query = query.Where(c =>
            c.Name.Contains(request.SearchAll) ||
            c.Description.Contains(request.SearchAll) ||
            c.Price.ToString() == request.SearchAll
            );
        }
        else
        {
            query = query.ApplyDynamicFilter(request);
        }

        // sort
        query = query.ApplyOrdering(request);

        return query;
    }
}
