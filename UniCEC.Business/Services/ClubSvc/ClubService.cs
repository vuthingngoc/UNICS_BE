﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using UniCEC.Business.Services.FileSvc;
using UniCEC.Data.Common;
using UniCEC.Data.Enum;
using UniCEC.Data.Models.DB;
using UniCEC.Data.Repository.ImplRepo.ClubRepo;
using UniCEC.Data.Repository.ImplRepo.CompetitionActivityRepo;
using UniCEC.Data.Repository.ImplRepo.CompetitionInClubRepo;
using UniCEC.Data.Repository.ImplRepo.CompetitionRepo;
using UniCEC.Data.Repository.ImplRepo.MemberRepo;
using UniCEC.Data.Repository.ImplRepo.TermRepo;
using UniCEC.Data.Repository.ImplRepo.UserRepo;
using UniCEC.Data.RequestModels;
using UniCEC.Data.ViewModels.Common;
using UniCEC.Data.ViewModels.Entities.Club;

namespace UniCEC.Business.Services.ClubSvc
{
    public class ClubService : IClubService
    {
        private IClubRepo _clubRepo;
        private ICompetitionActivityRepo _clubActivityRepo;
        private IMemberRepo _memberRepo;
        private ICompetitionInClubRepo _competitionInClubRepo;
        private ICompetitionRepo _competitionRepo;
        private ITermRepo _termRepo;
        private IUserRepo _userRepo;

        private IFileService _fileService;

        private JwtSecurityTokenHandler _tokenHandler;

        public ClubService(IClubRepo clubRepo, ICompetitionActivityRepo clubActivityRepo, ITermRepo termRepo
                            , IMemberRepo memberRepo, ICompetitionInClubRepo competitionInClubRepo
                                , ICompetitionRepo competitionRepo, IFileService fileService, IUserRepo userRepo)
        {
            _clubRepo = clubRepo;
            _clubActivityRepo = clubActivityRepo;
            _memberRepo = memberRepo;
            _competitionInClubRepo = competitionInClubRepo;
            _competitionRepo = competitionRepo;
            _termRepo = termRepo;
            _fileService = fileService;
            _userRepo = userRepo;
        }

        private int DecodeToken(string token, string nameClaim)
        {
            if (_tokenHandler == null) _tokenHandler = new JwtSecurityTokenHandler();
            var claim = _tokenHandler.ReadJwtToken(token).Claims.FirstOrDefault(selector => selector.Type.ToString().Equals(nameClaim));
            return Int32.Parse(claim.Value);
        }

        private async Task<ViewClub> AddMoreInfoClub(ViewClub club)
        {
            club.TotalActivity = await _clubActivityRepo.GetTotalActivityByClub(club.Id);
            club.TotalEvent = await _competitionInClubRepo.GetTotalEventOrganizedByClub(club.Id);
            club.MemberIncreaseThisMonth = await _memberRepo.GetQuantityNewMembersByClub(club.Id);
            if (!string.IsNullOrEmpty(club.Image))
            {
                try
                {
                    club.Image = await _fileService.GetUrlFromFilenameAsync(club.Image);
                }
                catch (Exception)
                {
                    club.Image = "";
                }
            }

            return club;
        }

        private async Task<PagingResult<ViewClub>> AddMoreInfoClub(PagingResult<ViewClub> clubs)
        {
            foreach (ViewClub element in clubs.Items)
            {
                element.TotalActivity = await _clubActivityRepo.GetTotalActivityByClub(element.Id);
                element.TotalEvent = await _competitionInClubRepo.GetTotalEventOrganizedByClub(element.Id);
                element.MemberIncreaseThisMonth = await _memberRepo.GetQuantityNewMembersByClub(element.Id);
                if (!string.IsNullOrEmpty(element.Image))
                {
                    try
                    {
                        element.Image = await _fileService.GetUrlFromFilenameAsync(element.Image);
                    }
                    catch (Exception)
                    {
                        element.Image = "";
                    }
                }
            }

            return clubs;
        }

        public async Task<ViewClub> GetById(string token, int id)
        {
            int roleId = DecodeToken(token, "RoleId");

            ViewClub club = await _clubRepo.GetById(id, roleId);
            if (club == null) throw new NullReferenceException("Not found this club");

            if (!roleId.Equals(4) && !roleId.Equals(2)) // not system admin and sponsor
            {
                int uniId = DecodeToken(token, "UniversityId");
                if (!uniId.Equals(club.UniversityId)) throw new UnauthorizedAccessException("You do not have permission to access this club");
            }

            return await AddMoreInfoClub(club);
        }

        public async Task<PagingResult<ViewClub>> GetByCompetition(string token, int competitionId, PagingRequest request)
        {
            int roleId = DecodeToken(token, "RoleId");

            if (!roleId.Equals(4) && !roleId.Equals(2))// not system admin and sponsor
            {
                int universityId = DecodeToken(token, "UniversityId");
                CompetitionScopeStatus scope = await _competitionRepo.GetScopeCompetition(competitionId);
                if (scope.Equals(CompetitionScopeStatus.InterUniversity))
                {
                    bool isValid = await _competitionRepo.CheckExisteUniInCompetition(competitionId, universityId);
                    if (!isValid) throw new UnauthorizedAccessException("You do not have permission to access this resource");
                }
            }

            PagingResult<ViewClub> clubs = await _clubRepo.GetByCompetition(competitionId, request);
            if (clubs == null) throw new NullReferenceException("Not found any club with this competition id");

            return await AddMoreInfoClub(clubs);
        }

        public async Task<PagingResult<ViewClub>> GetByConditions(string token, ClubRequestModel request)
        {
            int roleId = DecodeToken(token, "RoleId");

            if (!roleId.Equals(4) && !roleId.Equals(2)) // not system admin and sponsor
            {
                int uniId = DecodeToken(token, "UniversityId");
                if (!request.UniversityId.Equals(uniId)) throw new UnauthorizedAccessException("You do not have permission to access this club");
            }

            PagingResult<ViewClub> clubs = await _clubRepo.GetByConditions(request);
            if (clubs == null) throw new NullReferenceException("Not found any club with this name");

            return await AddMoreInfoClub(clubs);
        }

        public async Task<List<ViewClub>> GetByUser(string token, int id)
        {
            int roleId = DecodeToken(token, "RoleId");
            if (roleId.Equals(4) || roleId.Equals(2)) throw new UnauthorizedAccessException("You can not access this resource");

            int userId = DecodeToken(token, "Id");
            int universityId = DecodeToken(token, "UniversityId");

            bool isSameUni = await _userRepo.CheckExistedUser(universityId, id);      

            if(userId.Equals(id) || (roleId.Equals(1) && isSameUni))
            {
                List<ViewClub> clubs = await _clubRepo.GetByUser(id);
                if (clubs == null) throw new NullReferenceException("This user is not a member of any clubs");

                // add more info
                foreach (ViewClub element in clubs)
                {
                    element.TotalActivity = await _clubActivityRepo.GetTotalActivityByClub(element.Id);
                    element.TotalEvent = await _competitionInClubRepo.GetTotalEventOrganizedByClub(element.Id);
                    element.MemberIncreaseThisMonth = await _memberRepo.GetQuantityNewMembersByClub(element.Id);
                    if (!string.IsNullOrEmpty(element.Image))
                    {
                        try
                        {
                            element.Image = await _fileService.GetUrlFromFilenameAsync(element.Image);
                        }
                        catch (Exception)
                        {
                            element.Image = "";
                        }
                    }
                }

                return clubs;
            }

            throw new UnauthorizedAccessException("You can not access this resource");
        }

        //public async Task<PagingResult<ViewClub>> GetByUniversity(string token, int id, PagingRequest request)
        //{

        //    int roleId = DecodeToken(token, "RoleId");

        //    if (!roleId.Equals(4) && roleId.Equals(2)) // system admin and sponsor
        //    {
        //        int universityId = DecodeToken(token, "UniversityId");
        //        if (!universityId.Equals(id)) throw new UnauthorizedAccessException("You do not have permission to access this club");
        //    }

        //    PagingResult<ViewClub> clubs = await _clubRepo.GetByUniversity(id, request);
        //    if (clubs == null) throw new NullReferenceException("This university have no any clubs");

        //    // add more info
        //    return await AddMoreInfoClub(clubs);
        //}

        public async Task<ViewClub> Insert(string token, ClubInsertModel model)
        {
            int roleId = DecodeToken(token, "RoleId");
            if (roleId.Equals(4) || roleId.Equals(2)) throw new UnauthorizedAccessException("You can not access this resource");// if system admin or sponsor

            int universityId = DecodeToken(token, "UniversityId");

            if (!roleId.Equals(1) && !universityId.Equals(model.UniversityId))
                throw new UnauthorizedAccessException("You do not have permission to add new club");

            if (string.IsNullOrEmpty(model.Description) || model.UniversityId == 0 ||
                    string.IsNullOrEmpty(model.Name) || model.Founding == DateTime.MinValue)
                throw new ArgumentNullException("Description Null || UniversityId Null || Name Null || Founding Null");

            int checkClubId = await _clubRepo.CheckExistedClubName(model.UniversityId, model.Name);
            if (checkClubId > 0) throw new ArgumentException("Duplicated club name");

            int statusValidLeader = _memberRepo.CheckValidNewLeader(model.UserId, model.UniversityId);// 0 is valid case
            if (statusValidLeader.Equals(-1)) throw new ArgumentException("This user is not in the university"); 
            else if (statusValidLeader.Equals(1)) throw new ArgumentException("This user is leader in another club");

            Club club = new Club()
            {
                Description = model.Description,
                Founding = model.Founding,
                Name = model.Name,
                TotalMember = 1, // default number member 
                UniversityId = model.UniversityId,
                Status = true, // default status 
                Image = await _fileService.UploadFile(model.Image),
                ClubContact = model.ClubContact,
                ClubFanpage = model.ClubFanpage
            };
            int clubId = await _clubRepo.Insert(club);

            DateTime currentTime = new LocalTime().GetLocalTime().DateTime;
            Term term = new Term()
            {
                Name = "First Term", // default name
                CreateTime = currentTime,
                EndTime = currentTime.AddYears(1), // default distance of time
                Status = true // default status
            };
            int termId = await _termRepo.Insert(term);

            Member member = new Member()
            {
                ClubId = clubId,
                ClubRoleId = 1, // leader
                StartTime = currentTime,
                TermId = termId,
                UserId = model.UserId,
                Status = MemberStatus.Active // default status 
            };
            await _memberRepo.Insert(member);

            ViewClub viewClub = await _clubRepo.GetById(clubId, roleId);

            return await AddMoreInfoClub(viewClub);
        }

        public async Task Update(string token, ClubUpdateModel model)
        {
            int userId = DecodeToken(token, "Id");
            int clubRoleId = await _memberRepo.GetRoleMemberInClub(userId, model.Id);

            // if role is not leader or vice president
            if (!clubRoleId.Equals(1) && !clubRoleId.Equals(2)) throw new UnauthorizedAccessException("You do not have permission to update this club");

            Club club = await _clubRepo.Get(model.Id);
            if (club == null) throw new NullReferenceException("Not found this club");

            if (!string.IsNullOrEmpty(model.Name))
            {
                int clubId = await _clubRepo.CheckExistedClubName(club.UniversityId, model.Name);
                if (clubId > 0 && clubId != club.Id) throw new ArgumentException("Duplicated club name");
                club.Name = model.Name;
            }

            if (!string.IsNullOrEmpty(model.Description)) club.Description = model.Description;

            if (model.Founding != DateTime.MinValue) club.Founding = model.Founding;

            club.TotalMember = await _memberRepo.GetTotalMembersByClub(club.Id);

            if (model.Status != false) club.Status = model.Status;

            if (!string.IsNullOrEmpty(model.Image))
            {
                if (string.IsNullOrEmpty(club.Image)) club.Image = await _fileService.UploadFile(model.Image); // insert
                else await _fileService.UploadFile(club.Image, model.Image); // update                
            }

            if (!string.IsNullOrEmpty(model.ClubContact)) club.ClubContact = model.ClubContact;

            if (!string.IsNullOrEmpty(model.ClubFanpage)) club.ClubFanpage = model.ClubFanpage;

            await _clubRepo.Update();
        }

        public async Task Update(string token, int clubId, bool status) // for university admin
        {
            int roleId = DecodeToken(token, "RoleId");
            if (roleId.Equals(4) || roleId.Equals(2)) throw new UnauthorizedAccessException("You can not access this resource");

            int uniId = DecodeToken(token, "UniversityId");
            int universityId = await _clubRepo.GetUniversityByClub(clubId);

            if (!roleId.Equals(1) && !uniId.Equals(universityId))
                throw new UnauthorizedAccessException("You do not have permission to access this resource");

            Club club = await _clubRepo.Get(clubId);
            if (club == null) throw new NullReferenceException("Not found this club");
            club.Status = status;
            await _clubRepo.Update();
        }

        public async Task Delete(string token, int id)
        {
            int roleId = DecodeToken(token, "RoleId");
            if (roleId.Equals(4) || roleId.Equals(2)) throw new UnauthorizedAccessException("You can not access this resource");// if system admin or sponsor

            int universityId = DecodeToken(token, "UniversityId");            

            Club club = await _clubRepo.Get(id);
            if (club == null) throw new NullReferenceException("Not found this club");

            if (!roleId.Equals(1) && !universityId.Equals(club.UniversityId))
                throw new UnauthorizedAccessException("You do not have permission to delete this club");            
            
            club.Status = false; // default status for delete
            await _clubRepo.Update();

            await _termRepo.CloseOldTermByClub(id);
            await _memberRepo.UpdateEndTerm(id);
        }
    }
}
