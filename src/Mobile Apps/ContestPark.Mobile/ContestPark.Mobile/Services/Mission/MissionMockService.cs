using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Mission;
using ContestPark.Mobile.Models.PagingModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Mission
{
    public class MissionMockService : IMissionService
    {
        public async Task<MissionListModel> MissionListAsync(PagingModel pagingModel)
        {
            MissionModel missionModel1 = await GetMissionModel();
            MissionModel missionModel2 = await GetMissionModel();
            MissionModel missionModel3 = await GetMissionModel();
            MissionModel missionModel4 = await GetMissionModel();
            MissionModel missionModel5 = await GetMissionModel();

            return new MissionListModel
            {
                CompleteMissionCount = 99,
                Count = 1,
                Items = new List<MissionModel>
                {
                    missionModel1,
                    missionModel2,
                    missionModel3,
                    missionModel4,
                    missionModel5
                }
            };
        }

        public Task<bool> TakesMissionGoldAsync(short missionId)
        {
            return Task.FromResult(true);
        }

        private async Task<MissionModel> GetMissionModel()
        {
            Random rnd = new Random();

            await Task.Delay(500);

            return new MissionModel
            {
                Gold = rnd.Next(0, 500),
                IsCompleteMission = false,
                MissionDescription = "TestTest Test Test",
                MissionId = (short)rnd.Next(0, 500),
                MissionName = "Görev" + rnd.Next(1, 500).ToString(),
                MissionPicturePath = DefaultImages.DefaultLock,
                MissionStatus = rnd.Next(1, 51) % 2 == 0
            };
        }
    }
}