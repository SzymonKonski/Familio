using Application.Features.Groups.Queries.GetGroupDetails;
using Application.Features.Messages.Queries.GetMessages;
using Application.Features.Schedule.Queries.GetEvents;
using Application.Features.TodoItems.Queries.GetTodoItemDetails;
using Application.Features.TodoItems.Queries.GetTodoItems;
using Application.Features.Users.Queries.GetUserDetails;
using Application.Features.Users.Queries.GetUserGroups;
using AutoMapper;
using Domain.Entities;
using UserDto = Application.Features.Groups.Queries.GetGroupDetails.UserDto;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        MappingFunc();
    }

    private void MappingFunc()
    {
        CreateMap<Group, GroupVm>().ForMember(x => x.GroupId, opt => opt.MapFrom(src => src.Id))
            .ForMember(x => x.Users, opt => opt.MapFrom(src => src.DomainUserGroups));
        CreateMap<DomainUserGroup, GroupDto>().ForMember(x => x.Id, opt => opt.MapFrom(src => src.Group.Id))
            .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(x => x.Role, opt => opt.MapFrom(src => src.Role));
        CreateMap<DomainUserGroup, UserDto>().ForMember(x => x.Id, opt => opt.MapFrom(src => src.DomainUserId))
            .ForMember(x => x.Avatar, opt => opt.MapFrom(src => src.DomainUser.Avatar))
            .ForMember(x => x.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(x => x.Role, opt => opt.MapFrom(src => src.Role));

        CreateMap<DomainUser, UserDetailsDto>().ForMember(x => x.AvatarUrl, opt => opt.MapFrom(src => src.Avatar));
        CreateMap<Event, EventDto>()
            .ForMember(x => x.CreatedByUser, opt => opt.MapFrom(src => src.CreatedByUser.UserName));
        CreateMap<Message, MessageDto>()
            .ForMember(v => v.UserId, opt => opt.MapFrom(src => src.CreatedByUser.DomainUserId))
            .ForMember(v => v.UserName, opt => opt.MapFrom(src => src.CreatedByUser.UserName))
            .ForMember(v => v.UserAvatar, opt => opt.MapFrom(src => src.CreatedByUser.DomainUser.Avatar));
        CreateMap<TodoItem, TodoItemBriefDto>()
            .ForMember(v => v.AssignedUser, opt => opt.MapFrom(src => src.AssignedUser.UserName))
            .ForMember(v => v.AssignedUser, opt => opt.MapFrom(src => src.AssignedUser.UserName))
            .ForMember(v => v.UserId, opt => opt.MapFrom(src => src.UserId));
        CreateMap<TodoItem, TodoItemDetailsDto>()
            .ForMember(v => v.CreatedByUser, opt => opt.MapFrom(src => src.CreatedByUser.UserName))
            .ForMember(v => v.AssignedUser, opt => opt.MapFrom(src => src.AssignedUser.UserName))
            .ForMember(v => v.UserId, opt => opt.MapFrom(src => src.UserId));
    }
}