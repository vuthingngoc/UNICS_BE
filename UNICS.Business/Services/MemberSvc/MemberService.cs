﻿using System;
using System.Threading.Tasks;
using UNICS.Data.Repository.ImplRepo.MemberRepo;
using UNICS.Data.ViewModels.Common;
using UNICS.Data.ViewModels.Entities.Member;

namespace UNICS.Business.Services.MemberSvc
{
    public class MemberService : IMemberService
    {
        private IMemberRepo _memberRepo;

        public MemberService(IMemberRepo memberRepo)
        {
            _memberRepo = memberRepo;
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PagingResult<ViewMember>> GetAll(PagingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ViewMember> GetByMemberId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ViewMember> Insert(MemberInsertModel member)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(MemberUpdateModel member)
        {
            throw new NotImplementedException();
        }
    }
}
