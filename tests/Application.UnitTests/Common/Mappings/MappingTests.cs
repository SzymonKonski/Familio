using System.Runtime.Serialization;
using Application.Common.Mappings;
using Application.Features.Groups.Queries.GetGroupDetails;
using Application.Features.Messages.Queries.GetMessages;
using Application.Features.Schedule.Queries.GetEvents;
using Application.Features.TodoItems.Queries.GetTodoItemDetails;
using Application.Features.TodoItems.Queries.GetTodoItems;
using Application.Features.Users.Queries.GetUserDetails;
using Application.Features.Users.Queries.GetUserGroups;
using AutoMapper;
using Domain.Entities;
using NUnit.Framework;
using UserDto = Application.Features.Groups.Queries.GetGroupDetails.UserDto;

namespace Application.UnitTests.Common.Mappings;

public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(config =>
            config.AddProfile<MappingProfile>());

        _mapper = _configuration.CreateMapper();
    }

    [Test]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }

    [Test]
    [TestCase(typeof(TodoItem), typeof(TodoItemDetailsDto))]
    [TestCase(typeof(TodoItem), typeof(TodoItemBriefDto))]
    [TestCase(typeof(Message), typeof(MessageDto))]
    [TestCase(typeof(Event), typeof(EventDto))]
    [TestCase(typeof(DomainUser), typeof(UserDetailsDto))]
    [TestCase(typeof(DomainUserGroup), typeof(UserDto))]
    [TestCase(typeof(DomainUserGroup), typeof(GroupDto))]
    [TestCase(typeof(Group), typeof(GroupVm))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = GetInstanceOf(source);

        _mapper.Map(instance, source, destination);
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        // Type without parameterless constructor
        return FormatterServices.GetUninitializedObject(type);
    }
}