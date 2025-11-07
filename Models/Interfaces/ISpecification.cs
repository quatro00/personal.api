using System.Linq.Expressions;

namespace Personal.UI.Models.Interfaces
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<string> IncludeStrings { get; set; }
    }
}
