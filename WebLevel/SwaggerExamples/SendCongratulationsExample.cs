using Swashbuckle.AspNetCore.Filters;

namespace WebApplication1.SwaggerExamples;

public class SendCongratulationsExample: IExamplesProvider<object>
{
    public object GetExamples()
    {
        return new
        {
            message = "Congratulations successful"
        };
    }
}