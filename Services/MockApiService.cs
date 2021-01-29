using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FxaPortal.Helpers;
using FxaPortal.Models;
using RestClient;
using RestClient.Net.Abstractions;


namespace FxaPortal.Services
{
    public class MockApiService : IApiService
    {
        public Task<Response<T>> SimplePagedGet<T>(int page, int size, string methodCall)
        {
            throw new NotImplementedException();
            //var mHelper = new MockHelper();

            //return (Task<T>)Task.Run(async delegate
            //{
            //    return await GetSchedules();
            //}).Result;

        }

        public Task<Response<T>> SimpleSave<T>(T item, string call)
        {
            throw new NotImplementedException();
        }

        public Task<Response<T>> SimpleUpdate<T>(T item, string call, string id)
        {
            throw new NotImplementedException();
        }

        //private IEnumerable<Schedule> GetSchedules<Schedule>()
        //{
        //    var schedules = new List<Schedule>();
        //    var schedule = new Schedule();
        //    schedule.Name = "ZZZ";

        //    schedules.Add(schedule);

        //    return schedules;


        //}

        private IList<Schedule> GetSchedules()
        {
            var schedules = new List<Schedule>();
            var schedule = new Schedule();
            schedule.Name = "ZZZ";

            schedules.Add(schedule);

            return schedules;
        }
    }
}
