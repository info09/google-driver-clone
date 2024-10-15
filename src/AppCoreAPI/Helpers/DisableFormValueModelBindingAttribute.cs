using Microsoft.AspNetCore.Mvc.Filters;

namespace AppCoreAPI.Helpers
{
    public class DisableFormValueModelBindingAttribute : Attribute, IAsyncResourceFilter
    {
    }
}
