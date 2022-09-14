using WebApi.Models;

namespace WebApi;

public interface ICustomerService
{
    public IEnumerable<Customer> Get ();
}

public class CustomerService : ICustomerService
{
    private readonly ILogger<CustomerService> _logger;

    public CustomerService (ILogger<CustomerService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<Customer> Get ()
    {
        _logger.LogInformation("Get Customers Data");
        var names = new List<string> { "Bill", "Jack", "Tony", "Mathew", "Bryan", "CT" };

        return Enumerable.Range(0, names.Count).Select(i => new Customer() { Id = 1, Name = names[i] }).ToList();
    }
}