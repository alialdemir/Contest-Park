using Orleans;
using System;
using System.Threading.Tasks;

namespace ContestPark.Domain.Question.Interfaces
{
    public interface IAskedQuestionGrain : IGrainWithIntegerKey
    {
        Task Insert(Int16 subCategoryId, int questionInfoId, params string[] userIds);
    }
}