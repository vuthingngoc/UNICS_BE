﻿using System.Threading.Tasks;
using UniCEC.Data.ViewModels.Common;
using UniCEC.Data.ViewModels.Entities.User;

namespace UniCEC.Business.Services.UserSvc
{
    public interface IUserService
    {
        public Task<PagingResult<ViewUser>> GetAllPaging(PagingRequest request);
        public Task<ViewUser> GetByUserId(string userId);
        public Task<ViewUser> Insert(UserInsertModel user);
        public Task<bool> Update(ViewUser user);
        public Task<bool> Delete(int id);
    }
}
