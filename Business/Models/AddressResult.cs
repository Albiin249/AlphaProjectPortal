namespace Business.Models;

public class AddressResult<T> : ServiceResult
{
    public T? Result { get; set; }
}

public class AddressResult : ServiceResult
{
    public int AddressId { get; set; }
}

