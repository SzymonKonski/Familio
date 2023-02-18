using FluentValidation;

namespace Application.Features.Messages.Queries.GetMessages;

public class GetMessagesQueryValidator : AbstractValidator<GetMessagesQuery>
{
    public GetMessagesQueryValidator()
    {
        RuleFor(v => v.GroupId).NotEmpty();
    }
}