using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using LoggingSample_BLL.Helpers;
using LoggingSample_BLL.Models;
using LoggingSample_BLL.Services;
using LoggingSample_DAL.Context;
using NLog;
using static LoggingSample_BLL.Services.CustomerService;

namespace LoggingSample.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        private readonly AppDbContext _context = new AppDbContext();
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CustomerService _customerService = new CustomerService();

        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            Logger.Info("Start getting all customers.");

            var customers = await _customerService.GetAllCustomers();

            return Ok(customers.Select(InitCustomer));
        }

        [Route("{customerId}", Name = "Customer")]
        public async Task<IHttpActionResult> Get(int customerId)
        {
            Logger.Info($"Start getting customer with id {customerId}.");
            try
            {
                var customer = (await _context.Customers.SingleOrDefaultAsync(item => item.Id == customerId)).Map();

                if (customer == null)
                {
                    Logger.Info($"No customer with id {customerId} was found.");
                    return NotFound();
                }

                return Ok(InitCustomer(customer));
            }

            catch (CustomerServiceException ex)
            {
                if (ex.Type == CustomerServiceException.ErrorType.WrongCustomerId)
                {
                    Logger.Warn($"Wrong customerId has been request: {customerId}", ex);
                    return BadRequest($"Wrong customerId has been request: {customerId}");
                }

                throw;
            }
        }


        private object InitCustomer(CustomerModel model)
        {
            return new
            {
                _self = new UrlHelper(Request).Link("Customer", new { customerId = model.Id }),
                orders = new UrlHelper(Request).Link("Orders", new { customerId = model.Id }),
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