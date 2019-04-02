using LoggingSample_BLL.Models;
using LoggingSample_DAL.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LoggingSample_BLL.Helpers;

namespace LoggingSample_BLL.Services
{
    public class OrderService : IDisposable
    {
        private readonly AppDbContext _context = new AppDbContext();

        public Task<IEnumerable<OrderModel>> GetAllOrders(int customerId)
        {
            return _context.Orders.Where(x => x.CustomerId == customerId).ToArrayAsync().ContinueWith(task =>
            {
                var orders = task.Result.Select(order => order.Map());

                return orders;
            });
        }

        public Task<OrderModel> GetOrder(int customerId, int orderId)
        {
            return _context.Orders.SingleOrDefaultAsync(item => item.Id == customerId && item.Id == orderId).ContinueWith(task =>
            {
                var customer = task.Result;

                return customer?.Map();
            });
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

    }
}