using System;
using Ecommerce.Data;
using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Query;

public static class ShoppingCartQuery
{
    public static IQueryable<ShoppingCart> GetQuery(EcommerceDbContext context, PaginationRequestDto request, int userId)
    {
        // var query = context.ShoppingCarts.AsQueryable;
        IQueryable<ShoppingCart> query = context.ShoppingCarts.Include(c => c.Product).Include(c => c.User);

        if (!string.IsNullOrWhiteSpace(request.SearchAll))
        {
            query = query.Where(c =>
            c.Product != null && c.Product.Name.Contains(request.SearchAll) ||
            c.User != null && c.User.Name.Contains(request.SearchAll)
            );
        }

        // filter by userid
        query = query.Where(c => c.UserId == userId);

        query = query.OrderBy(c => c.Id);

        return query;
    }
}
