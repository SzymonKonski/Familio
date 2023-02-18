using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IInvitationDataProtector
{
    Task<InvitationToGroup> GetDataFromToken(string token);
}