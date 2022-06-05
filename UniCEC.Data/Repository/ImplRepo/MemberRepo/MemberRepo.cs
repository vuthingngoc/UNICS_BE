﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UniCEC.Data.Models.DB;
using UniCEC.Data.Repository.GenericRepo;
using UniCEC.Data.ViewModels.Entities.Member;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using UniCEC.Data.Enum;
using UniCEC.Data.ViewModels.Common;
using UniCEC.Data.RequestModels;
using UniCEC.Data.Repository.ImplRepo.TermRepo;
using UniCEC.Data.ViewModels.Entities.Term;

namespace UniCEC.Data.Repository.ImplRepo.MemberRepo
{
    public class MemberRepo : Repository<Member>, IMemberRepo
    {
        private ITermRepo _termRepo;
        public MemberRepo(UniCECContext context, ITermRepo termRepo) : base(context)
        {
            _termRepo = termRepo;
        }

        private async Task<DateTime> GetJoinDate(int userId, int clubId)
        {
            return await (from m in context.Members
                          where m.UserId.Equals(userId) && m.ClubId.Equals(clubId)
                          select m.StartTime).FirstOrDefaultAsync();
        }

        private async Task<DateTime> GetJoinDate(int memberId)
        {
            var member = await Get(memberId);
            return await GetJoinDate(member.UserId, member.ClubId);
        }

        public async Task<PagingResult<ViewMember>> GetMembersByClub(int clubId, int? termId, MemberStatus? status, PagingRequest request)
        {
            var query = from m in context.Members
                        join t in context.Terms on m.TermId equals t.Id
                        join cr in context.ClubRoles on m.ClubRoleId equals cr.Id
                        join u in context.Users on m.UserId equals u.Id
                        where m.ClubId.Equals(clubId)
                        select new { cr, m, u, t };

            if (termId.HasValue) query = query.Where(selector => selector.m.TermId.Equals(termId.Value));

            int totalCount = query.Count();
            List<ViewMember> members = await query.Skip((request.CurrentPage - 1) * request.PageSize).Take(request.PageSize)
                                                    .Select(selector => new ViewMember()
                                                    {
                                                        Id = selector.m.Id,
                                                        Name = selector.u.Fullname,
                                                        Avatar = selector.u.Avatar,
                                                        ClubRoleId = selector.cr.Id,
                                                        ClubRoleName = selector.cr.Name,
                                                        IsOnline = selector.u.IsOnline,
                                                        TermId = selector.t.Id,
                                                        TermName = selector.t.Name,

                                                    }).ToListAsync();

            return (totalCount > 0) ? new PagingResult<ViewMember>(members, totalCount, request.CurrentPage, request.PageSize) : null;
        }

        public async Task<List<Member>> GetMembersByClub(int clubId)
        {
            var query = from m in context.Members
                        join t in context.Terms on m.TermId equals t.Id
                        join cr in context.ClubRoles on m.ClubRoleId equals cr.Id
                        join u in context.Users on m.UserId equals u.Id
                        where m.ClubId.Equals(clubId) && m.Status.Equals(MemberStatus.Active)
                        select new { cr, m, u, t };

            return await query.Select(selector => new Member()
            {
                Id = selector.m.Id,
                ClubId = selector.m.ClubId,
                StartTime = selector.m.StartTime,
                EndTime = selector.m.EndTime,
                UserId = selector.m.UserId,
                TermId = selector.t.Id,
                ClubRoleId = selector.m.Id,
            }).ToListAsync();
        }

        public async Task<ViewDetailMember> GetDetailById(int memberId)
        {
            var query = from m in context.Members
                        join u in context.Users on m.UserId equals u.Id
                        join cr in context.ClubRoles on m.ClubRoleId equals cr.Id
                        where m.Id.Equals(memberId)
                        select new { m, u, cr };

            DateTime joinDate = await GetJoinDate(memberId);
            ViewDetailMember member = await query.Select(selector => new ViewDetailMember()
            {
                Id = memberId,
                Name = selector.u.Fullname,
                Avatar = selector.u.Avatar,
                ClubRoleId = selector.cr.Id,
                ClubRoleName = selector.cr.Name,
                Email = selector.u.Email,
                JoinDate = joinDate,
                PhoneNumber = selector.u.PhoneNumber,
                IsOnline = selector.u.IsOnline
            }).FirstOrDefaultAsync();

            return (query.Count() > 0) ? member : null;
        }

        public async Task<ViewMember> GetById(int memberId)
        {
            var query = from m in context.Members
                        join u in context.Users on m.UserId equals u.Id
                        join cr in context.ClubRoles on m.ClubRoleId equals cr.Id
                        join t in context.Terms on m.TermId equals t.Id
                        where m.Id.Equals(memberId)
                        select new { m, u, cr, t };

            DateTime joinDate = await GetJoinDate(memberId);
            ViewMember member = await query.Select(selector => new ViewMember()
            {
                Id = memberId,
                Name = selector.u.Fullname,
                Avatar = selector.u.Avatar,
                ClubRoleId = selector.cr.Id,
                ClubRoleName = selector.cr.Name,
                TermId = selector.m.TermId,
                TermName = selector.t.Name,
                StartTime = selector.m.StartTime,
                EndTime = selector.m.EndTime,
                Status = selector.m.Status,
                IsOnline = selector.u.IsOnline,                
            }).FirstOrDefaultAsync();

            return (query.Count() > 0) ? member : null;
        }

        public async Task<int> GetClubIdByMember(int memberId)
        {
            return await (from m in context.Members
                        where m.Id.Equals(memberId)
                        select m.ClubId).FirstOrDefaultAsync();            
        }

        public async Task<bool> CheckExistedMemberInClub(int userId, int clubId)
        {
            var query = from m in context.Members
                        where m.UserId.Equals(userId) && m.ClubId.Equals(clubId)
                                && m.Status == MemberStatus.Active
                        select m.Id;

            int memberId = await query.FirstOrDefaultAsync();
            return (memberId > 0) ? true : false;
        }

        public async Task<List<ViewIntroClubMember>> GetLeadersByClub(int clubId)
        {
            var query = from m in context.Members
                        join u in context.Users on m.UserId equals u.Id
                        join cr in context.ClubRoles on m.ClubRoleId equals cr.Id
                        where m.ClubId.Equals(clubId) && m.Status.Equals(MemberStatus.Active)
                                && (m.ClubRoleId.Equals(1) || m.ClubRoleId.Equals(2) || m.ClubRoleId.Equals(3) || m.ClubRoleId.Equals(4)) // Leader, Vice President, Manager, member
                        select new { m, u, cr }; // if the club don't have enough 3 leaders => replace by member            

            List<ViewIntroClubMember> members = await query.Take(3).Select(x => new ViewIntroClubMember()
            {
                Id = x.m.Id,
                Name = x.u.Fullname,
                ClubRoleId = x.m.ClubRoleId,
                ClubRoleName = x.cr.Name,
                Avatar = x.u.Avatar,
                IsOnline = x.u.IsOnline
            }).ToListAsync();

            return (members.Count > 0) ? members : null;
        }

        public async Task<int> GetQuantityNewMembersByClub(int clubId)
        {
            var club = await context.Clubs.FirstOrDefaultAsync(c => c.Id.Equals(clubId));
            if (club == null) return -1;

            var query = from m in context.Members
                        where m.ClubId.Equals(clubId) && m.Status.Equals(MemberStatus.Active)
                                && m.StartTime.Month.Equals(DateTime.Now.Month)
                                && m.StartTime.Year.Equals(DateTime.Now.Year)
                        select new { m };

            return await query.CountAsync();
        }

        public async Task<int> GetRoleMemberInClub(int userId, int clubId)
        {
            var query = from m in context.Members
                        where m.UserId.Equals(userId) && m.ClubId.Equals(clubId)
                                && m.Status.Equals(MemberStatus.Active)
                        select m.ClubRoleId;

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> GetRoleMemberInClub(int memberId)
        {
            var query = from m in context.Members
                        where m.Id.Equals(memberId) && m.Status.Equals(MemberStatus.Active)
                        select m.ClubRoleId;

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> CheckDuplicated(int clubId, int clubRoleId, int userId, int termId)
        {
            Member member = await context.Members.FirstOrDefaultAsync(m => m.ClubId.Equals(clubId)
                                                                && m.ClubRoleId.Equals(clubRoleId)
                                                                && m.UserId.Equals(userId)
                                                                && m.TermId.Equals(termId));
            return (member != null) ? member.Id : 0;
        }

        public async Task<PagingResult<ViewMember>> GetByConditions(int clubId, MemberRequestModel request)
        {
            var query = from m in context.Members
                        join cr in context.ClubRoles on m.ClubRoleId equals cr.Id
                        join c in context.Clubs on m.ClubId equals c.Id
                        join t in context.Terms on m.TermId equals t.Id
                        where m.ClubId.Equals(clubId)
                        select new { m, cr, c, t };

            if (request.ClubRoleId.HasValue) query = query.Where(x => x.m.ClubRoleId.Equals(request.ClubRoleId));

            if (request.StartTime.HasValue) query = query.Where(x => x.m.StartTime.Equals(request.StartTime));

            if (request.EndTime.HasValue) query = query.Where(x => x.m.EndTime.Equals(request.EndTime));

            if (request.TermId.HasValue) query = query.Where(x => x.m.TermId.Equals(request.TermId));

            if (request.Status.HasValue) query = query.Where(x => x.m.Status.Equals(request.Status));

            int totalCount = query.Count();
            List<ViewMember> items = await query.Skip((request.CurrentPage - 1) * request.PageSize).Take(request.PageSize)
                                                     .Select(x => new ViewMember()
                                                     {
                                                         Id = x.m.Id,
                                                         ClubRoleId = x.m.ClubRoleId,
                                                         ClubRoleName = x.cr.Name,
                                                         TermId = x.m.TermId,
                                                         TermName = x.t.Name,
                                                         StartTime = x.m.StartTime,
                                                         EndTime = x.m.EndTime,
                                                         Status = x.m.Status
                                                     }).ToListAsync();

            return (items.Count > 0) ? new PagingResult<ViewMember>(items, totalCount, request.CurrentPage, request.PageSize) : null;
        }

        //TA
        public async Task<ViewBasicInfoMember> GetBasicInfoMember(GetMemberInClubModel model)
        {
            var query = from m in context.Members
                        join us in context.Users on m.UserId equals us.Id
                        where m.ClubId == model.ClubId && m.TermId == model.TermId && m.UserId == model.UserId && m.Status == MemberStatus.Active
                        select new { us, m };


            return await query.Select(x => new ViewBasicInfoMember()
            {
                Name = x.us.Fullname,
                ClubRoleName = x.m.ClubRole.Name,
                ClubRoleId = x.m.ClubRoleId,
                Id = x.m.Id,
                TermId = x.m.TermId
            }).FirstOrDefaultAsync();
        }
        public async Task<Member> IsMemberInListClubCompetition(List<int> List_ClubId_In_Competition, User studentInfo)
        {

            //tìm user có là member trong 1 cuộc thi được tổ chức bởi nhiều Club
            // User -> Member -> Club 
            foreach (int ClubId_In_Competition in List_ClubId_In_Competition)
            {
                //Get current Term của 1 club
                ViewTerm term = await _termRepo.GetCurrentTermByClub(ClubId_In_Competition);

                var query = from us in context.Users
                            where us.Id == studentInfo.Id
                            from mem in context.Members
                            where mem.UserId == us.Id && term.Id == mem.TermId && mem.Status == MemberStatus.Active
                            from c in context.Clubs
                            where mem.ClubId == c.Id && c.Id == ClubId_In_Competition
                            select mem;

                Member member = await query.FirstOrDefaultAsync();

                if (member != null)
                {
                    return member;
                }
            }
            return null;
        }

        public async Task<Member> GetLeaderByClub(int clubId)
        {
            //Get current Term của 1 club
            ViewTerm term = await _termRepo.GetCurrentTermByClub(clubId);

            //current club leader 
            return await (from m in context.Members
                          where m.ClubId == clubId && m.ClubRoleId == 1 && m.Status == MemberStatus.Active && m.TermId == term.Id
                          select m).FirstOrDefaultAsync();
        }

        public async Task UpdateMemberRole(int memberId, int clubRoleId)
        {
            Member member = await (from m in context.Members
                                   where m.Id.Equals(memberId) && m.Status.Equals(MemberStatus.Active)
                                   select m).FirstOrDefaultAsync();

            if (member == null) throw new NullReferenceException("Not found this member in club");

            member.EndTime = DateTime.Now;
            member.Status = MemberStatus.Inactive;
            await Update();

            // add new record
            Member newRecord = new Member()
            {
                ClubId = clubRoleId,
                ClubRoleId = member.ClubRoleId,
                StartTime = DateTime.Now,
                TermId = member.TermId,
                UserId = member.UserId,
                Status = MemberStatus.Active, // default status new record                
            };
            await Insert(newRecord);
        }

        public async Task DeleteMember(int memberId)
        {
            Member record = await (from m in context.Members
                                   where m.Id.Equals(memberId) && m.Status.Equals(MemberStatus.Active)
                                   select m).FirstOrDefaultAsync();

            if (record != null)
            {
                record.EndTime = DateTime.Now;
                record.Status = MemberStatus.Inactive;
                await Update();
            }
        }

        public async Task UpdateEndTerm(int clubId)
        {
            (from m in context.Members
             where m.ClubId.Equals(clubId) && m.Status.Equals(MemberStatus.Active)
             select m).ToList().ForEach(record =>
             {
                 record.EndTime = DateTime.Now;
                 record.Status = MemberStatus.Inactive;
             });

            await Update();
        }
    }
}
