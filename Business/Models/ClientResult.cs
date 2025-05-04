using Domain.Models;

namespace Business.Models;

public class ClientResult : ServiceResult
{
    public IEnumerable<Client>? Result { get; set; }
}
public class SingleClientResult : ServiceResult
{
    public Client? Result { get; set; }
}



