﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using UniCEC.Data.Enum;
using UniCEC.Data.Models.DB;
using UniCEC.Data.Repository.ImplRepo.CompetitionRepo;
using UniCEC.Data.Repository.ImplRepo.ParticipantInTeamRepo;
using UniCEC.Data.Repository.ImplRepo.ParticipantRepo;
using UniCEC.Data.Repository.ImplRepo.TeamRepo;
using UniCEC.Data.Repository.ImplRepo.TeamRoleRepo;
using UniCEC.Data.ViewModels.Common;
using UniCEC.Data.ViewModels.Entities.ParticipantInTeam;
using UniCEC.Data.ViewModels.Entities.Team;

namespace UniCEC.Business.Services.TeamSvc
{
    public class TeamService : ITeamService
    {
        private ITeamRepo _teamRepo;
        private IParticipantRepo _participantRepo;
        private ICompetitionRepo _competitionRepo;
        private ITeamRoleRepo _teamRoleRepo;
        private IParticipantInTeamRepo _participantInTeamRepo;

        public TeamService(ITeamRepo teamRepo, IParticipantRepo participantRepo, ICompetitionRepo competitionRepo, ITeamRoleRepo teamRoleRepo)
        {
            _teamRepo = teamRepo;
            _participantRepo = participantRepo;
            _competitionRepo = competitionRepo;
            _teamRoleRepo = teamRoleRepo;


        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PagingResult<ViewTeam>> GetAllPaging(PagingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ViewTeam> GetByTeamId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(TeamUpdateModel team)
        {
            throw new NotImplementedException();
        }

        public async Task<ViewTeam> InsertTeam(TeamInsertModel model, string token)
        {


            //đã vào thấy screen này thì có nghĩa là đã trở thành Paricipant Check phase

            //---------------------------------------------------------------------INSERT
            //flow insert -- TEAM
            //CHECK Null
            //CHECK có phải là participant hay là không ? bởi vì chỉ có participant mới có quyền tạo
            //CHECK số lượng team trong 1 cuộc thi < Number Of Team
            //Add vào [Participant in Team] -> thằng tạo là leader của Team [TeamRole]
            //TeamStatus  
            //0.Available -> Can Join
            //1.IsLocked  -> Full Member
            //2.InActive  -> Delete Team by leader of team 


            //flow insert -- PARTICIPANT IN TEAM (CompetitionId, InvitedCode)
            //CHECK Null
            //CHECK Competition Id in system ?
            //CHECK InvitedCODE đó là của Team thuộc Competition nào?
            //CHECK Team Status
            //CHECK Member Of Team is available
            //add Participant by InvitedCode


            //ROLE OF Team Member
            //1.Leader
            //2.Member
            try
            {
                var jsonToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var UserIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));

                int UserId = Int32.Parse(UserIdClaim.Value);

                if (model.CompetitionId == 0
                 || string.IsNullOrEmpty(model.Name)
                 || string.IsNullOrEmpty(model.Description)) throw new ArgumentNullException("Competition Id Null || Name Null || Description Null");

                Competition competition = await _competitionRepo.Get(model.CompetitionId);

                if (competition != null)
                {
                    if (await _participantRepo.Participant_In_Competition(UserId, model.CompetitionId) != null)
                    {
                        if (await _teamRepo.CheckNumberOfTeam(model.CompetitionId))
                        {
                            int numberStuOfTeam = competition.NumberOfParticipation / competition.NumberOfTeam;
                            //-----------------Add Team
                            Team team = new Team()
                            {
                                CompetitionId = model.CompetitionId,
                                Name = model.Name,
                                Description = model.Description,
                                //number of student in team
                                NumberOfStudentInTeam = numberStuOfTeam,
                                //generate code
                                InvitedCode = await CheckExistCode(),
                                //status available
                                Status = TeamStatus.Available
                            };
                            int Team_Id = await _teamRepo.Insert(team);
                            if (Team_Id > 0)
                            {
                                Team getTeam = await _teamRepo.Get(Team_Id);
                                //-----------------Add ParticiPant in Team with Role Leader
                                ParticipantInTeam pit = new ParticipantInTeam()
                                {
                                    ParticipantId = model.CompetitionId,
                                    //Team Id
                                    TeamId = getTeam.Id,
                                    //auto leader 
                                    TeamRoleId = await _teamRoleRepo.GetRoleIdByName("Leader"),
                                    //auto status 
                                    Status = ParticipantInTeamStatus.InTeam
                                };
                                await _participantInTeamRepo.Insert(pit);
                                return TransformViewTeam(getTeam);

                            }//end add team
                            else
                            {
                                throw new ArgumentException("Add Team Failed");
                            }
                        }
                        //end check Number Of Team
                        else
                        {
                            throw new ArgumentException("Can't create Team beacause it's full");
                        }
                    }
                    //end check is Participant
                    else
                    {
                        throw new UnauthorizedAccessException("You aren't participant in Competition");
                    }
                }
                //end check competition != null
                else
                {
                    throw new ArgumentException("Competition is not found");
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<ViewParticipantInTeam> InsertMemberInTeam(ParticipantInTeamInsertModel model, string token)
        {

            //check xem nó có đang ở nhóm khác không
            //check xem nó có đang join lại chính nhóm này hay không
            try
            {
                var jsonToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var UserIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));

                int UserId = Int32.Parse(UserIdClaim.Value);

                if (string.IsNullOrEmpty(model.InvitedCode)) throw new ArgumentNullException("Invited Code Null");

                //check invited code 
                Team team = await _teamRepo.GetTeamByInvitedCode(model.InvitedCode);
                if (team != null)
                {
                    //check student is participant in that competiiton
                    Participant participant = await _participantRepo.Participant_In_Competition(UserId, team.CompetitionId);
                    if (participant != null)
                    {
                        //check number of member in team
                        Competition competition = await _competitionRepo.Get(team.CompetitionId);
                        int NumberOfStudentInTeam = competition.NumberOfParticipation / competition.NumberOfTeam;
                        if (await _participantInTeamRepo.CheckParticipantInTeam(team.Id, NumberOfStudentInTeam))
                        {
                            //------ Add Participant in team
                            ParticipantInTeam pit = new ParticipantInTeam()
                            {
                                ParticipantId = participant.Id,
                                TeamId = team.Id,
                                //auto member
                                TeamRoleId = await _teamRoleRepo.GetRoleIdByName("Member"),
                                //auto status 
                                Status = ParticipantInTeamStatus.InTeam
                            };

                            await _participantInTeamRepo.Insert(pit);

                            return TransformViewParticipantInTeam(pit,competition.Id);

                        }
                        //end check number of member in team
                        else
                        {
                            throw new ArgumentException("Team is full");
                        }
                    }
                    //end check is participant
                    else
                    {
                        throw new UnauthorizedAccessException("You aren't participant in Competition");
                    }
                }
                //end invited code 
                else
                {
                    throw new ArgumentException("Not found team with Invited Code");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }




        public ViewParticipantInTeam TransformViewParticipantInTeam(ParticipantInTeam participantInTeam, int CompetitionId)
        {
            return new ViewParticipantInTeam()
            {
                Id = participantInTeam.Id,
                ParticipantId = participantInTeam.ParticipantId,
                TeamId = participantInTeam.TeamId,
                CompetitionId = CompetitionId
            };
        }

        public ViewTeam TransformViewTeam(Team team)
        {
            return new ViewTeam()
            {
                Id = team.Id,
                Name = team.Name,
                CompetitionId = team.CompetitionId,
                Description = team.Description,
                InvitedCode = team.InvitedCode,
            };
        }




        //Generate Seed code length 15
        private string GenerateSeedCode()
        {
            string codePool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] chars = new char[15];
            string code = "";
            var random = new Random();

            for (int i = 0; i < chars.Length; i++)
            {
                code += string.Concat(codePool[random.Next(codePool.Length)]);
            }
            return code;
        }
        //----------------------------------------------------------------------------------------Check
        //check exist code
        private async Task<string> CheckExistCode()
        {
            //auto generate seedCode
            bool check = true;
            string seedCode = "";
            while (check)
            {
                string generateCode = GenerateSeedCode();
                check = await _teamRepo.CheckExistCode(generateCode);
                seedCode = generateCode;
            }
            return seedCode;
        }


    }
}
