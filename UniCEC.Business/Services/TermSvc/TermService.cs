﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using UniCEC.Business.Services.MemberSvc;
using UniCEC.Business.Utilities;
using UniCEC.Data.Models.DB;
using UniCEC.Data.Repository.ImplRepo.MemberRepo;
using UniCEC.Data.Repository.ImplRepo.TermRepo;
using UniCEC.Data.RequestModels;
using UniCEC.Data.ViewModels.Common;
using UniCEC.Data.ViewModels.Entities.Term;

namespace UniCEC.Business.Services.TermSvc
{
    public class TermService : ITermService
    {
        private ITermRepo _termRepo;
        private IMemberRepo _memberRepo;

        private IMemberService _memberService;
        private DecodeToken _decodeToken;

        public TermService(ITermRepo termRepo, IMemberRepo memberRepo, IMemberService memberService)
        {
            _termRepo = termRepo;
            _memberRepo = memberRepo;
            _memberService = memberService;
            _decodeToken = new DecodeToken();
        }

        public async Task<ViewTerm> GetCurrentTermByClub(string token, int clubId)
        {
            int userId = _decodeToken.Decode(token, "Id");

            bool isMember = await _memberRepo.CheckExistedMemberInClub(userId, clubId);
            if (!isMember) throw new UnauthorizedAccessException("You do not have permission to access this resource");

            ViewTerm term = await _termRepo.GetCurrentTermByClub(clubId);
            if (term == null) throw new NullReferenceException("Not found any term");
            return term;
        }

        public async Task<PagingResult<ViewTerm>> GetByConditions(string token, int clubId, TermRequestModel request)
        {
            int userId = _decodeToken.Decode(token, "Id");

            bool isMember = await _memberRepo.CheckExistedMemberInClub(userId, clubId);
            if (!isMember) throw new UnauthorizedAccessException("You do not have permission to access this resource");

            // check valid time
            if (request.CreateTime.HasValue && request.EndTime.HasValue)
            {
                int result = DateTime.Compare(request.CreateTime.Value, request.EndTime.Value);
                if (result > 0) throw new ArgumentException("CreateTime must earlier than EndTime");
            }

            PagingResult<ViewTerm> terms = await _termRepo.GetByConditions(clubId, request);
            if (terms == null) throw new NullReferenceException("Not found any term");
            return terms;
        }

        public async Task<ViewTerm> GetById(string token, int clubId, int id)
        {
            int userId = _decodeToken.Decode(token, "Id");

            bool isMember = await _memberRepo.CheckExistedMemberInClub(userId, clubId);
            if (!isMember) throw new UnauthorizedAccessException("You do not have permission to access this resource");

            ViewTerm term = await _termRepo.GetById(clubId, id);
            if (term == null) throw new NullReferenceException("Not found this term");
            return term;
        }

        public async Task<ViewTerm> Insert(string token, TermInsertModel model) 
        {
            // if user is not leader or vice president
            int userId = _decodeToken.Decode(token, "Id");
            int clubRoleId = await _memberRepo.GetRoleMemberInClub(userId, model.ClubId);
            if (!clubRoleId.Equals(1) && !clubRoleId.Equals(2)) throw new UnauthorizedAccessException("You do not have permission to access this resource");
            
            if (string.IsNullOrEmpty(model.Name) || model.CreateTime == DateTime.MinValue
                || model.EndTime == DateTime.MinValue) throw new ArgumentNullException("Name Null || CreateTime Null || EndTime Null");

            // check valid time
            if ((model.EndTime - model.CreateTime).TotalDays < 30) throw new ArgumentException("EndTime > CreateTime (min 30 days)");

            Term term = new Term()
            {
                Name = model.Name,
                CreateTime = model.CreateTime,
                EndTime = model.EndTime,
                Status = true // default status when insert
            };
            int id = await _termRepo.Insert(term);

            // insert new data for member
            await CloseOldTermByClub(model.ClubId);
            await _memberService.InsertForNewTerm(model.ClubId, id);

            return new ViewTerm()
            {
                Id = id,
                Name = model.Name,
                CreateTime = model.CreateTime,
                EndTime = model.EndTime
            };
        }

        public async Task Update(string token, TermUpdateModel model, int clubId)
        {
            int userId = _decodeToken.Decode(token, "Id");

            int clubRoleId = await _memberRepo.GetRoleMemberInClub(userId, clubId);
            if (!clubRoleId.Equals(1) && !clubRoleId.Equals(2)) throw new UnauthorizedAccessException("You do not have permission to access this resource");

            Term term = await _termRepo.Get(model.Id);
            if (term == null) throw new NullReferenceException("Not found this term");

            if (term.Status.Equals(false)) throw new ArgumentException("This term is not active anymore! Can not update");

            if(!string.IsNullOrEmpty(model.Name)) term.Name = model.Name;
            if(model.CreateTime != DateTime.MinValue) term.CreateTime = model.CreateTime.Value;
            if (model.EndTime != DateTime.MinValue) term.EndTime = model.EndTime.Value;
            await _termRepo.Update();
        }

        //public async Task Delete(int id)
        //{
        //    Term termObject = await _termRepo.Get(id);
        //    if (termObject == null) throw new NullReferenceException("Not found this term");

        //    termObject.Status = false;
        //    await _termRepo.Update();
        //}

        public async Task CloseOldTermByClub(int clubId)
        {
            bool isSuccess = await _termRepo.CloseOldTermByClub(clubId);
            if (!isSuccess) throw new NullReferenceException("Not found old term of this club");
        }
    }
}
