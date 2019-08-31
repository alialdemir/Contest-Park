using ContestPark.Identity.API.IntegrationEvents.Events;
using ContestPark.Identity.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace ContestPark.Identity.API.Data.Repositories.Reference
{
    public class ReferenceRepository : IReferenceRepository
    {
        #region Private variables

        private readonly ApplicationDbContext _applicationDbContext;

        #endregion Private variables

        #region Constructor

        public ReferenceRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Parametreden gelen referans kodu geçerli mi kontrol eder
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Referans kodu geçerli ise true değilse false döner</returns>
        public ReferenceModel IsCodeActive(string code)
        {
            string sql = @"(SELECT r.Amount, r.BalanceType FROM `References` r
                           WHERE r.CODE = @code AND CURRENT_DATE() <= r.FinishDate AND (SELECT COUNT(rc.ReferenceCodeId) FROM  `ReferenceCodes` rc
                           WHERE rc.CODE = @code) <= r.Menstruation)";

            DbCommand cmd = _applicationDbContext.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            var parameter = cmd.CreateParameter();
            parameter.ParameterName = "@code";
            parameter.Value = code;

            cmd.Parameters.Add(parameter);

            if (cmd.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }
            var dataReader = cmd.ExecuteReader();

            if (!dataReader.Read())
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Connection.Close();
                }

                return null;
            }

            ReferenceModel referenceModel = new ReferenceModel
            {
                Amount = dataReader.GetDecimal(0),
                BalanceType = (BalanceTypes)dataReader.GetByte(1)
            };

            if (cmd.Connection.State == ConnectionState.Open)
            {
                cmd.Connection.Close();
            }

            return referenceModel;
        }

        #endregion Methods
    }
}
