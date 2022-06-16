﻿using System.Threading.Tasks;
using UniCEC.Data.Models.DB;
using UniCEC.Data.Repository.GenericRepo;
using UniCEC.Data.RequestModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UniCEC.Data.ViewModels.Common;
using System.Collections.Generic;
using UniCEC.Data.ViewModels.Entities.Major;

namespace UniCEC.Data.Repository.ImplRepo.MajorRepo
{
    public class MajorRepo : Repository<Major>, IMajorRepo
    {
        public MajorRepo(UniCECContext context) : base(context)
        {

        }

        public async Task<PagingResult<ViewMajor>> GetByConditions(MajorRequestModel request)
        {
            var query = from m in context.Majors
                        where m.UniversityId.Equals(request.UniversityId)
                        select m;


            if (!string.IsNullOrEmpty(request.Name)) query = query.Where(m => m.Name.Contains(request.Name));

            if (request.DepartmentId.HasValue) query = query.Where(m => m.DepartmentId.Equals(request.DepartmentId));

            //if (!string.IsNullOrEmpty(request.MajorCode)) query = query.Where(m => m.MajorCode.Equals(request.MajorCode));

            if (request.Status.HasValue) query = query.Where(m => m.Status.Equals(request.Status.Value));

            int totalCount = query.Count();

            var items = await query.Skip((request.CurrentPage - 1) * request.PageSize).Take(request.PageSize)
                                    .Select(m => new ViewMajor()
                                    {
                                        Id = m.Id,
                                        UniversityId = m.UniversityId,
                                        DepartmentId = m.DepartmentId,
                                        Description = m.Description,
                                        MajorCode = m.MajorCode,
                                        Name = m.Name,
                                        Status = m.Status
                                    }).ToListAsync();

            return (query.Any()) ? new PagingResult<ViewMajor>(items, totalCount, request.CurrentPage, request.PageSize) : null;
        }

        public async Task<PagingResult<Major>> GetByUniversity(int universityId, PagingRequest request)
        {
            var query = from m in context.Majors
                        where m.UniversityId.Equals(universityId)
                        select m;

            int totalCount = query.Count();

            List<Major> majors = await query.Skip((request.CurrentPage - 1) * request.PageSize).Take(request.PageSize)
                .Select(m => new Major()
                {
                    Id = m.Id,
                    UniversityId = m.UniversityId,
                    DepartmentId = m.DepartmentId,
                    Description = m.Description,
                    MajorCode = m.MajorCode,
                    Name = m.Name,
                    Status = m.Status
                }).ToListAsync();

            return (majors.Count > 0) ? new PagingResult<Major>(majors, totalCount, request.CurrentPage, request.PageSize) : null;
        }

        public async Task<int> CheckExistedMajorCode(int universityId, string code)
        {
            Major major = await context.Majors.FirstOrDefaultAsync(m => m.UniversityId.Equals(universityId)
                                                                        && m.MajorCode.Equals(code));
            return (major != null) ? major.Id : 0;
        }

        public async Task<List<int>> GetIdsByDepartmentId(int departmentId, bool? status)
        {
            var query = from m in context.Majors
                        where m.DepartmentId == departmentId
                        select m;

            if (status.HasValue) query = query.Where(m => m.Status.Equals(status.Value));

            List<int> majorIds = await query.Select(m => m.Id).ToListAsync();
            return (majorIds.Count > 0) ? majorIds : null;
        }

        public async Task<ViewMajor> GetById(int id, bool? status, int? universityId)
        {
            var query = from m in context.Majors
                        where m.Id.Equals(id)
                        select m;

            if (status.HasValue) query = query.Where(m => m.Status.Equals(status.Value));
            if (universityId.HasValue) query = query.Where(m => m.UniversityId.Equals(universityId));

            return await query.Select(m => new ViewMajor()
            {
                UniversityId = m.UniversityId,
                DepartmentId = m.DepartmentId,
                Description = m.Description,
                Id = m.Id,
                MajorCode = m.MajorCode,
                Name = m.Name,
                Status = m.Status
            }).FirstOrDefaultAsync();
        }

        public async Task<ViewMajor> GetByCode(string majorCode, bool? status, int? universityId)
        {
            var query = from m in context.Majors
                        where m.MajorCode.Equals(majorCode)
                        select m;

            if (status.HasValue) query = query.Where(m => m.Status.Equals(status.Value));

            if (universityId.HasValue) query = query.Where(m => m.UniversityId.Equals(universityId));

            return await query.Select(m => new ViewMajor()
            {
                UniversityId = m.UniversityId,
                DepartmentId = m.DepartmentId,
                Description = m.Description,
                Id = m.Id,
                MajorCode = m.MajorCode,
                Name = m.Name,
                Status = m.Status
            }).FirstOrDefaultAsync();
        }

        public async Task<int> CheckDuplicatedName(int universityId, string name)
        {
            return await (from m in context.Majors
                          where m.UniversityId.Equals(universityId) && m.Name.ToLower().Equals(name.ToLower())
                          select m.Id).FirstOrDefaultAsync();
        }
    }
}
