using ContestPark.Infrastructure.Question.Repositories.AskedQuestion;
using ContestPark.Infrastructure.Question.Repositories.Question;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Infrastructure.Question
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddQuestionRegisterService(this IServiceCollection services)
        {
            services.AddTransient<IQuestionRepository, QuestionRepository>();

            services.AddTransient<IAskedQuestionRepository, AskedQuestionRepository>();

            return services;
        }
    }
}