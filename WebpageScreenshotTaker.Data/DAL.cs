using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using WebpageScreenshotTaker.Data.Models;

namespace WebpageScreenshotTaker.Data
{
    public static class DAL
    {
        #region DAL Method

        /// <summary>
        /// Loads the ScreenshotDetails from the database.
        /// </summary>
        /// <returns></returns>
        public static List<ScreenshotDetails> LoadAllData()
        {
            try
            {
                using (IDbConnection connection = new SQLiteConnection(ConnectionString.Get()))
                {
                    var output = connection.QueryAsync<ScreenshotDetails>("select * from ScreenshotDetails", new DynamicParameters());
                    return output.Result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<short> InsertNewScresnshot(string query, object parameter)
        {
            try
            {
                using (IDbConnection connection = new SQLiteConnection(ConnectionString.Get()))
                {
                    short newlyCreatedScreenshotId = await connection.ExecuteScalarAsync<short>(query, parameter);
                    return newlyCreatedScreenshotId;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<bool> ExecuteQueryAsync(string query, object value)
        {
            try
            {
                using (IDbConnection connection = new SQLiteConnection(ConnectionString.Get()))
                {
                    var affectedRows = await connection.ExecuteAsync(query,
                        value);
                    return affectedRows > 0 ? true : false;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion


        public static async Task<List<ScreenshotDetails>> GetScheduledScreenshotDetails(object timeScheduled)
        {
            using (IDbConnection connection = new SQLiteConnection(ConnectionString.Get()))
            {
                var output = await connection.
                    QueryAsync<ScreenshotDetails>("SELECT * FROM ScreenshotDetails WHERE TimeScheduled=" +
                    "@TimeScheduled",
                    timeScheduled);
                return output.ToList();
            }
        }
    }
}
