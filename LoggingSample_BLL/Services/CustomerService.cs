using LoggingSample_BLL.Models;
using LoggingSample_DAL.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoggingSample_BLL.Helpers;
using LoggingSample_BLL.Models;

namespace LoggingSample_BLL.Services
{
    public class CustomerService
    {
        private readonly AppDbContext _context = new AppDbContext();

        public Task<CustomerModel> GetCustomer(int customerId)
        {
            if (customerId == 62)
            {
                throw new CustomerServiceException("Wrong id",
                    CustomerServiceException.ErrorType.WrongCustomerId);
            }

            return _context.Customers.SingleOrDefaultAsync(item => item.Id == customerId).ContinueWith(task =>
            {
                var customer = task.Result;

                return customer?.Map();
            });
        }

        public Task<IEnumerable<CustomerModel>> GetAllCustomers()
        {
            return _context.Customers.ToArrayAsync().ContinueWith(task =>
            {
                var customers = task.Result.Select(customer => customer.Map());

                return customers;
            });
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public class CustomerServiceException : Exception
        {
            public enum ErrorType
            {
                WrongCustomerId
            }

            public ErrorType Type { get; set; }

            public CustomerServiceException(string message, ErrorType errorType) : base(message)
            {
                Type = errorType;
            }
        }
    }
}
