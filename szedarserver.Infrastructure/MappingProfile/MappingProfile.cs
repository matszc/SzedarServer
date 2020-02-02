using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using szedarserver.Core.Domain;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.MappingProfile
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<LoginModel, User>();
            CreateMap<User, AccountDTO>();
            CreateMap<Tournament, TournamentDTO>();
            CreateMap<Tournament, Tournament>();
        }
    }
}
