﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UniCEC.Data.Models.DB;
using UniCEC.Data.Repository.GenericRepo;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace UniCEC.Data.Repository.ImplRepo.ClubRepo
{
    public class ClubRepo : Repository<Club>, IClubRepo
    {
        public ClubRepo(UniCECContext context) : base(context)
        {

        }

        public async Task<List<Club>> GetByCompetition(int competitionId)
        {
            var query = from cil in context.CompetitionInClubs
                        join c in context.Clubs on cil.ClubId equals c.Id
                        where cil.CompetitionId == competitionId
                        select new { c };

            List<Club> clubs = await query.Select(x =>
                new Club()
                {
                    Id = x.c.Id,
                    Name = x.c.Name,
                    Description = x.c.Description,
                    Founding = x.c.Founding,
                    Status = x.c.Status,
                    TotalMember = x.c.TotalMember,
                    UniversityId = x.c.UniversityId
                }).ToListAsync();

            return (clubs.Count > 0) ? clubs : null;
        }

        public async Task<List<Club>> GetByName(string name)
        {
            var query = from c in context.Clubs
                        where c.Name.Contains(name)
                        select new { c };

            List<Club> clubs = await query.Select(x =>
                new Club()
                {
                    Id = x.c.Id,
                    Name = x.c.Name,
                    Description = x.c.Description,
                    Founding = x.c.Founding,
                    Status = x.c.Status,
                    TotalMember = x.c.TotalMember,
                    UniversityId = x.c.UniversityId
                }
            ).ToListAsync();

            return (clubs.Count > 0) ? clubs : null;
        }

        public async Task<bool> CheckExistedClubName(int universityId, string name)
        {
            Club club = await context.Clubs.FirstOrDefaultAsync(c => c.Name == name && c.UniversityId == universityId);
            return (club != null) ? true : false;
        }
    }
}
