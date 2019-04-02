using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using LoggingSample_BLL.Helpers;
using LoggingSample_BLL.Models;
using LoggingSample_BLL.Services;
using LoggingSample_DAL.Context;
using NLog;

namespace LoggingSample.Controllers
{
    [RoutePrefix("api")]
    public class OrdersController : ApiController
    {
        private readonly AppDbContext _context = new AppDbContext();
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private static OrderService _orderService = new OrderService();

        [Route("customers/{customerId}/orders", Name = "Orders")]
        public async Task<IHttpActionResult> Get(int customerId)
        {
            Logger.Info($"Start getting all orders by customer id: {customerId}.");

            var orders = await _orderService.GetAllOrders(customerId);

            Logger.Info($"Retrieving orders with customer id {customerId} to response.");

            return Ok(orders.Select(InitOrder));
        }

        [Route("customers/{customerId}/orders/{orderId}", Name = "Order")]
        public async Task<IHttpActionResult> Get(int customerId, int orderId)
        {
            Logger.Info($"Start getting order with id {orderId} by customer id {customerId}.");            

            try
            {
                var order = await _orderService.GetOrder(customerId, orderId);

                if (order == null)
                {
                    Logger.Info($"No order with id {orderId} was found.");
                    return NotFound();
                }

                Logger.Info($"Retrieving order with id {orderId} by customer id {customerId} to response.");

                return Ok(InitOrder(order));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Order id {orderId} by customer id {customerId} errors");
                throw;
            }            
        }

        private object InitOrder(OrderModel model)
        {
            return new
            {
                _self = new UrlHelper(Request).Link("Order", new {customerId = model.CustomerId, orderId = model.Id}),
                customer = new UrlHelper(Request).Link("Customer", new {customerId = model.CustomerId}),
                data = model
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}