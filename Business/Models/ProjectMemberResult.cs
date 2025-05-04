

namespace Business.Models;

public class ProjectMemberResult<T> : ServiceResult
{
    public T? Result { get; set; }
}

public class ProjectMemberResult : ServiceResult
{
}
