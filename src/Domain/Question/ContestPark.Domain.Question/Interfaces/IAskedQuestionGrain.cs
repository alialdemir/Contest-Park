using ContestPark.Core.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace ContestPark.Domain.Question.Interfaces
{
    public interface IAskedQuestionGrain : IGrainBase
    {
        Task Insert(Int16 subCategoryId, int questionInfoId, params string[] userIds);
    }
}