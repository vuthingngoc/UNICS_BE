﻿using System.Threading.Tasks;
using UNICS.Data.Models.DB;
using UNICS.Data.Repository.ImplRepo.UniversityRepo;
using UNICS.Data.ViewModels.Entities.University;

namespace UNICS.Business.Services.UniversitySvc
{
    public class UniversityService : IUniversityService
    {
        private IUniversityRepo _universityRepo;

        public UniversityService(IUniversityRepo universityRepo)
        {
            _universityRepo = universityRepo;
        }

        public async Task<ViewUniversity> GetUniversityById(string id)
        {
            University uni = await _universityRepo.Get(id);
            //
            ViewUniversity uniView = new ViewUniversity();
            //
            if (uni != null)
            {
                //gán vào các trường view
                uniView.Name = uni.Name;
                uniView.Description = uni.Description;
                uniView.Phone = uni.Phone;
                uniView.Email = uni.Email;
                uniView.Openning = uni.Openning;
                uniView.Closing = uni.Closing;
                //
            }
            return uniView;

        }

    }
}
